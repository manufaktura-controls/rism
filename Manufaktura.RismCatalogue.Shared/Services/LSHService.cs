using Manufaktura.Controls.Linq;
using Manufaktura.RismCatalogue.Model;
using Manufaktura.RismCatalogue.Shared.Algorithms;
using System.Linq;

namespace Manufaktura.RismCatalogue.Shared.Services
{
    public class LSHService
    {
        private readonly RismDbContext dbContext;
        private readonly PlaineAndEasieService plaineAndEasieService;
        private const int NumberOfDimensions = 12;

        public LSHService(RismDbContext dbContext, PlaineAndEasieService plaineAndEasieService)
        {
            this.dbContext = dbContext;
            this.plaineAndEasieService = plaineAndEasieService;
        }

        public void GenerateHashes(int numberOfGroups, int numberOfPlanes)
        {
            var count = dbContext.Incipits.Count(i => !dbContext.SpatialHashes.Any(h => h.IncipitId == i.Id) && i.MusicalNotation != null) * numberOfGroups;
            var pageSize = 500;

            foreach (var groupNumber in Enumerable.Range(1, numberOfGroups))
            {
                var lshAlgorithm = GetPlane(groupNumber, numberOfPlanes);

                while (true)
                {
                    var melodiesWithoutHashes = GetIncipitsBatch(pageSize, groupNumber);
                    if (!melodiesWithoutHashes.Any()) break;

                    foreach (var melody in melodiesWithoutHashes)
                    {
                        var position = GetIncipitVector(melody);
                        dbContext.SpatialHashes.Add(new SpatialHash
                        {
                            PlaneGroupNumber = groupNumber,
                            Hash = lshAlgorithm.ComputeHash(position),
                            IncipitId = melody.Id
                        });
                    }
                    dbContext.SaveChanges();
                }
            }
        }

        private Incipit[] GetIncipitsBatch(int take, int groupNumber)
        {
            return dbContext.Incipits.Where(m => m.MusicalNotation != null && !dbContext.SpatialHashes.Any(h => h.PlaneGroupNumber == groupNumber && h.IncipitId == m.Id))
                .OrderBy(m => m.Id).Take(take).ToArray();
        }

        private Vector<double> GetIncipitVector(Incipit incipit)
        {
            var intervals = plaineAndEasieService.Parse(incipit).ToIntervals().Take(NumberOfDimensions).Select(i => (double)i).ToList();
            while (intervals.Count < NumberOfDimensions) intervals.Add(0d);

            return new Vector<double>(intervals);
        }

        private LSHAlgorithm GetPlane(int groupNumber, int numberOfPlanes)
        {
            LSHAlgorithm lshAlgorithm;
            var planes = dbContext.Planes.Where(p => p.GroupNumber == groupNumber).ToArray();
            if (!planes.Any())
            {
                lshAlgorithm = new LSHAlgorithm(NumberOfDimensions, numberOfPlanes, -12, 12);
                foreach (var plane in lshAlgorithm.Planes)
                {
                    dbContext.Planes.Add(new Plane
                    {
                        GroupNumber = groupNumber,
                        Coordinate1 = plane[0],
                        Coordinate2 = plane[1],
                        Coordinate3 = plane[2],
                        Coordinate4 = plane[3],
                        Coordinate5 = plane[4],
                        Coordinate6 = plane[5],
                        Coordinate7 = plane[6],
                        Coordinate8 = plane[7],
                        Coordinate9 = plane[8],
                        Coordinate10 = plane[9],
                        Coordinate11 = plane[10],
                        Coordinate12 = plane[11],
                    });
                    dbContext.SaveChanges();
                }
            }
            else lshAlgorithm = new LSHAlgorithm(planes.Select(p => new Vector<double>(
                p.Coordinate1, p.Coordinate2, p.Coordinate3, p.Coordinate4,
                p.Coordinate5, p.Coordinate6, p.Coordinate7, p.Coordinate8,
                p.Coordinate9, p.Coordinate10, p.Coordinate11, p.Coordinate12)).ToArray());

            return lshAlgorithm;
        }
    }
}
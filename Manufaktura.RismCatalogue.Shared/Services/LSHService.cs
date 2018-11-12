using Manufaktura.Controls.Linq;
using Manufaktura.RismCatalogue.Model;
using Manufaktura.RismCatalogue.Shared.Algorithms;
using System;
using System.Linq;

namespace Manufaktura.RismCatalogue.Shared.Services
{
    public class LSHService
    {
        private readonly RismDbContext dbContext;
        private readonly PlaineAndEasieService plaineAndEasieService;
        private const int MaxNumberOfDimensions = 12;

        public LSHService(RismDbContext dbContext, PlaineAndEasieService plaineAndEasieService)
        {
            this.dbContext = dbContext;
            this.plaineAndEasieService = plaineAndEasieService;
        }

        public void GenerateHashes(int numberOfGroups, int numberOfPlanes)
        {
            var count = dbContext.Incipits.Count(i => !dbContext.SpatialHashes.Any(h => h.IncipitId == i.Id) && i.MusicalNotation != null) * numberOfGroups;
            var pageSize = 500;

            for (var numberOfDimensions = 1; numberOfDimensions <= MaxNumberOfDimensions; numberOfDimensions++)
            {
                foreach (var groupNumber in Enumerable.Range(1, numberOfGroups))
                {
                    Console.WriteLine($"Processing {numberOfDimensions}-dimensional hashes in group {groupNumber}.");
                    var lshAlgorithm = GetPlaneGroup(groupNumber, numberOfPlanes, numberOfDimensions);

                    while (true)
                    {
                        var melodiesWithoutHashes = GetIncipitsBatch(pageSize, groupNumber, numberOfDimensions);
                        if (!melodiesWithoutHashes.Any()) break;

                        foreach (var melody in melodiesWithoutHashes)
                        {
                            var position = GetIncipitVector(melody, numberOfDimensions);
                            dbContext.SpatialHashes.Add(new SpatialHash
                            {
                                PlaneGroupNumber = groupNumber,
                                NumberOfDimensions = numberOfDimensions,
                                Hash = lshAlgorithm.ComputeHash(position),
                                IncipitId = melody.Id
                            });
                        }
                        dbContext.SaveChanges();
                    }
                }
            }
        }

        private Incipit[] GetIncipitsBatch(int take, int groupNumber, int numberOfDimensions)
        {
            return dbContext.Incipits.Where(m => m.MusicalNotation != null &&
            !dbContext.SpatialHashes.Any(h => h.PlaneGroupNumber == groupNumber && h.NumberOfDimensions == numberOfDimensions && h.IncipitId == m.Id))
                .OrderBy(m => m.Id).Take(take).ToArray();
        }

        private Vector<double> GetIncipitVector(Incipit incipit, int numberOfDimensions)
        {
            var intervals = plaineAndEasieService.Parse(incipit).ToIntervals().Take(numberOfDimensions).Select(i => (double)i).ToList();
            while (intervals.Count < numberOfDimensions) intervals.Add(0d);

            return new Vector<double>(intervals);
        }

        private static double TryGetPlaneCoordinate(Vector<double> plane, int index)
        {
            if (plane.Length < index + 1) return 0;
            return plane[index];
        }

        private LSHAlgorithm GetPlaneGroup(int groupNumber, int numberOfPlanes, int numberOfDimensions)
        {
            LSHAlgorithm lshAlgorithm;
            var planes = dbContext.Planes.Where(p => p.GroupNumber == groupNumber && p.NumberOfDimensions == numberOfDimensions).ToArray();
            if (!planes.Any())
            {
                lshAlgorithm = new LSHAlgorithm(numberOfDimensions, numberOfPlanes, -12, 12);
                foreach (var plane in lshAlgorithm.Planes)
                {
                    dbContext.Planes.Add(new Plane
                    {
                        GroupNumber = groupNumber,
                        NumberOfDimensions = numberOfDimensions,
                        Coordinate1 = TryGetPlaneCoordinate(plane, 0),
                        Coordinate2 = TryGetPlaneCoordinate(plane, 1),
                        Coordinate3 = TryGetPlaneCoordinate(plane, 2),
                        Coordinate4 = TryGetPlaneCoordinate(plane, 3),
                        Coordinate5 = TryGetPlaneCoordinate(plane, 4),
                        Coordinate6 = TryGetPlaneCoordinate(plane, 5),
                        Coordinate7 = TryGetPlaneCoordinate(plane, 6),
                        Coordinate8 = TryGetPlaneCoordinate(plane, 7),
                        Coordinate9 = TryGetPlaneCoordinate(plane, 8),
                        Coordinate10 = TryGetPlaneCoordinate(plane, 9),
                        Coordinate11 = TryGetPlaneCoordinate(plane, 10),
                        Coordinate12 = TryGetPlaneCoordinate(plane, 11)
                    });
                    dbContext.SaveChanges();
                }
            }
            else lshAlgorithm = new LSHAlgorithm(planes.Select(p => new Vector<double>(new double[] {
                p.Coordinate1, p.Coordinate2, p.Coordinate3, p.Coordinate4,
                p.Coordinate5, p.Coordinate6, p.Coordinate7, p.Coordinate8,
                p.Coordinate9, p.Coordinate10, p.Coordinate11, p.Coordinate12 }.Take(numberOfDimensions))).ToArray());

            return lshAlgorithm;
        }
    }
}
using Manufaktura.Controls.Linq;
using Manufaktura.Controls.Model;
using Manufaktura.RismCatalogue.Model;
using Manufaktura.RismCatalogue.Shared.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Manufaktura.RismCatalogue.Shared.Services
{
    public class LSHService
    {
        private readonly RismDbContext dbContext;
        private readonly PlaineAndEasieService plaineAndEasieService;

        private List<Plane> planesCache = new List<Plane>();

        public LSHService(RismDbContext dbContext, PlaineAndEasieService plaineAndEasieService)
        {
            this.dbContext = dbContext;
            this.plaineAndEasieService = plaineAndEasieService;
        }

        public void GenerateHashes(int numberOfGroups, int numberOfPlanes)
        {
            var pageSize = 500;
            var skip = 0;
            while (true)
            {
                var incipits = GetIncipitsBatch(skip, pageSize);

                for (var numberOfDimensions = 1; numberOfDimensions <= Constants.MaxNumberOfDimensions; numberOfDimensions++)
                {
                    foreach (var groupNumber in Enumerable.Range(1, numberOfGroups))
                    {
                        Console.WriteLine($"Processing {numberOfDimensions}-dimensional hashes in group {groupNumber} for incipits {skip}-{skip + pageSize}.");
                        var lshAlgorithm = GetPlaneGroup(groupNumber, numberOfPlanes, numberOfDimensions);
                        foreach (var incipit in incipits)
                        {
                            var score = plaineAndEasieService.Parse(incipit);
                            
                            var position = GetIncipitVector(score, numberOfDimensions);
                            dbContext.SpatialHashes.Add(new SpatialHash
                            {
                                PlaneGroupNumber = groupNumber,
                                NumberOfDimensions = numberOfDimensions,
                                Hash = lshAlgorithm.ComputeHash(position),
                                IncipitId = incipit.Id
                            });
                        }
                        dbContext.SaveChanges();
                    }
                }

                skip += incipits.Length;
                if (incipits.Length < pageSize) break;
            }
        }

        private static Vector GetIncipitVector(Score score, int numberOfDimensions)
        {
            var intervals = score.ToIntervals().Take(numberOfDimensions).Select(i => (double)i).ToList();
            while (intervals.Count < numberOfDimensions) intervals.Add(0d);

            return new Vector(intervals);
        }

        private static double TryGetPlaneCoordinate(Vector plane, int index)
        {
            if (plane.Length < index + 1) return 0;
            return plane[index];
        }

        private static double TryGetPlaneTranslation(Vector plane, int index)
        {
            var translatedVector = plane as TranslatedVector;
            if (translatedVector == null) return 0;

            if (plane.Length < index + 1) return 0;
            return translatedVector.Translation[index];
        }

        private Incipit[] GetIncipitsBatch(int skip, int take)
        {
            return dbContext.Incipits.Where(m => m.MusicalNotation != null).OrderBy(m => m.Id).Skip(skip).Take(take).ToArray();
        }
        private LSHAlgorithm GetPlaneGroup(int groupNumber, int numberOfPlanes, int numberOfDimensions)
        {
            LSHAlgorithm lshAlgorithm;

            var planes = planesCache.Where(p => p.GroupNumber == groupNumber && p.NumberOfDimensions == numberOfDimensions).ToArray();
            if (!planes.Any())
            {
                planes = dbContext.Planes.Where(p => p.GroupNumber == groupNumber && p.NumberOfDimensions == numberOfDimensions).ToArray();
                planesCache.AddRange(planes);
            }

            if (!planes.Any())
            {
                lshAlgorithm = new LSHAlgorithm(numberOfDimensions, numberOfPlanes, -12, 12);
                foreach (var plane in lshAlgorithm.Planes)
                {
                    var newPlane = new Plane
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
                        Coordinate12 = TryGetPlaneCoordinate(plane, 11),
                        Translation1 = TryGetPlaneTranslation(plane, 0),
                        Translation2 = TryGetPlaneTranslation(plane, 1),
                        Translation3 = TryGetPlaneTranslation(plane, 2),
                        Translation4 = TryGetPlaneTranslation(plane, 3),
                        Translation5 = TryGetPlaneTranslation(plane, 4),
                        Translation6 = TryGetPlaneTranslation(plane, 5),
                        Translation7 = TryGetPlaneTranslation(plane, 6),
                        Translation8 = TryGetPlaneTranslation(plane, 7),
                        Translation9 = TryGetPlaneTranslation(plane, 8),
                        Translation10 = TryGetPlaneTranslation(plane, 9),
                        Translation11 = TryGetPlaneTranslation(plane, 10),
                        Translation12 = TryGetPlaneTranslation(plane, 11)
                    };
                    dbContext.Planes.Add(newPlane);
                    planesCache.Add(newPlane);
                }
                Console.WriteLine($"Planes created for group {groupNumber} in {numberOfDimensions} dimensions.");
                dbContext.SaveChanges();
            }
            else lshAlgorithm = new LSHAlgorithm(planes.Select(p => new TranslatedVector(new double[] {
                p.Coordinate1, p.Coordinate2, p.Coordinate3, p.Coordinate4,
                p.Coordinate5, p.Coordinate6, p.Coordinate7, p.Coordinate8,
                p.Coordinate9, p.Coordinate10, p.Coordinate11, p.Coordinate12 }.Take(numberOfDimensions).ToArray(),
                new double[] {
                p.Translation1, p.Translation2, p.Translation3, p.Translation4,
                p.Translation5, p.Translation6, p.Translation7, p.Translation8,
                p.Translation9, p.Translation10, p.Translation11, p.Translation12 }.Take(numberOfDimensions).ToArray())).ToArray());

            return lshAlgorithm;
        }
    }
}
using Manufaktura.RismCatalogue.Knockout.Extensions;
using Manufaktura.RismCatalogue.Model;
using Manufaktura.RismCatalogue.Shared.Algorithms;
using Manufaktura.RismCatalogue.Shared.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Manufaktura.RismCatalogue.Migration.Services
{
    public class HashGenerationServiceInIncipitTable
    {
        private RismDbContext dbContext;

        private List<Plane> planesCache = new List<Plane>();

        public HashGenerationServiceInIncipitTable(RismDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

       
        public void GenerateHashes()
        {
            var pageSize = 2000;
            var processedincipits = 0;
            while (true)
            {
                var sw = new Stopwatch();
                sw.Start();

                if (processedincipits % 10000 == 0) dbContext = Dependencies.CreateContext();  //Recreate
                Console.WriteLine($"Searching for incipits...");
                var incipits = GetIncipitsBatch(pageSize);

                var numberOfPlanes = 40;

                Console.WriteLine($"Processing hashes for incipits...");
                foreach (var incipit in incipits)
                {
                    for (var numberOfDimensions = 1; numberOfDimensions <= Constants.MaxNumberOfDimensionsForLsh; numberOfDimensions++)
                    {
                        var position = GetIncipitVector(new[] {
                            incipit.Interval1,
                            incipit.Interval2,
                            incipit.Interval3,
                            incipit.Interval4,
                            incipit.Interval5,
                            incipit.Interval6,
                            incipit.Interval7,
                            incipit.Interval8,
                            incipit.Interval9,
                            incipit.Interval10,
                            incipit.Interval11,
                            incipit.Interval12
                        }.Take(numberOfDimensions).ToArray());

                        var lshAlgorithm = GetPlaneGroup(1, numberOfPlanes, numberOfDimensions);
                        var hash = lshAlgorithm.ComputeHash(position);

                        if (numberOfDimensions == 1) incipit.Hash1d = hash;
                        if (numberOfDimensions == 2) incipit.Hash2d = hash;
                        if (numberOfDimensions == 3) incipit.Hash3d = hash;
                        if (numberOfDimensions == 4) incipit.Hash4d = hash;
                        if (numberOfDimensions == 5) incipit.Hash5d = hash;
                        if (numberOfDimensions == 6) incipit.Hash6d = hash;
                    }
                    dbContext.Entry(incipit).State = EntityState.Modified;
                }
                dbContext.SaveChanges();

                processedincipits += incipits.Length;

                sw.Stop();
                var timeRemaining = TimeSpan.FromSeconds(((1775885 - processedincipits) / pageSize) * sw.Elapsed.TotalSeconds);

                Console.WriteLine($"Completed {processedincipits} incipits. Time remaining: {timeRemaining}.");

                

                if (incipits.Length < pageSize) break;
            }
        }

        private static Vector GetIncipitVector(int[] intervals)
        {
            return new Vector(intervals.Select(i => (double)i));
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

        private Incipit[] GetIncipitsBatch(int take)
        {
            return dbContext.Incipits.Where(m => m.Hash1d == null)
                .OrderBy(i => i.Id).Take(take).ToArray();
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
                lshAlgorithm = new LSHAlgorithm(numberOfDimensions, numberOfPlanes, -12, 12, 0, 0);
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
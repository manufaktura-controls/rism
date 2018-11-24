using Manufaktura.RismCatalogue.Knockout.Extensions;
using Manufaktura.RismCatalogue.Model;
using Manufaktura.RismCatalogue.Shared.Algorithms;
using Manufaktura.RismCatalogue.Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manufaktura.RismCatalogue.Migration.Services
{
    public class HashGenerationServiceForDistinctIncipits
    {
        private RismDbContext dbContext;
        private readonly PlaineAndEasieService plaineAndEasieService;

        private List<Plane> planesCache = new List<Plane>();

        public HashGenerationServiceForDistinctIncipits(RismDbContext dbContext, PlaineAndEasieService plaineAndEasieService)
        {
            this.dbContext = dbContext;
            this.plaineAndEasieService = plaineAndEasieService;
        }

        private int[][] GetDistinctMelodies(int numberOfDimensions)
        {
            var queryBuilder = new StringBuilder("select distinct ");
            queryBuilder.Append(string.Join(", ", Enumerable.Range(1, numberOfDimensions).Select(i => $"i.Interval{i}")));
            queryBuilder.Append(" from incipits i");
            var query = queryBuilder.ToString();
            var results = dbContext.RawSqlQuery(query);
            return results.Select(row => row.Cast<int>().ToArray()).ToArray();
        }

        private Incipit[] GetIncipitsByIntervalPatternFromCache(int[] pattern, List<Incipit> cache)
        {
            var query = cache.AsEnumerable();
            if (pattern.Length > 0) query = query.Where(i => i.Interval1 == pattern[0]);
            if (pattern.Length > 1) query = query.Where(i => i.Interval2 == pattern[1]);
            if (pattern.Length > 2) query = query.Where(i => i.Interval3 == pattern[2]);
            if (pattern.Length > 3) query = query.Where(i => i.Interval4 == pattern[3]);
            if (pattern.Length > 4) query = query.Where(i => i.Interval5 == pattern[4]);
            if (pattern.Length > 5) query = query.Where(i => i.Interval6 == pattern[5]);
            if (pattern.Length > 6) query = query.Where(i => i.Interval7 == pattern[6]);
            if (pattern.Length > 7) query = query.Where(i => i.Interval8 == pattern[7]);
            if (pattern.Length > 8) query = query.Where(i => i.Interval9 == pattern[8]);
            if (pattern.Length > 9) query = query.Where(i => i.Interval10 == pattern[9]);
            if (pattern.Length > 10) query = query.Where(i => i.Interval11 == pattern[10]);
            if (pattern.Length > 11) query = query.Where(i => i.Interval12 == pattern[11]);
            return query.ToArray();
        }

        private Incipit[] GetIncipitsByIntervalPatternFromDb(int[] pattern, List<Incipit> incipitCache)
        {
            var queryBuilder = new StringBuilder("select i.Id, ");
            queryBuilder.Append(string.Join(", ", Enumerable.Range(1, 12).Select(i => $"i.Interval{i}")));
            queryBuilder.Append(" from incipits i where ");
            queryBuilder.Append(string.Join(" AND ", Enumerable.Range(0, pattern.Length).Select(i => $"i.Interval{i + 1} = {pattern[i]}")));
            var query = queryBuilder.ToString();
            var results = dbContext.RawSqlQuery(query);
            var typedResults = results.Select(row => new Incipit
            {
                Id = (long)row[0],
                Interval1 = (int)row[1],
                Interval2 = (int)row[2],
                Interval3 = (int)row[3],
                Interval4 = (int)row[4],
                Interval5 = (int)row[5],
                Interval6 = (int)row[6],
                Interval7 = (int)row[7],
                Interval8 = (int)row[8],
                Interval9 = (int)row[9],
                Interval10 = (int)row[10],
                Interval11 = (int)row[11],
                Interval12 = (int)row[12],
            }).ToArray();
            incipitCache.AddRange(typedResults);
            return typedResults;
        }

        public void GenerateHashes(int numberOfGroups)
        {
            var incipitCache = new List<Incipit>();
            for (var numberOfDimensions = 1; numberOfDimensions <= Constants.MaxNumberOfDimensionsForLsh; numberOfDimensions++)
            {
                var hashCache = new List<SpatialHash>();

                dbContext = Dependencies.CreateContext();  //Recreate
                Console.WriteLine($"Searching distinct melodies for {numberOfDimensions} dimensions...");
                var distinctMelodies = GetDistinctMelodies(numberOfDimensions);
                Console.WriteLine($"Found {distinctMelodies.Length} distinct melodies for {numberOfDimensions} dimensions.");

                var patternNumber = 1;
                foreach (var distinctIntervalPattern in distinctMelodies)
                {
                    Console.WriteLine($"Generating {numberOfDimensions}-dimensional hashes for pattern {string.Join(" ", distinctIntervalPattern)} ({patternNumber++} of {distinctMelodies.Length}).");

                    var position = GetIncipitVector(distinctIntervalPattern);
                    var numberOfPlanes = 15;    //There's no use to increase number of planes according to number of dimensions. Planes always divide space in two.

                    Console.WriteLine($"Searching incipit ids for pattern {string.Join(" ", distinctIntervalPattern)}...");
                    var allIncipitsForThisPattern = numberOfDimensions == 1 ? GetIncipitsByIntervalPatternFromDb(distinctIntervalPattern, incipitCache)
                        : GetIncipitsByIntervalPatternFromCache(distinctIntervalPattern, incipitCache);

                    foreach (var groupNumber in Enumerable.Range(1, numberOfGroups))
                    {
                        var lshAlgorithm = GetPlaneGroup(groupNumber, numberOfPlanes, numberOfDimensions);
                        var hash = lshAlgorithm.ComputeHash(position);

                        var spatialHash = new SpatialHash { Id = $"{numberOfDimensions}-{groupNumber}-{hash}" };
                        var existingHash = hashCache.FirstOrDefault(h => h.Id == spatialHash.Id);
                        if (existingHash == null)
                        {
                            dbContext.SpatialHashes.Add(spatialHash);
                            dbContext.SaveChanges();
                            hashCache.Add(spatialHash);
                        }
                        else spatialHash = existingHash;

                        var skip = 0;
                        var pageSize = 300;
                        while (true)
                        {
                            Console.WriteLine($"Adding incipit ids for pattern {string.Join(" ", distinctIntervalPattern)} ({skip}/{allIncipitsForThisPattern.Length})...");
                            var page = allIncipitsForThisPattern.Skip(skip).Take(pageSize).ToArray();
                            foreach (var incipit in page)
                            {
                                dbContext.SpatialHashIncipits.Add(new SpatialHashIncipit { SpatialHashId = spatialHash.Id, IncipitId = incipit.Id });
                            }
                            dbContext.SaveChanges();

                            if (page.Length < pageSize) break;
                            skip += page.Length;
                        }
                    }

                    dbContext.SaveChanges();
                    dbContext = Dependencies.CreateContext();  //Recreate
                }
                dbContext.SaveChanges();
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
                lshAlgorithm = new LSHAlgorithm(numberOfDimensions, numberOfPlanes, -12, 12, -12, 12);
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
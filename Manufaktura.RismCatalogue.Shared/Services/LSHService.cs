using Manufaktura.RismCatalogue.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Manufaktura.RismCatalogue.Shared.Services
{
    /*public class LSHService
    {
        private readonly RismDbContext dbContext;

        public LSHService(RismDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void GenerateHashees( int numberOfGroups, int numberOfPlanes)
        {
            try
            {
                var count = dbContext.Melodies.Count(m => !dbContext.SpatialHashes.Any(h => h.MelodyId == m.Id)) * numberOfGroups;
                var progress = new ProgressCounter(count);
                var pageSize = 500;

                foreach (var groupNumber in Enumerable.Range(1, numberOfGroups))
                {
                    progressService.ReportProgressMessage(operationId, $"Geerowanie hashy grupy {groupNumber}...", SeverityLevel.Info);
                    var lshAlgorithm = GetPlane(groupNumber, numberOfPlanes);

                    while (true)
                    {
                        var melodiesWithoutHashes = GetMelodyBatch(pageSize, groupNumber);
                        if (!melodiesWithoutHashes.Any()) break;

                        foreach (var melody in melodiesWithoutHashes)
                        {
                            var position = GetMelodyVector(melody);
                            dbContext.SpatialHashes.Add(new SpatialHash
                            {
                                Id = Guid.NewGuid(),
                                GroupNumber = groupNumber,
                                Hash = lshAlgorithm.ComputeHash(position),
                                MelodyId = melody.Id
                            });
                        }
                        dbContext.SaveChanges();

                        progressService.ReportProgress(operationId, progress += melodiesWithoutHashes.Length);
                    }
                }
                progressService.ReportProgress(operationId, 100);
            }
            catch (Exception ex)
            {
                progressService.ReportProgressMessage(operationId, ex.Message, SeverityLevel.Error);
            }
            finally
            {
                dbContext.AutoDetectChangesEnabled = true;
            }
        }

        private Melody[] GetMelodyBatch(int take, int groupNumber)
        {
            return dbContext.Melodies.Where(m => !dbContext.SpatialHashes.Any(h => h.GroupNumber == groupNumber && h.MelodyId == m.Id))
                .OrderBy(m => m.UploadDate).Take(take).ToArray();
        }

        private Vector<double> GetMelodyVector(Melody melody)
        {
            return new Vector<double>(new int[] {melody.MelodyContour1 ?? 0, melody.MelodyContour2 ?? 0, melody.MelodyContour3 ?? 0, melody.MelodyContour4 ?? 0,
                melody.MelodyContour5 ?? 0, melody.MelodyContour6 ?? 0, melody.MelodyContour7 ?? 0, melody.MelodyContour8 ?? 0}.Select(i => (double)i));
        }

        private LSHAlgorithm GetPlane(int groupNumber, int numberOfPlanes)
        {
            LSHAlgorithm lshAlgorithm;
            var planes = dbContext.Planes.Where(p => p.GroupNumber == groupNumber).ToArray();
            if (!planes.Any())
            {
                lshAlgorithm = new LSHAlgorithm(8, numberOfPlanes, -12, 12);
                foreach (var plane in lshAlgorithm.Planes)
                {
                    dbContext.Planes.Add(new Plane
                    {
                        Id = Guid.NewGuid(),
                        GroupNumber = groupNumber,
                        Coordinate1 = plane[0],
                        Coordinate2 = plane[1],
                        Coordinate3 = plane[2],
                        Coordinate4 = plane[3],
                        Coordinate5 = plane[4],
                        Coordinate6 = plane[5],
                        Coordinate7 = plane[6],
                        Coordinate8 = plane[7],
                    });
                    dbContext.SaveChanges();
                }
            }
            else lshAlgorithm = new LSHAlgorithm(planes.Select(p => new Vector<double>(p.Coordinate1, p.Coordinate2, p.Coordinate3, p.Coordinate4, p.Coordinate5, p.Coordinate6, p.Coordinate7, p.Coordinate8)).ToArray());

            return lshAlgorithm;
        }
    }*/
}

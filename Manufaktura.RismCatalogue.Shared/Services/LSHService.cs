using Manufaktura.Controls.Linq;
using Manufaktura.Controls.Model;
using Manufaktura.RismCatalogue.Model;
using Manufaktura.RismCatalogue.Shared.Algorithms;
using System.Collections.Generic;
using System.Linq;

namespace Manufaktura.RismCatalogue.Shared.Services
{
    public class LSHService
    {
        public LSHAlgorithm[] GeneratePlaneGroups(int numberOfGroups, int numberOfPlanes, int numberOfDimensions)
        {
            return Enumerable.Range(0, numberOfGroups).Select(i => new LSHAlgorithm(numberOfDimensions, numberOfPlanes, -12, 12)).ToArray();
        }

        public IEnumerable<SpatialHash> GenerateHashes(Score score, LSHAlgorithm[] planeGroups, int numberOfDimensions)
        {
            int groupIndex = 0;
            foreach (var planeGroup in planeGroups)
            {
                var intervals = score.ToIntervals().Take(planeGroup.Planes.Length).Select(i => (double)i).ToList();
                while (intervals.Count < numberOfDimensions) intervals.Add(0d);

                yield return new SpatialHash
                {
                    PlaneGroupNumber = groupIndex++,
                    Hash = planeGroup.ComputeHash(new Vector<double>(intervals))
                };
            }
        }
    }
}
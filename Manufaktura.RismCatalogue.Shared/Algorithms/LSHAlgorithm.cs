using System;
using System.Collections.Generic;

namespace Manufaktura.RismCatalogue.Shared.Algorithms
{
    /// <summary>
    /// Podobna implementacja: https://github.com/rushikesh988/LSHASHING-CSharp/blob/master/LSHashing.cs
    /// </summary>
    public class LSHAlgorithm
    {
        public LSHAlgorithm(Vector<double>[] planes)
        {
            Planes = planes;
        }

        public LSHAlgorithm(int numberOfDimensions, int numberOfPlanes, double minValue, double maxValue)
        {
            Planes = GeneratePlanes(numberOfDimensions, numberOfPlanes, minValue, maxValue);
        }

        public Vector<double>[] Planes { get; private set; }

        public int ComputeHash(Vector<double> point)
        {
            int hash = 0;
            int orderOfMagnitude = 1;
            foreach (var plane in Planes)
            {
                hash += (GetSideOfAPlane(point, plane) ? 1 : 0) * orderOfMagnitude;
                orderOfMagnitude *= 2;
            }
            return hash;
        }

        private Vector<double>[] GeneratePlanes(int numberOfDimensions, int numberOfPlanes, double minValue, double maxValue)
        {
            var planes = new List<Vector<double>>();
            var random = new Random();
            for (var i = 0; i < numberOfPlanes; i++)
            {
                var plane = new List<double>();
                for (var dim = 0; dim < numberOfDimensions; dim++)
                {
                    plane.Add(minValue + random.NextDouble() * (maxValue - minValue));
                }
                planes.Add(new Vector<double>(plane));
            }
            return planes.ToArray();
        }

        private bool GetSideOfAPlane(Vector<double> point, Vector<double> plane)
        {
            return Vector<double>.DotProduct(point, plane) > 0;
        }
    }
}
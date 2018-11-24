using System;
using System.Collections.Generic;

namespace Manufaktura.RismCatalogue.Shared.Algorithms
{
    /// <summary>
    /// Podobna implementacja: https://github.com/rushikesh988/LSHASHING-CSharp/blob/master/LSHashing.cs
    /// </summary>
    public class LSHAlgorithm
    {
        public LSHAlgorithm(Vector[] planes)
        {
            Planes = planes;
        }

        public LSHAlgorithm(int numberOfDimensions, int numberOfPlanes, double minValue, double maxValue, double minTransValue, double maxTransValue)
        {
            Planes = GeneratePlanes(numberOfDimensions, numberOfPlanes, minValue, maxValue, minTransValue, maxTransValue);
        }

        public Vector[] Planes { get; private set; }

        public long ComputeHash(Vector point)
        {
            long hash = 0;
            long orderOfMagnitude = 1;
            
            foreach (var plane in Planes)
            {
                var pointCopy = point.Clone();
                if (plane is TranslatedVector translatedPlane) pointCopy = pointCopy.Translate(new Vector(translatedPlane.Translation).Invert());

                hash += (GetSideOfAPlane(pointCopy, plane) ? 1 : 0) * orderOfMagnitude;
                orderOfMagnitude *= 2;
            }
            return hash;
        }

        private Vector[] GeneratePlanes(int numberOfDimensions, int numberOfPlanes, double minValue, double maxValue, double minTransValue, double maxTransValue)
        {
            var planes = new List<Vector>();
            var random = new Random();
            for (var i = 0; i < numberOfPlanes; i++)
            {
                var plane = new List<double>();
                for (var dim = 0; dim < numberOfDimensions; dim++)
                {
                    plane.Add(minValue + random.NextDouble() * (maxValue - minValue));
                }

                var translation = new List<double>();
                for (var dim = 0; dim < numberOfDimensions; dim++)
                {
                    //translation.Add(0);
                    translation.Add(minValue + random.NextDouble() * (maxTransValue - minTransValue));
                }

                planes.Add(new TranslatedVector(plane, translation));
            }
            return planes.ToArray();
        }

        private bool GetSideOfAPlane(Vector point, Vector plane)
        {
            return Vector.DotProduct(point, plane) > 0;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

namespace Manufaktura.RismCatalogue.Shared.Algorithms
{
    public class Vector<T> where T : struct
    {
        public Vector(IEnumerable<T> coordinates)
        {
            Coordinates = coordinates.ToArray();
        }

        public Vector(params T[] coordinates)
        {
            Coordinates = coordinates;
        }

        public T[] Coordinates { get; set; }

        public int Length => Coordinates.Length;

        public T this[int i]
        {
            get { return Coordinates[i]; }
            set { Coordinates[i] = value; }
        }

        public static double DotProduct(Vector<double> vector1, Vector<double> vector2)
        {
            if (vector1.Length != vector2.Length) throw new ArgumentException("Lengths do not match.");

            var sum = 0d;
            for (var i = 0; i < vector1.Length; i++)
            {
                sum += vector1[i] * vector2[i];
            }

            return sum;
        }

        public T[] ToArray() => Coordinates;
    }
}
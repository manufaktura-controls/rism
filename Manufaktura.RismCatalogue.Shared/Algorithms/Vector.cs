using System;
using System.Collections.Generic;
using System.Linq;

namespace Manufaktura.RismCatalogue.Shared.Algorithms
{
    public class Vector
    {
        public Vector(IEnumerable<double> coordinates)
        {
            Coordinates = coordinates.ToArray();
        }

        public Vector(params double[] coordinates)
        {
            Coordinates = coordinates;
        }

        public double[] Coordinates { get; set; }

        public int Length => Coordinates.Length;

        public double this[int i]
        {
            get { return Coordinates[i]; }
            set { Coordinates[i] = value; }
        }

        public static double DotProduct(Vector vector1, Vector vector2)
        {
            if (vector1.Length != vector2.Length) throw new ArgumentException("Lengths do not match.");

            var sum = 0d;
            for (var i = 0; i < vector1.Length; i++)
            {
                sum += vector1[i] * vector2[i];
            }

            return sum;
        }

        public Vector Invert()
        {
            return new Vector(Coordinates.Select(c => c *= -1));
        }

        public double[] ToArray() => Coordinates;

        public Vector Translate(Vector translation)
        {
            if (Length != translation.Length) throw new ArgumentException("Lengths do not match.");
            var coordinatesCopy = Coordinates.ToArray();
            for (var i = 0; i < Coordinates.Length; i++)
            {
                coordinatesCopy[i] += translation[i];
            }
            return new Vector(coordinatesCopy);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

namespace Manufaktura.RismCatalogue.Shared.Algorithms
{
    public class TranslatedVector : Vector
    {
        public double[] Translation { get; set; }

        public TranslatedVector(IEnumerable<double> coordinates, IEnumerable<double> translation) : base(coordinates)
        {
            if (coordinates.Count() != translation.Count()) throw new Exception("Lengths do not match.");
            Translation = translation.ToArray();
        }
    }
}
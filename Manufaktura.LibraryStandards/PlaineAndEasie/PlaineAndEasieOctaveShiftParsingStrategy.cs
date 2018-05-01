using System.Collections.Generic;
using System.Linq;

namespace Manufaktura.LibraryStandards.PlaineAndEasie
{
    public class PlaineAndEasieOctaveShiftParsingStrategy : PlaineAndEasieParsingStrategy
    {
        private Dictionary<string, int> octaves = new Dictionary<string, int>
        {
            { "'", 4 },
            { "''", 5 },
            { "'''", 6 },
            { "''''", 7 },
            { ",", 3 },
            { ",,", 2 },
            { ",,,", 1 },
        };

        public override int ControlSignLength => 0;

        public override bool IsRelevant(string s) => octaves.Keys.Any(k => s.StartsWith(k));

        public override int Parse(PlaineAndEasieParser parser, string s)
        {
            var matchingKey = octaves.Keys.OrderByDescending(k => k.Length).First(k => s.StartsWith(k));
            parser.CurrentOctave = octaves[matchingKey];
            return matchingKey.Length;
        }
    }
}
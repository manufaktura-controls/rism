using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manufaktura.LibraryStandards.PlaineAndEasie
{
    public class PlaineAndEasieBarlineParsingStrategy : PlaineAndEasieParsingStrategy
    {
        private static Dictionary<string, PlaineAndEasieBarlineTypes> barlineTypes = new Dictionary<string, PlaineAndEasieBarlineTypes>
        {
            { "/", PlaineAndEasieBarlineTypes.Single },
            { "//", PlaineAndEasieBarlineTypes.Double },
            { "//:", PlaineAndEasieBarlineTypes.RepeatForward },
            { "://", PlaineAndEasieBarlineTypes.RepeatBackward },
            { "://:", PlaineAndEasieBarlineTypes.RepeatBoth }
        };

        public override int ControlSignLength => 0;

        public override bool IsRelevant(string s) => barlineTypes.Keys.Any(bt => s.StartsWith(bt));

        public override int Parse(PlaineAndEasieParser parser, string s)
        {
            var matchingKey = barlineTypes.Keys.OrderByDescending(k => k.Length).First(k => s.StartsWith(k));
            parser.AddBarline(barlineTypes[matchingKey]);

            return matchingKey.Length;
        }
    }
}

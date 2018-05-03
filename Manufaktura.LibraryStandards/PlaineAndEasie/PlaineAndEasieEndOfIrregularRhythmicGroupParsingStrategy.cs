using System.Text.RegularExpressions;

namespace Manufaktura.LibraryStandards.PlaineAndEasie
{
    public class PlaineAndEasieEndOfIrregularRhythmicGroupParsingStrategy : PlaineAndEasieParsingStrategy
    {
        public override int ControlSignLength => 0;

        public override bool IsRelevant(string s) => Regex.IsMatch(s, @";[0-9]+\)");

        public override int Parse(PlaineAndEasieParser parser, string s)
        {
            parser.IsGroupingEnabled = false;
            parser.GroupSize = 0;
            //TODO: Implement

            return s.IndexOf(")") + 1;
        }
    }
}
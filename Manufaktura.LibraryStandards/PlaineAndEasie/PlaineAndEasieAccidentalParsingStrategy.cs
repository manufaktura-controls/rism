using System.Linq;

namespace Manufaktura.LibraryStandards.PlaineAndEasie
{
    public class PlaineAndEasieAccidentalParsingStrategy : PlaineAndEasieParsingStrategy
    {
        private static readonly char[] AlterationSigns = new[] { 'x', 'b', 'n' };

        public override int ControlSignLength => 0;

        public override bool IsRelevant(string s) => AlterationSigns.Any(a => s[0] == a);

        public override int Parse(PlaineAndEasieParser parser, string s)
        {
            if (s[0] == 'x') parser.PendingAlter++;
            else if (s[0] == 'b') parser.PendingAlter--;
            else if (s[0] == 'n') parser.PendingNatural = true;

            return 1;
        }
    }
}
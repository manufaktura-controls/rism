using System;
using System.Linq;

namespace Manufaktura.LibraryStandards.PlaineAndEasie
{
    public class PlaineAndEasieNoteParsingStrategy : PlaineAndEasieParsingStrategy
    {
        public override int ControlSignLength => 0;

        private static readonly char[] Steps = new[] { 'C', 'D', 'E', 'F', 'G', 'A', 'B' };
        private static readonly char[] Accidentals = new[] { 'x', 'b', 'n' };

        public override bool IsRelevant(string s) => Steps.Any(c => c == s[0]) || Accidentals.Any(a => a == s[0]);

        public override int Parse(PlaineAndEasieParser parser, string s)
        {
            var alter = 0;
            var hasNatural = false;
            var i = 0;
            for (; i < s.Length; i++)
            {
                if (s[i] == 'x') alter++;
                else if (s[i] == 'b') alter--;
                else if (s[i] == 'n') hasNatural = true;
                else break;
            }

            var potentialFermata = s.Length >= i + 3 ? s.Substring(i, 3) : null;
            if (potentialFermata != null && potentialFermata[0] == '(' && potentialFermata[2] == ')')
            {
                parser.AddNote(potentialFermata[1], alter, hasNatural, true);
                return 3 + Math.Abs(alter);
            }
            else
            {
                parser.AddNote(s[i], alter, hasNatural, false);
                return 1 + Math.Abs(alter);
            }
        }
    }
}
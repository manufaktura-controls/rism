using System.Linq;

namespace Manufaktura.LibraryStandards.PlaineAndEasie
{
    public class PlaineAndEasieNoteParsingStrategy : PlaineAndEasieParsingStrategy
    {
        private static readonly char[] PreceedingMarks = new[] { 'x', 'b', 'n' };
        private static readonly char[] Steps = new[] { 'C', 'D', 'E', 'F', 'G', 'A', 'B' };
        public override int ControlSignLength => 0;

        public override bool IsRelevant(string s) => Steps.Any(c => c == s[0]) || PreceedingMarks.Any(a => a == s[0]);

        public override int Parse(PlaineAndEasieParser parser, string s)
        {
            var alter = 0;
            var hasNatural = false;
            var hasTrill = false;
            var hasSlur = false;
            var hasFermata = false;
            char step;

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
                step = potentialFermata[1];
                i += 3;
            }
            else
            {
                step = s[i];
                i++;
            }

            for (; i < s.Length; i++)
            {
                if (s[i] == 't') hasTrill = true;
                else if (s[i] == '+') hasSlur = true;
                else break;
            }

            parser.AddNote(step, alter, hasNatural, hasFermata, hasTrill, hasSlur);

            return i;
        }
    }
}
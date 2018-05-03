using System.Linq;

namespace Manufaktura.LibraryStandards.PlaineAndEasie
{
    public class PlaineAndEasieNoteParsingStrategy : PlaineAndEasieParsingStrategy
    {
        private static readonly char[] Steps = new[] { 'C', 'D', 'E', 'F', 'G', 'A', 'B' };
        public override int ControlSignLength => 0;

        public override bool IsRelevant(string s) => Steps.Any(c => c == s[0]);

        public override int Parse(PlaineAndEasieParser parser, string s)
        {
            var hasTrill = false;
            var hasSlur = false;

            var i = 0;
            var step = s[i++];
            for (; i < s.Length; i++)
            {
                if (s[i] == 't') hasTrill = true;
                else if (s[i] == '+') hasSlur = true;
                else break;
            }

            if (parser.LastAddedStep != default(char) && parser.LastAddedStep != step)
            {
                parser.PendingAlter = 0;
                parser.PendingNatural = false;
            }

            parser.AddNote(step, parser.PendingAlter, parser.PendingNatural, hasTrill, hasSlur);
            parser.LastAddedStep = step;
            if (parser.IsGroupingEnabled) parser.GroupSize++;

            return i;
        }
    }
}
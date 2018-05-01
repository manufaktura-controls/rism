using System;
using System.Linq;

namespace Manufaktura.LibraryStandards.PlaineAndEasie
{
    public class PlaineAndEasieNoteParsingStrategy : PlaineAndEasieParsingStrategy
    {
        public override int ControlSignLength => 0;

        private static readonly char[] Steps = new[] { 'C', 'D', 'E', 'F', 'G', 'A', 'B' };

        public override bool IsRelevant(string s) => Steps.Any(c => c == s[0]);

        public override int Parse(PlaineAndEasieParser parser, string s)
        {
            var alter = 0;
            for (var i = 1; i < s.Length; i++)
            {
                if (s[i] == 'x') alter++;
                else if (s[i] == 'b') alter--;
                else break;
            }
            parser.AddNote(s[0], alter);
            return 1 + Math.Abs(alter);
        }
    }
}
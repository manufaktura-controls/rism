namespace Manufaktura.LibraryStandards.PlaineAndEasie
{
    public class PlaineAndEasieKeyParsingStrategy : PlaineAndEasieParsingStrategy
    {
        private readonly char[] Flats = new char[] { 'B', 'E', 'A', 'D', 'G', 'C', 'F' };

        private readonly char[] Sharps = new char[] { 'F', 'C', 'G', 'D', 'A', 'E', 'B' };

        public override int ControlSignLength => 1;

        public override bool IsRelevant(string s) => s[0] == '$';
        public override int Parse(PlaineAndEasieParser parser, string s)
        {
            if (string.IsNullOrWhiteSpace(s) || s.Length <= 1)
            {
                parser.AddKey(0);
                return s?.Length ?? 0;
            }

            var modifier = s.StartsWith("b") ? -1 : 1;
            var fifths = 0;
            for (var i = 1; i < 7 && i < s.Length; i++)
            {
                if (s[i] != (modifier == 1 ? Sharps : Flats)[i - 1]) break;
                fifths++;
            }

            parser.AddKey(fifths * modifier);

            return fifths + 1;
        }
    }
}
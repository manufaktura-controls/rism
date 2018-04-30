namespace Manufaktura.LibraryStandards.PlaineAndEasie
{
    public class PlaineAndEasieClefParsingStrategy : PlaineAndEasieParsingStrategy
    {
        public override int ControlSignLength => 1;

        public override bool IsRelevant(string s) => s[0] == '%';

        public override int Parse(PlaineAndEasieParser parser, string s)
        {
            if (string.IsNullOrWhiteSpace(s) || s.Length < 3)
            {
                parser.AddClef('G', 2, false);
                return s?.Length ?? 0;
            }

            var clefData = s.Substring(0, 3);
            var isMensural = clefData[1] == '+';
            if (!int.TryParse(clefData[2].ToString(), out int lineNumber)) lineNumber = 2;

            parser.AddClef(clefData[0], lineNumber, isMensural);
            return 3;
        }
    }
}
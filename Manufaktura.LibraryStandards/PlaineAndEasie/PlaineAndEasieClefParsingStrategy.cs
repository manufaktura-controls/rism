namespace Manufaktura.LibraryStandards.PlaineAndEasie
{
    public abstract class PlaineAndEasieClefParsingStrategy<TOutput> : PlaineAndEasieParsingStrategy<TOutput>
    {
        protected PlaineAndEasieClefParsingStrategy()
        {
        }

        public override bool IsRelevant(string s) => s[0] == '%';

        public override int Parse(string s, TOutput output)
        {
            if (string.IsNullOrWhiteSpace(s)) return 0;
            if (s.Length < 3) return s.Length;

            var clefData = s.Substring(0, 3);
            var isMensural = clefData[1] == '+';
            if (!int.TryParse(clefData[2].ToString(), out int lineNumber)) lineNumber = 2;

            ParseInternal(output, clefData[0], lineNumber, isMensural);

            return 3;
        }

        protected abstract void ParseInternal(TOutput output, char clefType, int lineNumber, bool isMensural);
    }
}
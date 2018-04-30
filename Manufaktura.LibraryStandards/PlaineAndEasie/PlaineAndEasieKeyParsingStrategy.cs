namespace Manufaktura.LibraryStandards.PlaineAndEasie
{
    public abstract class PlaineAndEasieKeyParsingStrategy<TOutput> : PlaineAndEasieParsingStrategy<TOutput>
    {
        public override bool IsRelevant(string s) => s[0] == '&';

        public override int Parse(string s, TOutput output)
        {
            if (string.IsNullOrWhiteSpace(s) || s.Length <= 1)
            {
                ParseInternal(output, 0);
                return 1;
            }

            var numberOfFifths = s.StartsWith("b") ? -1 : 1;
            ParseInternal(output, numberOfFifths);

            return s.Length;
        }

        protected abstract void ParseInternal(TOutput output, int numberOfFifths);
    }
}
namespace Manufaktura.LibraryStandards.PlaineAndEasie
{
    public abstract class PlaineAndEasieParsingStrategy<TOutput>
    {
        public abstract bool IsRelevant(string s);

        public abstract int Parse(string s, TOutput output);
    }
}
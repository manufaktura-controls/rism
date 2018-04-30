namespace Manufaktura.LibraryStandards.PlaineAndEasie
{
    public abstract class PlaineAndEasieParsingStrategy
    {
        public abstract bool IsRelevant(string s);

        public abstract int Parse(PlaineAndEasieParser parser, string s);

        public abstract int ControlSignLength { get; }
    }
}
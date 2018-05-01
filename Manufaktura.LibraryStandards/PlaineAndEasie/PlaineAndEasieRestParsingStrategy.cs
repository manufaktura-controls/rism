namespace Manufaktura.LibraryStandards.PlaineAndEasie
{
    public class PlaineAndEasieRestParsingStrategy : PlaineAndEasieParsingStrategy
    {
        public override int ControlSignLength => 0;

        public override bool IsRelevant(string s) => s[0] == '-';

        public override int Parse(PlaineAndEasieParser parser, string s)
        {
            parser.AddRest();
            return 1;
        }
    }
}
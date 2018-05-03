namespace Manufaktura.LibraryStandards.PlaineAndEasie
{
    public class PlaineAndEasieGroupingParsingStrategy : PlaineAndEasieParsingStrategy
    {
        public override int ControlSignLength => 0;

        public override bool IsRelevant(string s) => s[0] == '(';

        public override int Parse(PlaineAndEasieParser parser, string s)
        {
            parser.IsGroupingEnabled = true;
            return 1;
        }
    }
}
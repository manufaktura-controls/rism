namespace Manufaktura.LibraryStandards.PlaineAndEasie
{
    public class PlaineAndEasieFermataOrTupleParsingStrategy : PlaineAndEasieParsingStrategy
    {
        public override int ControlSignLength => 0;

        public override bool IsRelevant(string s) => s[0] == ')';

        public override int Parse(PlaineAndEasieParser parser, string s)
        {
            if (parser.GroupSize == 1) parser.AddFermata();
            else parser.MakeTuple();

            parser.IsGroupingEnabled = false;
            parser.GroupSize = 0;

            return 1;
        }
    }
}
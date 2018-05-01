namespace Manufaktura.LibraryStandards.PlaineAndEasie
{
    public class PlaineAndEasieBeamParsingStrategy : PlaineAndEasieParsingStrategy
    {
        public override int ControlSignLength => 0;

        public override bool IsRelevant(string s) => s[0] == '{' || s[0] == '}';

        public override int Parse(PlaineAndEasieParser parser, string s)
        {
            if (s[0] == '{') parser.IsBeamingEnabled = true;
            else
            {
                parser.IsBeamingEnabled = false;
                parser.OnBeamingEnded();
            }
            return 1;
        }
    }
}
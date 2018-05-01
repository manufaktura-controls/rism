namespace Manufaktura.LibraryStandards.PlaineAndEasie
{
    public class PlaineAndEasieWholeMeasureRestParsingStrategy : PlaineAndEasieParsingStrategy
    {
        public override int ControlSignLength => 1;

        public override bool IsRelevant(string s) => s[0] == '=';

        public override int Parse(PlaineAndEasieParser parser, string s)
        {
            var length = s.IndexOf("/");
            if (length < 0) return 0;

            var numberOfMeasures = 0;
            if (length > 0)
                int.TryParse(s.Substring(0, length), out numberOfMeasures);

            parser.AddWholeMeasureRests(numberOfMeasures);

            return length;
        }
    }
}
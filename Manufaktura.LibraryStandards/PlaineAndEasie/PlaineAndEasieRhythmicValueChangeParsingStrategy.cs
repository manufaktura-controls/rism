namespace Manufaktura.LibraryStandards.PlaineAndEasie
{
    public class PlaineAndEasieRhythmicValueChangeParsingStrategy : PlaineAndEasieParsingStrategy
    {
        public override int ControlSignLength => 0;

        public override bool IsRelevant(string s) => int.TryParse(s[0].ToString(), out _) && !s.StartsWith("7.");   //7. is neumatic notation

        public override int Parse(PlaineAndEasieParser parser, string s)
        {
            var dots = 0;
            for (var i = 1; i < s.Length; i++)
            {
                if (s[i] == '.') dots++;
                else break;
            }
            parser.CurrentNumberOfDots = dots;
            var rismRhythmicValue = int.Parse(s[0].ToString());
            parser.CurrentRhythmicLogValue = DetermineRhythmicLogValue(rismRhythmicValue);

            return 1 + dots;
        }

        private static int DetermineRhythmicLogValue(int rismRhythmicValue)
        {
            switch (rismRhythmicValue)
            {
                case 0: return -2;
                case 1: return 0;
                case 2: return 1;
                case 3: return 5;
                case 4: return 2;
                case 5: return 6;
                case 6: return 4;
                case 7: return 7;
                case 8: return 3;
                case 9: return -1;
                default: return 2;
            }
        }
    }
}
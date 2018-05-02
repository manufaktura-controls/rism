using System.Linq;

namespace Manufaktura.LibraryStandards.PlaineAndEasie
{
    public class PlaineAndEasieTimeSignatureParsingStrategy : PlaineAndEasieParsingStrategy
    {
        private readonly string[] specialSignatures = new[] { "c", "c/" };

        public override int ControlSignLength => 1;

        public override bool IsRelevant(string s) => s[0] == '@';

        public override int Parse(PlaineAndEasieParser parser, string s)
        {
            var matchingSignature = specialSignatures.OrderByDescending(ss => s.Length).FirstOrDefault(ss => s.StartsWith(ss));
            if (matchingSignature != null)
            {
                parser.AddTimeSignature(matchingSignature, 0, 0);
                return matchingSignature.Length;
            }

            var length = DetermineLength(s);
            var substring = s.Substring(0, length);
            var parts = substring.Split('/') ?? new string[0];

            if (parts.Length != 2)
            {
                parser.AddTimeSignature(substring, 0, 0);
            }
            else
            {
                if (!int.TryParse(parts[0], out int numerator) || !int.TryParse(parts[1], out int denominator))
                {
                    parser.AddTimeSignature(substring, 0, 0);
                }
                else
                {
                    parser.AddTimeSignature(null, numerator, denominator);
                }
            }

            return length;
        }

        private static int DetermineLength(string s)
        {
            var length = 0;
            for (var i = 0; i < s.Length; i++)
            {
                if (s[i] != 'c' && s[i] != 'o' && s[i] != '.' && s[i] != '/' && !int.TryParse(s[i].ToString(), out _)) break;
                length++;
            }
            return length;
        }
    }
}
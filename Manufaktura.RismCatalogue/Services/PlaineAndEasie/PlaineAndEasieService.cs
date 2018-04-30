using Manufaktura.Controls.Model;
using Manufaktura.RismCatalogue.Model;
using Manufaktura.RismCatalogue.Services.PlaineAndEasie;

namespace Manufaktura.RismCatalogue.Services
{
    public class PlaineAndEasieService
    {
        public Score Parse(Incipit incipit)
        {
            var score = new Score();
            score.Staves.Add(new Staff());
            new PlaineAndEasie2ScoreClefParsingStrategy().Parse(incipit.Clef, score);
            score.FirstStaff.Add(ParseKey(incipit.KeySignature));
            score.FirstStaff.Add(ParseTimeSignature(incipit.TimeSignature));

            //TODO: Parse notes

            return score;
        }

        private static Key ParseKey(string keySignature)
        {
            keySignature = keySignature?.Trim();
            if (string.IsNullOrWhiteSpace(keySignature) || keySignature.Length <= 1) return new Key(0);
            var modifier = keySignature.StartsWith("b") ? -1 : 1;
            return new Key(keySignature.Length * modifier);
        }

        private static TimeSignature ParseTimeSignature(string peTimeSignature)
        {
            peTimeSignature = peTimeSignature?.Trim();
            if (peTimeSignature == "c") return TimeSignature.CommonTime;
            if (peTimeSignature == "c/") return TimeSignature.CutTime;
            var parts = peTimeSignature?.Split("/") ?? new string[0];
            if (parts.Length != 2) return TimeSignature.CommonTime;
            if (!int.TryParse(parts[0], out int numerator)) return TimeSignature.CommonTime;
            if (!int.TryParse(parts[1], out int denominator)) return TimeSignature.CommonTime;
            return new TimeSignature(TimeSignatureType.Numbers, numerator, denominator);
        }
    }
}
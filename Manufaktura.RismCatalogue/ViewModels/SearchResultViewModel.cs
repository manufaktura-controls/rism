using Manufaktura.Controls.Model;
using Manufaktura.RismCatalogue.Model;

namespace Manufaktura.RismCatalogue.ViewModels
{
    public class SearchResultViewModel
    {
        public SearchResultViewModel(Incipit incipit) //TODO: Will be constructed from MusicalSource
        {
            var score = new Score();
            score.Staves.Add(new Staff());
            score.FirstStaff.Add(ParseClef(incipit.Clef));
            score.FirstStaff.Add(ParseKey(incipit.KeySignature));
            score.FirstStaff.Add(ParseTimeSignature(incipit.TimeSignature));
            FirstIncipit = score;
        }

        public Score FirstIncipit { get; set; }

        private static Clef ParseClef(string peClef)
        {
            var parts = peClef?.Split("-") ?? new string[0];
            if (parts.Length != 2) return Clef.Treble;

            var clefType = ParseClefType(parts[0]);
            if (!int.TryParse(parts[1], out int lineNumber))
            {
                if (clefType == ClefType.CClef) return Clef.Tenor;
                if (clefType == ClefType.FClef) return Clef.Bass;
                if (clefType == ClefType.GClef) return Clef.Treble;
            }
            return new Clef(clefType, lineNumber);
        }

        private static ClefType ParseClefType(string peClefType)
        {
            switch (peClefType)
            {
                case "C": return ClefType.CClef;
                case "F": return ClefType.FClef;
                case "G": return ClefType.GClef;
                default: return ClefType.GClef;
            }
        }

        private static Key ParseKey(string keySignature)
        {
            keySignature = keySignature.Trim();
            if (string.IsNullOrWhiteSpace(keySignature) || keySignature.Length <= 1) return new Key(0);
            var modifier = keySignature.StartsWith("b") ? -1 : 1;
            return new Key(keySignature.Length * modifier);
        }

        private static TimeSignature ParseTimeSignature(string peTimeSignature)
        {
            peTimeSignature = peTimeSignature.Trim();
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
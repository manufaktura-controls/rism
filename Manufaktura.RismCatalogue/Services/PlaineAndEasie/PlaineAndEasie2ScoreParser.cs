using Manufaktura.Controls.Model;
using Manufaktura.LibraryStandards.PlaineAndEasie;

namespace Manufaktura.RismCatalogue.Services.PlaineAndEasie
{
    public class PlaineAndEasie2ScoreParser : PlaineAndEasieParser<Score>
    {
        protected override void AddClef(char clefType, int lineNumber, bool isMensural)
        {
            var scoreClefType = ParseClefType(clefType);
            var octaveShift = clefType == 'g' ? -1 : 0;
            output.FirstStaff.Add(new Clef(scoreClefType, lineNumber, octaveShift));
        }

        protected override void AddKey(int numberOfFifths)
        {
            output.FirstStaff.Add(new Key(numberOfFifths));
        }

        protected override void AddTimeSignature(string symbol, int numerator, int denominator)
        {
            if (symbol == "c") output.FirstStaff.Add(TimeSignature.CommonTime);
            else if (symbol == "c/") output.FirstStaff.Add(TimeSignature.CutTime);
            else output.FirstStaff.Add(new TimeSignature(TimeSignatureType.Numbers, numerator, denominator));
        }

        protected override Score CreateOutputObject()
        {
            var score = new Score();
            score.Staves.Add(new Staff());
            return score;
        }

        private static ClefType ParseClefType(char peClefType)
        {
            switch (peClefType)
            {
                case 'C': return ClefType.CClef;
                case 'F': return ClefType.FClef;
                case 'G': return ClefType.GClef;
                case 'g': return ClefType.GClef;
                default: return ClefType.GClef;
            }
        }
    }
}
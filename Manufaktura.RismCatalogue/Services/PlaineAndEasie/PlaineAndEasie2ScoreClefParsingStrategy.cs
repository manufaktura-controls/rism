using Manufaktura.Controls.Model;
using Manufaktura.LibraryStandards.PlaineAndEasie;

namespace Manufaktura.RismCatalogue.Services.PlaineAndEasie
{
    public class PlaineAndEasie2ScoreClefParsingStrategy : PlaineAndEasieClefParsingStrategy<Score>
    {
        protected override void ParseInternal(Score output, char clefType, int lineNumber, bool isMensural)
        {
            var scoreClefType = ParseClefType(clefType);
            var octaveShift = clefType == 'g' ? -1 : 0;
            output.FirstStaff.Add(new Clef(scoreClefType, lineNumber, octaveShift));
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
using Manufaktura.Controls.Model;
using Manufaktura.LibraryStandards.PlaineAndEasie;

namespace Manufaktura.RismCatalogue.Services.PlaineAndEasie
{
    public class PlaineAndEasie2ScoreKeyParsingStrategy : PlaineAndEasieKeyParsingStrategy<Score>
    {
        protected override void ParseInternal(Score output, int numberOfFifths)
        {
            output.FirstStaff.Add(new Key(numberOfFifths));
        }
    }
}
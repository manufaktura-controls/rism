using Manufaktura.Controls.Model;
using Manufaktura.RismCatalogue.Common.Services.PlaineAndEasie;
using Manufaktura.RismCatalogue.Model;

namespace Manufaktura.RismCatalogue.Common.Services
{
    public class PlaineAndEasieService
    {
        public Score Parse(Incipit incipit)
        {
            if (string.IsNullOrWhiteSpace(incipit.MusicalNotation)) return null;

            var parser = new PlaineAndEasie2ScoreParser();
            var score = parser.Parse(incipit.Clef, incipit.KeySignature, incipit.TimeSignature, incipit.MusicalNotation);
            return score;
        }
    }
}
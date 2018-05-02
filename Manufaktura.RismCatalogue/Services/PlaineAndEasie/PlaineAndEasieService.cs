using Manufaktura.Controls.Model;
using Manufaktura.RismCatalogue.Model;
using Manufaktura.RismCatalogue.Services.PlaineAndEasie;
using System.Text;

namespace Manufaktura.RismCatalogue.Services
{
    public class PlaineAndEasieService
    {
        public Score Parse(Incipit incipit)
        {
            if (string.IsNullOrWhiteSpace(incipit.MusicalNotation)) return null;

            var parser = new PlaineAndEasie2ScoreParser();
            var sb = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(incipit.Clef)) sb.Append($"%{incipit.Clef}");
            if (!string.IsNullOrWhiteSpace(incipit.KeySignature)) sb.Append($"${incipit.KeySignature}");
            if (!string.IsNullOrWhiteSpace(incipit.TimeSignature)) sb.Append($"@{incipit.TimeSignature}");
            sb.Append(incipit.MusicalNotation);

            var score = parser.Parse(sb.ToString());
            return score;
        }
    }
}
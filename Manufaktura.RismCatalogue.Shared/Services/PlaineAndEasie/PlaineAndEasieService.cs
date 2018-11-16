using Manufaktura.Controls.Model;
using Manufaktura.Controls.Primitives;
using Manufaktura.RismCatalogue.Model;
using Manufaktura.RismCatalogue.Shared.Services.PlaineAndEasie;
using System.Linq;

namespace Manufaktura.RismCatalogue.Shared.Services
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

        public Score ParseAndColorMatchingIntervals(Incipit incipit, int[] intervals)
        {
            var score = Parse(incipit);
            var notes = score.FirstStaff.Elements.OfType<Note>().Take(intervals.Length + 1).ToArray();
            if (notes.Length < 2) return score;

            var previousPitch = notes[0].Pitch;
            notes[0].CustomColor = new Color(0, 197, 144, 1);
            for (var i = 0; i + 1 < notes.Length && i < intervals.Length; i++)
            {
                notes[i + 1].CustomColor = notes[i + 1].Pitch.MidiPitch - previousPitch.MidiPitch == intervals[i] ? new Color(0, 197, 144, 1) : new Color(252, 61, 50, 1);
                previousPitch = notes[i + 1].Pitch;
            }
            return score;
        }
    }
}
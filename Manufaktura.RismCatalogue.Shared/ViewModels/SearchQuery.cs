using Manufaktura.Music.Model;
using Manufaktura.RismCatalogue.Shared.Serialization;
using Newtonsoft.Json;

namespace Manufaktura.RismCatalogue.Shared.ViewModels
{
    public class SearchQuery
    {
        [JsonConverter(typeof(PitchConverter))]
        public Pitch[] Pitches { get; set; }

        public RhythmicDuration[] RhythmicDurations { get; set; }
    }
}
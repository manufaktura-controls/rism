using Manufaktura.Music.Model;
using Manufaktura.RismCatalogue.Shared.ViewModels;
using Newtonsoft.Json;
using Xunit;

namespace Manufaktura.Rism.Tests
{
    public class SerializationTests
    {
        [Fact]
        public void SearchQuerySerializationTest()
        {
            var query = new SearchQuery()
            {
                Pitches = new Pitch[] { Pitch.C4, Pitch.E4, Pitch.G4 },
                RhythmicDurations = new RhythmicDuration[] { RhythmicDuration.Quarter, RhythmicDuration.Eighth.AddDots(1), RhythmicDuration.Sixteenth }
            };
            var serializedQuery = JsonConvert.SerializeObject(query);

            var deserializedQuery = JsonConvert.DeserializeObject<SearchQuery>(serializedQuery);

            for (var i = 0; i < query.Pitches.Length; i++)
            {
                Assert.Equal(query.Pitches[i], deserializedQuery.Pitches[i]);
            }
        }
    }
}
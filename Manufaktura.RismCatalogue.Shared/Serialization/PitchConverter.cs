using Manufaktura.Music.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Manufaktura.RismCatalogue.Shared.Serialization
{
    public class PitchConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(Pitch[]);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var array = JArray.Parse((string)reader.Value);
            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var pitches = value as Pitch[];
            writer.WriteStartArray();
            foreach (var pitch in pitches) writer.WriteValue(pitch.MidiPitch);
            writer.WriteEndArray();
        }
    }
}

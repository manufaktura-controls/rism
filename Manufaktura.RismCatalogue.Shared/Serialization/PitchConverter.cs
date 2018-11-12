using Manufaktura.Music.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Manufaktura.RismCatalogue.Shared.Serialization
{
    public class PitchConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(Pitch[]);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var pitches = new List<Pitch>();
            while (reader.TokenType != JsonToken.EndArray)
            {
                var midiPitch = reader.ReadAsInt32();
                if (!midiPitch.HasValue) continue;
                pitches.Add(Pitch.FromMidiPitch(midiPitch.Value, Pitch.MidiPitchTranslationMode.Auto));
            }
            return pitches.ToArray();
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
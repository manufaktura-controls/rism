using Manufaktura.Music.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Manufaktura.RismCatalogue.Shared.Serialization
{
    public class RhythmicDurationConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(RhythmicDuration[]);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var durations = value as RhythmicDuration[];

        }
    }
}

using System;
using Newtonsoft.Json;

namespace rosette_api {

    /// <summary>
    /// JsonConverter for Unfielded Record Similarity objects
    /// </summary>
    public class UnfieldedRecordSimilarityConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
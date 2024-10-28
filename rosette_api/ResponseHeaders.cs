using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace rosette_api
{
    /// <summary>
    /// A class to represent the response headers returned by the Analytics API
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    [JsonConverter(typeof(ResponseHeadersConverter))]
    public class ResponseHeaders
    {
        internal const string responseHeadersKey = "responseHeaders";
        internal const string contentTypeKey = "content-type";
        internal const string dateKey = "date";
        internal const string serverKey = "server";
        internal const string strictTransportSecurityKey = "strict-transport-security";
        internal const string xRosetteAPIAppIDKey = "x-rosetteapi-app-id";
        internal const string xRosetteAPIConcurrencyKey = "x-rosetteapi-concurrency";
        internal const string xRosetteAPIProcessedLanguageKey = "x-rosetteapi-processedlanguage";
        internal const string xRosetteAPIRequestIDKey = "x-rosetteapi-request-id";
        internal const string contentLengthKey = "content-length";
        internal const string connectionKey = "connection";

        /// <summary>
        /// The collection of all response headers returned by the Analytics API
        /// </summary>
        [JsonProperty("allResponseHeaders")]
        public IDictionary<string, string> AllResponseHeaders;

        /// <summary>
        /// The DateTime of the response
        /// </summary>
        [JsonIgnore]
        public string Date;

        /// <summary>
        /// The language in which the API processed the input text
        /// </summary>
        [JsonIgnore]
        public string XRosetteAPIProcessedLanguage;

        /// <summary>
        /// Creaqtes a ResponseHeaders object from the given headers
        /// </summary>
        /// <param name="headers">The headers from the API</param>
        public ResponseHeaders(IDictionary<string, string> headers)
        {
            if (headers.ContainsKey(dateKey))
            {
                this.Date = headers[dateKey];
            }
            if (headers.ContainsKey(xRosetteAPIProcessedLanguageKey))
            {
                this.XRosetteAPIProcessedLanguage = headers[xRosetteAPIProcessedLanguageKey];
            }
            this.AllResponseHeaders = headers;
        }

        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj">The object to compare against</param>
        /// <returns>True if equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is ResponseHeaders)
            {
                ResponseHeaders other = obj as ResponseHeaders;
                List<bool> conditions = new List<bool>() {
                    this.AllResponseHeaders.Count == other.AllResponseHeaders.Count &! this.AllResponseHeaders.Except(other.AllResponseHeaders).Any()
                };
                return conditions.All(condition => condition);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// HashCode override
        /// </summary>
        /// <returns>The hashcode</returns>
        public override int GetHashCode()
        {
            return this.AllResponseHeaders.Aggregate<KeyValuePair<string, string>, int>(1, (seed, kvp) => seed ^ kvp.GetHashCode());
        }

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns>The response headers in JSON form</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this.AllResponseHeaders);
        }
    }

    internal class ResponseHeadersConverter : JsonConverter
    {

        public override bool CanConvert(Type objectType)
        {
            return objectType.Equals(typeof(ResponseHeaders));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize(reader, objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            ResponseHeaders responseHeadersObj = value as ResponseHeaders;
            serializer.Serialize(writer, responseHeadersObj.AllResponseHeaders);
        }
    }
}
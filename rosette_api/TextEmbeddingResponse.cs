using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace rosette_api
{
    /// <summary>
    /// Class for representing responses from the API when the TextEmbedding endpoint has been called
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class TextEmbeddingResponse : RosetteResponse
    {
        /// <summary>
        /// Gets the averaged text vector
        /// </summary>
        [JsonProperty(embeddingKey)]
        public IEnumerable<double> TextEmbedding { get; set; }

        private const String embeddingKey = "embedding";

        /// <summary>
        /// Creates a TextEmbeddingResponse from the API's raw output
        /// </summary>
        /// <param name="apiResult">The API's output</param>
        public TextEmbeddingResponse(HttpResponseMessage apiResult)
            : base(apiResult)
        {
            List<double> textEmbedding = new List<double>();
            JArray enumerableResults = this.ContentDictionary.ContainsKey(embeddingKey) ? this.ContentDictionary[embeddingKey] as JArray : new JArray();
            foreach (JValue result in enumerableResults)
            {
                textEmbedding.Add(result.ToObject<double>());
            }
            this.TextEmbedding = textEmbedding;
        }

        /// <summary>
        /// Constructs a TextEmbedding Response from a text embedding, a collection of response headers, and content in a dictionary or content as JSON
        /// </summary>
        /// <param name="textEmbedding">The averaged text vector (text embedding)</param>
        /// <param name="responseHeaders">The response headers from the API</param>
        /// <param name="content">The content of the response (i.e. the textEmbedding list)</param>
        /// <param name="contentAsJson">The content as a JSON string</param>
        public TextEmbeddingResponse(IEnumerable<double> textEmbedding, Dictionary<string, string> responseHeaders, Dictionary<string, object> content = null, String contentAsJson = null)
            : base(responseHeaders, content, contentAsJson)
        {
            this.TextEmbedding = textEmbedding;
        }

        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns>True if equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is TextEmbeddingResponse)
            {
                TextEmbeddingResponse other = obj as TextEmbeddingResponse;
                List<bool> conditions = new List<bool>() {
                    this.TextEmbedding != null && other.TextEmbedding != null ? this.TextEmbedding.SequenceEqual(other.TextEmbedding) : this.TextEmbedding == other.TextEmbedding,
                    this.ResponseHeaders != null && other.ResponseHeaders != null ? this.ResponseHeaders.Equals(other.ResponseHeaders) : this.ResponseHeaders == other.ResponseHeaders
                };
                return conditions.All(condition => condition);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Hashcode override
        /// </summary>
        /// <returns>The hashcode</returns>
        public override int GetHashCode()
        {
            int h0 = this.TextEmbedding != null ? this.TextEmbedding.Aggregate<double, int>(1, (seed, item) => seed ^ item.GetHashCode()) : 1;
            int h1 = this.ResponseHeaders != null ? this.ResponseHeaders.GetHashCode() : 1;
            return h0 ^ h1;
        }
    }
}

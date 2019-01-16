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
    /// Class for representing responses from the API when the SemanticVectors endpoint has been called
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class SemanticVectorsResponse : RosetteResponse
    {
        /// <summary>
        /// Gets the averaged text vector
        /// </summary>
        [JsonProperty(embeddingKey)]
        public IEnumerable<double> TextEmbedding { get; set; }

        /// <summary>
        /// Lists the tokens in the document
        /// </summary>
        [JsonProperty(tokenKey)]
        public IEnumerable<string> tokens { get; set; }

        /// <summary>
        /// Lists the token embeddings
        /// </summary>
        [JsonProperty(tokenEmbeddingsKey)]
        public IEnumerable<List<double>> tokenEmbeddings { get; set; }

        private const String embeddingKey = "documentEmbedding";
        private const String tokenKey = "tokens";
        private const String tokenEmbeddingsKey = "tokenEmbeddings";

        /// <summary>
        /// Creates a SemanticVectorsResponse from the API's raw output
        /// </summary>
        /// <param name="apiResult">The API's output</param>
        public SemanticVectorsResponse(HttpResponseMessage apiResult)
            : base(apiResult)
        {
            List<double> textEmbedding = new List<double>();
            JArray enumerableResults = this.ContentDictionary.ContainsKey(embeddingKey) ? this.ContentDictionary[embeddingKey] as JArray : new JArray();
            foreach (JValue result in enumerableResults)
            {
                textEmbedding.Add(result.ToObject<double>());
            }
            JArray tokensArr = this.ContentDictionary.ContainsKey(tokenKey) ? this.ContentDictionary[tokenKey] as JArray : null;
            List<string> tokens = tokensArr != null ? new List<string>(tokensArr.Select((jToken) => jToken?.ToString())) : null;
            JArray tokenEmbeddingsArr = this.ContentDictionary.ContainsKey(tokenEmbeddingsKey) ? this.ContentDictionary[tokenEmbeddingsKey] as JArray : null;
            List<List<double>> tokenEmbeddings = tokenEmbeddingsArr != null ? new List<List<double>>(tokenEmbeddingsArr.Select<JToken, List<double>>((jToken) => jToken?.ToObject<List<double>>())) : null;
            this.TextEmbedding = textEmbedding;
            this.tokens = tokens;
            this.tokenEmbeddings = tokenEmbeddings;
        }

        /// <summary>
        /// Constructs a SemanticVectors Response from a text embedding, a collection of response headers, and content in a dictionary or content as JSON
        /// </summary>
        /// <param name="textEmbedding">The averaged text vector (text embedding)</param>
        /// <param name="tokens">The tokens from the document</param>
        /// <param name="tokenEmbeddings">The embeddings for each token</param>
        /// <param name="responseHeaders">The response headers from the API</param>
        /// <param name="content">The content of the response (i.e. the textEmbedding list)</param>
        /// <param name="contentAsJson">The content as a JSON string</param>
        public SemanticVectorsResponse(IEnumerable<double> textEmbedding, IEnumerable<string> tokens, IEnumerable<List<double>> tokenEmbeddings,
            Dictionary<string, string> responseHeaders, Dictionary<string, object> content = null, String contentAsJson = null)
            : base(responseHeaders, content, contentAsJson)
        {
            this.TextEmbedding = textEmbedding;
            this.tokens = tokens;
            this.tokenEmbeddings = tokenEmbeddings;
        }

        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns>True if equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is SemanticVectorsResponse)
            {
                SemanticVectorsResponse other = obj as SemanticVectorsResponse;
                List<bool> conditions = new List<bool>() {
                    this.TextEmbedding != null && other.TextEmbedding != null ? this.TextEmbedding.SequenceEqual(other.TextEmbedding) : this.TextEmbedding == other.TextEmbedding,
                    this.tokens != null && other.tokens != null ? this.tokens.SequenceEqual(other.tokens) : this.tokens == other.tokens,
                    this.tokenEmbeddings != null && other.tokenEmbeddings != null ? this.tokenEmbeddings.Any(a => other.tokenEmbeddings.Any(b => a.SequenceEqual(b))) : this.tokenEmbeddings == other.tokenEmbeddings,
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
            int h0 = this.ResponseHeaders != null ? this.ResponseHeaders.GetHashCode() : 1;
            int h1 = this.TextEmbedding != null ? this.TextEmbedding.Aggregate<double, int>(1, (seed, item) => seed ^ item.GetHashCode()) : 1;
            int h2 = this.tokens != null ? this.tokens.GetHashCode() : 1;
            int h3 = this.tokenEmbeddings != null ? this.tokenEmbeddings.Aggregate<List<double>, int>(1, (seed, item) => seed ^ item.GetHashCode()) : 1;
            return h0 ^ h1 ^ h2 ^ h3;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http;
using System.Collections;

namespace rosette_api
{
    /// <summary>
    /// A class to represent responses from the Sentence Tagging endpoint of the Analytics API
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class SentenceTaggingResponse : RosetteResponse
    {
        private const string sentencesKey ="sentences";
   
        /// <summary>
        /// Gets or sets the sentences identified by the Analytics API
        /// </summary>
        [JsonProperty(sentencesKey)]
        public List<String> Sentences { get; set; }

        /// <summary>
        /// Creates a SentenceTaggingResponse from the given apiResults
        /// </summary>
        /// <param name="apiResults">The message from the API</param>
        public SentenceTaggingResponse(HttpResponseMessage apiResults) :base(apiResults)
        {
            JArray sentenceArr = this.ContentDictionary.ContainsKey(sentencesKey) ? this.ContentDictionary[sentencesKey] as JArray : new JArray();
            this.Sentences = new List<string>(sentenceArr.Select<JToken, string>((jsonToken) => jsonToken.ToString()));
        }

        /// <summary>
        /// Creates a SentenceTaggingResponse from its components
        /// </summary>
        /// <param name="sentences"></param>
        /// <param name="responseHeaders"></param>
        /// <param name="content"></param>
        /// <param name="contentAsJson"></param>
        public SentenceTaggingResponse(List<string> sentences, Dictionary<string, string> responseHeaders, Dictionary<string, object> content, string contentAsJson)
            : base(responseHeaders, content, contentAsJson)
        {
            this.Sentences = sentences;
        }

        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj">The object to compare against</param>
        /// <returns>True if equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is SentenceTaggingResponse)
            {
                SentenceTaggingResponse other = obj as SentenceTaggingResponse;
                List<bool> conditions = new List<bool>() {
                    this.Sentences != null && other.Sentences != null ? this.Sentences.SequenceEqual(other.Sentences) : this.Sentences == other.Sentences,
                    this.ResponseHeaders != null && other.ResponseHeaders != null ? this.ResponseHeaders.Equals(other.ResponseHeaders) : this.ResponseHeaders == other.ResponseHeaders,
                    this.GetHashCode() == other.GetHashCode()
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
            int h0 = this.Sentences != null ? this.Sentences.Aggregate<string, int>(1, (seed, item) => seed ^ item.GetHashCode()) : 1;
            int h1 = this.ResponseHeaders != null ? this.ResponseHeaders.GetHashCode() : 1;
            return h0 ^ h1;
        }
    }
}
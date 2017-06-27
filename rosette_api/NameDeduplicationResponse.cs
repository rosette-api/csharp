using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace rosette_api {
    /// <summary>
    /// A class to represent the results from the Name Deduplication endpoint of the Rosette API
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class NameDeduplicationResponse : RosetteResponse {
        private const string nameDeduplicationKey = "results";

        /// <summary>
        /// The transliterated string
        /// </summary>
        [JsonProperty(nameDeduplicationKey)]
        public List<string> Results;

        /// <summary>
        /// Creates a NameDeduplicationResponse from the given apiResults
        /// </summary>
        /// <param name="apiResults">The message from the API</param>
        public NameDeduplicationResponse(HttpResponseMessage apiResults) : base(apiResults) {
            if (this.ContentDictionary.ContainsKey(nameDeduplicationKey)) {
                this.Results = this.ContentDictionary[nameDeduplicationKey] as List<string>;
                JArray resultsArr = this.ContentDictionary.ContainsKey(nameDeduplicationKey) ? this.ContentDictionary[nameDeduplicationKey] as JArray : null;
                this.Results = resultsArr != null ? new List<string>(resultsArr.Select<JToken, string>((jToken) => jToken?.ToString())) : null;

            }
        }

        /// <summary>
        /// Creates a NameDeduplicationResponse from its headers
        /// </summary>
        /// <param name="responseHeaders">The response headers from the API</param>
        /// <param name="content">The content of the response (the score) in dictionary form</param>
        /// <param name="contentAsJSON">The content in JSON</param>
        public NameDeduplicationResponse(Dictionary<string, string> responseHeaders, Dictionary<string, object> content, string contentAsJSON) : base(responseHeaders, content, contentAsJSON) {
            if (this.ContentDictionary.ContainsKey(nameDeduplicationKey)) {
                this.Results = this.ContentDictionary[nameDeduplicationKey] as List<string>;
                JArray resultsArr = this.ContentDictionary.ContainsKey(nameDeduplicationKey) ? this.ContentDictionary[nameDeduplicationKey] as JArray : null;
                this.Results = resultsArr != null ? new List<string>(resultsArr.Select<JToken, string>((jToken) => jToken?.ToString())) : null;

            }
        }

        /// <summary>
        /// Equals override.
        /// </summary>
        /// <param name="obj">The object to compare against</param>
        /// <returns>True if equal.</returns>
        public override bool Equals(object obj) {
            if (obj is NameDeduplicationResponse) {
                NameDeduplicationResponse other = obj as NameDeduplicationResponse;
                return this.Results == other.Results && this.ResponseHeaders.Equals(other.ResponseHeaders);
            }
            else {
                return false;
            }
        }

        /// <summary>
        /// HashCode override
        /// </summary>
        /// <returns>The hashcode</returns>
        public override int GetHashCode() {
            return this.Results.GetHashCode() ^ this.ResponseHeaders.GetHashCode();
        }
    }
}


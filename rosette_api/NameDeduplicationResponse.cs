using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net.Http;

namespace rosette_api {
    /// <summary>
    /// A class to represnt the results from the Name Deduplication endpoint of the Rosette API
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class NameDeduplicationResponse : RosetteResponse {
        private List<string> _results = new List<string>();

        /// <summary>
        /// Creates a NameDeduplicationResponse from the given apiResults
        /// </summary>
        /// <param name="apiResults">The message from the API</param>
        public NameDeduplicationResponse(HttpResponseMessage apiResults) : base(apiResults) {
            if (this.ContentDictionary.ContainsKey("results")) {
                this._results = this.ContentDictionary["results"] as List<string>;
            }
        }

        /// <summary>
        /// Creates a NameDeduplicationResponse from its headers
        /// </summary>
        /// <param name="responseHeaders">The response headers from the API</param>
        /// <param name="content">The content of the response (the score) in dictionary form</param>
        /// <param name="contentAsJSON">The content in JSON</param>
        public NameDeduplicationResponse(Dictionary<string, string> responseHeaders, Dictionary<string, object> content, string contentAsJSON) : base(responseHeaders, content, contentAsJSON) {
            if (this.ContentDictionary.ContainsKey("results")) {
                this._results = this.ContentDictionary["results"] as List<string>;
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
                return this._results == other._results && this.ResponseHeaders.Equals(other.ResponseHeaders);
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
            return this._results.GetHashCode() ^ this.ResponseHeaders.GetHashCode();
        }
    }
}


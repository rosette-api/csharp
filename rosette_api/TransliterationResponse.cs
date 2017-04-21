using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net.Http;

namespace rosette_api {
    /// <summary>
    /// A class to represnt the results from the Name Deduplication endpoint of the Rosette API
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class TransliterationResponse : RosetteResponse {
        private string _transliteration;

        /// <summary>
        /// Creates a TransliterationResponse from the given apiResults
        /// </summary>
        /// <param name="apiResults">The message from the API</param>
        public TransliterationResponse(HttpResponseMessage apiResults) : base(apiResults) {
            if (this.ContentDictionary.ContainsKey("transliteration")) {
                this._transliteration = this.ContentDictionary["transliteration"] as string;
            }
        }

        /// <summary>
        /// Creates a TransliterationResponse from its headers
        /// </summary>
        /// <param name="responseHeaders">The response headers from the API</param>
        /// <param name="content">The content of the response (the score) in dictionary form</param>
        /// <param name="contentAsJSON">The content in JSON</param>
        public TransliterationResponse(Dictionary<string, string> responseHeaders, Dictionary<string, object> content, string contentAsJSON) : base(responseHeaders, content, contentAsJSON) {
            if (this.ContentDictionary.ContainsKey("transliteration")) {
                this._transliteration = this.ContentDictionary["transliteration"] as string;
            }
        }

        /// <summary>
        /// Equals override.
        /// </summary>
        /// <param name="obj">The object to compare against</param>
        /// <returns>True if equal.</returns>
        public override bool Equals(object obj) {
            if (obj is TransliterationResponse) {
                TransliterationResponse other = obj as TransliterationResponse;
                return this._transliteration == other._transliteration && this.ResponseHeaders.Equals(other.ResponseHeaders);
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
            return this._transliteration.GetHashCode() ^ this.ResponseHeaders.GetHashCode();
        }
    }
}


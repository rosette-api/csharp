using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net.Http;

namespace rosette_api {
    /// <summary>
    /// A class to represent the results from the Transliteration endpoint of the Analytics API
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class TransliterationResponse : RosetteResponse {
        private const string transliterationKey = "transliteration";

        /// <summary>
        /// The transliterated string
        /// </summary>
        [JsonProperty(transliterationKey)]
        public string Transliteration;

        /// <summary>
        /// Creates a TransliterationResponse from the given apiResults
        /// </summary>
        /// <param name="apiResults">The message from the API</param>
        public TransliterationResponse(HttpResponseMessage apiResults) : base(apiResults) {
            if (this.ContentDictionary.ContainsKey("transliteration")) {
                this.Transliteration = this.ContentDictionary["transliteration"] as string;
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
                this.Transliteration = this.ContentDictionary["transliteration"] as string;
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
                return this.Transliteration == other.Transliteration && this.ResponseHeaders.Equals(other.ResponseHeaders);
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
            return this.Transliteration.GetHashCode() ^ this.ResponseHeaders.GetHashCode();
        }
    }
}


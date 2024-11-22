using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace rosette_api
{
    /// <summary>
    /// A class to represent responses from the Language Identification endpoint of the Analytics API
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class LanguageIdentificationResponse : RosetteResponse
    {
        private const string languageDetectionsKey = "languageDetections";
        internal const string languageKey = "language";
        internal const string confidenceKey = "confidence";

        /// <summary>
        /// Get the sorted collection of likely languages and their confidence scores
        /// </summary>
        [JsonProperty(languageDetectionsKey)]
        public List<LanguageDetection> LanguageDetections { get; private set; }

        /// <summary>
        /// Creates a LanguageIdentificationResponse from the given apiResults
        /// </summary>
        /// <param name="apiResults">The message from the API</param>
        public LanguageIdentificationResponse(HttpResponseMessage apiResults) : base(apiResults)
        {
            List<LanguageDetection> languageDetections = new List<LanguageDetection>();
            JArray languageDetectionArr = this.ContentDictionary.ContainsKey(languageDetectionsKey) ? this.ContentDictionary[languageDetectionsKey] as JArray : new JArray();
            foreach (JObject languageDetectionObj in languageDetectionArr)
            {
                string language = languageDetectionObj.Properties().Where((p) => String.Equals(p.Name, languageKey, StringComparison.OrdinalIgnoreCase)).Any() ? languageDetectionObj[languageKey].ToString() : null;
                Nullable<decimal> confidence = languageDetectionObj.Properties().Where((p) => String.Equals(p.Name, confidenceKey, StringComparison.OrdinalIgnoreCase)).Any() ? languageDetectionObj[confidenceKey].ToObject<decimal?>() : null;
                languageDetections.Add(new LanguageDetection(language, confidence));
            }
            this.LanguageDetections = languageDetections;
        }
        
        /// <summary>
        /// Creates a LanguageIdentificationResponse from its components
        /// </summary>
        /// <param name="languageDetections">The list of language identifications</param>
        /// <param name="responseHeaders">The response headers</param>
        /// <param name="content">The content (language identifications) in dictionary form</param>
        /// <param name="contentAsJosn">The content in JSON form</param>
        public LanguageIdentificationResponse(List<LanguageDetection> languageDetections, Dictionary<string, string> responseHeaders, Dictionary<string, object> content = null, string contentAsJosn = null) : base(responseHeaders, content, contentAsJosn) {
            this.LanguageDetections = languageDetections;
        }

        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj">The object to compare against</param>
        /// <returns>True if equal</returns>
        public override bool Equals(Object obj)
        {
            if (obj is LanguageIdentificationResponse)
            {
                LanguageIdentificationResponse other = obj as LanguageIdentificationResponse;
                List<bool> conditions = new List<bool>() {
                    this.LanguageDetections != null && other.LanguageDetections != null ? this.LanguageDetections.SequenceEqual(other.LanguageDetections) : this.LanguageDetections == other.LanguageDetections,
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
        /// HashCode override
        /// </summary>
        /// <returns>The hashcode</returns>
        public override int GetHashCode()
        {
            int h0 = this.LanguageDetections != null ? this.LanguageDetections.Aggregate<LanguageDetection, int>(1, (seed, item) => seed ^item.GetHashCode()) : 1;
            int h1 = this.ResponseHeaders != null ? this.ResponseHeaders.GetHashCode() : 1;
            return h0 ^ h1;
        }
    }

    /// <summary>
    /// A struct to represent a detected language and the likelihood it was the correct language to detect
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class LanguageDetection
    {
        /// <summary>
        /// Gets the abbreviated language
        /// </summary>
        [JsonProperty(LanguageIdentificationResponse.languageKey)]
        public string Language { get; private set; }

        /// <summary>
        /// Gets the confidence this was the correct language
        /// </summary>
        [JsonProperty(LanguageIdentificationResponse.confidenceKey)]
        public Nullable<decimal> Confidence { get; private set; }

        /// <summary>
        /// Creates a LanguageDetection, which identifies a language and the confidence that it is the correct language
        /// </summary>
        /// <param name="language">The language in its abbreviated form</param>
        /// <param name="confidence">The confidence the language was the correct language to identify</param>
        public LanguageDetection(string language, Nullable<decimal> confidence)
        {
            this.Language = language;
            this.Confidence = confidence;
        }

        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj">The object to compare against</param>
        /// <returns>True if equal</returns>
        public override bool Equals(Object obj)
        {
            if (obj is LanguageDetection)
            {
                LanguageDetection other = obj as LanguageDetection;
                List<bool> conditions = new List<bool>() {
                    this.Language == other.Language,
                    this.Confidence == other.Confidence
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
            int h0 = this.Language != null ? this.Language.GetHashCode() : 1;
            int h1 = this.Confidence != null ? this.Confidence.GetHashCode() : 1;
            return h0 ^ h1;
        }

        /// <summary>
        /// ToString override:  Gets this object in JSON form
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}

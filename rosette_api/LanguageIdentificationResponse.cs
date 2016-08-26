using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Net.Http;

namespace rosette_api
{
    /// <summary>
    /// A class to represent responses from the Language Identification endpoint of the Rosette API
    /// </summary>
    public class LanguageIdentificationResponse : RosetteResponse
    {
        private const string languageDetectionsKey = "languageDetections";
        private const string languageKey = "language";
        private const string confidenceKey = "confidence";

        /// <summary>
        /// Get the sorted collection of likely languages and their confidence scores
        /// </summary>
        public List<LanguageDetection> LanguageDetections { get; private set; }

        /// <summary>
        /// Gets the response headers returned from the API
        /// </summary>
        public ResponseHeaders ResponseHeaders { get; private set; }

        /// <summary>
        /// Creates a LanguageIdentificationResponse from the given apiResults
        /// </summary>
        /// <param name="apiResults">The message from the API</param>
        public LanguageIdentificationResponse(HttpResponseMessage apiResults) : base(apiResults)
        {
            List<LanguageDetection> languageDetections = new List<LanguageDetection>();
            Dictionary<string, object>[] languageDetectionArr = this.Content.ContainsKey(languageDetectionsKey) ? this.Content[languageDetectionsKey] as Dictionary<string, object>[] : new Dictionary<string, object>[0];
            foreach (Dictionary<string, object> languageDetection in languageDetectionArr)
            {
                string language = languageDetection[languageKey] as String;
                Nullable<double> confidence = languageDetection[confidenceKey] as Nullable<double>;
                languageDetections.Add(new LanguageDetection(language, confidence));
            }
            this.LanguageDetections = languageDetections;
            this.ResponseHeaders = new ResponseHeaders(this.Headers);
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
                    this.LanguageDetections.SequenceEqual(other.LanguageDetections),
                    this.ResponseHeaders.Equals(other.ResponseHeaders)
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
            return this.LanguageDetections.GetHashCode() ^ this.ResponseHeaders.GetHashCode();
        }
    }

    /// <summary>
    /// A struct to represent a detected language and the likelihood it was the correct language to detect
    /// </summary>
    public class LanguageDetection
    {
        /// <summary>
        /// Gets the abbreviated language
        /// </summary>
        public string Language_Abbr { get; private set; }

        /// <summary>
        /// Gets the confidence this was the correct language
        /// </summary>
        public Nullable<double> Confidence { get; private set; }

        /// <summary>
        /// Creates a LanguageDetection, which identifies a language and the confidence that it is the correct language
        /// </summary>
        /// <param name="language_Abbr">The language in its abbreviated form</param>
        /// <param name="confidence">The confidence the language was the correct language to identify</param>
        public LanguageDetection(string language_Abbr, Nullable<double> confidence)
        {
            this.Language_Abbr = language_Abbr;
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
                    this.Language_Abbr == other.Language_Abbr,
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
            return this.Language_Abbr.GetHashCode() ^ this.Confidence.GetHashCode();
        }
    }
}

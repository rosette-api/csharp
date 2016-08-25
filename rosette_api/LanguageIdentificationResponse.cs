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
        private static string languageDetectionsKey = "languageDetections";
        private static string languageKey = "language";
        private static string confidenceKey = "confidence";

        /// <summary>
        /// The sorted collection of likely languages and their confidence scores
        /// </summary>
        List<LanguageDetection> LanguageDetections;

        /// <summary>
        /// The response headers returned from the API
        /// </summary>
        public ResponseHeaders ResponseHeaders;

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
    }

    /// <summary>
    /// A struct to represent a detected language and the likelihood it was the correct language to detect
    /// </summary>
    public struct LanguageDetection
    {
        /// <summary>
        /// The abbreviated language
        /// </summary>
        public string Language_Abbr;

        /// <summary>
        /// The confidence this was the correct language
        /// </summary>
        public Nullable<double> Confidence;

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
    }
}

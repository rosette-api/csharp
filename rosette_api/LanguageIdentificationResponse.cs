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
            object[] languageDetectionArr = this.Content.ContainsKey(languageDetectionsKey) ? this.Content[languageDetectionsKey] as object[] : new object[0];
            foreach (object languageDetectionObj in languageDetectionArr)
            {
                Dictionary<string, object> languageDetection = languageDetectionObj as Dictionary<string, object>;
                string language = languageDetection[languageKey] as String;
                Nullable<decimal> confidence = languageDetection[confidenceKey] as Nullable<decimal>;
                languageDetections.Add(new LanguageDetection(language, confidence));
            }
            this.LanguageDetections = languageDetections;
            this.ResponseHeaders = new ResponseHeaders(this.Headers);
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
            this.ResponseHeaders = new ResponseHeaders(responseHeaders);
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

        /// <summary>
        /// ToString override.  Writes this LanguageIdentificationResponse in JSON form.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "{\"languageDetections\": [" + String.Join<LanguageDetection>(", ", this.LanguageDetections) + "], \"responseHeaders\": " + this.ResponseHeaders.ToString() + "}";
        }

        /// <summary>
        /// Writes this LanguageIdentificationResponse's content in JSON form
        /// </summary>
        /// <returns></returns>
        public string ContentToString()
        {
            return "{\"languageDetections\": [" + String.Join<LanguageDetection>(", ", this.LanguageDetections) + "]}";
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
        public string Language { get; private set; }

        /// <summary>
        /// Gets the confidence this was the correct language
        /// </summary>
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
            return this.Language.GetHashCode() ^ this.Confidence.GetHashCode();
        }

        /// <summary>
        /// ToString override:  Gets this object in JSON form
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "{" + String.Format("\"language\": \"{0}\", \"confidence\": {1}", new Object[]{this.Language, this.Confidence}) + "}";
        }
    }
}

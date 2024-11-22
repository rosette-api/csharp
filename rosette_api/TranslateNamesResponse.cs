using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;

namespace rosette_api
{
    /// <summary>
    /// A class to represent responses from the TranslatedNames endpoint of the Analytics API
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class TranslateNamesResponse : RosetteResponse
    {
        /// <summary>
        /// Gets or sets the type of entity of the name being translated
        /// </summary>
        [JsonProperty(entityTypeKey, NullValueHandling=NullValueHandling.Ignore)]
        public string EntityType { get; set; }
        /// <summary>
        /// Gets or sets the script of the name being translated
        /// </summary>
        [JsonProperty(sourceScriptKey, NullValueHandling = NullValueHandling.Ignore)]
        public string SourceScript { get; set; }
        /// <summary>
        /// Gets or sets the language of origin of the name being translated
        /// </summary>
        [JsonProperty(sourceLanguageOfOriginKey, NullValueHandling = NullValueHandling.Ignore)]
        public string SourceLanguageOfOrigin { get; set; }
        /// <summary>
        /// Gets or sets the language in which the name being translated is written
        /// </summary>
        [JsonProperty(sourceLanguageOfUseKey, NullValueHandling = NullValueHandling.Ignore)]
        public string SourceLanguageOfUse { get; set; }
        /// <summary>
        /// Gets or sets the language to which the name has been translated
        /// </summary>
        [JsonProperty(targetLanguageKey)]
        public string TargetLanguage { get; set; }
        /// <summary>
        /// Gets or sets the script in which the translated name is written
        /// </summary>
        [JsonProperty(targetScriptKey, NullValueHandling = NullValueHandling.Ignore)]
        public string TargetScript { get; set; }
        /// <summary>
        /// Gets or sets the scheme in which the translated name is written
        /// </summary>
        [JsonProperty(targetSchemeKey, NullValueHandling=NullValueHandling.Ignore)]
        public string TargetScheme { get; set; }
        /// <summary>
        /// Gets or sets the translation of the name
        /// </summary>
        [JsonProperty(translationKey)]
        public string Translation { get; set; }
        /// <summary>
        /// Gets or sets the confidence of the translation
        /// </summary>
        [JsonProperty(confidenceKey, NullValueHandling=NullValueHandling.Ignore)]
        public Nullable<double> Confidence { get; set; }

        private const string sourceScriptKey = "sourceScript";
        private const string sourceLanguageOfOriginKey = "sourceLanguageOfOrigin";
        private const string sourceLanguageOfUseKey = "sourceLanguageOfUse";
        private const string translationKey = "translation";
        private const string targetLanguageKey = "targetLanguage";
        private const string targetScriptKey = "targetScript";
        private const string targetSchemeKey = "targetScheme";
        private const string entityTypeKey = "entityType";
        private const string confidenceKey = "confidence";

        /// <summary>
        /// Creates a TranslateNamesResponse from the given apiResult
        /// </summary>
        /// <param name="apiResult">The message from the API</param>
        public TranslateNamesResponse(HttpResponseMessage apiResult) :base(apiResult)
        {
            this.SourceScript = this.ContentDictionary.ContainsKey(sourceScriptKey) ? this.SourceScript = this.ContentDictionary[sourceScriptKey] as string : null;
            this.SourceLanguageOfOrigin = this.ContentDictionary.ContainsKey(sourceLanguageOfOriginKey) ? this.ContentDictionary[sourceLanguageOfOriginKey] as string : null;
            this.SourceLanguageOfUse = this.ContentDictionary.ContainsKey(sourceLanguageOfUseKey) ? this.ContentDictionary[sourceLanguageOfUseKey] as string : null;
            this.Translation = this.ContentDictionary.ContainsKey(translationKey) ? this.ContentDictionary[translationKey] as string : null;
            this.TargetLanguage = this.ContentDictionary.ContainsKey(targetLanguageKey) ? this.ContentDictionary[targetLanguageKey] as string : null;
            this.TargetScript = this.ContentDictionary.ContainsKey(targetScriptKey) ? this.ContentDictionary[targetScriptKey] as string : null;
            this.TargetScheme = this.ContentDictionary.ContainsKey(targetSchemeKey) ? this.ContentDictionary[targetSchemeKey] as string : null;
            this.EntityType = this.ContentDictionary.ContainsKey(entityTypeKey) ? this.ContentDictionary[entityTypeKey] as string : null;
            this.Confidence = this.ContentDictionary.ContainsKey(confidenceKey) ? this.ContentDictionary[confidenceKey] as double? : null;
        }

        /// <summary>
        /// Creates a TranslateNamesResponse from its components
        /// </summary>
        /// <param name="translation">The translation</param>
        /// <param name="targetLanguage">The target language</param>
        /// <param name="targetScheme">The target scheme</param>
        /// <param name="targetScript">The target script</param>
        /// <param name="entityType">The entity type</param>
        /// <param name="sourceLanguageOfOrigin">The source language of origin</param>
        /// <param name="sourceLanguageOfUse">The source language of use</param>
        /// <param name="sourceScript">The source script</param>
        /// <param name="confidence">The confidence</param>
        /// <param name="responseHeaders">The response headers returned from the API</param>
        /// <param name="content">The content in Dictionary form</param>
        /// <param name="contentAsJson">The content in JSON form</param>
        public TranslateNamesResponse(string translation, string targetLanguage, string targetScheme = null, string targetScript = null, string entityType = null, string sourceLanguageOfOrigin = null,
            string sourceLanguageOfUse = null, string sourceScript = null, double? confidence = null, Dictionary<string, string> responseHeaders = null, Dictionary<string, object> content = null, string contentAsJson = null) : base(responseHeaders, content, contentAsJson)
         {
            this.SourceLanguageOfOrigin = sourceLanguageOfOrigin;
            this.SourceLanguageOfUse = sourceLanguageOfUse;
            this.SourceScript = sourceScript;
            this.TargetLanguage = targetLanguage;
            this.TargetScheme = targetScheme;
            this.Translation = translation;
            this.TargetScript = targetScript;
            this.EntityType = entityType;
            this.Confidence = confidence;
        }

        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj">The object to compare against</param>
        /// <returns>True if equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is TranslateNamesResponse)
            {
                TranslateNamesResponse other = obj as TranslateNamesResponse;
                List<bool> conditions = new List<bool>() {
                    this.ResponseHeaders != null && other.ResponseHeaders != null ? this.ResponseHeaders.Equals(other.ResponseHeaders) : this.ResponseHeaders == other.ResponseHeaders,
                    this.SourceLanguageOfOrigin != null && other.SourceLanguageOfOrigin != null ? this.SourceLanguageOfOrigin.Equals(other.SourceLanguageOfOrigin) : this.SourceLanguageOfOrigin == other.SourceLanguageOfOrigin,
                    this.SourceLanguageOfUse != null && other.SourceLanguageOfUse != null ? this.SourceLanguageOfUse.Equals(other.SourceLanguageOfUse) : this.SourceLanguageOfUse == other.SourceLanguageOfUse,
                    this.SourceScript != null && other.SourceScript != null ? this.SourceScript.Equals(other.SourceScript) : this.SourceScript == other.SourceScript,
                    this.TargetLanguage != null && other.TargetLanguage != null ? this.TargetLanguage.Equals(other.TargetLanguage) : this.TargetLanguage == other.TargetLanguage,
                    this.TargetScheme != null && other.TargetScheme != null ? this.TargetScheme.Equals(other.TargetScheme) : this.TargetScheme == other.TargetScheme,
                    this.Translation != null && other.Translation != null ? this.Translation.Equals(other.Translation) : this.Translation == other.Translation,
                    this.TargetScript != null && other.TargetScript != null ? this.TargetScript.Equals(other.TargetScript) : this.TargetScript == other.TargetScript,
                    this.EntityType != null && other.EntityType != null ? this.EntityType.Equals(other.EntityType) : this.EntityType == other.EntityType, 
                    this.Confidence == other.Confidence,
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
            int h0 = this.EntityType != null ? this.EntityType.GetHashCode() : 1;
            int h1 = this.ResponseHeaders != null ? this.ResponseHeaders.GetHashCode() : 1;
            int h2 = this.SourceLanguageOfOrigin != null ? this.SourceLanguageOfOrigin.GetHashCode() : 1;
            int h3 = this.SourceLanguageOfUse != null ? this.SourceLanguageOfUse.GetHashCode() : 1;
            int h4 = this.SourceScript != null ? this.SourceScript.GetHashCode() : 1;
            int h5 = this.TargetScript != null ? this.TargetLanguage.GetHashCode() : 1;
            int h6 = this.TargetScheme != null ? this.TargetScheme.GetHashCode() : 1;
            int h7 = this.TargetScript != null ? this.TargetScript.GetHashCode() : 1;
            int h8 = this.Translation != null ? this.Translation.GetHashCode() : 1;
            int h9 = this.Confidence != null ? this.Confidence.GetHashCode() : 1;
            return h0 ^ h1 ^ h2 ^ h3 ^ h4 ^ h5 ^ h6 ^ h7 ^ h8 ^ h9;
        }
    }
}

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
    /// A class to represent responses from the TranslatedNames endpoint of the RosetteAPI
    /// </summary>
    public class TranslateNamesResponse : RosetteResponse
    {
        /// <summary>
        /// Gets or sets the type of entity of the name being translated
        /// </summary>
        public string EntityType { get; set; }
        /// <summary>
        /// Gets or sets the script of the name being translated
        /// </summary>
        public string SourceScript { get; set; }
        /// <summary>
        /// Gets or sets the language of origin of the name being translated
        /// </summary>
        public string SourceLanguageOfOrigin { get; set; }
        /// <summary>
        /// Gets or sets the language in which the name being translated is written
        /// </summary>
        public string SourceLanguageOfUse { get; set; }
        /// <summary>
        /// Gets or sets the language to which the name has been translated
        /// </summary>
        public string TargetLanguage { get; set; }
        /// <summary>
        /// Gets or sets the script in which the translated name is written
        /// </summary>
        public string TargetScript { get; set; }
        /// <summary>
        /// Gets or sets the scheme in which the translated name is written
        /// </summary>
        public string TargetScheme { get; set; }
        /// <summary>
        /// Gets or sets the translation of the name
        /// </summary>
        public string Translation { get; set; }
        /// <summary>
        /// Gets or sets the confidence of the translation
        /// </summary>
        public Nullable<decimal> Confidence { get; set; }

        /// <summary>
        /// The response headers returned from the API
        /// </summary>
        public ResponseHeaders ResponseHeaders { get; private set; }

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
            this.SourceScript = this.Content.ContainsKey(sourceScriptKey) ? this.SourceScript = this.Content[sourceScriptKey] as string : null;
            this.SourceLanguageOfOrigin = this.Content.ContainsKey(sourceLanguageOfOriginKey) ? this.Content[sourceLanguageOfOriginKey] as string : null;
            this.SourceLanguageOfUse = this.Content.ContainsKey(sourceLanguageOfUseKey) ? this.Content[sourceLanguageOfUseKey] as string : null;
            this.Translation = this.Content.ContainsKey(translationKey) ? this.Content[translationKey] as string : null;
            this.TargetLanguage = this.Content.ContainsKey(targetLanguageKey) ? this.Content[targetLanguageKey] as string : null;
            this.TargetScript = this.Content.ContainsKey(targetScriptKey) ? this.Content[targetScriptKey] as string : null;
            this.TargetScheme = this.Content.ContainsKey(targetSchemeKey) ? this.Content[targetSchemeKey] as string : null;
            this.EntityType = this.Content.ContainsKey(entityTypeKey) ? this.Content[entityTypeKey] as string : null;
            this.Confidence = this.Content.ContainsKey(confidenceKey) ? this.Content[confidenceKey] as Nullable<decimal> : null;
            this.ResponseHeaders = new ResponseHeaders(this.Headers);
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
            string sourceLanguageOfUse = null, string sourceScript = null, decimal? confidence = null, Dictionary<string, string> responseHeaders = null, Dictionary<string, object> content = null, string contentAsJson = null) : base(responseHeaders, content, contentAsJson)
         {
            this.ResponseHeaders = new ResponseHeaders(responseHeaders);
            this.SourceLanguageOfOrigin = sourceLanguageOfOrigin;
            this.SourceLanguageOfUse = sourceLanguageOfUse;
            this.SourceScript = sourceScript;
            this.TargetLanguage = targetLanguage;
            this.TargetScheme = TargetScheme;
            this.Translation = translation;
            this.TargetScript = TargetScript;
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

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns>The TranslationNameResponse in JSON form</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder("{");
            if (this.Translation != null) { builder.AppendFormat("\"{0}\": \"{1}\", ", translationKey, this.Translation.ToString()); }
            if (this.TargetLanguage != null) { builder.AppendFormat("\"{0}\": \"{1}\", ", targetLanguageKey, this.TargetLanguage.ToString()); }
            if (this.TargetScript != null) { builder.AppendFormat("\"{0}\": \"{1}\", ", targetScriptKey, this.TargetScript.ToString()); }
            if (this.TargetScheme != null) { builder.AppendFormat("\"{0}\": \"{1}\", ", targetSchemeKey, this.TargetScheme.ToString()); }
            if (this.SourceLanguageOfOrigin != null) { builder.AppendFormat("\"{0}\": \"{1}\", ", sourceLanguageOfUseKey, this.SourceLanguageOfOrigin.ToString()); }
            if (this.SourceLanguageOfUse != null) { builder.AppendFormat("\"{0}\": \"{1}\", ", sourceLanguageOfUseKey, this.SourceLanguageOfUse.ToString()); }
            if (this.SourceScript != null) { builder.AppendFormat("\"{0}\": \"{1}\", ", sourceScriptKey, this.SourceScript.ToString()); }
            if (this.EntityType != null) { builder.AppendFormat("\"{0}\": \"{1}\", ", entityTypeKey, this.EntityType.ToString()); }
            if (this.Confidence != null) { builder.AppendFormat("\"{0}\": {1}, ", confidenceKey, this.Confidence.ToString()); }
            if (this.ResponseHeaders != null) { builder.AppendFormat("responseHeaders: {0}, ", this.ResponseHeaders.ToString()); }
            if (builder.Length > 2) { builder.Remove(builder.Length - 2, 2); }
            builder.Append("}");
            return builder.ToString();
        }

        /// <summary>
        /// Gets the content in JSON form
        /// </summary>
        /// <returns>The content in JSON form</returns>
        public string ContentToString()
        {
            StringBuilder builder = new StringBuilder("{");
            if (this.Translation != null) { builder.AppendFormat("\"{0}\": \"{1}\", ", translationKey, this.Translation.ToString()); }
            if (this.TargetLanguage != null) { builder.AppendFormat("\"{0}\": \"{1}\", ", targetLanguageKey, this.TargetLanguage.ToString()); }
            if (this.TargetScript != null) { builder.AppendFormat("\"{0}\": \"{1}\", ", targetScriptKey, this.TargetScript.ToString()); }
            if (this.TargetScheme != null) { builder.AppendFormat("\"{0}\": \"{1}\", ", targetSchemeKey, this.TargetScheme.ToString()); }
            if (this.SourceLanguageOfOrigin != null) { builder.AppendFormat("\"{0}\": \"{1}\", ", sourceLanguageOfUseKey, this.SourceLanguageOfOrigin.ToString()); }
            if (this.SourceLanguageOfUse != null) { builder.AppendFormat("\"{0}\": \"{1}\", ", sourceLanguageOfUseKey, this.SourceLanguageOfUse.ToString()); }
            if (this.SourceScript != null) { builder.AppendFormat("\"{0}\": \"{1}\", ", sourceScriptKey, this.SourceScript.ToString()); }
            if (this.EntityType != null) { builder.AppendFormat("\"{0}\": \"{1}\", ", entityTypeKey, this.EntityType.ToString()); }
            if (this.Confidence != null) { builder.AppendFormat("\"{0}\": {1}, ", confidenceKey, this.Confidence.ToString()); }
            if (builder.Length > 2) { builder.Remove(builder.Length - 2, 2); }
            builder.Append("}");
            return builder.ToString();
        }
    }
}

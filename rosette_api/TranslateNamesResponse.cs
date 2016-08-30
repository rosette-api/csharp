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

        private string sourceScriptKey = "sourceScript";
        private string sourceLanguageOfOriginKey = "sourceLanguageOfOrigin";
        private string sourceLanguageOfUseKey = "sourceLanguageOfUse";
        private string translationKey = "translation";
        private string targetLanguageKey = "targetLanguage";
        private string targetScriptKey = "targetScript";
        private string targetSchemeKey = "targetScheme";

        /// <summary>
        /// Creates a TranslateNamesResponse from the given apiResult
        /// </summary>
        /// <param name="apiResult">The message from the API</param>
        public TranslateNamesResponse(HttpResponseMessage apiResult) :base(apiResult)
        {
            if (this.Content.ContainsKey(sourceScriptKey))
            {
                this.SourceScript = this.Content[sourceScriptKey] as string;
            }
            if (this.Content.ContainsKey(sourceLanguageOfOriginKey))
            {
                this.SourceLanguageOfOrigin = this.Content[sourceLanguageOfOriginKey] as string;
            }
            if (this.Content.ContainsKey(sourceLanguageOfUseKey))
            {
                this.SourceLanguageOfUse = this.Content[sourceLanguageOfUseKey] as string;
            }
            if (this.Content.ContainsKey(translationKey))
            {
                this.Translation = this.Content[translationKey] as string;
            }
            if (this.Content.ContainsKey(targetLanguageKey))
            {
                this.TargetLanguage = this.Content[targetLanguageKey] as string;
            }
            if (this.Content.ContainsKey(targetScriptKey))
            {
                this.TargetScript = this.Content[targetScriptKey] as string;
            }
            if (this.Content.ContainsKey(targetSchemeKey))
            {
                this.TargetScheme = this.Content[targetSchemeKey] as string;
            }
            this.ResponseHeaders = new ResponseHeaders(this.Headers);
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
                    this.ResponseHeaders.Equals(other.ResponseHeaders),
                    this.SourceLanguageOfOrigin.Equals(other.SourceLanguageOfOrigin),
                    this.SourceLanguageOfUse.Equals(other.SourceLanguageOfUse),
                    this.SourceScript.Equals(other.SourceScript),
                    this.TargetLanguage.Equals(other.TargetLanguage),
                    this.TargetScheme.Equals(other.TargetScheme),
                    this.Translation.Equals(other.Translation),
                    this.TargetScript.Equals(other.TargetScript),
                    this.EntityType.Equals(other.EntityType)
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
            return this.EntityType.GetHashCode() ^ this.ResponseHeaders.GetHashCode()
                ^ this.SourceLanguageOfOrigin.GetHashCode() ^ this.SourceLanguageOfUse.GetHashCode()
                ^ this.SourceScript.GetHashCode() ^ this.TargetLanguage.GetHashCode()
                ^ this.TargetScheme.GetHashCode() ^ this.TargetScript.GetHashCode() ^ this.Translation.GetHashCode();
        } 
    }
}

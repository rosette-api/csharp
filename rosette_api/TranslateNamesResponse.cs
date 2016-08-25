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
        /// If present, the type of entity of the name being translated
        /// </summary>
        public Optional<string> EntityType { get; private set; }
        /// <summary>
        /// If present, the script of the name being translated
        /// </summary>
        public Optional<string> SourceScript { get; private set; }
        /// <summary>
        /// If present, the language of origin of the name being translated
        /// </summary>
        public Optional<string> SourceLanguageOfOrigin { get; private set; }
        /// <summary>
        /// If present, the language in which the name being translated is written
        /// </summary>
        public Optional<string> SourceLanguageOfUse { get; private set; }
        /// <summary>
        /// If present, the language to which the name has been translated
        /// </summary>
        public Optional<string> TargetLanguage { get; private set; }
        /// <summary>
        /// If present, the script in which the translated name is written
        /// </summary>
        public Optional<string> TargetScript { get; private set; }
        /// <summary>
        /// If present, the scheme in which the translated name is written
        /// </summary>
        public Optional<string> TargetScheme { get; private set; }
        /// <summary>
        /// If present, the translation of the name
        /// </summary>
        public Optional<string> Translation { get; private set; }

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
    }
}

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
        public Optional<string> EntityType;
        /// <summary>
        /// If present, the script of the name being translated
        /// </summary>
        public Optional<string> SourceScript;
        /// <summary>
        /// If present, the language of origin of the name being translated
        /// </summary>
        public Optional<string> SourceLanguageOfOrigin;
        /// <summary>
        /// If present, the language in which the name being translated is written
        /// </summary>
        public Optional<string> SourceLanguageOfUse;
        /// <summary>
        /// If present, the language to which the name has been translated
        /// </summary>
        public Optional<string> TargetLanguage;
        /// <summary>
        /// If present, the script in which the translated name is written
        /// </summary>
        public Optional<string> TargetScript;
        /// <summary>
        /// If present, the scheme in which the translated name is written
        /// </summary>
        public Optional<string> TargetScheme;
        /// <summary>
        /// If present, the translation of the name
        /// </summary>
        public Optional<string> Translation;

        /// <summary>
        /// The response headers returned from the API
        /// </summary>
        public ResponseHeaders ResponseHeaders;

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
            this.ResponseHeaders = new ResponseHeaders(apiResult.Headers);
        }
    }
}

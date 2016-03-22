using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace rosette_api {
    /// <summary>
    /// Enum to provide feature options for Morphology
    /// </summary>
    public enum MorphologyFeature { 
        /// <summary>provide complete morphology</summary>
        complete, 
        /// <summary>provide lemmas</summary>
        lemmas, 
        /// <summary>provide parts of speech</summary>
        partsOfSpeech, 
        /// <summary>provide compound components</summary>
        compoundComponents, 
        /// <summary>provide han readings</summary>
        hanReadings 
    };

    /// <summary>C# Rosette API.
    /// <para>
    /// Primary class for interfacing with the Rosette API
    /// @copyright 2014-2015 Basis Technology Corporation.
    /// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance
    /// with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
    /// Unless required by applicable law or agreed to in writing, software distributed under the License is
    /// distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    /// See the License for the specific language governing permissions and limitations under the License.
    /// </para>
    /// </summary>
    public class CAPI {
        /// <summary>
        /// Internal string to hold the uri ending for each endpoint. 
        /// Set when an endpoint is called.
        /// </summary>
        private string _uri = null;

        /// <summary>
        /// Internal check to see if the version matches. Defaults to false and set during initialization.
        /// </summary>
        private bool version_checked;

        /// <summary>
        /// Internal time value of the last version check. Set on first version check. Resets the version check after 24hrs. 
        /// </summary>
        private DateTime last_version_check;

        /// <summary>C# API class
        /// <para>Rosette Python Client Binding API; representation of a Rosette server.
        /// Instance methods of the C# API provide communication with specific Rosette server endpoints.
        /// Requires user_key to start and has 3 additional parameters to be specified. 
        /// Will run a Version Check against the Rosette Server. If the version check fails, a 
        /// RosetteException will be thrown. 
        /// </para>
        /// </summary>
        /// <param name="user_key">string: API key required by the Rosette server to allow access to endpoints</param>
        /// <param name="uristring">(string, optional): Base URL for the HttpClient requests. If none is given, will use the default API URI</param>
        /// <param name="maxRetry">(int, optional): Maximum number of times to retry a request on HttpResponse error. Default is 3 times.</param> 
        /// <param name="client">(HttpClient, optional): Forces the API to use a custom HttpClient.</param> 
        public CAPI(string user_key, string uristring = "https://api.rosette.com/rest/v1/", int maxRetry = 1, HttpClient client = null) {
            UserKey = user_key;
            URIstring = (uristring == null) ? "https://api.rosette.com/rest/v1/" : uristring;
            MaxRetry = (maxRetry == 0) ? 1 : maxRetry;
            Debug = false;
            Timeout = 300;
            Client = client;
            version_checked = checkVersion();
            last_version_check = default(DateTime);
        }

        /// <summary>UserKey
        /// <para>
        /// Getter, Setter for the UserKey
        /// UserKey: API key required by the Rosette Server
        /// Allows users to change their UserKey later (e.g. if instantiated class incorrectly)
        /// </para>
        /// </summary>
        public string UserKey { get; set; }

        /// <summary>URIstring
        /// <para>
        /// Getter, Setter for the URIstring
        /// URIstring: Base URI for the HttpClient.
        /// Allows users to change their URIstring later (e.g. if instantiated class incorrectly)
        /// </para>
        /// </summary>
        public string URIstring { get; set; }

        /// <summary>
        /// Internal version number that is used to sync with the running Rosette API server
        /// </summary>
        private static string _bindingVersion = "0.10";

        /// <summary>Version
        /// <para>
        /// Getter, Setter for the Version
        /// Version: Internal Server Version number.
        /// </para>
        /// </summary>
        public static string Version {
            get { return _bindingVersion; }
            private set {
                _bindingVersion = value;
            }
        }
        /// <summary>MaxRetry
        /// <para>
        /// Getter, Setter for the MaxRetry
        /// MaxRetry: Maximum number of times to retry a request on HTTPResponse error.
        /// Allows users to change their MaxRetry later (e.g. if instantiated class incorrectly)
        /// </para>
        /// </summary>
        public int MaxRetry { get; set; }

        /// <summary>Client
        /// <para>
        /// Getter, Setter for the Client
        /// Client: Forces the API to use a custom HttpClient.
        /// </para>
        /// </summary>
        public HttpClient Client { get; set; }

        /// <summary>Debug
        /// <para>
        /// Getter, Setter for the Debug
        /// Debug: Sets the debug mode parameter for the Rosette server.
        /// </para>
        /// </summary>
        public bool Debug { get; set; }

        /// <summary>Timeout
        /// <para>
        /// Getter, Setter for the Timeout
        /// Timeout: Sets the Timeout for the HttpClient.
        /// </para>
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>Categories
        /// <para>
        /// (POST)Categories Endpoint: Returns an ordered list of categories identified in the input. The categories are Tier 1 contextual categories defined in the QAG Taxonomy.
        /// </para>
        /// </summary>
        /// <param name="content">(string, optional): Input to process (JSON string or base64 encoding of non-JSON string)</param>
        /// <param name="language">(string, optional): Language: ISO 639-3 code (ignored for the /language endpoint)</param>
        /// <param name="contentType">(string, optional): not used at this time</param>
        /// <param name="contentUri">(string, optional): URI to accessible content (content and contentUri are mutually exclusive)</param>
        /// <returns>RosetteResponse containing the results of the request.
        /// The response is the contextual categories identified in the input.
        /// </returns>
        public RosetteResponse Categories(string content = null, string language = null, string contentType = null, string contentUri = null) {
            _uri = "categories";
            return Process(content, language, contentType, contentUri);
        }

        /// <summary>Categories
        /// <para>
        /// (POST)Categories Endpoint: Returns an ordered list of categories identified in the input. The categories are Tier 1 contextual categories defined in the QAG Taxonomy.
        /// </para>
        /// </summary>
        /// <param name="dict">Dictionary&lt;object, object&gt;: Dictionary containing parameters as (key,value) pairs</param>
        /// <returns>RosetteResponse containing the results of the request.
        /// The response is the contextual categories identified in the input.
        /// </returns>
        public RosetteResponse Categories(Dictionary<object, object> dict) {
            _uri = "categories";
            return getResponse(SetupClient(), new JavaScriptSerializer().Serialize(dict));
        }

        /// <summary>Categories
        /// <para>
        /// (POST)Categories Endpoint: Returns an ordered list of categories identified in the input. The categories are Tier 1 contextual categories defined in the QAG Taxonomy.
        /// </para>
        /// </summary>
        /// <param name="file">RosetteFile: RosetteFile Object containing the file (and possibly options) to upload</param>
        /// <returns>RosetteResponse containing the results of the request.
        /// The response is the contextual categories identified in the input.
        /// </returns>
        public RosetteResponse Categories(RosetteFile file) {
            _uri = "categories";
            return Process(file);
        }

        /// <summary>EntitiesLinked
        /// <para>
        /// (POST)EntitiesLinked Endpoint: Links entities in the input to entities in the knowledge base.
        /// </para>
        /// </summary>
        /// <param name="content">(string, optional): Input to process (JSON string or base64 encoding of non-JSON string)</param>
        /// <param name="language">(string, optional): Language: ISO 639-3 code (ignored for the /language endpoint)</param>
        /// <param name="contentType">(string, optional): not used at this time</param>
        /// <param name="contentUri">(string, optional): URI to accessible content (content and contentUri are mutually exclusive)</param>
        /// <returns>RosetteResponse containing the results of the request.
        /// </returns>
        public RosetteResponse EntitiesLinked(string content = null, string language = null, string contentType = null, string contentUri = null) {
            _uri = "entities/linked";
            return Process(content, language, contentType, contentUri);
        }

        /// <summary>EntitiesLinked
        /// <para>
        /// (POST)EntitiesLinked Endpoint: Links entities in the input to entities in the knowledge base.
        /// </para>
        /// </summary>
        /// <param name="dict">Dictionary&lt;object, object&gt;: Dictionary containing parameters as (key,value) pairs</param>
        /// <returns>RosetteResponse containing the results of the request.
        /// </returns>
        public RosetteResponse EntitiesLinked(Dictionary<object, object> dict) {
            _uri = "entities/linked";
            return getResponse(SetupClient(), new JavaScriptSerializer().Serialize(dict));
        }

        /// <summary>EntitiesLinked
        /// <para>
        /// (POST)EntitiesLinked Endpoint: Links entities in the input to entities in the knowledge base.
        /// </para>
        /// </summary>
        /// <param name="file">RosetteFile: RosetteFile Object containing the file (and possibly options) to upload</param>
        /// <returns>RosetteResponse containing the results of the request.
        /// </returns>
        public RosetteResponse EntitiesLinked(RosetteFile file) {
            _uri = "entities/linked";
            return Process(file);
        }

        /// <summary>Entity
        /// <para>
        /// (POST)Entity Endpoint: Returns each entity extracted from the input.
        /// </para>
        /// </summary>
        /// <param name="content">(string, optional): Input to process (JSON string or base64 encoding of non-JSON string)</param>
        /// <param name="language">(string, optional): Language: ISO 639-3 code (ignored for the /language endpoint)</param>
        /// <param name="contentType">(string, optional): not used at this time</param>
        /// <param name="contentUri">(string, optional): URI to accessible content (content and contentUri are mutually exclusive)</param>
        /// <returns>RosetteResponse containing the results of the request. 
        /// </returns>
        public RosetteResponse Entity(string content = null, string language = null, string contentType = null, string contentUri = null) {
            _uri = "entities";
            return Process(content, language, contentType, contentUri);
        }

        /// <summary>Entity
        /// <para>
        /// (POST)Entity Endpoint: Returns each entity extracted from the input.
        /// </para>
        /// </summary>
        /// <param name="dict">Dictionary&lt;object, object&gt;: Dictionary containing parameters as (key,value) pairs</param>
        /// <returns>RosetteResponse containing the results of the request. 
        /// </returns>
        public RosetteResponse Entity(Dictionary<object, object> dict) {
            _uri = "entities";
            return getResponse(SetupClient(), new JavaScriptSerializer().Serialize(dict));
        }

        /// <summary>Entity
        /// <para>
        /// (POST)Entity Endpoint: Returns each entity extracted from the input.
        /// </para>
        /// </summary>
        /// <param name="file">RosetteFile: RosetteFile Object containing the file (and possibly options) to upload</param>
        /// <returns>RosetteResponse containing the results of the request. 
        /// </returns>
        public RosetteResponse Entity(RosetteFile file) {
            _uri = "entities";
            return Process(file);
        }

        /// <summary>Info
        /// <para>
        /// (GET)Info Endpoint: Response is a JSON string with Rosette API information including buildNumber, name, version, and buildTime.
        /// </para>
        /// </summary>
        /// <returns>RosetteResponse containing the results of the info GET.</returns>
        public RosetteResponse Info() {
            _uri = "info";
            return getResponse(SetupClient());
        }

        /// <summary>Language
        /// <para>
        /// (POST)Language Endpoint: Returns list of candidate languages.
        /// </para>
        /// </summary>
        /// <param name="content">(string, optional): Input to process (JSON string or base64 encoding of non-JSON string)</param>
        /// <param name="language">(string, optional): Language: ISO 639-3 code (ignored for the /language endpoint)</param>
        /// <param name="contentType">(string, optional): MIME type of the input (required for base64 content; if content type is unknown, set to "application/octet-stream")</param>
        /// <param name="contentUri">(string, optional): URI to accessible content (content and contentUri are mutually exclusive)</param>
        /// <returns>RosetteResponse containing the results of the request. 
        /// The response is an ordered list of detected languages.
        /// </returns>
        public RosetteResponse Language(string content = null, string language = null, string contentType = null, string contentUri = null) {
            _uri = "language";
            return Process(content, language, contentType, contentUri);
        }

        /// <summary>Language
        /// <para>
        /// (POST)Language Endpoint: Returns list of candidate languages.
        /// </para>
        /// </summary>
        /// <param name="dict">Dictionary&lt;object, object&gt;: Dictionary containing parameters as (key,value) pairs</param>
        /// <returns>RosetteResponse containing the results of the request. 
        /// The response is an ordered list of detected languages.
        /// </returns>
        public RosetteResponse Language(Dictionary<object, object> dict) {
            _uri = "language";
            return getResponse(SetupClient(), new JavaScriptSerializer().Serialize(dict));
        }

        /// <summary>Language
        /// <para>
        /// (POST)Language Endpoint: Returns list of candidate languages.
        /// </para>
        /// </summary>
        /// <param name="file">RosetteFile: RosetteFile Object containing the file (and possibly options) to upload</param>
        /// <returns>RosetteResponse containing the results of the request. 
        /// The response is an ordered list of detected languages.
        /// </returns>
        public RosetteResponse Language(RosetteFile file) {
            _uri = "language";
            return Process(file);
        }

        /// <summary>Morphology
        /// <para>
        /// (POST)Morphology Endpoint: Returns morphological analysis of input.
        /// </para>
        /// </summary>
        /// <param name="content">(string, optional): Input to process (JSON string or base64 encoding of non-JSON string)</param>
        /// <param name="language">(string, optional): Language: ISO 639-3 code (ignored for the /language endpoint)</param>
        /// <param name="contentType">(string, optional): not used at this time</param>
        /// <param name="contentUri">(string, optional): URI to accessible content (content and contentUri are mutually exclusive)</param>
        /// <param name="feature">(string, optional): Description of what morphology feature to request from the Rosette server</param>
        /// <returns>RosetteResponse containing the results of the request. 
        /// The response may include lemmas, part of speech tags, compound word components, and Han readings. 
        /// Support for specific return types depends on language.
        /// </returns>
        public RosetteResponse Morphology(string content = null, string language = null, string contentType = null, string contentUri = null, MorphologyFeature feature = MorphologyFeature.complete) {
            _uri = "morphology/" + feature.MorphologyEndpoint();
            return Process(content, language, contentType, contentUri);
        }

        /// <summary>Morphology
        /// <para>
        /// (POST)Morphology Endpoint: Returns morphological analysis of input.
        /// </para>
        /// </summary>
        /// <param name="dict">Dictionary&lt;object, object&gt;: Dictionary containing parameters as (key,value) pairs</param>
        /// <param name="feature">(string, optional): Description of what morphology feature to request from the Rosette server</param>
        /// <returns>RosetteResponse containing the results of the request. 
        /// The response may include lemmas, part of speech tags, compound word components, and Han readings. 
        /// Support for specific return types depends on language.
        /// </returns>
        public RosetteResponse Morphology(Dictionary<object, object> dict, MorphologyFeature feature = MorphologyFeature.complete) {
            _uri = "morphology/" + feature.MorphologyEndpoint();
            return getResponse(SetupClient(), new JavaScriptSerializer().Serialize(dict));
        }

        /// <summary>Morphology
        /// <para>
        /// (POST)Morphology Endpoint: Returns morphological analysis of input.
        /// </para>
        /// </summary>
        /// <param name="file">RosetteFile: RosetteFile Object containing the file (and possibly options) to upload</param>
        /// <param name="feature">(string, optional): Description of what morphology feature to request from the Rosette server</param>
        /// <returns>RosetteResponse containing the results of the request. 
        /// The response may include lemmas, part of speech tags, compound word components, and Han readings. 
        /// Support for specific return types depends on language.
        /// </returns>
        public RosetteResponse Morphology(RosetteFile file, MorphologyFeature feature = MorphologyFeature.complete) {
            _uri = "morphology/" + feature.MorphologyEndpoint();
            return Process(file);
        }

        /// <summary>NameSimilarity
        /// <para>
        /// (POST)NameSimilarity Endpoint: Returns the result of matching 2 names.
        /// </para>
        /// </summary>
        /// <param name="n1">Name: First name to be matched</param>
        /// <param name="n2">Name: Second name to be matched</param>
        /// <returns>RosetteResponse containing the results of the request. 
        /// </returns>
        public RosetteResponse NameSimilarity(Name n1, Name n2) {
            _uri = "name-similarity";

            return getResponse(SetupClient(), new JavaScriptSerializer().Serialize(new Dictionary<string, object>(){
                { "name1", n1},
                { "name2", n2}
            }));
        }

        /// <summary>MatchedName
        /// <para>
        /// (POST)MatchedName Endpoint: Returns the result of matching 2 names.
        /// </para>
        /// </summary>
        /// <param name="n1">Name: First name to be matched</param>
        /// <param name="n2">Name: Second name to be matched</param>
        /// <returns>RosetteResponse containing the results of the request. 
        /// </returns>
        [Obsolete("Use NameSimilarity")]
        public RosetteResponse MatchedName(Name n1, Name n2) {
            return NameSimilarity(n1, n2);
        }

        /// <summary>NameSimilarity
        /// <para>
        /// (POST)NameSimilarity Endpoint: Returns the result of matching 2 names.
        /// </para>
        /// </summary>
        /// <param name="dict">Dictionary&lt;object, object&gt;: Dictionary containing parameters as (key,value) pairs</param>
        /// <returns>RosetteResponse containing the results of the request. 
        /// </returns>
        public RosetteResponse NameSimilarity(Dictionary<object, object> dict) {
            _uri = "name-similarity";
            return getResponse(SetupClient(), new JavaScriptSerializer().Serialize(dict));
        }

        /// <summary>MatchedName
        /// <para>
        /// (POST)MatchedName Endpoint: Returns the result of matching 2 names.
        /// </para>
        /// </summary>
        /// <param name="dict">Dictionary&lt;object, object&gt;: Dictionary containing parameters as (key,value) pairs</param>
        /// <returns>RosetteResponse containing the results of the request. 
        /// </returns>
        [Obsolete("Use NameSimilarity")]
        public RosetteResponse MatchedName(Dictionary<object, object> dict) {
            return NameSimilarity(dict);
        }

        /// <summary>Ping
        /// (GET)Ping Endpoint: Pings Rosette API for a response indicting that the service is available
        /// </summary>
        /// <returns>RosetteResponse containing the results of the info GET.
        /// The reponse contains a message and time.
        /// </returns>
        public RosetteResponse Ping() {
            _uri = "ping";
            return getResponse(SetupClient());
        }

        /// <summary>Relationships
        /// <para>
        /// (POST)Relationships Endpoint: Returns each relationship extracted from the input.
        /// </para>
        /// </summary>
        /// <param name="content">(string, optional): Input to process (JSON string or base64 encoding of non-JSON string)</param>
        /// <param name="language">(string, optional): Language: ISO 639-3 code (ignored for the /language endpoint)</param>
        /// <param name="contentType">(string, optional): not used at this time</param>
        /// <param name="contentUri">(string, optional): URI to accessible content (content and contentUri are mutually exclusive)</param>
        /// <returns>
        /// RosetteResponse of extracted relationships. A relationship contains
        /// 
        /// predicate - usually the main verb, property or action that is expressed by the text
        /// arg1 - usually the subject, agent or main actor of the relationship
        /// arg2 [optional] - complements the predicate and is usually the object, theme or patient of the relationship
        /// arg3 [optional] - usually an additional object in ditransitive verbs
        /// adjuncts [optional] - contain all optional parts of a relationship which are not temporal or locative expressions
        /// locatives [optional] - usually express the locations the action expressed by the relationship took place
        /// temporals [optional] - usually express the time in which the action expressed by the relationship took place
        /// </returns>
        public RosetteResponse Relationships(string content = null, string language = null, string contentType = null, string contentUri = null) {
            _uri = "relationships";
            return Process(content, language, contentType, contentUri);
        }

        /// <summary>Relationships
        /// <para>
        /// (POST)Relationships Endpoint: Returns each relationship extracted from the input.
        /// </para>
        /// </summary>
        /// <param name="dict">Dictionary&lt;object, object&gt;: Dictionary containing parameters as (key,value) pairs</param>
        /// <returns>
        /// RosetteResponse of extracted relationships. A relationship contains
        /// 
        /// predicate - usually the main verb, property or action that is expressed by the text
        /// arg1 - usually the subject, agent or main actor of the relationship
        /// arg2 [optional] - complements the predicate and is usually the object, theme or patient of the relationship
        /// arg3 [optional] - usually an additional object in ditransitive verbs
        /// adjuncts [optional] - contain all optional parts of a relationship which are not temporal or locative expressions
        /// locatives [optional] - usually express the locations the action expressed by the relationship took place
        /// temporals [optional] - usually express the time in which the action expressed by the relationship took place
        /// </returns>
        public RosetteResponse Relationships(Dictionary<object, object> dict) {
            _uri = "relationships";
            return getResponse(SetupClient(), new JavaScriptSerializer().Serialize(dict));
        }

        /// <summary>Relationships
        /// <para>
        /// (POST)Relationships Endpoint: Returns each relationship extracted from the input.
        /// </para>
        /// </summary>
        /// <param name="file">RosetteFile: RosetteFile Object containing the file (and possibly options) to upload</param>
        /// <returns>
        /// RosetteResponse of extracted relationships. A relationship contains
        /// 
        /// predicate - usually the main verb, property or action that is expressed by the text
        /// arg1 - usually the subject, agent or main actor of the relationship
        /// arg2 [optional] - complements the predicate and is usually the object, theme or patient of the relationship
        /// arg3 [optional] - usually an additional object in ditransitive verbs
        /// adjuncts [optional] - contain all optional parts of a relationship which are not temporal or locative expressions
        /// locatives [optional] - usually express the locations the action expressed by the relationship took place
        /// temporals [optional] - usually express the time in which the action expressed by the relationship took place
        /// </returns>
        public RosetteResponse Relationships(RosetteFile file) {
            _uri = "relationships";
            return Process(file);
        }

        /// <summary>Sentences
        /// <para>
        /// (POST)Sentences Endpoint: Divides the input into sentences.
        /// </para>
        /// </summary>
        /// <param name="content">(string, optional): Input to process (JSON string or base64 encoding of non-JSON string)</param>
        /// <param name="language">(string, optional): Language: ISO 639-3 code (ignored for the /language endpoint)</param>
        /// <param name="contentType">(string, optional): not used at this time</param>
        /// <param name="contentUri">(string, optional): URI to accessible content (content and contentUri are mutually exclusive)</param>
        /// <returns>Dictionary&lt;string, object&gt;: Dictionary containing the results of the request. 
        /// The response contains a list of sentences.
        /// </returns>
        public RosetteResponse Sentences(string content = null, string language = null, string contentType = null, string contentUri = null) {
            _uri = "sentences";
            return Process(content, language, contentType, contentUri);
        }

        /// <summary>Sentences
        /// <para>
        /// (POST)Sentences Endpoint: Divides the input into sentences.
        /// </para>
        /// </summary>
        /// <param name="dict">Dictionary&lt;object, object&gt;: Dictionary containing parameters as (key,value) pairs</param>
        /// <returns>RosetteResponse containing the results of the request. 
        /// The response contains a list of sentences.
        /// </returns>
        public RosetteResponse Sentences(Dictionary<object, object> dict) {
            _uri = "sentences";
            return getResponse(SetupClient(), new JavaScriptSerializer().Serialize(dict));
        }

        /// <summary>Sentences
        /// <para>
        /// (POST)Sentences Endpoint: Divides the input into sentences.
        /// </para>
        /// </summary>
        /// <param name="file">RosetteFile: RosetteFile Object containing the file (and possibly options) to upload</param>
        /// <returns>RosetteResponse containing the results of the request. 
        /// The response contains a list of sentences.
        /// </returns>
        public RosetteResponse Sentences(RosetteFile file) {
            _uri = "sentences";
            return Process(file);
        }

        /// <summary>Sentiment
        /// <para>
        /// (POST)Sentiment Endpoint: Analyzes the positive and negative sentiment expressed by the input.
        /// </para>
        /// </summary>
        /// <param name="content">(string, optional): Input to process (JSON string or base64 encoding of non-JSON string)</param>
        /// <param name="language">(string, optional): Language: ISO 639-3 code (ignored for the /language endpoint)</param>
        /// <param name="contentType">(string, optional): not used at this time</param>
        /// <param name="contentUri">(string, optional): URI to accessible content (content and contentUri are mutually exclusive)</param>
        /// <returns>RosetteResponse containing the results of the request. 
        /// The response contains sentiment analysis results.
        /// </returns>
        public RosetteResponse Sentiment(string content = null, string language = null, string contentType = null, string contentUri = null) {
            _uri = "sentiment";
            return Process(content, language, contentType, contentUri);
        }

        /// <summary>Sentiment
        /// <para>
        /// (POST)Sentiment Endpoint: Analyzes the positive and negative sentiment expressed by the input.
        /// </para>
        /// </summary>
        /// <param name="dict">Dictionary&lt;object, object&gt;: Dictionary containing parameters as (key,value) pairs</param>
        /// <returns>RosetteResponse containing the results of the request. 
        /// The response contains sentiment analysis results.
        /// </returns>
        public RosetteResponse Sentiment(Dictionary<object, object> dict) {
            _uri = "sentiment";
            return getResponse(SetupClient(), new JavaScriptSerializer().Serialize(dict));
        }

        /// <summary>Sentiment
        /// <para>
        /// (POST)Sentiment Endpoint: Analyzes the positive and negative sentiment expressed by the input.
        /// </para>
        /// </summary>
        /// <param name="file">RosetteFile: RosetteFile Object containing the file (and possibly options) to upload</param>
        /// <returns>RosetteResponse containing the results of the request. 
        /// The response contains sentiment analysis results.
        /// </returns>
        public RosetteResponse Sentiment(RosetteFile file) {
            _uri = "sentiment";
            return Process(file);
        }

        /// <summary>Tokens
        /// <para>
        /// (POST)Tokens Endpoint: Divides the input into tokens.
        /// </para>
        /// </summary>
        /// <param name="content">(string, optional): Input to process (JSON string or base64 encoding of non-JSON string)</param>
        /// <param name="language">(string, optional): Language: ISO 639-3 code (ignored for the /language endpoint)</param>
        /// <param name="contentType">(string, optional): not used at this time</param>
        /// <param name="contentUri">(string, optional): URI to accessible content (content and contentUri are mutually exclusive)</param>
        /// <returns>RosetteResponse containing the results of the request. 
        /// The response contains a list of tokens.
        /// </returns>
        public RosetteResponse Tokens(string content = null, string language = null, string contentType = null, string contentUri = null) {
            _uri = "tokens";
            return Process(content, language, contentType, contentUri);
        }

        /// <summary>Tokens
        /// <para>
        /// (POST)Tokens Endpoint: Divides the input into tokens.
        /// </para>
        /// </summary>
        /// <param name="dict">Dictionary&lt;object, object&gt;: Dictionary containing parameters as (key,value) pairs</param>
        /// <returns>RosetteResponse containing the results of the request. 
        /// The response contains a list of tokens.
        /// </returns>
        public RosetteResponse Tokens(Dictionary<object, object> dict) {
            _uri = "tokens";
            return getResponse(SetupClient(), new JavaScriptSerializer().Serialize(dict));
        }

        /// <summary>Tokens
        /// <para>
        /// (POST)Tokens Endpoint: Divides the input into tokens.
        /// </para>
        /// </summary>
        /// <param name="file">RosetteFile: RosetteFile Object containing the file (and possibly options) to upload</param>
        /// <returns>RosetteResponse containing the results of the request. 
        /// The response contains a list of tokens.
        /// </returns>
        public RosetteResponse Tokens(RosetteFile file) {
            _uri = "tokens";
            return Process(file);
        }

        /// <summary>NameTranslation
        /// <para>
        /// (POST)NameTranslation Endpoint: Returns the translation of a name. You must specify the name to translate and the target language for the translation.
        /// </para>
        /// </summary>
        /// <param name="name">string: Name to be translated</param>
        /// <param name="sourceLanguageOfUse">(string, optional): ISO 639-3 code for the name's language of use</param>
        /// <param name="sourceScript">(string, optional): ISO 15924 code for the name's script</param>
        /// <param name="targetLanguage">(string): ISO 639-3 code for the translation language</param>
        /// <param name="targetScript">(string, optional): ISO 15924 code for the translation script</param>
        /// <param name="targetScheme">(string, optional): transliteration scheme for the translation</param>
        /// <param name="sourceLanguageOfOrigin">(string, optional): ISO 639-3 code for the name's language of origin</param>
        /// <param name="entityType">(string, optional): Entity type of the name: PERSON, LOCATION, or ORGANIZATION</param>
        /// <returns>RosetteResponse containing the results of the request. 
        /// </returns>
        public RosetteResponse NameTranslation(string name, string sourceLanguageOfUse = null, string sourceScript = null, string targetLanguage = null, string targetScript = null, string targetScheme = null, string sourceLanguageOfOrigin = null, string entityType = null) {
            _uri = "name-translation";

            return getResponse(SetupClient(), new JavaScriptSerializer().Serialize(new Dictionary<string, string>(){
                { "name", name},
                { "sourceLanguageOfUse", sourceLanguageOfUse},
                { "sourceScript", sourceScript},
                { "targetLanguage", targetLanguage},
                { "targetScript", targetScript},
                { "targetScheme", targetScheme},
                { "sourceLanguageOfOrigin", sourceLanguageOfOrigin},
                { "entityType", entityType}
            }.Where(f => !String.IsNullOrEmpty(f.Value)).ToDictionary(x => x.Key, x => x.Value)));
        }

        /// <summary>TranslatedName
        /// <para>
        /// (POST)TranslatedName Endpoint: Returns the translation of a name. You must specify the name to translate and the target language for the translation.
        /// </para>
        /// </summary>
        /// <param name="name">string: Name to be translated</param>
        /// <param name="sourceLanguageOfUse">(string, optional): ISO 639-3 code for the name's language of use</param>
        /// <param name="sourceScript">(string, optional): ISO 15924 code for the name's script</param>
        /// <param name="targetLanguage">(string): ISO 639-3 code for the translation language</param>
        /// <param name="targetScript">(string, optional): ISO 15924 code for the translation script</param>
        /// <param name="targetScheme">(string, optional): transliteration scheme for the translation</param>
        /// <param name="sourceLanguageOfOrigin">(string, optional): ISO 639-3 code for the name's language of origin</param>
        /// <param name="entityType">(string, optional): Entity type of the name: PERSON, LOCATION, or ORGANIZATION</param>
        /// <returns>RosetteResponse containing the results of the request. 
        /// </returns>
        [Obsolete("Use NameTranslation")]
        public RosetteResponse TranslatedName(string name, string sourceLanguageOfUse = null, string sourceScript = null, string targetLanguage = null, string targetScript = null, string targetScheme = null, string sourceLanguageOfOrigin = null, string entityType = null) {
            return NameTranslation(name, sourceLanguageOfUse, sourceScript, targetLanguage, targetScript, targetScheme, sourceLanguageOfOrigin, entityType);
        }

        /// <summary>NameTranslation
        /// <para>
        /// (POST)NameTranslation Endpoint: Returns the translation of a name. You must specify the name to translate and the target language for the translation.
        /// </para>
        /// </summary>
        /// <param name="dict">Dictionary&lt;object, object&gt;: Dictionary containing parameters as (key,value) pairs</param>
        /// <returns>RosetteResponse containing the results of the request. </returns>
        public RosetteResponse NameTranslation(Dictionary<object, object> dict) {
            _uri = "name-translation";
            return getResponse(SetupClient(), new JavaScriptSerializer().Serialize(dict));
        }

        /// deprecated
        /// <summary>TranslatedName
        /// <para>
        /// (POST)TranslatedName Endpoint: Returns the translation of a name. You must specify the name to translate and the target language for the translation.
        /// </para>
        /// </summary>
        /// <param name="dict">Dictionary&lt;object, object&gt;: Dictionary containing parameters as (key,value) pairs</param>
        /// <returns>RosetteResponse containing the results of the request. </returns>
        [Obsolete("Use NameTranslation")]
        public RosetteResponse TranslatedName(Dictionary<object, object> dict) {
            return NameTranslation(dict);
        }

        /// <summary>getResponse
        /// <para>
        /// getResponse: Internal function to get the response from the Rosette API server using the request
        /// </para>
        /// </summary>
        /// <param name="client">HttpClient: Client to use to access the Rosette server.</param>
        /// <param name="jsonRequest">(string, optional): Content to use as the request to the server with POST. If none given, assume an Info endpoint and use GET</param>
        /// <param name="multiPart">(MultipartFormDataContent, optional): Used for file uploads</param>
        /// <returns>RosetteResponse</returns>
        private RosetteResponse getResponse(HttpClient client, string jsonRequest = null, MultipartFormDataContent multiPart = null) {
            if (client != null && version_checked) {
                HttpResponseMessage responseMsg = null;
                string wholeURI = _uri;
                if (wholeURI.StartsWith("/")) {
                    wholeURI = wholeURI.Substring(1);
                }

                if (jsonRequest != null) {
                    HttpContent content = new StringContent(jsonRequest);
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    Task<HttpResponseMessage> task = Task.Run<HttpResponseMessage>(async () => await client.PostAsync(wholeURI, content));
                    responseMsg = task.Result;
                }
                else if (multiPart != null) {
                    Task<HttpResponseMessage> task = Task.Run<HttpResponseMessage>(async () => await client.PostAsync(wholeURI, multiPart));
                    responseMsg = task.Result;
                }
                else {
                    Task<HttpResponseMessage> task = Task.Run<HttpResponseMessage>(async () => await client.GetAsync(wholeURI));
                    responseMsg = task.Result;
                }

                RosetteResponse response = new RosetteResponse(responseMsg);

                return response;

            }
            return null;
        }

        /// <summary>Process
        /// <para>
        /// Process: Internal function to convert a RosetteFile into a dictionary to use for getResponse
        /// </para>
        /// </summary>
        /// <param name="file">RosetteFile: File being uploaded to use as a request to the Rosette server.</param>
        /// <returns>RosetteResponse containing the results of the response from the server from the getResponse call.</returns>
        private RosetteResponse Process(RosetteFile file) {
            return getResponse(SetupClient(), null, file.AsMultipart());
        }

        /// <summary>Process
        /// <para>
        /// Process: Internal function to convert a RosetteFile into a dictionary to use for getResponse
        /// </para>
        /// </summary>
        /// <param name="content">(string, optional): Input to process (JSON string or base64 encoding of non-JSON string)</param>
        /// <param name="language">(string, optional): Language: ISO 639-3 code (ignored for the /language endpoint)</param>
        /// <param name="contentType">(string, optional): not used at this time</param>
        /// <param name="contentUri">(string, optional): URI to accessible content (content and contentUri are mutually exclusive)</param>
        /// <returns>RosetteResponse containing the results of the response from the server from the getResponse call.</returns>
        private RosetteResponse Process(string content = null, string language = null, string contentType = null, string contentUri = null) {
            if (content == null) {
                if (contentUri == null) {
                    throw new RosetteException("Must supply one of Content or ContentUri", -3);
                }
            }
            else {
                if (contentUri != null) {
                    throw new RosetteException("Cannot supply both Content and ContentUri", -3);
                }
            }

            Dictionary<string, string> dict = new Dictionary<string, string>(){
                { "language", language},
                { "content", content},
                { "contentUri", contentUri}
            }.Where(f => !String.IsNullOrEmpty(f.Value)).ToDictionary(x => x.Key, x => x.Value);

            return getResponse(SetupClient(), new JavaScriptSerializer().Serialize(dict));
        }

        /// <summary>SetupClient
        /// <para>
        /// SetupClient: Internal function to setup the HttpClient
        /// Uses the Client if one has been set. Otherwise create a new one. 
        /// </para>
        /// </summary>
        /// <returns>HttpClient client to use to access the Rosette server.</returns>
        private HttpClient SetupClient() {
            HttpClient client;
            if (!URIstring.EndsWith("/")) {
                URIstring = URIstring + "/";
            }

            if (Client == null) {
                client =
                    new HttpClient(
                        new HttpClientHandler {
                            AutomaticDecompression = DecompressionMethods.GZip
                                                     | DecompressionMethods.Deflate
                        });
                client.BaseAddress = new Uri(URIstring);
                client.Timeout = new TimeSpan(0, 0, Timeout);
            }
            else {
                client = Client;
                if (client.BaseAddress == null) {
                    client.BaseAddress = new Uri(URIstring);
                }
                if (client.Timeout == TimeSpan.Zero) {
                    client.Timeout = new TimeSpan(0, 0, Timeout);
                }
            }
            try {
                client.DefaultRequestHeaders.Clear();
            }
            catch {
                // exception can be ignored
            }
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("X-RosetteAPI-Key", UserKey);
            if (Debug) {
                client.DefaultRequestHeaders.Add("X-RosetteAPI-Devel", "true");
            }
            client.DefaultRequestHeaders.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("gzip"));
            client.DefaultRequestHeaders.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("deflate"));
            client.DefaultRequestHeaders.Add("User-Agent", "RosetteAPICsharp/" + Version);

            return client;
        }

        /// <summary>checkVersion
        /// <para>
        /// checkVersion: Internal function to check whether or not the version matches the server version
        /// </para>
        /// </summary>
        /// <param name="versionToCheck">(string, optional): Version to check against the server version</param>
        /// <returns>bool: Whether or not the versions match</returns>
        private bool checkVersion(string versionToCheck = null) {
            if (!version_checked || last_version_check.AddDays(1) < DateTime.Now) {
                if (versionToCheck == null) {
                    versionToCheck = Version;
                }
                HttpClient client = SetupClient();
                HttpResponseMessage responseMsg = null;
                int retry = 0;

                while (responseMsg == null || (!responseMsg.IsSuccessStatusCode && retry <= MaxRetry)) {
                    if (retry > 0) {
                        System.Threading.Thread.Sleep(500);
                    }
                    string url = string.Format("info?clientVersion={0}", versionToCheck);
                    HttpContent content = new StringContent(string.Empty);
                    responseMsg = client.PostAsync(url, content).Result;
                    retry = retry + 1;
                }
                string text = "";
                try {
                    text = getMessage(responseMsg)[0];
                }
                catch (RosetteException) {
                    throw;
                }
                var result = new JavaScriptSerializer().Deserialize<dynamic>(text);
                // compatibility with server side is at minor version level of semver
                if (!result["versionChecked"]) {
                    throw new RosetteException("The server version is not compatible with binding version " + versionToCheck, -6);
                }
                else {
                    version_checked = true;
                    last_version_check = DateTime.Now;
                }
            }
            return version_checked;
        }

        /// <summary>getMessage
        /// <para>Helper function to parse out responseMsg based on gzip or not</para>
        /// </summary>
        /// <param name="responseMsg">(HttpResponseMessage): Response Message sent from the server</param>
        /// <returns>(string): Content of the response message</returns>
        private List<string> getMessage(HttpResponseMessage responseMsg) {
            if (responseMsg.IsSuccessStatusCode) {
                byte[] byteArray = responseMsg.Content.ReadAsByteArrayAsync().Result;
                IEnumerator<KeyValuePair<string, IEnumerable<string>>> responseHeadersEnum = responseMsg.Headers.GetEnumerator();
                Dictionary<string, string> responseHeadersDict = new Dictionary<string, string>();
                while (responseHeadersEnum.MoveNext()) {
                    KeyValuePair<string, IEnumerable<string>> pair = responseHeadersEnum.Current;
                    responseHeadersDict.Add(pair.Key, pair.Value.ToArray()[0]);
                }

                if (responseMsg.Content.Headers.ContentEncoding.Contains("gzip") || (byteArray[0] == '\x1f' && byteArray[1] == '\x8b' && byteArray[2] == '\x08')) {
                    byteArray = Decompress(byteArray);
                }
                MemoryStream stream = new MemoryStream(byteArray);
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                List<string> message = new List<string>();
                message.Add(reader.ReadToEnd());
                message.Add(new JavaScriptSerializer().Serialize(responseHeadersDict));
                return message;
            }
            else {
                throw new RosetteException(responseMsg.ReasonPhrase, (int)responseMsg.StatusCode);
            }
        }

        /// <summary>Decompress
        /// <para>Method to decompress GZIP files
        /// Source: http://www.dotnetperls.com/decompress
        /// </para>
        /// </summary>
        /// <param name="gzip">(byte[]): Data in byte form to decompress</param>
        /// <returns>(byte[]) Decompressed data</returns>
        private static byte[] Decompress(byte[] gzip) {
            // Create a GZIP stream with decompression mode.
            // ... Then create a buffer and write into while reading from the GZIP stream.
            using (GZipStream stream = new GZipStream(new MemoryStream(gzip), CompressionMode.Decompress)) {
                const int size = 4096;
                byte[] buffer = new byte[size];
                using (MemoryStream memory = new MemoryStream()) {
                    int count = 0;
                    do {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0) {
                            memory.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);
                    return memory.ToArray();
                }
            }
        }
    }
}





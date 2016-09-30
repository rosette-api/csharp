using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Collections;
using Newtonsoft.Json.Converters;

namespace rosette_api
{
    /// <summary>
    /// A class for representing responses from the syntax/dependencies endpoint of the Rosette API
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class SyntaxDependenciesResponse : RosetteResponse
    {
        private const string DEPENDENCIES = "dependencies";
        private const string TOKENS = "tokens";
        internal const string DEPENDENCY_TYPE = "dependencyType";
        internal const string GOVERNOR_TOKEN_INDEX = "governorTokenIndex";
        internal const string DEPENDENT_TOKEN_INDEX = "dependentTokenIndex";

        /// <summary>
        /// Gets or sets the syntactic dependencies identified by the Rosette API
        /// </summary>
        [JsonProperty(DEPENDENCIES)]
        public List<Dependency> Dependencies { get; set; }

        /// <summary>
        /// Gets or sets the tokens identified by the Rosette API
        /// </summary>
        [JsonProperty(TOKENS)]
        public List<string> Tokens { get; set; }

        /// <summary>
        /// Creates a SyntaxDependenciesResponse from the given apiResult
        /// </summary>
        /// <param name="apiResult">The message from the API</param>
        public SyntaxDependenciesResponse(HttpResponseMessage apiResult)
            : base(apiResult)
        {
            List<Dependency> dependencies = new List<Dependency>();
            JArray enumerableResults = this.ContentDictionary.ContainsKey(DEPENDENCIES) ? this.ContentDictionary[DEPENDENCIES] as JArray : new JArray();
            foreach (JObject result in enumerableResults)
            {
                string dependencyType = result.Properties().Where((p) => p.Name == DEPENDENCY_TYPE).Any() ? result[DEPENDENCY_TYPE].ToString() : null;
                int? governorTokenIndex = result.Properties().Where((p) => p.Name == GOVERNOR_TOKEN_INDEX).Any() ? result[GOVERNOR_TOKEN_INDEX].ToObject<int?>() : null;
                int? dependentTokenIndex = result.Properties().Where((p) => p.Name == DEPENDENT_TOKEN_INDEX).Any() ? result[DEPENDENT_TOKEN_INDEX].ToObject<int?>() : null;
                dependencies.Add(new Dependency(dependencyType, governorTokenIndex, dependentTokenIndex));
            }
            this.Dependencies = dependencies;
            JArray tokensArr = this.ContentDictionary.ContainsKey(TOKENS) ? this.ContentDictionary[TOKENS] as JArray : new JArray();
            this.Tokens = tokensArr != null ? new List<string>(tokensArr.Select<JToken, string>((jToken) => jToken != null ? jToken.ToString() : null)) : null;
        }

        /// <summary>
        /// Creates a SyntaxDependenciesResponse from its components
        /// </summary>
        /// <param name="dependencies">The syntactic dependencies the entire document/input text</param>
        /// <param name="tokens">The tokens found in the input text</param>
        /// <param name="responseHeaders">The response headers returned from the API</param>
        /// <param name="content">The content (the doc and entity sentiments) in Dictionary form</param>
        /// <param name="contentAsJson">The content in JSON form</param>
        public SyntaxDependenciesResponse(List<Dependency> dependencies, List<string> tokens, Dictionary<string, string> responseHeaders, Dictionary<string, object> content, string contentAsJson)
            : base(responseHeaders, content, contentAsJson)
        {
            this.Dependencies = dependencies;
            this.Tokens = tokens;
        }

        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj">The object to compare against</param>
        /// <returns>True if equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is SyntaxDependenciesResponse)
            {
                SyntaxDependenciesResponse other = obj as SyntaxDependenciesResponse;
                List<bool> conditions = new List<bool>() {
                    this.Dependencies != null && other.Dependencies != null ? this.Dependencies.SequenceEqual(other.Dependencies) : this.Dependencies == other.Dependencies,
                    this.Tokens != null && other.Tokens != null ? this.Tokens.SequenceEqual(other.Tokens) : this.Tokens == other.Tokens,
                    this.ResponseHeaders != null && other.ResponseHeaders != null ? this.ResponseHeaders.Equals(other.ResponseHeaders) : this.ResponseHeaders == other.ResponseHeaders,
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
        /// Hashcode override
        /// </summary>
        /// <returns>The hashcode</returns>
        public override int GetHashCode()
        {
            int h0 = this.ResponseHeaders != null ? this.ResponseHeaders.GetHashCode() : 1;
            int h1 = this.Dependencies != null ? this.Dependencies.Aggregate<Dependency, int>(1, (seed, item) => seed ^ item.GetHashCode()) : 1;
            int h2 = this.Tokens != null ? this.Tokens.Aggregate<string, int>(1, (seed, item) => seed ^ item.GetHashCode()) : 1;
            return h0 ^ h1 ^ h2;
        }

        /// <summary>
        /// A class to represent an entity returned by the Sentiment Analysis endpoint of the Rosette API
        /// </summary>
        [JsonObject(MemberSerialization.OptOut)]
        public class Dependency
        {
            /// <summary>
            /// The type of this dependency
            /// </summary>
            [JsonProperty(SyntaxDependenciesResponse.DEPENDENCY_TYPE)]
            public string DependencyType { get; set; }

            /// <summary>
            /// The index of the token that governs the dependency
            /// </summary>
            [JsonProperty(SyntaxDependenciesResponse.GOVERNOR_TOKEN_INDEX)]
            public int? GovernorTokenIndex { get; set; }

            /// <summary>
            /// The index of the token that is associated with this dependency
            /// </summary>
            [JsonProperty(SyntaxDependenciesResponse.DEPENDENT_TOKEN_INDEX)]
            public int? DependentTokenIndex { get; set; }

            /// <summary>
            /// Creates an entity that has a sentiment associated with it
            /// </summary>
            /// <param name="dependencyType">The type of dependency</param>
            /// <param name="governorTokenIndex">The index of the token that governs the dependency</param>
            /// <param name="dependentTokenIndex">The index of the token associated with the dependency</param>
            public Dependency(string dependencyType, int? governorTokenIndex, int? dependentTokenIndex)
            {
                this.DependencyType = dependencyType;
                this.GovernorTokenIndex = governorTokenIndex;
                this.DependentTokenIndex = dependentTokenIndex;
            }

            /// <summary>
            /// Equals override
            /// </summary>
            /// <param name="obj">The object to compare against</param>
            /// <returns>True if equal</returns>
            public override bool Equals(object obj)
            {
                if (obj is Dependency)
                {
                    Dependency other = obj as Dependency;
                    List<bool> conditions = new List<bool>() {
                    this.DependencyType == other.DependencyType,
                    this.GovernorTokenIndex == other.GovernorTokenIndex,
                    this.DependentTokenIndex == other.DependentTokenIndex,
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
                int h0 = this.DependencyType != null ? this.DependencyType.GetHashCode() : 1;
                int h1 = this.GovernorTokenIndex != null ? this.GovernorTokenIndex.GetHashCode() : 1;
                int h2 = this.DependentTokenIndex != null ? this.DependentTokenIndex.GetHashCode() : 1;
                return h0 ^ h1 ^ h2;
            }

            /// <summary>
            /// ToString override.
            /// </summary>
            /// <returns>This RosetteSentimentEntity in JSON form</returns>
            public override string ToString()
            {
                return JsonConvert.SerializeObject(this);
            }
        }
    }
}
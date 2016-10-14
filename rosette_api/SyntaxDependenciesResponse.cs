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
        internal const string SENTENCE_WITH_DEPENDENCIES = "sentenceWithDependencies";
        internal const string TOKENS = "tokens";
        internal const string SENTENCE_START_TOKEN_OFFSET = "startOffset";
        internal const string SENTENCE_END_TOKEN_OFFSET = "endOffset";
        internal const string DEPENDENCIES = "dependencies";
        internal const string DEPENDENCY_TYPE = "dependencyType";
        internal const string GOVERNOR_TOKEN_INDEX = "governorTokenIndex";
        internal const string DEPENDENT_TOKEN_INDEX = "dependentTokenIndex";

        /// <summary>
        /// Gets or sets the syntactic dependencies identified by the Rosette API
        /// </summary>
        [JsonProperty(SENTENCE_WITH_DEPENDENCIES)]
        public List<SentenceWithDependencies> Sentences { get; set; }

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
            List<SentenceWithDependencies> sentences = new List<SentenceWithDependencies>();
            JArray enumerableResults = this.ContentDictionary.ContainsKey(SENTENCE_WITH_DEPENDENCIES) ? this.ContentDictionary[SENTENCE_WITH_DEPENDENCIES] as JArray : new JArray();
            foreach (JObject result in enumerableResults)
            {
                List<Dependency> dependencies = new List<Dependency>();
                JArray depArr = result.Properties().Where((p) => p.Name == DEPENDENCIES).Any() ? result[DEPENDENCIES] as JArray : new JArray();
                foreach (JObject dependency in depArr)
                {
                    string dependencyType = dependency.Properties().Where((p) => p.Name == DEPENDENCY_TYPE).Any() ? dependency[DEPENDENCY_TYPE].ToString() : null;
                    int? governorTokenIndex = dependency.Properties().Where((p) => p.Name == GOVERNOR_TOKEN_INDEX).Any() ? dependency[GOVERNOR_TOKEN_INDEX].ToObject<int?>() : null;
                    int? dependentTokenIndex = dependency.Properties().Where((p) => p.Name == DEPENDENT_TOKEN_INDEX).Any() ? dependency[DEPENDENT_TOKEN_INDEX].ToObject<int?>() : null;
                    dependencies.Add(new Dependency(dependencyType, governorTokenIndex, dependentTokenIndex));
                }
                int? startOffset = result.Properties().Where((p) => p.Name == SENTENCE_START_TOKEN_OFFSET).Any() ? result[SENTENCE_START_TOKEN_OFFSET].ToObject<int?>() : null;
                int? endOffset = result.Properties().Where((p) => p.Name == SENTENCE_END_TOKEN_OFFSET).Any() ? result[SENTENCE_END_TOKEN_OFFSET].ToObject<int?>() : null;
                sentences.Add(new SentenceWithDependencies(startOffset, endOffset, dependencies));
            }
            this.Sentences = sentences;
            JArray tokensArr = this.ContentDictionary.ContainsKey(TOKENS) ? this.ContentDictionary[TOKENS] as JArray : new JArray();
            this.Tokens = tokensArr != null ? new List<string>(tokensArr.Select<JToken, string>((jToken) => jToken != null ? jToken.ToString() : null)) : null;
        }

        /// <summary>
        /// Creates a SyntaxDependenciesResponse from its components
        /// </summary>
        /// <param name="sentences">The syntactic dependencies the entire document/input text</param>
        /// <param name="tokens">The tokens found in the input text</param>
        /// <param name="responseHeaders">The response headers returned from the API</param>
        /// <param name="content">The content (the doc and syntax dependencies) in Dictionary form</param>
        /// <param name="contentAsJson">The content in JSON form</param>
        public SyntaxDependenciesResponse(List<SentenceWithDependencies> sentences, List<string> tokens, Dictionary<string, string> responseHeaders, Dictionary<string, object> content, string contentAsJson)
            : base(responseHeaders, content, contentAsJson)
        {
            this.Sentences = sentences;
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
                    this.Sentences != null && other.Sentences != null ? this.Sentences.SequenceEqual(other.Sentences) : this.Sentences == other.Sentences,
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
            int h1 = this.Sentences != null ? this.Sentences.Aggregate<SentenceWithDependencies, int>(1, (seed, item) => seed ^ item.GetHashCode()) : 1;
            int h2 = this.Tokens != null ? this.Tokens.Aggregate<string, int>(1, (seed, item) => seed ^ item.GetHashCode()) : 1;
            return h0 ^ h1 ^ h2;
        }

        /// <summary>
        /// A class to represent a SentenceWithDependencies returned by the Syntax Dependencies endpoint of the Rosette API
        /// </summary>
        [JsonObject(MemberSerialization.OptOut)]
        public class SentenceWithDependencies
        {
            /// <summary>
            /// A list of dependencies
            /// </summary>
            [JsonProperty(SyntaxDependenciesResponse.DEPENDENCIES)]
            public List<Dependency> Dependencies { get; set; }

            /// <summary>
            /// The start token offset of this sentence
            /// </summary>
            [JsonProperty(SyntaxDependenciesResponse.SENTENCE_START_TOKEN_OFFSET)]
            public int? StartOffset { get; set; }

            /// <summary>
            /// The end token offset of this sentence
            /// </summary>
            [JsonProperty(SyntaxDependenciesResponse.SENTENCE_END_TOKEN_OFFSET)]
            public int? EndOffset { get; set; }

            /// <summary>
            /// Creates a list of sentences that has dependencies associated with it
            /// </summary>
            /// <param name="startOffset">The start token offset of this sentence</param>
            /// <param name="endOffset">The end token offset of this sentence</param>
            /// <param name="dependencies">A list of dependencies</param>
            public SentenceWithDependencies(int? startOffset, int? endOffset, List<Dependency> dependencies)
            {
                this.StartOffset = startOffset;
                this.EndOffset = endOffset;
                this.Dependencies = dependencies;
            }

            /// <summary>
            /// Equals override
            /// </summary>
            /// <param name="obj">The object to compare against</param>
            /// <returns>True if equal</returns>
            public override bool Equals(object obj)
            {
                if (obj is SentenceWithDependencies)
                {
                    SentenceWithDependencies other = obj as SentenceWithDependencies;
                    List<bool> conditions = new List<bool>() {
                    this.Dependencies != null && other.Dependencies != null ? this.Dependencies.SequenceEqual(other.Dependencies) : this.Dependencies == other.Dependencies,
                    this.StartOffset.Equals(other.StartOffset),
                    this.EndOffset.Equals(other.EndOffset),
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
                int h0 = this.Dependencies != null ? this.Dependencies.GetHashCode() : 1;
                int h1 = this.StartOffset != null ? this.StartOffset.GetHashCode() : 1;
                int h2 = this.EndOffset != null ? this.EndOffset.GetHashCode() : 1;
                return h0 ^ h1 ^ h2;
            }

            /// <summary>
            /// ToString override.
            /// </summary>
            /// <returns>This SentenceWithDependencies in JSON form</returns>
            public override string ToString()
            {
                return JsonConvert.SerializeObject(this);
            }
        }

        /// <summary>
        /// A class to represent a dependency returned by the Syntax Dependencies endpoint of the Rosette API
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
            /// Creates a dependency
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
                    this.DependencyType.Equals(other.DependencyType),
                    this.GovernorTokenIndex.Equals(other.GovernorTokenIndex),
                    this.DependentTokenIndex.Equals(other.DependentTokenIndex),
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
            /// <returns>This Dependency in JSON form</returns>
            public override string ToString()
            {
                return JsonConvert.SerializeObject(this);
            }
        }
    }
}
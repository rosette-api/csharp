using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace rosette_api {
    /// <summary>
    /// A class to represent responses from the Topics endpoint of the Analytics API
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class TopicsResponse : RosetteResponse {
        private const string conceptsKey = "concepts";
        private const string keyPhrasesKey = "keyphrases";
        private const string phraseKey = "phrase";
        private const string salienceKey = "salience";
        private const string conceptIdKey = "conceptId";
        /// <summary>
        /// Gets the list of Concepts
        /// </summary>
        [JsonProperty(conceptsKey)]
        public List<Concept> Concepts { get; private set; }
        /// <summary>
        /// Gets the list of Key Phrases
        /// </summary>
        [JsonProperty(keyPhrasesKey)]
        public List<KeyPhrase> KeyPhrases { get; private set; }

        /// <summary>
        /// Creates a TopicsResponse from the given apiResults
        /// </summary>
        /// <param name="apiResults">The message from the API</param>
        public TopicsResponse(HttpResponseMessage apiResults) : base(apiResults) {
            Concepts = new List<Concept>();
            KeyPhrases = new List<KeyPhrase>();

            JArray enumerableResults = this.ContentDictionary.ContainsKey(conceptsKey) ? this.ContentDictionary[conceptsKey] as JArray : new JArray();
            foreach (JObject result in enumerableResults) {
                string phrase = result.Properties().Where((p) => String.Equals(p.Name, phraseKey, StringComparison.OrdinalIgnoreCase)).Any() ? result[phraseKey].ToString() : null;
                double? salience = result.Properties().Where((p) => String.Equals(p.Name, salienceKey)).Any() ? result[salienceKey].ToObject<double?>() : null;
                string conceptId = result.Properties().Where((p) => String.Equals(p.Name, conceptIdKey, StringComparison.OrdinalIgnoreCase)).Any() ? result[conceptIdKey].ToString() : null;

                Concepts.Add(new Concept(phrase, salience, conceptId));
            }
            enumerableResults.Clear();
            enumerableResults = this.ContentDictionary.ContainsKey(keyPhrasesKey) ? this.ContentDictionary[keyPhrasesKey] as JArray : new JArray();
            foreach (JObject result in enumerableResults) {
                string phrase = result.Properties().Where((p) => String.Equals(p.Name, phraseKey, StringComparison.OrdinalIgnoreCase)).Any() ? result[phraseKey].ToString() : null;
                double? salience = result.Properties().Where((p) => String.Equals(p.Name, salienceKey)).Any() ? result[salienceKey].ToObject<double?>() : null;

                KeyPhrases.Add(new KeyPhrase(phrase, salience));
            }
        }

        /// <summary>
        /// Creates a TopicsResponse from its components
        /// </summary>
        /// <param name="concepts">The concepts</param>
        /// <param name="keyPhrases">The key phrases</param>
        /// <param name="responseHeaders">The response headers returned from the API</param>
        /// <param name="content">The content (the tokens) in dictionary form</param>
        /// <param name="contentAsJson">The content in JSON form</param>
        public TopicsResponse(List<Concept> concepts, List<KeyPhrase> keyPhrases, Dictionary<string, string> responseHeaders, Dictionary<string, object> content, string contentAsJson)
            : base(responseHeaders, content, contentAsJson) {
            Concepts = concepts;
            KeyPhrases = keyPhrases;
        }

        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj">The other object to compare against</param>
        /// <returns>True if equal</returns>
        public override bool Equals(object obj) {
            if (obj is TopicsResponse) {
                TopicsResponse other = obj as TopicsResponse;
                List<bool> conditions = new List<bool>() {
                    this.Concepts != null && other.Concepts != null ? this.Concepts.SequenceEqual(other.Concepts) : this.Concepts == other.Concepts,
                    this.KeyPhrases != null && other.KeyPhrases != null ? this.KeyPhrases.SequenceEqual(other.KeyPhrases) : this.KeyPhrases == other.KeyPhrases,
                    this.ResponseHeaders != null && other.ResponseHeaders != null ? this.ResponseHeaders.Equals(other.ResponseHeaders) : this.ResponseHeaders == other.ResponseHeaders,
                    this.GetHashCode() == other.GetHashCode()
                };
                return conditions.All(condition => condition);
            }
            else {
                return false;
            }
        }

        /// <summary>
        /// Hashcode override
        /// </summary>
        /// <returns>The hashcode</returns>
        public override int GetHashCode() {
            int h0 = this.Concepts != null ? this.Concepts.Aggregate<Concept, int>(1, (seed, item) => seed ^ item.GetHashCode()) : 1;
            int h1 = this.KeyPhrases != null ? this.KeyPhrases.Aggregate<KeyPhrase, int>(1, (seed, item) => seed ^ item.GetHashCode()) : 1;
            int h2 = this.ResponseHeaders != null ? this.ResponseHeaders.GetHashCode() : 1;
            return h0 ^ h1 ^ h2;
        }
    }
}


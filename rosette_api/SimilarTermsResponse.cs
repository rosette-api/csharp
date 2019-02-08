using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace rosette_api
{
    /// <summary>
    /// Class for representing responses from the API when the SimilarTerms endpoint has been called
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class SimilarTermsResponse : RosetteResponse
    {
        /// <summary>
        /// Gets the mapping of languages to similar terms
        /// </summary>
        [JsonProperty(similarTermsKey)]
        public IDictionary<string, List<SimilarTerm>> SimilarTerms { get; set; }

        private const String similarTermsKey = "similarTerms";
        /// <summary>
        /// Creates a SimilarTermsResponse from the API's raw output
        /// </summary>
        /// <param name="apiResult">The API's output</param>
        public SimilarTermsResponse(HttpResponseMessage apiResult)
            : base(apiResult)
        {
            if (this.ContentDictionary.ContainsKey(similarTermsKey))
            {
                JObject termsJObj = this.ContentDictionary[similarTermsKey] as JObject;
                this.SimilarTerms = termsJObj.ToObject<Dictionary<string, List<SimilarTerm>>>();
            }
            else
            {
                this.SimilarTerms = new Dictionary<string, List<SimilarTerm>>();
            }
        }

        /// <summary>
        /// Constructs a SimilarTerms Response from a mapping of languages to similar terms, a collection of response headers, and content in a dictionary or content as JSON
        /// </summary>
        /// <param name="similarTerms">The mapping of languages to similar terms</param>
        /// <param name="responseHeaders">The response headers from the API</param>
        /// <param name="content">The content of the response (i.e. the language to terms mapping)</param>
        /// <param name="contentAsJson">The content as a JSON string</param>
        public SimilarTermsResponse(IDictionary<string, List<SimilarTerm>> similarTerms,
            Dictionary<string, string> responseHeaders, Dictionary<string, object> content = null, String contentAsJson = null)
            : base(responseHeaders, content, contentAsJson)
        {
            this.SimilarTerms = similarTerms;
        }

        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns>True if equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is SimilarTermsResponse)
            {
                SimilarTermsResponse other = obj as SimilarTermsResponse;
                List<bool> conditions = new List<bool>() {
                    this.SimilarTerms != null && other.SimilarTerms != null ? this.DictionaryEqual(this.SimilarTerms, other.SimilarTerms) : this.SimilarTerms == other.SimilarTerms,
                    this.ResponseHeaders != null && other.ResponseHeaders != null ? this.ResponseHeaders.Equals(other.ResponseHeaders) : this.ResponseHeaders == other.ResponseHeaders
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
            int h1 = this.SimilarTerms != null ? this.DictionaryHashCode(this.SimilarTerms) : 1;
            return h0 ^ h1;
        }

        /// <summary>
        /// compares two dictionaries for equality
        /// </summary>
        /// <returns>if the two dictionaries are equal</returns>
        private bool DictionaryEqual(IDictionary<string, List<SimilarTerm>> d1, IDictionary<string, List<SimilarTerm>> d2)
        {
            bool equal = false;
            if (d1.Count == d2.Count)
            {
                equal = true;
                foreach (var pair in d1)
                {
                    List<SimilarTerm> value = null;
                    if (d2.TryGetValue(pair.Key, out value))
                    {
                        if (!pair.Value.SequenceEqual(value))
                        {
                            equal = false;
                            break;
                        }
                    }
                    else
                    {
                        equal = false;
                        break;
                    }
                }
            }
            return equal;
        }

        /// <summary>
        /// get the hash code of a dictionary
        /// </summary>
        /// <returns>the hash code</returns>
        private int DictionaryHashCode(IDictionary<string, List<SimilarTerm>> dict)
        {
            List<string> keys = dict.Keys.ToList();
            keys.Sort();
            return keys.Aggregate(1, ((seed, item) => seed ^ dict[item].Aggregate(1, (seed2, item2) => seed2 ^ item2.GetHashCode())));
        }
    }

    /// <summary>
    /// Class for representing a similar term and its properties
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class SimilarTerm
    {

        private const string TERM = "term";
        private const string SIMILARITY = "similarity";

        /// <summary>
        /// Gets or sets the term
        /// </summary>
        [JsonProperty(PropertyName = TERM)]
        public string Term { get; set; }

        /// <summary>
        /// Gets or sets the similarity:
        /// On a range of 0-1, the similarity to the input term
        /// </summary>
        [JsonProperty(PropertyName = SIMILARITY)]
        public Nullable<decimal> Similarity { get; set; }


        /// <summary>
        /// Creates a SimilarTerm from the term and similarity
        /// </summary>
        /// <param name="term">The term</param>
        /// <param name="similarity">The similarity to the input term</param>
        public SimilarTerm(String term, Nullable<decimal> similarity)
        {
            this.Term = term;
            this.Similarity = similarity;
        }

        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns>True if equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is SimilarTerm)
            {
                SimilarTerm other = obj as SimilarTerm;
                List<bool> conditions = new List<bool>() {
                    this.Term == other.Term,
                    this.Similarity == other.Similarity
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
            int h0 = this.Term != null ? this.Term.GetHashCode() : 1;
            int h1 = this.Similarity != null ? this.Similarity.GetHashCode() : 1;
            return h0 ^ h1;
        }

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns>This SimilarTerm in JSON form</returns>
        public override string ToString() {
            return JsonConvert.SerializeObject(this);
        }
    }
}

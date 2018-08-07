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
    /// Class for representing responses from the API when the Categories endpoint has been called
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class CategoriesResponse : RosetteResponse
    {
        /// <summary>
        /// Gets the collection of identified categories
        /// </summary>
        [JsonProperty(PropertyName = categoriesKey)]
        public List<RosetteCategory> Categories { get; set; }

        private const String categoryKey = "label";
        private const String confidenceKey = "confidence";
        private const String scoreKey = "score";
        private const String categoriesKey = "categories";

        /// <summary>
        /// Creates a CategoriesResponse from the API's raw output
        /// </summary>
        /// <param name="apiResult">The API's output</param>
        public CategoriesResponse(HttpResponseMessage apiResult)
            : base(apiResult)
        {
            List<RosetteCategory> categories = new List<RosetteCategory>();
            JArray enumerableResults = this.ContentDictionary.ContainsKey(categoriesKey) ? this.ContentDictionary[categoriesKey] as JArray : new JArray();
            foreach (JObject result in enumerableResults)
            {
                String label = result.Properties().Where<JProperty>((p) => p.Name == categoryKey).Any() ? result[categoryKey].ToString() : null;
                Nullable<decimal> confidence = result.Properties().Where<JProperty>((p) => p.Name == confidenceKey).Any() ? result[confidenceKey].ToObject<decimal?>() : null;
                Nullable<decimal> score = result.Properties().Where<JProperty>((p) => p.Name == scoreKey).Any() ? result[scoreKey].ToObject<decimal?>() : null;
                categories.Add(new RosetteCategory(label, confidence, score));
            }
            this.Categories = categories;
        }

        /// <summary>
        /// Constructs a Categories Response from a list of RosetteCategories, a collection of response headers, and content in a dictionary or content as JSON
        /// </summary>
        /// <param name="categories">The list of RosetteCategories</param>
        /// <param name="responseHeaders">The response headers from the API</param>
        /// <param name="content">The content of the response (i.e. the categories list)</param>
        /// <param name="contentAsJson">The content as a JSON string</param>
        public CategoriesResponse(List<RosetteCategory> categories, Dictionary<string, string> responseHeaders, Dictionary<string, object> content = null, String contentAsJson = null)
            : base(responseHeaders, content, contentAsJson)
        {
            this.Categories = categories;
        }

        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns>True if equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is CategoriesResponse)
            {
                CategoriesResponse other = obj as CategoriesResponse;
                List<bool> conditions = new List<bool>() {
                    this.Categories != null && other.Categories != null ? this.Categories.SequenceEqual(other.Categories) : this.Categories == other.Categories,
                    this.ContentAsJson == other.ContentAsJson,
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
            int h0 = this.Categories != null ? this.Categories.Aggregate<RosetteCategory, int>(1, (seed, item) => seed ^ item.GetHashCode()) : 1;
            int h1 = this.ResponseHeaders != null ? this.ResponseHeaders.GetHashCode() : 1;
            return h0 ^ h1;
        }
    }

    /// <summary>
    /// Class for representing an identified category and its properties
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class RosetteCategory
    {
        private const string LABEL = "label";
        private const string CONFIDENCE = "confidence";
        private const string SCORE = "score";

        /// <summary>
        /// Gets or sets the category label
        /// </summary>
        [JsonProperty(PropertyName = LABEL)]
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the confidence:
        /// On a range of 0-1, the confidence in the categorization
        /// </summary>
        [JsonProperty(PropertyName = CONFIDENCE)]
        public Nullable<decimal> Confidence { get; set; }

		/// <summary>
		/// Gets or sets the raw score:
		/// On a range from -INF to INF, the raw score of the categorization
		/// </summary>
		[JsonProperty(PropertyName = SCORE)]
		public Nullable<decimal> Score { get; set; }

        /// <summary>
        /// Creates a Rosette Category from a category label and confidence
        /// </summary>
        /// <param name="label">The category label</param>
        /// <param name="confidence">The confidence this was the correct category</param>
        /// <param name="score">The raw score of this category prediction
        public RosetteCategory(String label, Nullable<decimal> confidence, Nullable<decimal> score)
        {
            this.Label = label;
            this.Confidence = confidence;
            this.Score = score;
        }

        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns>True if equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is RosetteCategory)
            {
                RosetteCategory other = obj as RosetteCategory;
                List<bool> conditions = new List<bool>() {
                    this.Confidence == other.Confidence,
                    this.Label == other.Label,
                    this.Score == other.Score
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
            int h0 = this.Label != null ? this.Label.GetHashCode() : 1;
            int h1 = this.Confidence != null ? this.Confidence.GetHashCode() : 1;
            int h2 = this.Score != null ? this.Score.GetHashCode() : 1;
            return h0 ^ h1 ^ h2;
        }

        /// <summary>
        /// ToString override.
        /// </summary>
        /// <returns>This category in JSON form</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace rosette_api
{
    /// <summary>
    /// Class for representing responses from the API when the Categories endpoint has been called
    /// </summary>
    public class CategoriesResponse : RosetteResponse
    {
        /// <summary>
        /// Gets the collection of identified categories
        /// </summary>
        public List<RosetteCategory> Categories { get; set; }

        /// <summary>
        /// Gets the response headers returned from the API
        /// </summary>
        public ResponseHeaders ResponseHeaders { get; private set; }

        private const String categoryKey = "label";
        private const String confidenceKey = "confidence";
        private const String categoriesKey = "categories";

        /// <summary>
        /// Creates a CategoriesResponse from the API's raw output
        /// </summary>
        /// <param name="apiResult">The API's output</param>
        public CategoriesResponse(HttpResponseMessage apiResult) :base(apiResult)
        {
            List<RosetteCategory> categories = new List<RosetteCategory>();
            JArray enumerableResults = this.ContentDictionary.ContainsKey(categoriesKey) ? this.ContentDictionary[categoriesKey] as JArray : new JArray();
            foreach (JObject result in enumerableResults)
            {
                String label = result.Properties().Where<JProperty>((p) => p.Name == categoryKey).Any() ? result[categoryKey].ToString() : null;
                Nullable<decimal> confidence = result.Properties().Where<JProperty>((p) => p.Name == confidenceKey).Any() ? result[confidenceKey].ToObject<decimal?>() : null;
                categories.Add(new RosetteCategory(label, confidence));
            }
            this.Categories = categories;
            this.ResponseHeaders = new ResponseHeaders(this.Headers);
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
            this.ResponseHeaders = new ResponseHeaders(responseHeaders);
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

        /// <summary>
        /// ToString override.
        /// </summary>
        /// <returns>This response in JSON form</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            string categoriesString = this.Categories != null ? new StringBuilder("[").Append(String.Join(", ", this.Categories)).Append("]").ToString() : null;
            string responseHeadersString = this.ResponseHeaders != null ? this.ResponseHeaders.ToString() : null;
            builder.Append("\"categories\": ").Append(categoriesString)
                .Append(", responseHeaders: ").Append(responseHeadersString).Append("}");
            return builder.ToString();
        }

        /// <summary>
        /// Gets the content in JSON form
        /// </summary>
        /// <returns>The content in JSON form</returns>
        public string ContentToString()
        {
            StringBuilder builder = new StringBuilder("{");
            string categoriesString = this.Categories != null ? new StringBuilder("[").Append(String.Join(", ", this.Categories)).Append("]").ToString() : null;
            builder.Append("\"categories\": ").Append(categoriesString).Append("}");
            return builder.ToString();
        }
    }

    /// <summary>
    /// Class for representing an identified category and its properties
    /// </summary>
    public class RosetteCategory
    {
        /// <summary>
        /// Gets or sets the category label
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// Gets or sets the confidence:
        /// On a range of 0-1, the confidence in the categorization
        /// </summary>
        public Nullable<decimal> Confidence { get; set; }

        /// <summary>
        /// Creates a Rosette Category from a category label and confidence
        /// </summary>
        /// <param name="label">The category label</param>
        /// <param name="confidence">The confidence this was the correct category</param>
        public RosetteCategory(String label, Nullable<decimal> confidence)
        {
            this.Label = label;
            this.Confidence = confidence;
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
                    this.Label == other.Label 
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
            return h0 ^ h1;
        }

        /// <summary>
        /// ToString override.
        /// </summary>
        /// <returns>This category in JSON form</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder("{");
            string labelString = this.Label != null ? new StringBuilder("\"").Append(this.Label).Append("\"").ToString() : null;
            builder.Append("\"label\": ").Append(labelString).Append(", ")
                .Append("\"confidence\": ").Append(this.Confidence).Append("}");
            return builder.ToString();
        }
    }
}

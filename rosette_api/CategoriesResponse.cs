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
    /// Class for representing responses from the API when the Categories endpoint has been called
    /// </summary>
    public class CategoriesResponse : RosetteResponse
    {
        /// <summary>
        /// Gets the collection of identified categories
        /// </summary>
        public List<RosetteCategory> Categories { get; private set; }

        /// <summary>
        /// Gets the response headers returned from the API
        /// </summary>
        public ResponseHeaders ResponseHeaders { get; private set; }

        internal String categoryKey = "label";
        internal String confidenceKey = "confidence";
        internal String categoriesKey = "categories";

        /// <summary>
        /// Creates a CategoriesResponse from the API's raw output
        /// </summary>
        /// <param name="apiResult">The API's output</param>
        public CategoriesResponse(HttpResponseMessage apiResult) :base(apiResult)
        {
            List<RosetteCategory> categories = new List<RosetteCategory>();
            IEnumerable<Object> enumerableResults = this.Content.ContainsKey(categoriesKey) ? this.Content[categoriesKey] as IEnumerable<Object> : new List<Object>();
            foreach (Object result in enumerableResults)
            {
                Dictionary<string, object> dictResult = result as Dictionary<string, object>;
                String label = dictResult.ContainsKey(categoryKey) ? dictResult[categoryKey] as String : null;
                bool hasConfidence = dictResult.ContainsKey(confidenceKey);
                Nullable<Decimal> confidence = dictResult.ContainsKey(confidenceKey) ? new Nullable<decimal>((decimal)dictResult[confidenceKey]) : null;
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
                    this.Categories.SequenceEqual(other.Categories),
                    this.ContentAsJson == other.ContentAsJson,
                    this.ResponseHeaders.Equals(other.ResponseHeaders)
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
            return this.Categories.GetHashCode() ^ this.ResponseHeaders.GetHashCode();
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
        public String Label { get; set; }
        /// <summary>
        /// Gets or sets the confidence:
        /// On a range of 0-1, the confidence in the categorization
        /// </summary>
        public Nullable<Decimal> Confidence { get; set; }

        /// <summary>
        /// Creates a Rosette Category from a category label and confidence
        /// </summary>
        /// <param name="label">The category label</param>
        /// <param name="confidence">The confidence this was the correct category</param>
        public RosetteCategory(String label, Nullable<Decimal> confidence)
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
            return this.Label.GetHashCode() ^ this.Confidence.GetHashCode();
        }
    }
}

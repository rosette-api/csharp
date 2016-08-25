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
        /// The collection of identified categories
        /// </summary>
        public List<RosetteCategory> Categories;

        /// <summary>
        /// The response headers returned from the API
        /// </summary>
        public ResponseHeaders ResponseHeaders;

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
                Nullable<Double> confidence = dictResult.ContainsKey(confidenceKey) ? dictResult[confidenceKey] as Nullable<Double> : null;
                categories.Add(new RosetteCategory(label, confidence));
            }
            this.Categories = categories;
            this.ResponseHeaders = new ResponseHeaders(apiResult.Headers);
        }

    }

    /// <summary>
    /// Class for representing an identified category and its properties
    /// </summary>
    public class RosetteCategory
    {
        /// <summary>
        /// The category label
        /// </summary>
        public String Label;
        /// <summary>
        /// On a range of 0-1, how confident we are in the categorization
        /// </summary>
        public Nullable<Double> Confidence;

        internal RosetteCategory(String label, Nullable<Double> confidence)
        {
            this.Label = label;
            this.Confidence = confidence;
        }
    }
}

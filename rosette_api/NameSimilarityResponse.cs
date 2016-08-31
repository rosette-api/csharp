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
    /// A class to represnt the results from the Name Similarity endpoint of the Rosette API
    /// </summary>
    public class NameSimilarityResponse : RosetteResponse
    {
        private static string scoreKey = "score";

        /// <summary>
        /// The score, on a range of 0-1, of how closely the names match
        /// </summary>
        public decimal Score;

        /// <summary>
        /// The response headers returned from the API
        /// </summary>
        public ResponseHeaders ResponseHeaders;

        /// <summary>
        /// Creates a NameSimilarityResponse from the given apiResults
        /// </summary>
        /// <param name="apiResults">The message from the API</param>
        public NameSimilarityResponse(HttpResponseMessage apiResults) :base(apiResults)
        {
            if (this.ContentDictionary.ContainsKey(scoreKey))
            {
                this.Score = (decimal)this.ContentDictionary[scoreKey];
            }
            this.ResponseHeaders = new ResponseHeaders(this.Headers);
        }

        /// <summary>
        /// Creates a NameSimilarityResponse from its headers
        /// </summary>
        /// <param name="score">The name simiarity score: 0-1</param>
        /// <param name="responseHeaders">The response headers from the API</param>
        /// <param name="content">The content of the response (the score) in dictionary form</param>
        /// <param name="contentAsJSON">The content in JSON</param>
        public NameSimilarityResponse(decimal score, Dictionary<string, string> responseHeaders, Dictionary<string, object> content, string contentAsJSON) : base(responseHeaders, content, contentAsJSON)
        {
            this.Score = score;
            this.ResponseHeaders = new ResponseHeaders(responseHeaders);
        }

        /// <summary>
        /// Equals override.
        /// </summary>
        /// <param name="obj">The object to compare against</param>
        /// <returns>True if equal.</returns>
        public override bool Equals(object obj)
        {
            if (obj is NameSimilarityResponse)
            {
                NameSimilarityResponse other = obj as NameSimilarityResponse;
                return this.Score == other.Score && this.ResponseHeaders.Equals(other.ResponseHeaders);
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
            return this.Score.GetHashCode() ^ this.ResponseHeaders.GetHashCode();
        }

        /// <summary>
        /// ToString override.  Writes this NameSimilarityResponse in JSON form
        /// </summary>
        /// <returns>The response in JSON form</returns>
        public override string ToString()
        {
            return "{ score: " + this.Score + ", responseHeaders: " + this.ResponseHeaders.ToString() + "}";
        }

        /// <summary>
        /// Writes the content of this response in JSON form.
        /// </summary>
        /// <returns>The content in JSON</returns>
        public string ContentToString()
        {
            return "{\"score\":" + this.Score + "}";
        }
    }
}

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
        Nullable<double> Score;

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
            if (this.Content.ContainsKey(scoreKey))
            {
                this.Score = this.Content[scoreKey] as Nullable<double>;
            }
            this.ResponseHeaders = new ResponseHeaders(apiResults.Headers);
        }
    }
}

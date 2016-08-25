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
    /// A class to represent responses from the Sentence Tagging endpoint of the Rosette API
    /// </summary>
    public class SentenceTaggingResponse : RosetteResponse
    {
        private static string sentencesKey ="sentences";
   
        /// <summary>
        /// The sentences identified by the Rosette API
        /// </summary>
        List<String> Sentences;

        /// <summary>
        /// The response headers returned from the API
        /// </summary>
        public ResponseHeaders ResponseHeaders;

        /// <summary>
        /// Creates a SentenceTaggingResponse from the given apiResults
        /// </summary>
        /// <param name="apiResults">The message from the API</param>
        public SentenceTaggingResponse(HttpResponseMessage apiResults) :base(apiResults)
        {
            this.Sentences = this.Content.ContainsKey(sentencesKey) ? this.Content[sentencesKey] as List<string> : new List<string>();
            this.ResponseHeaders = new ResponseHeaders(apiResults.Headers);
        }
    }
}

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
        private const string sentencesKey ="sentences";
   
        /// <summary>
        /// Gets or sets the sentences identified by the Rosette API
        /// </summary>
        List<String> Sentences { get; set; }

        /// <summary>
        /// Gets the response headers returned from the API
        /// </summary>
        public ResponseHeaders ResponseHeaders { get; private set; }

        /// <summary>
        /// Creates a SentenceTaggingResponse from the given apiResults
        /// </summary>
        /// <param name="apiResults">The message from the API</param>
        public SentenceTaggingResponse(HttpResponseMessage apiResults) :base(apiResults)
        {
            this.Sentences = this.Content.ContainsKey(sentencesKey) ? this.Content[sentencesKey] as List<string> : new List<string>();
            this.ResponseHeaders = new ResponseHeaders(this.Headers);
        }

        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj">The object to compare against</param>
        /// <returns>True if equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is SentenceTaggingResponse)
            {
                SentenceTaggingResponse other = obj as SentenceTaggingResponse;
                List<bool> conditions = new List<bool>() {
                    this.Sentences.SequenceEqual(other.Sentences),
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
        /// HashCode override
        /// </summary>
        /// <returns>The hashcode</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
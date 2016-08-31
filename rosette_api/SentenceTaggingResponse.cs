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
        public List<String> Sentences { get; set; }

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
            object[] sentenceArrOBj = this.ContentDictionary.ContainsKey(sentencesKey) ? this.ContentDictionary[sentencesKey] as object[] : new object[0];
            this.Sentences = sentenceArrOBj.ToList().ConvertAll<string>(new Converter<object, string>(o => o.ToString()));
            this.ResponseHeaders = new ResponseHeaders(this.Headers);
        }

        /// <summary>
        /// Creates a SentenceTaggingResponse from its components
        /// </summary>
        /// <param name="sentences"></param>
        /// <param name="responseHeaders"></param>
        /// <param name="content"></param>
        /// <param name="contentAsJson"></param>
        public SentenceTaggingResponse(List<string> sentences, Dictionary<string, string> responseHeaders, Dictionary<string, object> content, string contentAsJson)
            : base(responseHeaders, content, contentAsJson)
        {
            this.Sentences = sentences;
            this.ResponseHeaders = new ResponseHeaders(responseHeaders);
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
                    this.Sentences != null && other.Sentences != null ? this.Sentences.SequenceEqual(other.Sentences) : this.Sentences == other.Sentences,
                    this.ResponseHeaders != null && other.ResponseHeaders != null ? this.ResponseHeaders.Equals(other.ResponseHeaders) : this.ResponseHeaders == other.ResponseHeaders,
                    this.GetHashCode() == other.GetHashCode()
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
            int h0 = this.Sentences != null ? this.Sentences.Aggregate<string, int>(1, (seed, item) => seed ^ item.GetHashCode()) : 1;
            int h1 = this.ResponseHeaders != null ? this.ResponseHeaders.GetHashCode() : 1;
            return h0 ^ h1;
        }

        /// <summary>
        /// ToString override.
        /// </summary>
        /// <returns>This SentenceTaggingResponse in JSON form</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            string sentencesString = this.Sentences != null ? String.Format("[\"{0}\"]", String.Join<string>("\", \"", this.Sentences)) : null;
            return builder.AppendFormat("{\"sentences\": ", sentencesString)
                .Append(", responseHeaders: ").Append(this.ResponseHeaders).Append("}").ToString();
        }

        /// <summary>
        /// Gets the content of the response in JSON form
        /// </summary>
        /// <returns>The content in JSON form</returns>
        public string ContentToString()
        {
            StringBuilder builder = new StringBuilder();
            string sentencesString = this.Sentences != null ? String.Format("[\"{0}\"]", String.Join<string>("\", \"", this.Sentences)) : null;
            return builder.AppendFormat("{\"sentences\": ", sentencesString).Append("}").ToString();
        }
    }
}
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
    /// A class to represent responses from the Tokenization endpoint of the Rosette API
    /// </summary>
    public class TokenizationResponse : RosetteResponse
    {
        private const string tokensKey = "tokens";

        /// <summary>
        /// Gets the list of tokens
        /// </summary>
        public List<String> Tokens { get; private set; }

        /// <summary>
        /// Gets the response headers returned from the API
        /// </summary>
        public ResponseHeaders ResponseHeaders { get; private set; }

        /// <summary>
        /// Creates a TokenizationResponse from the given apiResults
        /// </summary>
        /// <param name="apiResults">The message from the API</param>
        public TokenizationResponse(HttpResponseMessage apiResults) :base(apiResults)
        {
            if (this.ContentDictionary.ContainsKey(tokensKey))
            {
                object[] tokenObjArr = this.ContentDictionary[tokensKey] as object[];
                this.Tokens = tokenObjArr.ToList().ConvertAll<string>((o) => o.ToString());
            }
            this.ResponseHeaders = new ResponseHeaders(this.Headers);
        }

        /// <summary>
        /// Creates a TokenizationResponse from its components
        /// </summary>
        /// <param name="tokens">The tokens</param>
        /// <param name="responseHeaders">The response headers returned from the API</param>
        /// <param name="content">The content (the tokens) in dictionary form</param>
        /// <param name="contentAsJson">The content in JSON form</param>
        public TokenizationResponse(List<string> tokens, Dictionary<string, string> responseHeaders, Dictionary<string, object> content, string contentAsJson)
            : base(responseHeaders, content, contentAsJson)
        {
            this.Tokens = tokens;
            this.ResponseHeaders = new ResponseHeaders(responseHeaders);
        } 

        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj">The other object to compare against</param>
        /// <returns>True if equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is TokenizationResponse) {
                TokenizationResponse other = obj as TokenizationResponse;
                List<bool> conditions = new List<bool>() {
                    this.Tokens != null && other.Tokens != null ? this.Tokens.SequenceEqual(other.Tokens) : this.Tokens == other.Tokens,
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
        /// Hashcode override
        /// </summary>
        /// <returns>The hashcode</returns>
        public override int GetHashCode()
        {
            int h0 = this.Tokens != null ? this.Tokens.Aggregate<string, int>(1, (seed, item) => seed ^ item.GetHashCode()) : 1;
            int h1 = this.ResponseHeaders != null ? this.ResponseHeaders.GetHashCode() : 1;
            return h0 ^ h1;
        }

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns>This TokenizationResponse in JSON form</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder("{");
            string tokensString = this.Tokens != null ? String.Format("[\"{0}\"]", String.Join("\", \"", this.Tokens)) : null;
            builder.AppendFormat("\"tokens\": {0}", tokensString)
                .Append(", responseHeaders: ").Append(this.ResponseHeaders).Append("}");
            return builder.ToString();
        }

        /// <summary>
        /// Gets the content in JSON form
        /// </summary>
        /// <returns>The content in JSON form</returns>
        public string ContentToString()
        {
            StringBuilder builder = new StringBuilder("{");
            string tokensString = this.Tokens != null ? String.Format("[\"{0}\"]", String.Join("\", \"", this.Tokens)) : null;
            builder.AppendFormat("\"tokens\": {0}", tokensString).Append("}");
            return builder.ToString();
        }
    }
}

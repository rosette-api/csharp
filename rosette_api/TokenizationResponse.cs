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
            this.Tokens = this.Content.ContainsKey(tokensKey) ? this.Content[tokensKey] as List<string> : new List<string>();
            this.ResponseHeaders = new ResponseHeaders(this.Headers);
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
                    this.Tokens.SequenceEqual(other.Tokens),
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
            return this.Tokens.GetHashCode() ^ this.ResponseHeaders.GetHashCode();
        }
    }
}

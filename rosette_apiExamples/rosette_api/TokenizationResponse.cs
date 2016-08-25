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
        private static string tokensKey = "tokens";
        List<String> Tokens;

        /// <summary>
        /// The response headers returned from the API
        /// </summary>
        public ResponseHeaders ResponseHeaders;

        /// <summary>
        /// Creates a TokenizationResponse from the given apiResults
        /// </summary>
        /// <param name="apiResults">The message from the API</param>
        public TokenizationResponse(HttpResponseMessage apiResults) :base(apiResults)
        {
            this.Tokens = this.Content.ContainsKey(tokensKey) ? this.Content[tokensKey] as List<string> : new List<string>();
            this.ResponseHeaders = new ResponseHeaders(apiResults.Headers);
        }
    }
}

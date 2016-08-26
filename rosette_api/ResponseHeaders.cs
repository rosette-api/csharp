using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace rosette_api
{
    /// <summary>
    /// A class to represent the response headers returned by the Rosette API
    /// </summary>
    public class ResponseHeaders
    {
        internal static string responseHeadersKey = "responseHeaders";
        internal static String contentTypeKey = "content-type";
        internal static String dateKey = "date";
        internal static String serverKey = "server";
        internal static string strictTransportSecurityKey = "strict-transport-security";
        internal static string xRosetteAPIAppIDKey = "x-rosetteapi-app-id";
        internal static string xRosetteAPIConcurrencyKey = "x-rosetteapi-concurrency";
        internal static string xRosetteAPIProcessedLanguageKey = "x-rosetteapi-processedlanguage";
        internal static string xRosetteAPIRequestIDKey = "x-rosetteapi-request-id";
        internal static string contentLengthKey = "content-length";
        internal static string connectionKey = "connection";

        /// <summary>
        /// The collection of all resposne headers returned by the Rosette API
        /// </summary>
        public IDictionary<string, string> AllResponseHeaders;

        /// <summary>
        /// The DateTime of the response
        /// </summary>
        public String Date;

        /// <summary>
        /// The language in which the API processed the input text
        /// </summary>
        public String XRosetteAPIProcessedLanguage;

        /// <summary>
        /// Creaqtes a ResponseHeaders object from the given headers
        /// </summary>
        /// <param name="headers">The headers from the API</param>
        public ResponseHeaders(IDictionary<string, string> headers)
        {
            if (headers.ContainsKey(dateKey))
            {
                this.Date = headers[dateKey];
            }
            if (headers.ContainsKey(xRosetteAPIProcessedLanguageKey))
            {
                this.XRosetteAPIProcessedLanguage = headers[xRosetteAPIProcessedLanguageKey];
            }
            this.AllResponseHeaders = headers;
        }

        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj">The object to compare against</param>
        /// <returns>True if equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is ResponseHeaders)
            {
                ResponseHeaders other = obj as ResponseHeaders;
                List<bool> conditions = new List<bool>() {
                    this.AllResponseHeaders.Count == other.AllResponseHeaders.Count &! this.AllResponseHeaders.Except(other.AllResponseHeaders).Any(),
                    (this.Date == null && other.Date == null) || this.Date.Equals(other.Date),
                    (this.XRosetteAPIProcessedLanguage == null && other.XRosetteAPIProcessedLanguage == null) || this.XRosetteAPIProcessedLanguage.Equals(other.XRosetteAPIProcessedLanguage)
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
            return this.AllResponseHeaders.GetHashCode();
        }
    }
}
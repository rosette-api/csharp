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
        public Dictionary<string, object> AllResponseHeaders;

        /// <summary>
        /// The DateTime of the response
        /// </summary>
        public Nullable<DateTime> Date;

        /// <summary>
        /// The language in which the API processed the input text
        /// </summary>
        public String XRosetteAPIProcessedLanguage;

        /// <summary>
        /// Creaqtes a ResponseHeaders object from the given headers
        /// </summary>
        /// <param name="headers">The HttpResponseHeaders from the API</param>
        public ResponseHeaders(HttpResponseHeaders headers)
        {
            Dictionary<string, object> responseHeaders = new Dictionary<string,object>();
            foreach (var header in headers) 
            {
                responseHeaders.Add(header.Key, header.Value);
            }
            if (responseHeaders.ContainsKey(dateKey))
            {
                this.Date = responseHeaders[dateKey] as Nullable<DateTime>;
            }
            if (responseHeaders.ContainsKey(xRosetteAPIProcessedLanguageKey))
            {
                this.XRosetteAPIProcessedLanguage = responseHeaders[xRosetteAPIProcessedLanguageKey] as String;
            }
            this.AllResponseHeaders = responseHeaders;
        }
    }
}

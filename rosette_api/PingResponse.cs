using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace rosette_api
{
    /// <summary>
    /// A class to represent Rosette API responses when Ping() is called
    /// </summary>
    public class PingResponse : RosetteResponse
    {
        private const string messageKey = "message";
        private const string timeKey = "time";

        /// <summary>
        /// Gets the status message
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Gets the time of the response
        /// </summary>
        public Nullable<long> Time { get; private set; }

        /// <summary>
        /// Gets the response headers from the API
        /// </summary>
        public ResponseHeaders ResponseHeaders { get; private set; }

        /// <summary>
        /// Creates a PingResposne from the given apiMsg
        /// </summary>
        /// <param name="apiMsg">The message from the API</param>
        public PingResponse(HttpResponseMessage apiMsg) :base(apiMsg)
        {
            this.Message = this.ContentDictionary.ContainsKey(messageKey) ? this.ContentDictionary[messageKey] as String : null;
            this.Time = this.ContentDictionary.ContainsKey(timeKey) ? this.ContentDictionary[timeKey] as Nullable<long>: null;
            this.ResponseHeaders = new ResponseHeaders(this.Headers);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="time"></param>
        /// <param name="headers"></param>
        /// <param name="content"></param>
        /// <param name="contentAsJson"></param>
        public PingResponse(String message, long time, Dictionary<string, string> headers, Dictionary<string, object> content = null, String contentAsJson = null) 
            : base(headers, content, contentAsJson)
        {
            this.Message = message;
            this.Time = time;
            this.ResponseHeaders = new ResponseHeaders(headers);
        }

        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj">The object to compare against</param>
        /// <returns>True if equal</returns>
        public override bool Equals(Object obj)
        {
            if (obj is PingResponse)
            {
                PingResponse other = obj as PingResponse;
                List<bool> conditions = new List<bool>() {
                    this.Time == other.Time,
                    this.Message == other.Message,
                    this.ResponseHeaders != null && other.ResponseHeaders != null ? this.ResponseHeaders.Equals(other.ResponseHeaders) :this.ResponseHeaders == other.ResponseHeaders
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
        /// <returns>The HashCode</returns>
        public override int GetHashCode()
        {
            int h0 = this.Message != null ? this.Message.GetHashCode() : 1;
            int h1 = this.Time != null ? this.Time.GetHashCode() : 1;
            return h0 ^ h1;
        }

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns>The InfoResponse in JSON form</returns>
        public override string ToString()
        {
            string messageString = this.Message != null ? String.Format("\"{0}\"", this.Message) : null;
            string timeString = this.Time != null ? String.Format("\"{0}\"", this.Time) : null;
            string responseHeadersString = this.ResponseHeaders != null ? this.ResponseHeaders.ToString() : null;
            StringBuilder builder = new StringBuilder("{");
            if (this.Message != null) { builder.AppendFormat("\"{0}\": \"{1}\", ", messageKey, messageString); }
            if (this.Time != null) { builder.AppendFormat("\"{0}\": \"{1}\", ", timeKey, timeString); }
            if (this.ResponseHeaders != null) { builder.AppendFormat("responseHeaders: {0}, ", responseHeadersString); }
            if (builder.Length > 2) { builder.Remove(builder.Length - 2, 2); }
            builder.Append("}");
            return builder.ToString();
        }
    }
}

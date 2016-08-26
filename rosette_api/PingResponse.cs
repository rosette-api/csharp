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
            this.Message = this.Content.ContainsKey(messageKey) ? this.Content[messageKey] as String : null;
            this.Time = this.Content.ContainsKey(timeKey) ? this.Content[timeKey] as Nullable<long>: null;
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
        /// <param name="other">The object to compare against</param>
        /// <returns>True if equal</returns>
        public override bool Equals(Object other)
        {
            if (other is PingResponse)
            {
                PingResponse otherResponse = other as PingResponse;
                List<bool> conditions = new List<bool>() {
                    this.Time == otherResponse.Time,
                    this.Message == otherResponse.Message,
                    this.ResponseHeaders.Equals(otherResponse.ResponseHeaders)
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
            return this.Message.GetHashCode() ^ this.Time.GetHashCode();
        }
    }
}

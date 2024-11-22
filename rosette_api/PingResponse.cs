using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace rosette_api
{
    /// <summary>
    /// A class to represent Analytics API responses when Ping() is called
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class PingResponse : RosetteResponse
    {
        private const string messageKey = "message";
        private const string timeKey = "time";

        /// <summary>
        /// Gets the status message
        /// </summary>
        [JsonProperty(messageKey)]
        public string Message { get; private set; }

        /// <summary>
        /// Gets the time of the response
        /// </summary>
        [JsonProperty(timeKey)]
        public Nullable<long> Time { get; private set; }

        /// <summary>
        /// The entire content of the info response message
        /// </summary>
        [JsonProperty("content")]
        private IDictionary<string, object> AllContent { get { return this.ContentDictionary; } }

        /// <summary>
        /// Creates a PingResposne from the given apiMsg
        /// </summary>
        /// <param name="apiMsg">The message from the API</param>
        public PingResponse(HttpResponseMessage apiMsg) :base(apiMsg)
        {
            this.Message = this.ContentDictionary.ContainsKey(messageKey) ? this.ContentDictionary[messageKey] as String : null;
            this.Time = this.ContentDictionary.ContainsKey(timeKey) ? this.ContentDictionary[timeKey] as Nullable<long>: null;
        }

        /// <summary>
        /// Creates a PingResponse from its components
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="time">The time of the response</param>
        /// <param name="headers">The headers returned by the API</param>
        /// <param name="content">The content in dictionary form</param>
        /// <param name="contentAsJson">The content in JSON compatible string form</param>
        public PingResponse(String message, long time, Dictionary<string, string> headers, Dictionary<string, object> content = null, String contentAsJson = null) 
            : base(headers, content, contentAsJson)
        {
            this.Message = message;
            this.Time = time;
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
    }
}

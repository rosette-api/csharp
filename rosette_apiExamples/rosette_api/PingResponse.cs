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
        /// The status message
        /// </summary>
        public string Message { get; private set; }

        public Nullable<long> Time { get; private set; }

        /// <summary>
        /// Creates a PingResposne from the given apiMsg
        /// </summary>
        /// <param name="apiMsg">The message from the API</param>
        public PingResponse(HttpResponseMessage apiMsg) :base(apiMsg)
        {
            this.Message = this.Content.ContainsKey(messageKey) ? this.Content[messageKey] as String : null;
            this.Time = this.Content.ContainsKey(timeKey) ? this.Content[timeKey] as Nullable<long>: null;
        }
    }
}

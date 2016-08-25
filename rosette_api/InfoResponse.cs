using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace rosette_api
{
    /// <summary>
    /// A class to represent Rosette API responses when Info() is called
    /// </summary>
    public class InfoResponse : RosetteResponse
    {
        private const string nameKey = "name";
        private const string versionKey = "version";
        private const string buildNumberKey = "message";
        private const string buildTimeKey= "time";

        /// <summary>
        /// The status message
        /// </summary>
        public string Name { get; private set; }

        ///<summary>
        /// The status message
        /// </summary>
        public string Version { get; private set; }

        /// <summary>
        /// The status message
        /// </summary>
        public string BuildNumber { get; private set; }

        /// <summary>
        /// The status message
        /// </summary>
        public string BuildTime { get; private set; }

        /// <summary>
        /// Gets the response headers returned from the API
        /// </summary>
        public ResponseHeaders ResponseHeaders { get; private set; }

        /// <summary>
        /// Creates an InfoResponse from the given apiMsg
        /// </summary>
        /// <param name="apiMsg">The message from the API</param>
        public InfoResponse(HttpResponseMessage apiMsg) : base(apiMsg)
        {
            this.Name = this.Content.ContainsKey(nameKey) ? this.Content[nameKey] as String : null;
            this.Version = this.Content.ContainsKey(versionKey) ? this.Content[versionKey] as String : null;
            this.BuildNumber = this.Content.ContainsKey(buildNumberKey) ? this.Content[buildNumberKey] as String : null;
            this.BuildTime = this.Content.ContainsKey(buildTimeKey) ? this.Content[buildTimeKey] as String : null;
            this.ResponseHeaders = new ResponseHeaders(this.Headers);
        }

        public InfoResponse(String name, String version, String buildNumber, String buildTime, Dictionary<string, string> headers, Dictionary<string, object> content = null, String contentAsJson = null) 
            : base(headers, content, contentAsJson)
        {
            this.Name = name;
            this.Version = version;
            this.BuildNumber = buildNumber;
            this.BuildTime = buildTime;
            this.ResponseHeaders = new ResponseHeaders(headers);
        }

        public override bool Equals(Object other)
        {
            if (other is InfoResponse)
            {
                InfoResponse otherResponse = other as InfoResponse;
                List<bool> conditions = new List<bool>() {
                this.BuildNumber == otherResponse.BuildNumber,
                this.BuildTime == otherResponse.BuildTime,
                this.Version == otherResponse.Version,
                this.Name == otherResponse.Name,
                (this.Content.Count == otherResponse.Content.Count &! this.Content.Except(otherResponse.Content).Any()) || this.ContentAsJson == otherResponse.ContentAsJson,
                this.Headers.Count == otherResponse.Headers.Count &! this.Headers.Except(otherResponse.Headers).Any()};
                return conditions.All(condition => condition);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ this.Name.GetHashCode() ^ this.Version.GetHashCode() ^ this.BuildTime.GetHashCode() ^ this.BuildNumber.GetHashCode();
        }
    }
}
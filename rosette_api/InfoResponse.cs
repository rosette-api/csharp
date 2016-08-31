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
            this.Name = this.ContentDictionary.ContainsKey(nameKey) ? this.ContentDictionary[nameKey] as String : null;
            this.Version = this.ContentDictionary.ContainsKey(versionKey) ? this.ContentDictionary[versionKey] as String : null;
            this.BuildNumber = this.ContentDictionary.ContainsKey(buildNumberKey) ? this.ContentDictionary[buildNumberKey] as String : null;
            this.BuildTime = this.ContentDictionary.ContainsKey(buildTimeKey) ? this.ContentDictionary[buildTimeKey] as String : null;
            this.ResponseHeaders = new ResponseHeaders(this.Headers);
        }

        /// <summary>
        /// Creates an InfoResponse from its component.  This consturctor was created for testing purposes and is not intended to be used by customers.
        /// </summary>
        /// <remarks>This consturctor was created for testing purposes and is not intended to be used by customers.</remarks>
        /// <param name="name">The name of the API</param>
        /// <param name="version">The version number of the API</param>
        /// <param name="buildNumber">The build number of the API</param>
        /// <param name="buildTime">The time of the last build</param>
        /// <param name="headers">The headers from the API</param>
        /// <param name="content">The content of the InfoResponse in dictionary form</param>
        /// <param name="contentAsJson">The content of the InfoResponse as JSON</param>
        public InfoResponse(String name, String version, String buildNumber, String buildTime, Dictionary<string, string> headers, Dictionary<string, object> content = null, String contentAsJson = null) 
            : base(headers, content, contentAsJson)
        {
            this.Name = name;
            this.Version = version;
            this.BuildNumber = buildNumber;
            this.BuildTime = buildTime;
            this.ResponseHeaders = new ResponseHeaders(headers);
        }

        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj">The object to compare against</param>
        /// <returns>True if equal</returns>
        public override bool Equals(Object obj)
        {
            if (obj is InfoResponse)
            {
                InfoResponse other = obj as InfoResponse;
                List<bool> conditions = new List<bool>() {
                    this.BuildNumber == other.BuildNumber,
                    this.BuildTime == other.BuildTime,
                    this.Version == other.Version,
                    this.Name == other.Name,
                    this.ResponseHeaders != null && other.ResponseHeaders != null ? this.ResponseHeaders.Equals(other.ResponseHeaders) : this.ResponseHeaders == other.ResponseHeaders
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
            int h0 = this.Name != null ? this.Name.GetHashCode() : 1;
            int h1 = this.Version != null ? this.Version.GetHashCode() : 1;
            int h2 = this.BuildTime != null ? this.BuildTime.GetHashCode() : 1;
            int h3 = this.BuildNumber != null ? this.BuildNumber.GetHashCode() : 1;
            int h4 = this.ResponseHeaders != null ? this.ResponseHeaders.GetHashCode() : 1;
            return h0 ^ h1 ^ h2 ^ h3 ^ h4;
        }

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns>The InfoResponse in JSON form</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder("{");
            if (this.Name != null) { builder.AppendFormat("\"{0}\": \"{1}\", ", nameKey, this.Name); }
            if (this.Version != null) { builder.AppendFormat("\"{0}\": \"{1}\", ", versionKey, this.Version); }
            if (this.BuildNumber != null) { builder.AppendFormat("\"{0}\": \"{1}\", ", buildNumberKey, this.BuildNumber); }
            if (this.BuildTime != null) { builder.AppendFormat("\"{0}\": \"{1}\", ", buildTimeKey, this.BuildTime); }
            if (this.ResponseHeaders != null) { builder.AppendFormat("responseHeaders: {0}, ", this.ResponseHeaders); }
            if (builder.Length > 2) { builder.Remove(builder.Length - 2, 2); }
            builder.Append("}");
            return builder.ToString();
        }
    }
}
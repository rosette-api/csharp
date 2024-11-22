using System;

namespace rosette_api {
    /// <summary>RosetteException Class
    /// <para>
    /// RosetteException: Custom exception to describe an exception from the Analytics API.
    /// </para>
    /// </summary>
    [Serializable]
    public class RosetteException : Exception {
        /// <summary>RosetteException
        /// <para>
        /// RosetteException: Custom exception to describe an exception from the Analytics API.
        /// </para>
        /// </summary>
        /// <param name="message">(string, optional): Message describing exception details</param>
        /// <param name="code">(int, optional): Code number of the exception</param>
        /// <param name="requestid">(string, optional): RequestID if there is one</param>
        /// <param name="file">(string, optional): Filename if in file</param>
        /// <param name="line">(string, optional): Line if in file</param>
        public RosetteException(string message = null, int code = 0, string requestid = null, string file = null, string line = null)
            : base(message) {
            Code = code;
            RequestID = requestid;
            File = file;
            Line = line;
        }

        /// <summary>Code
        /// <para>
        /// Getter, Setter for the Code
        /// Code: Code number of the exception
        /// Allows users to access the Exception Code
        /// </para>
        /// </summary>
        public int Code { get; set; }

        /// <summary>RequestID
        /// <para>
        /// Getter, Setter for the RequestID
        /// RequestID: RequestID if there is one
        /// Allows users to access the Exception RequestID
        /// </para>
        /// </summary>
        public string RequestID { get; set; }

        /// <summary>File
        /// <para>
        /// Getter, Setter for the File
        /// File: Filename if in file
        /// Allows users to access the Exception File if in file
        /// </para>
        /// </summary>
        public string File { get; set; }

        /// <summary>Line
        /// <para>
        /// Getter, Setter for the Line
        /// Line: Line if in file
        /// Allows users to access the Exception Line if in file
        /// </para>
        /// </summary>
        public string Line { get; set; }

        /// <summary>
        /// GetObjectData override
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) {
            base.GetObjectData(info, context);
        }

    }
}

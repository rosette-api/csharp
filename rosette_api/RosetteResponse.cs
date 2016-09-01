using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace rosette_api {
    /// <summary>
    /// Encapsulates the response from RosetteAPI
    /// </summary>  
    public class RosetteResponse {

        /// <summary>
        /// IDictionary of response content
        /// </summary>
        [Obsolete("The structure of this property is subject to change.  Please use the data structures provided in the Response classes that inherit from the RosetteResponse class instead.")]
        public IDictionary<string, object> Content { get; private set; }
        /// <summary>
        /// IDictionary of response content
        /// </summary>
        internal IDictionary<string, object> ContentDictionary { get; private set; }
        /// <summary>
        /// IDictionary of response headers
        /// </summary>
        public IDictionary<string, string> Headers { get; private set; }
        /// <summary>
        /// As returned by the API, the JSON string of response content
        /// </summary>
        public string ContentAsJson { get; private set; }

        /// <summary>
        /// Creates a Rosette Response from its components
        /// </summary>
        /// <param name="headers">The headers from the API</param>
        /// <param name="content">The content of the response in dictionary form</param>
        /// <param name="contentAsJson">The content of the response in JSON</param>
        public RosetteResponse(IDictionary<string, string> headers, IDictionary<string, object> content= null, string contentAsJson = null)
        {
            this.ContentDictionary = content != null ? content : contentAsJson != null ? new JavaScriptSerializer().Deserialize<dynamic>(contentAsJson) : new Dictionary<string, object>();
#pragma warning disable 618
            this.Content = ContentDictionary;
#pragma warning restore 618
            this.Headers = headers != null ? headers : new Dictionary<string, string>();
            this.ContentAsJson = contentAsJson != null ? contentAsJson : content != null ? new JavaScriptSerializer().Serialize(content) : "";
        }

        /// <summary>
        /// RosetteResponse ctor
        /// </summary>
        /// <param name="responseMsg">HttpResponseMessage</param>
        public RosetteResponse(HttpResponseMessage responseMsg) {
            Console.WriteLine("Creating a RosetteResponse");
            ContentDictionary = new Dictionary<string, object>();
            Headers = new Dictionary<string, string>();

            if (responseMsg.IsSuccessStatusCode) {
                foreach (var header in responseMsg.Headers) {
                    Headers.Add(header.Key, string.Join("", header.Value));
                }
                foreach (var header in responseMsg.Content.Headers)
                {
                    Headers.Add(header.Key, string.Join("", header.Value));
                }

                byte[] byteArray = responseMsg.Content.ReadAsByteArrayAsync().Result;
                if (responseMsg.Content.Headers.ContentEncoding.Contains("gzip") || (byteArray[0] == '\x1f' && byteArray[1] == '\x8b' && byteArray[2] == '\x08')) {
                    byteArray = decompress(byteArray);
                }
                using (MemoryStream stream = new MemoryStream(byteArray)) {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8)) {
                        ContentAsJson = reader.ReadToEnd();
                    }
                }
                ContentDictionary = new JavaScriptSerializer().Deserialize<dynamic>(ContentAsJson);
# pragma warning disable 618
                this.Content = ContentDictionary;
# pragma warning restore 618
            }
            else {
                throw new RosetteException(string.Format("{0}: {1}", responseMsg.ReasonPhrase, contentToString(responseMsg.Content)), (int)responseMsg.StatusCode);
            }
        }

        /// <summary>
        /// Reads the httpContent value into a string
        /// </summary>
        /// <param name="httpContent"></param>
        /// <returns></returns>
        internal static string contentToString(HttpContent httpContent) {
            if (httpContent != null) {
                var readAsStringAsync = httpContent.ReadAsStringAsync();
                return readAsStringAsync.Result;
            }
            else {
                return string.Empty;
            }
        }

        private string headersAsString() {
            StringBuilder itemString = new StringBuilder();
            foreach (var item in Headers)
                itemString.AppendFormat("-- {0}:{1} -- ", item.Key, item.Value);

            return itemString.ToString();
        }


        /// <summary>Decompress
        /// <para>Method to decompress GZIP files
        /// Source: http://www.dotnetperls.com/decompress
        /// </para>
        /// </summary>
        /// <param name="gzip">(byte[]): Data in byte form to decompress</param>
        /// <returns>(byte[]) Decompressed data</returns>
        private byte[]decompress(byte[] gzip) {
            // Create a GZIP stream with decompression mode.
            // ... Then create a buffer and write into while reading from the GZIP stream.
            using (GZipStream stream = new GZipStream(new MemoryStream(gzip), CompressionMode.Decompress)) {
                const int size = 4096;
                byte[] buffer = new byte[size];
                using (MemoryStream memory = new MemoryStream()) {
                    int count = 0;
                    do {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0) {
                            memory.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);
                    return memory.ToArray();
                }
            }
        }
    }
}

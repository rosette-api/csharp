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
        public IDictionary<string, object> Content { get; private set; }
        /// <summary>
        /// IDictionary of response header
        /// </summary>
        public IDictionary<string, string> Headers { get; private set; }
        /// <summary>
        /// JSON string of response content
        /// </summary>
        public string ContentAsJson { get; private set; }

        /// <summary>
        /// RosetteResponse ctor
        /// </summary>
        /// <param name="responseMsg">HttpResponseMessage</param>
        public RosetteResponse(HttpResponseMessage responseMsg) {
            Content = new Dictionary<string, object>();
            Headers = new Dictionary<string, string>();

            if (responseMsg.IsSuccessStatusCode) {
                foreach (var header in responseMsg.Headers) {
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
                Content = new JavaScriptSerializer().Deserialize<dynamic>(ContentAsJson);
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
        private string contentToString(HttpContent httpContent) {
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

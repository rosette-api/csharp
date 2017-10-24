using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace rosette_api {
    /// <summary>
    /// Encapsulates the response from RosetteAPI
    /// </summary>  
   [JsonObject(MemberSerialization=MemberSerialization.OptIn)]
    public class RosetteResponse {

        private JsonSerializer Serializer = new JsonSerializer();

        /// <summary>
        /// IDictionary of response content
        /// </summary>
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
        /// The response headers returned from the API
        /// </summary>
        [JsonPropertyAttribute("responseHeaders")]
        public ResponseHeaders ResponseHeaders { get; private set; }
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
            if (content != null)
            {
                this.ContentDictionary = content;
            }
            else if (contentAsJson != null)
            {
                this.ContentDictionary = Serializer.Deserialize<Dictionary<string, object>>(new JsonTextReader(new StringReader(contentAsJson)));
            }
            else
            {
                this.ContentDictionary = new Dictionary<string, object>();
            }
#pragma warning disable 618
            this.Content = ContentDictionary;
#pragma warning restore 618
            this.Headers = headers ?? new Dictionary<string, string>();
            this.ResponseHeaders = new ResponseHeaders(this.Headers);
            if (contentAsJson != null)
            {
                this.ContentAsJson = contentAsJson;
            } 
            else if (content != null) 
            {
                StringBuilder contentAsJsonWriter = new StringBuilder();
                Serializer.ContractResolver = new RosetteResponseContractResolver();
                Serializer.Serialize(new StringWriter(contentAsJsonWriter), content);
                this.ContentAsJson = contentAsJsonWriter.ToString();
            }
        }

        /// <summary>
        /// RosetteResponse ctor
        /// </summary>
        /// <param name="responseMsg">HttpResponseMessage</param>
        public RosetteResponse(HttpResponseMessage responseMsg) {
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
                this.ResponseHeaders = new ResponseHeaders(this.Headers);
                byte[] byteArray = responseMsg.Content.ReadAsByteArrayAsync().Result;
                if(byteArray[0] == '\x1f' && byteArray[1] == '\x8b' && byteArray[2] == '\x08') {
                    byteArray = Decompress(byteArray);
                }
                using (StreamReader reader = new StreamReader(new MemoryStream(byteArray), Encoding.UTF8)) {
                    ContentAsJson = reader.ReadToEnd();
                }

                    this.ContentDictionary = Serializer.Deserialize<Dictionary<string, object>>(new JsonTextReader(new StringReader(this.ContentAsJson)));
# pragma warning disable 618
                this.Content = ContentDictionary;
# pragma warning restore 618
            }
            else {
                throw new RosetteException(string.Format("{0}: {1}", responseMsg.ReasonPhrase, ContentToString(responseMsg.Content)), (int)responseMsg.StatusCode);
            }
        }

        /// <summary>
        /// ToString override.
        /// </summary>
        /// <returns>This response in JSON form</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            JsonWriter writer = new JsonTextWriter(new StringWriter(builder));
            JsonSerializer serializer = JsonSerializer.CreateDefault();
            serializer.ContractResolver = new RosetteResponseContractResolver();
            serializer.Serialize(writer, this);
            return builder.ToString();
        }

        /// <summary>
        /// Gets the content in JSON form
        /// </summary>
        /// <returns>The content in JSON form</returns>
        public virtual string ContentToString()
        {
            StringBuilder builder = new StringBuilder();
            JsonWriter writer = new JsonTextWriter(new StringWriter(builder));
            JsonSerializer serializer = JsonSerializer.CreateDefault();
            serializer.ContractResolver = new RosetteResponseContractResolver();
# pragma warning disable 618
            serializer.Serialize(writer, this.Content);
# pragma warning restore 618
            return builder.ToString();
        }

        /// <summary>
        /// Provides a method for recursively printing the Content dictionary to the console.
        /// </summary>
        public void PrintContent(IDictionary<string, object> content = null) {
            if (content == null) {
                content = Content;
            }

            foreach (var pair in content) {
                if (content[pair.Key].GetType().GetInterfaces().Any(x => x.Name == "IDictionary")) {
                    PrintContent((IDictionary<string, object>)(content[pair.Key]));
                }
                Console.WriteLine("{0}: {1}", pair.Key, pair.Value.ToString());
            }
        }

        /// <summary>
        /// Reads the httpContent value into a string
        /// </summary>
        /// <param name="httpContent"></param>
        /// <returns></returns>
        internal static string ContentToString(HttpContent httpContent) {
            if (httpContent != null) {
                var readAsStringAsync = httpContent.ReadAsStringAsync();
                return readAsStringAsync.Result;
            }
            else {
                return string.Empty;
            }
        }

        private string HeadersAsString() {
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
        private byte[]Decompress(byte[] gzip) {
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

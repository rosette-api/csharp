using LinqToExcel;
using NUnit.Framework;
using rosette_api;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace rosette_apiUnitTests
{
    [TestFixture]
    public class volumeTest {
        // uncomment [Test] to enable. It's not a true unit test, so it shouldn't run under regular conditions, but 
        // it is useful for debugging.
        //[Test]
        public void testSentiment() {
            // provide a complete path to a test xls
            var excel = new ExcelQueryFactory(@"C:\Dev\Bindings\csharp\testdata.xls");
            var worksheetNames = excel.GetWorksheetNames();
            excel.ReadOnly = true;

            foreach (string name in worksheetNames) {
                var data = from cell in excel.Worksheet(name)
                           select cell;
                foreach (int i in Enumerable.Range(0, 10)) {
                    foreach (var cell in data) {
                        CAPI rosetteApi = new CAPI("bogus_key", "https://api.rosette.com/rest/v1", 5);
                        RosetteResponse result = rosetteApi.Sentiment(cell[0].Value.ToString());
                        System.Diagnostics.Debug.WriteLine(result.ContentAsJson);
                    }
                }
            }

        }
    }
    /// <summary>
    /// Live test for multiPart.  To run, uncomment [Test]
    /// </summary>
    [TestFixture]
    public class tempMultiPartTest {
        //[Test]
        public void doTest() {
            string tmpFile = Path.GetTempFileName();
            StreamWriter sw = File.AppendText(tmpFile);
            sw.WriteLine(@"<html><head><title>New Ghostbusters Film</title></head><body><p>Original Ghostbuster Dan Aykroyd, who also co-wrote the 1984 Ghostbusters film, couldn’t be more pleased with the new all-female Ghostbusters cast, telling The Hollywood Reporter, “The Aykroyd family is delighted by this inheritance of the Ghostbusters torch by these most magnificent women in comedy.”</p></body></html>");
            sw.Flush();
            sw.Close();

            RosetteFile rf = new RosetteFile(tmpFile, "text/plain", @"{""language"":""eng""}");

            CAPI rosetteApi = new CAPI("boguskey", "http://seuss.basistech.net:8181/rest/v1", 1);
            RosetteResponse result = rosetteApi.Sentiment(rf);
            System.Diagnostics.Debug.WriteLine(result.ContentAsJson);

            if (File.Exists(tmpFile)) {
                File.Delete(tmpFile);
            }
        }
    }

    [TestFixture]
    public class rosetteResponseTests {
        private string _testHeaderKey;
        private string _testHeaderValue;
        private string _testJson;
        private int  _testItemCount;

        [SetUp]
        public void Init() {
            _testHeaderKey = "X-RosetteAPI-RequestId";
            _testHeaderValue = "123456789";
            _testJson = @"{ ""item1"":""value1"", ""item2"":""value2""}";
            _testItemCount = 2;
        }

        [Test]
        public void RosetteResponse_HeaderTest() {
            HttpResponseMessage message = new HttpResponseMessage {
                StatusCode = (HttpStatusCode)200,
                ReasonPhrase = "OK",
                Content = new StringContent(_testJson)
            };
            message.Headers.Add(_testHeaderKey, _testHeaderValue);
            RosetteResponse rr = new RosetteResponse(message);
            Assert.AreEqual(_testHeaderValue, rr.Headers[_testHeaderKey], "RosetteResponse: header mismatch");
        }

        [Test]
        public void RosetteResponse_ContentTest() {
            HttpResponseMessage message = new HttpResponseMessage {
                StatusCode = (HttpStatusCode)200,
                ReasonPhrase = "OK",
                Content = new StringContent(_testJson)
            };
            message.Headers.Add(_testHeaderKey, _testHeaderValue);
            RosetteResponse rr = new RosetteResponse(message);
# pragma warning disable 618
            Assert.AreEqual(_testItemCount, rr.Content.Count, "RosetteResponse: header mismatch");
# pragma warning restore 618
        }

        [Test]
        public void RosetteResponse_ContentAsJsonTest() {
            HttpResponseMessage message = new HttpResponseMessage {
                StatusCode = (HttpStatusCode)200,
                ReasonPhrase = "OK",
                Content = new StringContent(_testJson)
            };
            message.Headers.Add(_testHeaderKey, _testHeaderValue);
            RosetteResponse rr = new RosetteResponse(message);
            Assert.AreEqual(_testJson, rr.ContentAsJson, "RosetteResponse: json mismatch");
        }

        [Test]
        public void RosetteResponse_ExceptionTest() {
            HttpResponseMessage message = new HttpResponseMessage {
                StatusCode = (HttpStatusCode)404,
                ReasonPhrase = "Not Found",
                Content = new StringContent(_testJson)
            };
            message.Headers.Add(_testHeaderKey, _testHeaderValue);
            try {
                new RosetteResponse(message);
                Assert.Fail("Exception should have been thrown");
            }
            catch (RosetteException ex) {
                Assert.AreEqual(404, ex.Code, "RosetteResponse: Exception mismatch");
            }
        }
    }

    [TestFixture]
    public class rosetteExtensionsTests {
        [Test]
        public void MorphologyEndpointTest() {
            string expected = "han-readings";
            Assert.AreEqual(expected, RosetteExtensions.MorphologyEndpoint(MorphologyFeature.hanReadings), "Morphology endpoint mismatch");
        }
    }

    [TestFixture]
    public class rosette_errorTests : IDisposable {
        bool disposed = false;

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (disposed) {
                return;
            }

            if (disposing) {
                _mockHttp.Dispose();
            }
            disposed = true;
        }

        private MockHttpMessageHandler _mockHttp;
        private CAPI _rosetteApi;
        private string _testUrl = @"https://api.rosette.com/rest/v1/";

        [OneTimeSetUp]
        public void Init() {
            _mockHttp = new MockHttpMessageHandler();
            var client = new HttpClient(_mockHttp);

            string jsonResponse = string.Format("{{'response': 'OK', 'version': '{0}'}}", CAPI.Version);

            _mockHttp.When(_testUrl + "info")
                .WithQueryString(string.Format("clientVersion={0}", CAPI.Version))
                .Respond("applciation/json", jsonResponse);

            _rosetteApi = new CAPI("userkey", null, 1, client);
        }

        [OneTimeTearDown]
        public void Cleanup() {
        }

        [Test]
        public void Error409_Test() {
            try {
                _mockHttp.When(_testUrl + "entities").Respond(HttpStatusCode.Conflict);
                _rosetteApi.Entity("content");
                Assert.Fail("Exception not thrown");
            }
            catch (RosetteException ex) {
                Console.WriteLine("Error code: " + ex.Code);
                Assert.AreEqual(ex.Code, 409);
                return;
            }
            catch (Exception) {
                Assert.Fail("RosetteException not thrown");
                return;
            }
        }
    }

    [TestFixture]
    public class rosette_classTests {
        [Test]
        public void NameClassTest() {
            Name name = new Name("text", "language", "script", "entityType");
            Assert.AreEqual("text", name.text, "Name does not match");
            Assert.AreEqual("language", name.language, "Language does not match");
            Assert.AreEqual("script", name.script, "Script does not match");
            Assert.AreEqual("entityType", name.entityType, "EntityType does not match");
        }

        [Test]
        public void RosetteFileClassTest() {
            string tmpFile = Path.GetTempFileName();
            StreamWriter sw = File.AppendText(tmpFile);
            sw.WriteLine("Rosette API Unit Test");
            sw.Flush();
            sw.Close();
            
            RosetteFile f = new RosetteFile(tmpFile, "application/octet-stream", null);
            Assert.IsNotNull(f.Filename, "Filename is null");
            Assert.AreEqual(tmpFile, f.Filename, "Filename does not match");
            Assert.AreEqual("application/octet-stream", f.ContentType, "ContentType does not match");
            Assert.IsNull(f.Options, "Options does not match");

            byte[] b = f.getFileData();
            Assert.IsTrue(b.Count() > 0, "File is empty");

            string content = f.getFileDataString();
            Assert.IsTrue(content.Length > 0, "File is empty");

            MultipartContent multiPart = f.AsMultipart();
            Assert.IsTrue(multiPart.Headers.Count() > 0, "Multipart not populated");
            f.Dispose();

            if (File.Exists(tmpFile)) {
                File.Delete(tmpFile);
            }
        }

        [Test]
        public void RosetteExceptionClassTest() {
            RosetteException ex = new RosetteException("message", 1, "requestID", "file", "line");
            Assert.AreEqual("message", ex.Message, "Message does not match");
            Assert.AreEqual(1, ex.Code, "Code does not match");
            Assert.AreEqual("requestID", ex.RequestID, "RequestID does not match");
            Assert.AreEqual("file", ex.File, "File does not match");
            Assert.AreEqual("line", ex.Line, "Line does not match");
        }
    }

    [TestFixture]
    public class rosette_apiUnitTests : IDisposable {
        bool disposed = false;

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (disposed) {
                return;
            }

            if (disposing) {
                _mockHttp.Dispose();
            }
            disposed = true;
        }
        /// <summary>Compress
        /// <para>
        /// Takes in byte data and compresses it using gzip.
        /// Source: http://www.dotnetperls.com/compress
        /// </para>
        /// </summary>
        /// <param name="raw">(byte[]): Raw data to be compressed</param>
        /// <returns>(byte[]): Compressed data</returns>
        public static byte[] Compress(byte[] raw) {
            MemoryStream memory = new MemoryStream();
            using (GZipStream gzip = new GZipStream(memory, CompressionMode.Compress, true)) {
                gzip.Write(raw, 0, raw.Length);
            }
            return memory.ToArray();
        }

        /// <summary>Decompress
        /// <para>Method to decompress GZIP files
        /// Source: http://www.dotnetperls.com/decompress
        /// </para>
        /// </summary>
        /// <param name="gzip">(byte[]): Data in byte form to decompress</param>
        /// <returns>(byte[]) Decompressed data</returns>
        private static byte[] Decompress(byte[] gzip) {
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

        private MockHttpMessageHandler _mockHttp;
        private CAPI _rosetteApi;
        private string _testUrl = @"https://api.rosette.com/rest/v1/";
        private string _tmpFile = null;

        [OneTimeSetUp]
        public void Init() {
            // Create a temporary file for use with file testing
            _tmpFile = Path.GetTempFileName();
            StreamWriter sw = File.AppendText(_tmpFile);
            sw.WriteLine("Rosette API Unit Test.  This file is used for testing file operations.");
            sw.Flush();
            sw.Close(); 
            _mockHttp = new MockHttpMessageHandler();
            var client = new HttpClient(_mockHttp);

            string jsonResponse = string.Format("{{'response': 'OK', 'version': '{0}'}}", CAPI.Version);

            _mockHttp.When(_testUrl + "info")
                .WithQueryString(string.Format("clientVersion={0}", CAPI.Version))
                .Respond("applciation/json", jsonResponse);

            _rosetteApi = new CAPI("userkey", null, 1, client);
        }

        [OneTimeTearDown]
        public void Cleanup() {
            if (File.Exists(_tmpFile)) {
                File.Delete(_tmpFile);
            }
            _mockHttp.Clear();
        }

        //------------------------- Simple Options Tests ----------------------------------------


        [Test]
        public void OptionsTest() {
            KeyValuePair<string, string> expected = new KeyValuePair<string, string>("test", "testValue");

            _rosetteApi.SetOption(expected.Key, expected.Value);

            Assert.AreEqual(expected.Value, _rosetteApi.GetOption(expected.Key));
        }

        [Test]
        public void ClearOptionsTest() {
            _rosetteApi.SetOption("option1", "value1");
            _rosetteApi.SetOption("option2", "value2");

            _rosetteApi.ClearOptions();

            Assert.IsNull(_rosetteApi.GetOption("option1"));
        }

        //------------------------- Simple Custom Header Tests ----------------------------------------


        [Test]
        public void CustomHeadersTest() {
            KeyValuePair<string, string> expected = new KeyValuePair<string, string>("X-RosetteAPI-Test", "testValue");

            _rosetteApi.SetCustomHeaders(expected.Key, expected.Value);

            Assert.AreEqual(expected.Value, _rosetteApi.GetCustomHeaders()[expected.Key]);
        }

        [Test]
        public void ClearHeadersTest() {
            _rosetteApi.SetCustomHeaders("X-RosetteAPI-Test", "testValue");

            _rosetteApi.ClearCustomHeaders();

            Assert.IsEmpty(_rosetteApi.GetCustomHeaders());
        }

        [Test]
        public void CheckInvalidCustomHeader() {
            KeyValuePair<string, string> expected = new KeyValuePair<string, string>("Test", "testValue");

            _rosetteApi.SetCustomHeaders(expected.Key, expected.Value);

            try {
                _rosetteApi.Info();
            }
            catch (RosetteException ex) {
                Assert.AreEqual(ex.Message, "Custom header name must begin with \"X-RosetteAPI-\"");
                return;
            }
        }


        //------------------------- Get Calls (Info and Ping) ----------------------------------------

        [Test]
        public void InfoTest() {
            _mockHttp.When(_testUrl + "info")
                .Respond("application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Info();
# pragma warning disable 618
            Assert.AreEqual(response.Content["response"], "OK");
# pragma warning restore 618
        }

        [Test]
        public void InfoTestFull()
        {
            Init();
            string name = "Rosette API";
            string version = "1.2.3";
            string buildNumber = null;
            string buildTime = null;
            string headersAsString = " { \"Content-Type\": \"application/json\", \"date\": \"Thu, 11 Aug 2016 15:47:32 GMT\", \"server\": \"openresty\", \"strict-transport-security\": \"max-age=63072000; includeSubdomains; preload\", \"x-rosetteapi-app-id\": \"1409611723442\", \"x-rosetteapi-concurrency\": \"50\", \"x-rosetteapi-request-id\": \"d4176692-4f14-42d7-8c26-4b2d8f7ff049\", \"content-length\": 72, \"connection\": \"Close\" }";
            Dictionary<string, object> content = new Dictionary<string, object>();
            content.Add("name", name);
            content.Add("version", version);
            content.Add("buildNumber", buildNumber);
            content.Add("buildTime", buildTime);
            Dictionary<string, string> responseHeaders = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(headersAsString);
            HttpResponseMessage mockedMessage = MakeMockedMessage(responseHeaders, HttpStatusCode.OK, new JavaScriptSerializer().Serialize(content));
            _mockHttp.When(_testUrl + "info").Respond(mockedMessage);
            InfoResponse expected = new InfoResponse(name, version, buildNumber, buildTime, responseHeaders, content);
            InfoResponse response = _rosetteApi.Info();
            Assert.AreEqual(expected, response);
        }

        private HttpResponseMessage MakeMockedMessage(Dictionary<string, string> responseHeaders, HttpStatusCode statusCode, String content)
        {
            HttpResponseMessage mockedMessage = new HttpResponseMessage(statusCode);
            mockedMessage.Content = new StringContent(content);
            foreach (KeyValuePair<string, string> header in responseHeaders)
            {
                try
                {
                    mockedMessage.Headers.Add(header.Key, header.Value.ToString());
                }
                catch
                {
                    try
                    {
                        mockedMessage.Content.Headers.Add(header.Key, header.Value.ToString());
                    }
                    catch
                    {
                        switch (header.Key)
                        {
                            case "Content-Type": mockedMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(header.Value);
                                break;
                            case "content-length": mockedMessage.Content.Headers.ContentLength = long.Parse(header.Value);
                                break;
                            default: throw;
                        }
                    }
                }
            }
            return mockedMessage;
        }

        [Test]
        public void PingTestFull() {
            Init();
            string message = "Rosette API at your service.";
            long time = 1470930452887;
            string headersAsString = " { \"Content-Type\": \"application/json\", \"date\": \"Thu, 11 Aug 2016 15:47:32 GMT\", \"server\": \"openresty\", \"strict-transport-security\": \"max-age=63072000; includeSubdomains; preload\", \"x-rosetteapi-app-id\": \"1409611723442\", \"x-rosetteapi-concurrency\": \"50\", \"x-rosetteapi-request-id\": \"d4176692-4f14-42d7-8c26-4b2d8f7ff049\", \"content-length\": \"72\", \"connection\": \"Close\" }";
            Dictionary<string, object> content = new Dictionary<string, object>();
            content.Add("message", message);
            content.Add("time", time);
            Dictionary<string, string> responseHeaders = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(headersAsString);
            HttpResponseMessage mockedMessage = MakeMockedMessage(responseHeaders, HttpStatusCode.OK, new JavaScriptSerializer().Serialize(content));
            _mockHttp.When(_testUrl + "ping").Respond(mockedMessage);
            PingResponse expected = new PingResponse(message, time, responseHeaders, content);
            PingResponse response = _rosetteApi.Ping();
            Assert.AreEqual(expected, response);
        }

        [Test]
        public void PingTest()
        {
            _mockHttp.When(_testUrl + "ping")
                .Respond("application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Ping();
# pragma warning disable 618
            Assert.AreEqual(response.Content["response"], "OK");
# pragma warning restore 618
        }

        //------------------------- Exceptions ----------------------------------------
        
        // Currently, the two exceptions returned by the binding (not server) occur if
        // neither content nor contentUri are provided or both are provided.  Categories is
        // used for convenience, but the tests could be run against almost all of the 
        // endpoints.

        [Test]
        public void NoParams_Test() {
            _mockHttp.When(_testUrl + "categories")
                .Respond("application/json", "{'response': 'OK'}");

            try {
                _rosetteApi.Categories();
            }
            catch (RosetteException ex) {
                Assert.AreEqual(ex.Message, "Must supply one of Content or ContentUri");
                return;
            }
            Assert.Fail("Exception not thrown");
        }

        [Test]
        public void ConflictingParams_Test() {
            _mockHttp.When(_testUrl + "categories")
                .Respond("application/json", "{'response': 'OK'}");

            try {
                _rosetteApi.Categories("content", null, null, "contentUri");
            }
            catch (RosetteException ex) {
                Assert.AreEqual(ex.Message, "Cannot supply both Content and ContentUri");
                return;
            }
            Assert.Fail("Exception not thrown");
        }

        //------------------------- Categories ----------------------------------------

        [Test]
        public void Categories_Content_Test() {
            _mockHttp.When(_testUrl + "categories")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");
            var response = _rosetteApi.Categories("content");
# pragma warning disable 618
            Assert.AreEqual(response.Content["response"], "OK");
# pragma warning restore 618
        }

        [Test]
        public void CategoriesContentTestFull()
        {
            Init();
            JsonSerializer serializer = new JsonSerializer();
            List<RosetteCategory> categories = new List<RosetteCategory>();
            RosetteCategory cat0 = new RosetteCategory("ARTS_AND_ENTERTAINMENT", (decimal)0.23572849069656435);
            categories.Add(cat0);
            string headersAsString = " { \"Content-Type\": \"application/json\", \"date\": \"Thu, 11 Aug 2016 15:47:32 GMT\", \"server\": \"openresty\", \"strict-transport-security\": \"max-age=63072000; includeSubdomains; preload\", \"x-rosetteapi-app-id\": \"1409611723442\", \"x-rosetteapi-concurrency\": \"50\", \"x-rosetteapi-request-id\": \"d4176692-4f14-42d7-8c26-4b2d8f7ff049\", \"content-length\": \"72\", \"connection\": \"Close\" }";
            Dictionary<string, object> content = new Dictionary<string, object>();
            content.Add("categories", categories);
            Dictionary<string, string> responseHeaders = serializer.Deserialize<Dictionary<string, string>>(new JsonTextReader(new StringReader(headersAsString)));
            String mockedContent = "{\"categories\": [ { \"label\": \"" + cat0.Label + "\", \"confidence\": " + cat0.Confidence + "} ] }";
            HttpResponseMessage mockedMessage = MakeMockedMessage(responseHeaders, HttpStatusCode.OK, mockedContent);
            _mockHttp.When(_testUrl + "categories").Respond(mockedMessage);
            CategoriesResponse expected = new CategoriesResponse(categories, responseHeaders, null, mockedContent);
            CategoriesResponse response = _rosetteApi.Categories("Sony Pictures is planning to shoot a good portion of the new \"\"Ghostbusters\"\" in Boston as well.");
            Assert.AreEqual(expected, response);
        }

        [Test]
        public void Categories_Dict_Test() {
            _mockHttp.When(_testUrl + "categories")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Categories(new Dictionary<object, object>(){ {"contentUri", "contentUrl"} });
# pragma warning disable 618
            Assert.AreEqual(response.Content["response"], "OK");
# pragma warning restore 618
        }

        [Test]
        public void Categories_File_Test() {
            _mockHttp.When(_testUrl + "categories")
                .Respond("application/json", "{'response': 'OK'}");

            RosetteFile f = new RosetteFile(_tmpFile);
            var response = _rosetteApi.Categories(f);
# pragma warning disable 618
            Assert.AreEqual(response.Content["response"], "OK");
# pragma warning restore 618
        }

        //------------------------- Entities Linked ----------------------------------------

        [Test]
        public void EntitiesLinked_Content_Test() {
            _mockHttp.When(_testUrl + "entities/linked")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Entity("content", null, null, null, true);
# pragma warning disable 618
            Assert.AreEqual(response.Content["response"], "OK");
# pragma warning restore 618
        }

        [Test]
        public void EntitiesLinked_Dict_Test() {
            _mockHttp.When(_testUrl + "entities/linked")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Entity(new Dictionary<object, object>() { { "contentUri", "contentUrl" } }, true);
# pragma warning disable 618
            Assert.AreEqual(response.Content["response"], "OK");
# pragma warning restore 618
        }

        [Test]
        public void EntitiesLinked_File_Test() {
            _mockHttp.When(_testUrl + "entities/linked")
                .Respond("application/json", "{'response': 'OK'}");

            RosetteFile f = new RosetteFile(_tmpFile);
            var response = _rosetteApi.Entity(f, true);
# pragma warning disable 618
            Assert.AreEqual(response.Content["response"], "OK");
# pragma warning restore 618
        }

        //------------------------- Entity ----------------------------------------
        [Test]
        public void EntityTestFull()
        {
            Init();
            RosetteEntity e0 = new RosetteEntity("Dan Akroyd", "Dan Akroyd", new EntityID("Q105221"), "PERSON", 2);
            RosetteEntity e1 = new RosetteEntity("The Hollywood Reporter", "The Hollywood Reporter", new EntityID("Q61503"), "ORGANIZATION", 1);
            List<RosetteEntity> entities = new List<RosetteEntity>() { e0, e1 };
            string headersAsString = " { \"Content-Type\": \"application/json\", \"date\": \"Thu, 11 Aug 2016 15:47:32 GMT\", \"server\": \"openresty\", \"strict-transport-security\": \"max-age=63072000; includeSubdomains; preload\", \"x-rosetteapi-app-id\": \"1409611723442\", \"x-rosetteapi-concurrency\": \"50\", \"x-rosetteapi-request-id\": \"d4176692-4f14-42d7-8c26-4b2d8f7ff049\", \"content-length\": \"72\", \"connection\": \"Close\" }";
            Dictionary<string, string> responseHeaders = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(headersAsString);
            Dictionary<string, object> content = new Dictionary<string, object>();
            content.Add("entities", entities);
            EntitiesResponse expected = new EntitiesResponse(entities, responseHeaders, content, null);
            String mockedContent = expected.ContentToString();
            HttpResponseMessage mockedMessage = MakeMockedMessage(responseHeaders, HttpStatusCode.OK, mockedContent);
            _mockHttp.When(_testUrl + "entities").Respond(mockedMessage);
            EntitiesResponse response = _rosetteApi.Entity("Original Ghostbuster Dan Aykroyd, who also co-wrote the 1984 Ghostbusters film, couldn’t be more pleased with the new all-female Ghostbusters cast, telling The Hollywood Reporter, “The Aykroyd family is delighted by this inheritance of the Ghostbusters torch by these most magnificent women in comedy.”");
            Assert.AreEqual(expected, response);
        }

        [Test]
        public void Entity_Content_Test() {
            _mockHttp.When(_testUrl + "entities")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Entity("content");
# pragma warning disable 618
            Assert.AreEqual(response.Content["response"], "OK");
# pragma warning restore 618
        }

        [Test]
        public void Entity_Dict_Test() {
            _mockHttp.When(_testUrl + "entities")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Entity(new Dictionary<object, object>() { { "contentUri", "contentUrl" } });
# pragma warning disable 618
            Assert.AreEqual(response.Content["response"], "OK");
# pragma warning restore 618
        }

        [Test]
        public void Entity_File_Test() {
            _mockHttp.When(_testUrl + "entities")
                .Respond("application/json", "{'response': 'OK'}");

            RosetteFile f = new RosetteFile(_tmpFile);
            var response = _rosetteApi.Entity(f);
# pragma warning disable 618
            Assert.AreEqual(response.Content["response"], "OK");
# pragma warning restore 618
        }

        [Test]
        public void EntityIDTestPassOnCreate()
        {
            EntityID pass = new EntityID("Q1");
            pass.ID = "Q1";
            Assert.AreEqual("https://en.wikipedia.org/wiki/Universe", pass.GetWikipedaURL());
        }

        [Test]
        public void EntityIDTestLinkValidOnSet() {
            EntityID tidAtFirst = new EntityID("T423");
            Assert.AreEqual(null, tidAtFirst.GetWikipedaURL());
            tidAtFirst.ID = "Q2";
            Assert.AreEqual("https://en.wikipedia.org/wiki/Earth", tidAtFirst.GetWikipedaURL());
        }

        [Test]
        public void EntityIDLinkNullOnSetToNull()
        {
            EntityID eid = new EntityID(null);
            Assert.AreEqual(null, eid.GetWikipedaURL());
        }

        //------------------------- Language ----------------------------------------

        [Test]
        public void LanguageTestFull()
        {
            Init();
            LanguageDetection lang0 = new LanguageDetection("spa", (decimal)0.38719602327387076);
            LanguageDetection lang1 = new LanguageDetection("eng", (decimal)0.32699986625091865);
            LanguageDetection lang2 = new LanguageDetection("por", (decimal)0.05569054210624943);
            LanguageDetection lang3 = new LanguageDetection("deu", (decimal)0.030069489878380328);
            LanguageDetection lang4 = new LanguageDetection("zho", (decimal)0.23572849069656435);
            LanguageDetection lang5 = new LanguageDetection("swe", (decimal)0.027734757034048835);
            LanguageDetection lang6 = new LanguageDetection("ces", (decimal)0.02583105013400886);
            LanguageDetection lang7 = new LanguageDetection("fin", (decimal)0.23572849069656435);
            LanguageDetection lang8 = new LanguageDetection("fra", (decimal)0.023298946617300347);
            List<LanguageDetection> languageDetections = new List<LanguageDetection>() { lang0, lang1, lang2, lang3, lang4, lang5, lang6, lang7, lang8 };
            string headersAsString = " { \"Content-Type\": \"application/json\", \"date\": \"Thu, 11 Aug 2016 15:47:32 GMT\", \"server\": \"openresty\", \"strict-transport-security\": \"max-age=63072000; includeSubdomains; preload\", \"x-rosetteapi-app-id\": \"1409611723442\", \"x-rosetteapi-concurrency\": \"50\", \"x-rosetteapi-request-id\": \"d4176692-4f14-42d7-8c26-4b2d8f7ff049\", \"content-length\": \"72\", \"connection\": \"Close\" }";
            Dictionary<string, string> responseHeaders = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(headersAsString);
            Dictionary<string, object> content = new Dictionary<string, object>();
            content.Add("languageDetections", languageDetections);
            LanguageIdentificationResponse expected = new LanguageIdentificationResponse(languageDetections, responseHeaders, content, null);
            String mockedContent = expected.ContentToString();
            HttpResponseMessage mockedMessage = MakeMockedMessage(responseHeaders, HttpStatusCode.OK, mockedContent);
            _mockHttp.When(_testUrl + "language").Respond(mockedMessage);
            LanguageIdentificationResponse response = _rosetteApi.Language("Por favor Señorita, says the man.");
            Assert.AreEqual(expected, response);
        }

        [Test]
        public void Language_Content_Test() {
            _mockHttp.When(_testUrl + "language")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Language("content");
# pragma warning disable 618
            Assert.AreEqual(response.Content["response"], "OK");
# pragma warning restore 618
        }

        [Test]
        public void Language_Dict_Test() {
            _mockHttp.When(_testUrl + "language")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Language(new Dictionary<object, object>() { { "contentUri", "contentUrl" } });
# pragma warning disable 618
            Assert.AreEqual(response.Content["response"], "OK");
# pragma warning restore 618
        }

        [Test]
        public void Language_File_Test() {
            _mockHttp.When(_testUrl + "language")
                .Respond("application/json", "{'response': 'OK'}");

            RosetteFile f = new RosetteFile(_tmpFile);
            var response = _rosetteApi.Language(f);
# pragma warning disable 618
            Assert.AreEqual(response.Content["response"], "OK");
# pragma warning restore 618
        }

        //------------------------- Morphology ----------------------------------------
        [Test]
        public void MorphologyTestFullComplete()
        {
            Init();
            MorphologyItem m0 = new MorphologyItem("The", "DET", "the", new List<string>(), new List<string>());
            MorphologyItem m1 = new MorphologyItem("quick", "ADJ", "quick", new List<string>(), new List<string>());
            MorphologyItem m2 = new MorphologyItem("brown", "ADJ", "brown", new List<string>(), new List<string>());
            MorphologyItem m3 = new MorphologyItem("fox", "NOUN", "fox", new List<string>(), new List<string>());
            MorphologyItem m4 = new MorphologyItem("jumped", "VERB", "jump", new List<string>(), new List<string>());
            MorphologyItem m5 = new MorphologyItem(".", "PUNCT", ".", new List<string>(), new List<string>());
            List<MorphologyItem> morphology = new List<MorphologyItem>() { m0, m1, m2, m3, m4, m5 };
            string headersAsString = " { \"Content-Type\": \"application/json\", \"date\": \"Thu, 11 Aug 2016 15:47:32 GMT\", \"server\": \"openresty\", \"strict-transport-security\": \"max-age=63072000; includeSubdomains; preload\", \"x-rosetteapi-app-id\": \"1409611723442\", \"x-rosetteapi-concurrency\": \"50\", \"x-rosetteapi-request-id\": \"d4176692-4f14-42d7-8c26-4b2d8f7ff049\", \"content-length\": \"72\", \"connection\": \"Close\" }";
            Dictionary<string, string> responseHeaders = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(headersAsString);
            Dictionary<string, object> content = new Dictionary<string, object>();
            content.Add("tokens", new List<string>(morphology.Select<MorphologyItem, string>((item) => item.Token)));
            content.Add("posTags", new List<string>(morphology.Select<MorphologyItem, string>((item) => item.PosTag)));
            content.Add("lemmas", new List<string>(morphology.Select<MorphologyItem, string>((item) => item.Lemma)));
            content.Add("compoundComponents", new List<List<string>>(morphology.Select<MorphologyItem, List<string>>((item) => item.CompoundComponents)));
            content.Add("hanReadings", new List<List<string>>(morphology.Select<MorphologyItem, List<string>>((item) => item.HanReadings)));
            MorphologyResponse expected = new MorphologyResponse(morphology, responseHeaders, content, null);
            String mockedContent = expected.ContentAsJson;
            HttpResponseMessage mockedMessage = MakeMockedMessage(responseHeaders, HttpStatusCode.OK, mockedContent);
            _mockHttp.When(_testUrl + "morphology/complete").Respond(mockedMessage);
            MorphologyResponse response = _rosetteApi.Morphology("The quick brown fox jumped.");
            Assert.AreEqual(expected, response);
        }

        [Test]
        public void MorphologyTestFullLemmas()
        {
            Init();
            MorphologyItem m0 = new MorphologyItem("The", null, "the", null, null);
            MorphologyItem m1 = new MorphologyItem("quick", null, "quick", null, null);
            MorphologyItem m2 = new MorphologyItem("brown", null, "brown", null, null);
            MorphologyItem m3 = new MorphologyItem("fox", null, "fox", null, null);
            MorphologyItem m4 = new MorphologyItem("jumped", null, "jump", null, null);
            MorphologyItem m5 = new MorphologyItem(".", null, ".", null, null);
            List<MorphologyItem> morphology = new List<MorphologyItem>() { m0, m1, m2, m3, m4, m5 };
            string headersAsString = " { \"Content-Type\": \"application/json\", \"date\": \"Thu, 11 Aug 2016 15:47:32 GMT\", \"server\": \"openresty\", \"strict-transport-security\": \"max-age=63072000; includeSubdomains; preload\", \"x-rosetteapi-app-id\": \"1409611723442\", \"x-rosetteapi-concurrency\": \"50\", \"x-rosetteapi-request-id\": \"d4176692-4f14-42d7-8c26-4b2d8f7ff049\", \"content-length\": \"72\", \"connection\": \"Close\" }";
            Dictionary<string, string> responseHeaders = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(headersAsString);
            Dictionary<string, object> content = new Dictionary<string, object>();
            content.Add("tokens", new List<string>(morphology.Select<MorphologyItem, string>((item) => item.Token)));;
            content.Add("lemmas", new List<string>(morphology.Select<MorphologyItem, string>((item) => item.Lemma)));
            MorphologyResponse expected = new MorphologyResponse(morphology, responseHeaders, content, null);
            String mockedContent = expected.ContentAsJson;
            HttpResponseMessage mockedMessage = MakeMockedMessage(responseHeaders, HttpStatusCode.OK, mockedContent);
            _mockHttp.When(_testUrl + "morphology/lemmas").Respond(mockedMessage);
            MorphologyResponse response = _rosetteApi.Morphology("The quick brown fox jumped.", feature: MorphologyFeature.lemmas);
            Assert.AreEqual(expected, response);
        }

        [Test]
        public void MorphologyTestFullCompoundComponents()
        {
            Init();
            MorphologyItem m0 = new MorphologyItem("Er", null, null, new List<string>(), null);
            List<string> compoundComponents = new List<string>() { "Rechts", "Schutz", "Versicherungs", "Gesellschaft" };
            MorphologyItem m1 = new MorphologyItem("Rechtsschutzversicherungsgesellschaft", null, null, compoundComponents, null);
            List<MorphologyItem> morphology = new List<MorphologyItem>() { m0, m1 };
            string headersAsString = " { \"Content-Type\": \"application/json\", \"date\": \"Thu, 11 Aug 2016 15:47:32 GMT\", \"server\": \"openresty\", \"strict-transport-security\": \"max-age=63072000; includeSubdomains; preload\", \"x-rosetteapi-app-id\": \"1409611723442\", \"x-rosetteapi-concurrency\": \"50\", \"x-rosetteapi-request-id\": \"d4176692-4f14-42d7-8c26-4b2d8f7ff049\", \"content-length\": \"72\", \"connection\": \"Close\" }";
            Dictionary<string, string> responseHeaders = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(headersAsString);
            Dictionary<string, object> content = new Dictionary<string, object>();
            content.Add("tokens", new List<string>(morphology.Select<MorphologyItem, string>((item) => item.Token))); ;
            content.Add("compoundComponents", new List<List<string>>(morphology.Select<MorphologyItem, List<string>>((item) => item.CompoundComponents)));
            MorphologyResponse expected = new MorphologyResponse(morphology, responseHeaders, content, null);
            String mockedContent = expected.ContentAsJson;
            HttpResponseMessage mockedMessage = MakeMockedMessage(responseHeaders, HttpStatusCode.OK, mockedContent);
            _mockHttp.When(_testUrl + "morphology/compound-components").Respond(mockedMessage);
            MorphologyResponse response = _rosetteApi.Morphology("Er Rechtsschutzversicherungsgesellschaft.", feature: MorphologyFeature.compoundComponents);
            Assert.AreEqual(expected, response);
        }

        [Test]
        public void MorphologyTestFullHanReadings()
        {
            Init();
            List<string> h0 = new List<string>() { "Bei3-jing1-Da4-xue2" };
            List<string> h1 = null;
            List<string> h2 = new List<string>() { "zhu3-ren4" };            
            MorphologyItem m0 = new MorphologyItem("北京大学", null, null, null, h0);
            MorphologyItem m1 = new MorphologyItem("生物系", null, null, null, h1);
            MorphologyItem m2 = new MorphologyItem("主任", null, null, null, h2);
            List<MorphologyItem> morphology = new List<MorphologyItem>() { m0, m1, m2 };
            string headersAsString = " { \"Content-Type\": \"application/json\", \"date\": \"Thu, 11 Aug 2016 15:47:32 GMT\", \"server\": \"openresty\", \"strict-transport-security\": \"max-age=63072000; includeSubdomains; preload\", \"x-rosetteapi-app-id\": \"1409611723442\", \"x-rosetteapi-concurrency\": \"50\", \"x-rosetteapi-request-id\": \"d4176692-4f14-42d7-8c26-4b2d8f7ff049\", \"content-length\": \"72\", \"connection\": \"Close\" }";
            Dictionary<string, string> responseHeaders = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(headersAsString);
            Dictionary<string, object> content = new Dictionary<string, object>();
            content.Add("tokens", new List<string>(morphology.Select<MorphologyItem, string>((item) => item.Token))); ;
            content.Add("hanReadings", new List<List<string>>(morphology.Select<MorphologyItem, List<string>>((item) => item.HanReadings)));
            MorphologyResponse expected = new MorphologyResponse(morphology, responseHeaders, content, null);
            String mockedContent = expected.ContentAsJson;
            HttpResponseMessage mockedMessage = MakeMockedMessage(responseHeaders, HttpStatusCode.OK, mockedContent);
            _mockHttp.When(_testUrl + "morphology/han-readings").Respond(mockedMessage);
            MorphologyResponse response = _rosetteApi.Morphology("北京大学生物系主任.", feature: MorphologyFeature.hanReadings);
            Assert.AreEqual(expected, response);
        }

        [Test]
        public void Morphology_Content_Test() {
            _mockHttp.When(_testUrl + "morphology/complete")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Morphology("content");
# pragma warning disable 618
            Assert.AreEqual(response.Content["response"], "OK");
# pragma warning restore 618
        }

        [Test]
        public void Morphology_Dict_Test() {
            _mockHttp.When(_testUrl + "morphology/complete")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Morphology(new Dictionary<object, object>() { { "contentUri", "contentUrl" } });
# pragma warning disable 618
            Assert.AreEqual(response.Content["response"], "OK");
# pragma warning restore 618
        }

        [Test]
        public void Morphology_File_Test() {
            _mockHttp.When(_testUrl + "morphology/complete")
                .Respond("application/json", "{'response': 'OK'}");

            RosetteFile f = new RosetteFile(_tmpFile);
            var response = _rosetteApi.Morphology(f);
# pragma warning disable 618
            Assert.AreEqual(response.Content["response"], "OK");
# pragma warning restore 618
        }

        //------------------------- Name Similarity ----------------------------------------
        [Test]
        public void NameSimilarityTestFull()
        {
            Init();
            double score = (double)0.9486632809417912;
            string headersAsString = " { \"Content-Type\": \"application/json\", \"date\": \"Thu, 11 Aug 2016 15:47:32 GMT\", \"server\": \"openresty\", \"strict-transport-security\": \"max-age=63072000; includeSubdomains; preload\", \"x-rosetteapi-app-id\": \"1409611723442\", \"x-rosetteapi-concurrency\": \"50\", \"x-rosetteapi-request-id\": \"d4176692-4f14-42d7-8c26-4b2d8f7ff049\", \"content-length\": \"72\", \"connection\": \"Close\" }";
            Dictionary<string, string> responseHeaders = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(headersAsString);
            Dictionary<string, object> content = new Dictionary<string, object>();
            content.Add("score", score);
            NameSimilarityResponse expected = new NameSimilarityResponse(score, responseHeaders, content, null);
            String mockedContent = expected.ContentToString();
            HttpResponseMessage mockedMessage = MakeMockedMessage(responseHeaders, HttpStatusCode.OK, mockedContent);
            _mockHttp.When(_testUrl + "name-similarity").Respond(mockedMessage);
            NameSimilarityResponse response = _rosetteApi.NameSimilarity(new Name("Влади́мир Влади́мирович Пу́тин", "rus", null, "PERSON"), new Name("Vladmir Putin", "eng", null, "PERSON"));
            Assert.AreEqual(expected, response);
        }

        [Test]
        public void NameSimilarity_Content_Test() {
            _mockHttp.When(_testUrl + "name-similarity").Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");
            Name name1 = new Name("Name One");
            Name name2 = new Name("Name Two");
            var response = _rosetteApi.NameSimilarity(name1, name2);
# pragma warning disable 618
            Assert.AreEqual(response.Content["response"], "OK");
# pragma warning restore 618
        }

        [Test]
        public void NameSimilarity_Dict_Test() {
            _mockHttp.When(_testUrl + "name-similarity")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.NameSimilarity(new Dictionary<object, object>() { { "name1", "Name One" }, { "name2", "Name Two" } });
# pragma warning disable 618
            Assert.AreEqual(response.Content["response"], "OK");
# pragma warning restore 618
        }

        //------------------------- Relationships ----------------------------------------
        [Test]
        public void RelationshipsTestFull()
        {
            Init();
            decimal confidence = (decimal)0.8541343114184464;
            string predicate = "be filmed";
            string arg1 = "The Ghostbusters movie";
            string arg1ID = "Q20120108";
            string loc1 = "in Boston";
            List<string> locatives = new List<string>() {loc1};
            string source = "rules:3";
            HashSet<string> modalities = new HashSet<string>() {"assertion", "someOtherModality"};
            string headersAsString = " { \"Content-Type\": \"application/json\", \"date\": \"Thu, 11 Aug 2016 15:47:32 GMT\", \"server\": \"openresty\", \"strict-transport-security\": \"max-age=63072000; includeSubdomains; preload\", \"x-rosetteapi-app-id\": \"1409611723442\", \"x-rosetteapi-concurrency\": \"50\", \"x-rosetteapi-request-id\": \"d4176692-4f14-42d7-8c26-4b2d8f7ff049\", \"content-length\": \"72\", \"connection\": \"Close\" }";
            Dictionary<string, string> responseHeaders = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(headersAsString);
            List<RosetteRelationship> relationships = new List<RosetteRelationship>() {
                new RosetteRelationship(predicate, new Dictionary<int, string>() {{1, arg1}}, new Dictionary<int, string>() {{1, arg1ID}}, null, locatives, null, confidence, source, modalities)
            };
            Dictionary<string, object> content = new Dictionary<string, object>();
            content.Add("relationships", relationships);
            RelationshipsResponse expected = new RelationshipsResponse(relationships, responseHeaders, content, null);
            String mockedContent = expected.ContentToString();
            HttpResponseMessage mockedMessage = MakeMockedMessage(responseHeaders, HttpStatusCode.OK, mockedContent);
            _mockHttp.When(_testUrl + "relationships").Respond(mockedMessage);
            RelationshipsResponse response = _rosetteApi.Relationships("The Ghostbusters movie was filmed in Boston.");
            Assert.AreEqual(expected, response);
        }

        [Test]
        public void RelationshipsTestFullNoArgID()
        {
            Init();
            decimal confidence = (decimal)0.8541343114184464;
            string predicate = "be filmed";
            string arg1 = "The Ghostbusters movie";
            string loc1 = "in Boston";
            List<string> locatives = new List<string>() { loc1 };
            string source = "rules:3";
            HashSet<string> modalities = new HashSet<string>() { "assertion", "someOtherModality" };
            string headersAsString = " { \"Content-Type\": \"application/json\", \"date\": \"Thu, 11 Aug 2016 15:47:32 GMT\", \"server\": \"openresty\", \"strict-transport-security\": \"max-age=63072000; includeSubdomains; preload\", \"x-rosetteapi-app-id\": \"1409611723442\", \"x-rosetteapi-concurrency\": \"50\", \"x-rosetteapi-request-id\": \"d4176692-4f14-42d7-8c26-4b2d8f7ff049\", \"content-length\": \"72\", \"connection\": \"Close\" }";
            Dictionary<string, string> responseHeaders = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(headersAsString);
            List<RosetteRelationship> relationships = new List<RosetteRelationship>() {
                new RosetteRelationship(predicate, new Dictionary<int, string>() {{1, arg1}}, new Dictionary<int, string>(), null, locatives, null, confidence, source, modalities)
            };
            Dictionary<string, object> content = new Dictionary<string, object>();
            content.Add("relationships", relationships);
            RelationshipsResponse expected = new RelationshipsResponse(relationships, responseHeaders, content, null);
            String mockedContent = expected.ContentToString();
            HttpResponseMessage mockedMessage = MakeMockedMessage(responseHeaders, HttpStatusCode.OK, mockedContent);
            _mockHttp.When(_testUrl + "relationships").Respond(mockedMessage);
            RelationshipsResponse response = _rosetteApi.Relationships("The Ghostbusters movie was filmed in Boston.");
            Assert.AreEqual(expected, response);
        }

        [Test]
        public void Relationships_All_Constructors_Equal_Test()
        {
            Init();
            decimal confidence = (decimal)0.8541343114184464;
            string predicate = "be filmed";
            string arg1 = "The Ghostbusters movie";
            string loc1 = "in Boston";
            List<string> locatives = new List<string>() {loc1};
            string headersAsString = " { \"Content-Type\": \"application/json\", \"date\": \"Thu, 11 Aug 2016 15:47:32 GMT\", \"server\": \"openresty\", \"strict-transport-security\": \"max-age=63072000; includeSubdomains; preload\", \"x-rosetteapi-app-id\": \"1409611723442\", \"x-rosetteapi-concurrency\": \"50\", \"x-rosetteapi-request-id\": \"d4176692-4f14-42d7-8c26-4b2d8f7ff049\", \"content-length\": \"72\", \"connection\": \"Close\" }";
            Dictionary<string, string> responseHeaders = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(headersAsString);
            RosetteRelationship relationshipFromOriginalConstructor = new RosetteRelationship(predicate, new List<string>() {arg1}, null, locatives, null, confidence); 
            RosetteRelationship relationshipFromDoubleDictConstructor = new RosetteRelationship(predicate, new Dictionary<int, string>() {{1, arg1}}, new Dictionary<int, string>(), null, locatives, null, confidence, null, null);
            RosetteRelationship relationshipFromFullArgumentsConstructor = new RosetteRelationship(predicate, new List<Argument>() {new Argument(1, arg1, null)}, null, locatives, null, confidence, null, null); 
            Assert.True(relationshipFromOriginalConstructor.Equals(relationshipFromDoubleDictConstructor) && relationshipFromDoubleDictConstructor.Equals(relationshipFromFullArgumentsConstructor) &&
                relationshipFromOriginalConstructor.ToString().Equals(relationshipFromDoubleDictConstructor.ToString()) && relationshipFromDoubleDictConstructor.ToString().Equals(relationshipFromFullArgumentsConstructor.ToString()));
        }

        [Test]
        public void Relationships_Content_Test() {
            _mockHttp.When(_testUrl + "relationships")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Relationships("content");
# pragma warning disable 618
            Assert.AreEqual(response.Content["response"], "OK");
# pragma warning restore 618
        }

        [Test]
        public void Relationships_Dict_Test() {
            _mockHttp.When(_testUrl + "relationships")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Relationships(new Dictionary<object, object>() { { "contentUri", "contentUrl" } });
# pragma warning disable 618
            Assert.AreEqual(response.Content["response"], "OK");
# pragma warning restore 618
        }

        [Test]
        public void Relationships_File_Test() {
            _mockHttp.When(_testUrl + "relationships")
                .Respond("application/json", "{'response': 'OK'}");

            RosetteFile f = new RosetteFile(_tmpFile);
            var response = _rosetteApi.Relationships(f);
# pragma warning disable 618
            Assert.AreEqual(response.Content["response"], "OK");
# pragma warning restore 618
        }

        //------------------------- Text Embedding ----------------------------------------
        [Test]
        public void TextEmbeddingTestFull()
        {
            Init();
            List<double> vector = new List<double>() {0.02164695, 0.0032850206, 0.0038508752, -0.009704393, -0.0016203842};
            string headersAsString = " { \"Content-Type\": \"application/json\", \"date\": \"Thu, 11 Aug 2016 15:47:32 GMT\", \"server\": \"openresty\", \"strict-transport-security\": \"max-age=63072000; includeSubdomains; preload\", \"x-rosetteapi-app-id\": \"1409611723442\", \"x-rosetteapi-concurrency\": \"50\", \"x-rosetteapi-request-id\": \"d4176692-4f14-42d7-8c26-4b2d8f7ff049\", \"content-length\": \"72\", \"connection\": \"Close\" }";
            Dictionary<string, string> responseHeaders = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(headersAsString);
            Dictionary<string, object> content = new Dictionary<string, object>();
            content.Add("embedding", vector);
            TextEmbeddingResponse expected = new TextEmbeddingResponse(vector, responseHeaders, content, null);
            String mockedContent = expected.ContentToString();
            HttpResponseMessage mockedMessage = MakeMockedMessage(responseHeaders, HttpStatusCode.OK, mockedContent);
            _mockHttp.When(_testUrl + "text-embedding").Respond(mockedMessage);
            TextEmbeddingResponse response = _rosetteApi.TextEmbedding("The Ghostbusters movie was filmed in Boston.");
            Assert.AreEqual(expected, response);
        }

        [Test]
        public void TextEmbedding_Content_Test()
        {
            _mockHttp.When(_testUrl + "text-embedding")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.TextEmbedding("content");
# pragma warning disable 618
            Assert.AreEqual(response.Content["response"], "OK");
# pragma warning restore 618
        }

        [Test]
        public void TextEmbedding_Dict_Test()
        {
            _mockHttp.When(_testUrl + "text-embedding")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.TextEmbedding(new Dictionary<object, object>() { { "contentUri", "contentUrl" } });
# pragma warning disable 618
            Assert.AreEqual(response.Content["response"], "OK");
# pragma warning restore 618
        }

        [Test]
        public void TextEmbedding_File_Test()
        {
            _mockHttp.When(_testUrl + "text-embedding")
                .Respond("application/json", "{'response': 'OK'}");

            RosetteFile f = new RosetteFile(_tmpFile);
            var response = _rosetteApi.TextEmbedding(f);
# pragma warning disable 618
            Assert.AreEqual(response.Content["response"], "OK");
# pragma warning restore 618
        }

        //------------------------- Sentences ----------------------------------------

        [Test]
        public void SentencesTestFull()
        {
            Init();
            string s0 = "This land is your land. ";
            string s1 = "This land is my land\nFrom California to the New York island;\nFrom the red wood forest to the Gulf Stream waters\n\nThis land was made for you and Me.\n\n";
            string s2 = "As I was walking that ribbon of highway,\nI saw above me that endless skyway:\nI saw below me that golden valley:\nThis land was made for you and me.";
            List<string> sentences = new List<string>() { s0, s1, s2 };
            string headersAsString = " { \"Content-Type\": \"application/json\", \"date\": \"Thu, 11 Aug 2016 15:47:32 GMT\", \"server\": \"openresty\", \"strict-transport-security\": \"max-age=63072000; includeSubdomains; preload\", \"x-rosetteapi-app-id\": \"1409611723442\", \"x-rosetteapi-concurrency\": \"50\", \"x-rosetteapi-request-id\": \"d4176692-4f14-42d7-8c26-4b2d8f7ff049\", \"content-length\": \"72\", \"connection\": \"Close\" }";
            Dictionary<string, string> responseHeaders = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(headersAsString);
            Dictionary<string, object> content = new Dictionary<string, object>();
            content.Add("sentences", sentences);
            SentenceTaggingResponse expected = new SentenceTaggingResponse(sentences, responseHeaders, content, null);
            String mockedContent = expected.ContentToString();
            HttpResponseMessage mockedMessage = MakeMockedMessage(responseHeaders, HttpStatusCode.OK, mockedContent);
            _mockHttp.When(_testUrl + "sentences").Respond(mockedMessage);
            SentenceTaggingResponse response = _rosetteApi.Sentences("This land is your land. This land is my land\\nFrom California to the New York island;\\nFrom the red wood forest to the Gulf Stream waters\\n\\nThis land was made for you and Me.\\n\\nAs I was walking that ribbon of highway,\\nI saw above me that endless skyway:\\nI saw below me that golden valley:\\nThis land was made for you and me.");
            Assert.AreEqual(expected, response);
        }

        [Test]
        public void Sentences_Content_Test() {
            _mockHttp.When(_testUrl + "sentences")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Sentences("content");
# pragma warning disable 618
            Assert.AreEqual(response.Content["response"], "OK");
# pragma warning restore 618
        }

        [Test]
        public void Sentences_Dict_Test() {
            _mockHttp.When(_testUrl + "sentences")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Sentences(new Dictionary<object, object>() { { "contentUri", "contentUrl" } });
# pragma warning disable 618
            Assert.AreEqual(response.Content["response"], "OK");
# pragma warning restore 618
        }

        [Test]
        public void Sentences_File_Test() {
            _mockHttp.When(_testUrl + "sentences")
                .Respond("application/json", "{'response': 'OK'}");

            RosetteFile f = new RosetteFile(_tmpFile);
            var response = _rosetteApi.Sentences(f);
# pragma warning disable 618
            Assert.AreEqual(response.Content["response"], "OK");
# pragma warning restore 618
        }

        //------------------------- Sentiment ----------------------------------------
        [Test]
        public void SentimentTestFull()
        {
            Init();
            SentimentResponse.RosetteSentiment docSentiment = new SentimentResponse.RosetteSentiment("pos", (decimal)0.7962072011038756);
            RosetteSentimentEntity e0 = new RosetteSentimentEntity("Dan Akroyd", "Dan Akroyd", new EntityID("Q105221"), "PERSON", 2, "neg", (decimal)0.5005508052749595);
            RosetteSentimentEntity e1 = new RosetteSentimentEntity("The Hollywood Reporter", "The Hollywood Reporter", new EntityID("Q61503"), "ORGANIZATION", 1, "pos", (decimal)0.5338094035254866);
            List<RosetteSentimentEntity> entities = new List<RosetteSentimentEntity>() { e0, e1 };
            string headersAsString = " { \"Content-Type\": \"application/json\", \"date\": \"Thu, 11 Aug 2016 15:47:32 GMT\", \"server\": \"openresty\", \"strict-transport-security\": \"max-age=63072000; includeSubdomains; preload\", \"x-rosetteapi-app-id\": \"1409611723442\", \"x-rosetteapi-concurrency\": \"50\", \"x-rosetteapi-request-id\": \"d4176692-4f14-42d7-8c26-4b2d8f7ff049\", \"content-length\": \"72\", \"connection\": \"Close\" }";
            Dictionary<string, string> responseHeaders = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(headersAsString);
            Dictionary<string, object> content = new Dictionary<string, object>();
            content.Add("document", docSentiment);
            content.Add("entities", entities);
            SentimentResponse expected = new SentimentResponse(docSentiment, entities, responseHeaders, content, null);
            String mockedContent = expected.ContentToString();
            HttpResponseMessage mockedMessage = MakeMockedMessage(responseHeaders, HttpStatusCode.OK, mockedContent);
            _mockHttp.When(_testUrl + "sentiment").Respond(mockedMessage);
            SentimentResponse response = _rosetteApi.Sentiment("Original Ghostbuster Dan Aykroyd, who also co-wrote the 1984 Ghostbusters film, couldn’t be more pleased with the new all-female Ghostbusters cast, telling The Hollywood Reporter, “The Aykroyd family is delighted by this inheritance of the Ghostbusters torch by these most magnificent women in comedy.”");
            Assert.AreEqual(expected, response);
        }

        [Test]
        public void Sentiment_Content_Test() {
            _mockHttp.When(_testUrl + "sentiment")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Sentiment("content");
# pragma warning disable 618
            Assert.AreEqual(response.Content["response"], "OK");
# pragma warning restore 618
        }

        [Test]
        public void Sentiment_Dict_Test() {
            _mockHttp.When(_testUrl + "sentiment")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Sentiment(new Dictionary<object, object>() { { "contentUri", "contentUrl" } });
# pragma warning disable 618
            Assert.AreEqual(response.Content["response"], "OK");
# pragma warning restore 618
        }

        [Test]
        public void Sentiment_File_Test() {
            _mockHttp.When(_testUrl + "sentiment")
                .Respond("application/json", "{'response': 'OK'}");

            RosetteFile f = new RosetteFile(_tmpFile);
            var response = _rosetteApi.Sentiment(f);
# pragma warning disable 618
            Assert.AreEqual(response.Content["response"], "OK");
# pragma warning restore 618
        }

        //------------------------- Text Embedding ------------------------------------

        [Test]
        public void Embedding_Content_Test()
        {
            _mockHttp.When(_testUrl + "text-embedding")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.TextEmbedding("content");
#pragma warning disable 618
            Assert.AreEqual(response.Content["response"], "OK");
#pragma warning restore 618
        }

        [Test]
        public void Embedding_Dict_Test()
        {
            _mockHttp.When(_testUrl + "text-embedding")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.TextEmbedding(new Dictionary<object, object>() { { "contentUri", "contentUrl" } });
#pragma warning disable 618
            Assert.AreEqual(response.Content["response"], "OK");
#pragma warning restore 618
        }

        [Test]
        public void Embedding_File_Test()
        {
            _mockHttp.When(_testUrl + "text-embedding")
                .Respond("application/json", "{'response': 'OK'}");

            RosetteFile f = new RosetteFile(_tmpFile);
            var response = _rosetteApi.TextEmbedding(f);
#pragma warning disable 618
            Assert.AreEqual(response.Content["response"], "OK");
#pragma warning restore 618
        }

        //------------------------- Syntax Dependencies ------------------------------------
        public void SyntaxDependenciesTestFull()
        {
            Init();
            SyntaxDependenciesResponse.Dependency e0 = new SyntaxDependenciesResponse.Dependency("compound", 1, 0);
            SyntaxDependenciesResponse.Dependency e1 = new SyntaxDependenciesResponse.Dependency("nsubj", 3, 1);
            SyntaxDependenciesResponse.Dependency e2 = new SyntaxDependenciesResponse.Dependency("aux", 3, 2);
            SyntaxDependenciesResponse.Dependency e3 = new SyntaxDependenciesResponse.Dependency("root", -1, 3);
            SyntaxDependenciesResponse.Dependency e4 = new SyntaxDependenciesResponse.Dependency("punc", 3, 4);
            List<SyntaxDependenciesResponse.Dependency> dependencies = new List<SyntaxDependenciesResponse.Dependency>() { e0, e1, e2, e3, e4 };
            List<string> tokens = new List<string>() { "Sony", "Pictures", "is", "planning", "."};
            string headersAsString = " { \"Content-Type\": \"application/json\", \"date\": \"Thu, 11 Aug 2016 15:47:32 GMT\", \"server\": \"openresty\", \"strict-transport-security\": \"max-age=63072000; includeSubdomains; preload\", \"x-rosetteapi-app-id\": \"1409611723442\", \"x-rosetteapi-concurrency\": \"50\", \"x-rosetteapi-request-id\": \"d4176692-4f14-42d7-8c26-4b2d8f7ff049\", \"content-length\": \"72\", \"connection\": \"Close\" }";
            Dictionary<string, string> responseHeaders = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(headersAsString);
            Dictionary<string, object> content = new Dictionary<string, object>();
            content.Add("dependencies", dependencies);
            content.Add("tokens", tokens);
            SyntaxDependenciesResponse expected = new SyntaxDependenciesResponse(dependencies, tokens, responseHeaders, content, null);
            String mockedContent = expected.ContentToString();
            HttpResponseMessage mockedMessage = MakeMockedMessage(responseHeaders, HttpStatusCode.OK, mockedContent);
            _mockHttp.When(_testUrl + "syntax/dependencies").Respond(mockedMessage);
            SyntaxDependenciesResponse response = _rosetteApi.SyntaxDependencies("Sony Pictures is planning.");
            Assert.AreEqual(expected, response);
        }

        [Test]
        public void SyntaxDependencies_Content_Test()
        {
            _mockHttp.When(_testUrl + "syntax/dependencies")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            SyntaxDependenciesResponse response = _rosetteApi.SyntaxDependencies("content");
#pragma warning disable 618
            Assert.AreEqual(response.Content["response"], "OK");
#pragma warning restore 618
        }

        [Test]
        public void SyntaxDependencies_Dict_Test()
        {
            _mockHttp.When(_testUrl + "syntax/dependencies")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            SyntaxDependenciesResponse response = _rosetteApi.SyntaxDependencies(new Dictionary<object, object>() { { "contentUri", "contentUrl" } });
#pragma warning disable 618
            Assert.AreEqual(response.Content["response"], "OK");
#pragma warning restore 618
        }

        [Test]
        public void SyntaxDependencies_File_Test()
        {
            _mockHttp.When(_testUrl + "syntax/dependencies")
                .Respond("application/json", "{'response': 'OK'}");

            RosetteFile f = new RosetteFile(_tmpFile);
            SyntaxDependenciesResponse response = _rosetteApi.SyntaxDependencies(f);
#pragma warning disable 618
            Assert.AreEqual(response.Content["response"], "OK");
#pragma warning restore 618
        }

        //------------------------- Tokens ----------------------------------------
        [Test]
        public void TokensTestFull()
        {
            Init();
            List<string> tokens = new List<string>() {
                "北京大学",
                "生物系",
                "主任",
                "办公室",
                "内部",
                "会议"
            };
            string headersAsString = " { \"Content-Type\": \"application/json\", \"date\": \"Thu, 11 Aug 2016 15:47:32 GMT\", \"server\": \"openresty\", \"strict-transport-security\": \"max-age=63072000; includeSubdomains; preload\", \"x-rosetteapi-app-id\": \"1409611723442\", \"x-rosetteapi-concurrency\": \"50\", \"x-rosetteapi-request-id\": \"d4176692-4f14-42d7-8c26-4b2d8f7ff049\", \"content-length\": \"72\", \"connection\": \"Close\" }";
            Dictionary<string, string> responseHeaders = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(headersAsString);
            Dictionary<string, object> content = new Dictionary<string, object>();
            content.Add("tokens", tokens);
            TokenizationResponse expected = new TokenizationResponse(tokens, responseHeaders, content, null);
            String mockedContent = expected.ContentToString();
            HttpResponseMessage mockedMessage = MakeMockedMessage(responseHeaders, HttpStatusCode.OK, mockedContent);
            _mockHttp.When(_testUrl + "tokens").Respond(mockedMessage);
            TokenizationResponse response = _rosetteApi.Tokens("北京大学生物系主任办公室内部会议");
            Assert.AreEqual(expected, response);
        }

        [Test]
        public void Tokens_Content_Test() {
            _mockHttp.When(_testUrl + "tokens")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Tokens("content");
# pragma warning disable 618
            Assert.AreEqual(response.Content["response"], "OK");
# pragma warning restore 618
        }

        [Test]
        public void Tokens_Dict_Test() {
            _mockHttp.When(_testUrl + "tokens")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Tokens(new Dictionary<object, object>() { { "contentUri", "contentUrl" } });
# pragma warning disable 618
            Assert.AreEqual(response.Content["response"], "OK");
# pragma warning restore 618
        }

        [Test]
        public void Tokens_File_Test() {
            _mockHttp.When(_testUrl + "tokens")
                .Respond("application/json", "{'response': 'OK'}");

            RosetteFile f = new RosetteFile(_tmpFile);
            var response = _rosetteApi.Tokens(f);
# pragma warning disable 618
            Assert.AreEqual(response.Content["response"], "OK");
# pragma warning restore 618
        }

        //------------------------- Name Translation ----------------------------------------
        [Test]
        public void NameTranslationTestFull()
        {
            Init();
            string translation = "Mu'ammar Muhammad Abu-Minyar al-Qadhaf";
            string targetLanguage = "eng";
            string targetScript = "Latn";
            string targetScheme = "IC";
            double confidence = (double)0.06856099342585828;
            string headersAsString = " { \"Content-Type\": \"application/json\", \"date\": \"Thu, 11 Aug 2016 15:47:32 GMT\", \"server\": \"openresty\", \"strict-transport-security\": \"max-age=63072000; includeSubdomains; preload\", \"x-rosetteapi-app-id\": \"1409611723442\", \"x-rosetteapi-concurrency\": \"50\", \"x-rosetteapi-request-id\": \"d4176692-4f14-42d7-8c26-4b2d8f7ff049\", \"content-length\": \"72\", \"connection\": \"Close\" }";
            Dictionary<string, string> responseHeaders = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(headersAsString);
            Dictionary<string, object> content = new Dictionary<string, object>();
            content.Add("translation", translation);
            content.Add("targetLanguage", targetLanguage);
            content.Add("targetScript", targetScript);
            content.Add("targetScheme", targetScheme);
            content.Add("confidence", confidence);
            TranslateNamesResponse expected = new TranslateNamesResponse(translation, targetLanguage: targetLanguage, targetScript:targetScript, targetScheme:targetScheme, confidence: confidence, responseHeaders:responseHeaders, content: content);
            String mockedContent = expected.ContentToString();
            HttpResponseMessage mockedMessage = MakeMockedMessage(responseHeaders, HttpStatusCode.OK, mockedContent);
            _mockHttp.When(_testUrl + "name-translation").Respond(mockedMessage);
            Dictionary<object, object> input = new Dictionary<object, object>() {
                {"name", "معمر محمد أبو منيار القذاف"},
                {"targetLanguage", "eng"},
                {"targetScript", "Latn"}
            };
            TranslateNamesResponse response = _rosetteApi.NameTranslation(input);
            Assert.AreEqual(expected, response);
        }

        [Test]
        public void NameTranslation_Content_Test() {
            _mockHttp.When(_testUrl + "name-translation")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.NameTranslation("content");
# pragma warning disable 618
            Assert.AreEqual(response.Content["response"], "OK");
# pragma warning restore 618
        }

        [Test]
        public void NameTranslation_Dict_Test() {
            _mockHttp.When(_testUrl + "name-translation")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            Dictionary<object, object> sampleDict = new Dictionary<object, object>(){
                { "name", "name"},
                { "sourceLanguageOfUse", "sourceLanguageOfUse"},
                { "sourceScript", "sourceScript"},
                { "targetLanguage", "targetLanguage"},
                { "targetScript", "targetScript"},
                { "targetScheme", "targetScheme"},
                { "sourceLanguageOfOrigin", "sourceLanguageOfOrigin"},
                { "entityType", "entityType"}
            };
            var response = _rosetteApi.NameTranslation(sampleDict);
# pragma warning disable 618
            Assert.AreEqual(response.Content["response"], "OK");
# pragma warning restore 618
        }

    }
}

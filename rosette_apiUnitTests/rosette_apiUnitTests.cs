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
            Assert.AreEqual(_testItemCount, rr.Content.Count, "RosetteResponse: header mismatch");
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

        [TestFixtureSetUp]
        public void Init() {
            _mockHttp = new MockHttpMessageHandler();
            var client = new HttpClient(_mockHttp);

            string jsonResponse = string.Format("{{'response': 'OK', 'version': '{0}'}}", CAPI.Version);

            _mockHttp.When(_testUrl + "info")
                .WithQueryString(string.Format("clientVersion={0}", CAPI.Version))
                .Respond("applciation/json", jsonResponse);

            _rosetteApi = new CAPI("userkey", null, 1, client);
        }

        [TestFixtureTearDown]
        public void Cleanup() {
        }

        [Test]
        public void Error409_Test() {
            _mockHttp.When(_testUrl + "entities")
                .Respond(HttpStatusCode.Conflict);

            try {
                var response = _rosetteApi.Entity("content");
                Assert.Fail("Exception not thrown");
            }
            catch (RosetteException ex) {
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

        [Test, ExpectedException(typeof(RosetteException), ExpectedMessage = "RosetteException thrown", MatchType = MessageMatch.Exact)]
        public void ThrowRosetteExceptionTest() {
            throw new RosetteException("RosetteException thrown");
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

        [TestFixtureSetUp]
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

        [TestFixtureTearDown]
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
            Assert.AreEqual(response.Content["response"], "OK");
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
            Assert.AreEqual(response.Content["response"], "OK");
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
            Assert.AreEqual(response.Content["response"], "OK");
        }

        [Test]
        public void CategoriesContentTestFull()
        {
            Init();
            List<RosetteCategory> categories = new List<RosetteCategory>();
            RosetteCategory cat0 = new RosetteCategory("ARTS_AND_ENTERTAINMENT", (Decimal)0.23572849069656435);
            categories.Add(cat0);
            string headersAsString = " { \"Content-Type\": \"application/json\", \"date\": \"Thu, 11 Aug 2016 15:47:32 GMT\", \"server\": \"openresty\", \"strict-transport-security\": \"max-age=63072000; includeSubdomains; preload\", \"x-rosetteapi-app-id\": \"1409611723442\", \"x-rosetteapi-concurrency\": \"50\", \"x-rosetteapi-request-id\": \"d4176692-4f14-42d7-8c26-4b2d8f7ff049\", \"content-length\": \"72\", \"connection\": \"Close\" }";
            Dictionary<string, object> content = new Dictionary<string, object>();
            content.Add("categories", categories);
            Dictionary<string, string> responseHeaders = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(headersAsString);
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
            Assert.AreEqual(response.Content["response"], "OK");
        }

        [Test]
        public void Categories_File_Test() {
            _mockHttp.When(_testUrl + "categories")
                .Respond("application/json", "{'response': 'OK'}");

            RosetteFile f = new RosetteFile(_tmpFile);
            var response = _rosetteApi.Categories(f);
            Assert.AreEqual(response.Content["response"], "OK");
        }

        //------------------------- Entities Linked ----------------------------------------

        [Test]
        public void EntitiesLinked_Content_Test() {
            _mockHttp.When(_testUrl + "entities/linked")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Entity("content", null, null, null, true);
            Assert.AreEqual(response.Content["response"], "OK");
        }

        [Test]
        public void EntitiesLinked_Dict_Test() {
            _mockHttp.When(_testUrl + "entities/linked")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Entity(new Dictionary<object, object>() { { "contentUri", "contentUrl" } }, true);
            Assert.AreEqual(response.Content["response"], "OK");
        }

        [Test]
        public void EntitiesLinked_File_Test() {
            _mockHttp.When(_testUrl + "entities/linked")
                .Respond("application/json", "{'response': 'OK'}");

            RosetteFile f = new RosetteFile(_tmpFile);
            var response = _rosetteApi.Entity(f, true);
            Assert.AreEqual(response.Content["response"], "OK");
        }

        //------------------------- Entity ----------------------------------------

        [Test]
        public void Entity_Content_Test() {
            _mockHttp.When(_testUrl + "entities")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Entity("content");
            Assert.AreEqual(response.Content["response"], "OK");
        }

        [Test]
        public void Entity_Dict_Test() {
            _mockHttp.When(_testUrl + "entities")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Entity(new Dictionary<object, object>() { { "contentUri", "contentUrl" } });
            Assert.AreEqual(response.Content["response"], "OK");
        }

        [Test]
        public void Entity_File_Test() {
            _mockHttp.When(_testUrl + "entities")
                .Respond("application/json", "{'response': 'OK'}");

            RosetteFile f = new RosetteFile(_tmpFile);
            var response = _rosetteApi.Entity(f);
            Assert.AreEqual(response.Content["response"], "OK");
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
            LanguageDetection lang0 = new LanguageDetection("spa", (Decimal)0.38719602327387076);
            LanguageDetection lang1 = new LanguageDetection("eng", (Decimal)0.32699986625091865);
            LanguageDetection lang2 = new LanguageDetection("por", (Decimal)0.05569054210624943);
            LanguageDetection lang3 = new LanguageDetection("deu", (Decimal)0.030069489878380328);
            LanguageDetection lang4 = new LanguageDetection("zho", (Decimal)0.23572849069656435);
            LanguageDetection lang5 = new LanguageDetection("swe", (Decimal)0.027734757034048835);
            LanguageDetection lang6 = new LanguageDetection("ces", (Decimal)0.02583105013400886);
            LanguageDetection lang7 = new LanguageDetection("fin", (Decimal)0.23572849069656435);
            LanguageDetection lang8 = new LanguageDetection("fra", (Decimal)0.023298946617300347);
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
            Assert.AreEqual(response.Content["response"], "OK");
        }

        [Test]
        public void Language_Dict_Test() {
            _mockHttp.When(_testUrl + "language")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Language(new Dictionary<object, object>() { { "contentUri", "contentUrl" } });
            Assert.AreEqual(response.Content["response"], "OK");
        }

        [Test]
        public void Language_File_Test() {
            _mockHttp.When(_testUrl + "language")
                .Respond("application/json", "{'response': 'OK'}");

            RosetteFile f = new RosetteFile(_tmpFile);
            var response = _rosetteApi.Language(f);
            Assert.AreEqual(response.Content["response"], "OK");
        }

        //------------------------- Morphology ----------------------------------------

        [Test]
        public void Morphology_Content_Test() {
            _mockHttp.When(_testUrl + "morphology/complete")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Morphology("content");
            Assert.AreEqual(response.Content["response"], "OK");
        }

        [Test]
        public void Morphology_Dict_Test() {
            _mockHttp.When(_testUrl + "morphology/complete")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Morphology(new Dictionary<object, object>() { { "contentUri", "contentUrl" } });
            Assert.AreEqual(response.Content["response"], "OK");
        }

        [Test]
        public void Morphology_File_Test() {
            _mockHttp.When(_testUrl + "morphology/complete")
                .Respond("application/json", "{'response': 'OK'}");

            RosetteFile f = new RosetteFile(_tmpFile);
            var response = _rosetteApi.Morphology(f);
            Assert.AreEqual(response.Content["response"], "OK");
        }

        //------------------------- Name Similarity ----------------------------------------

        [Test]
        public void NameSimilarity_Content_Test() {
            _mockHttp.When(_testUrl + "name-similarity")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            Name name1 = new Name("Name One");
            Name name2 = new Name("Name Two");
            var response = _rosetteApi.NameSimilarity(name1, name2);
            Assert.AreEqual(response.Content["response"], "OK");
        }

        [Test]
        public void NameSimilarity_Dict_Test() {
            _mockHttp.When(_testUrl + "name-similarity")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.NameSimilarity(new Dictionary<object, object>() { { "name1", "Name One" }, { "name2", "Name Two" } });
            Assert.AreEqual(response.Content["response"], "OK");
        }

        //------------------------- Relationships ----------------------------------------

        [Test]
        public void Relationships_Content_Test() {
            _mockHttp.When(_testUrl + "relationships")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Relationships("content");
            Assert.AreEqual(response.Content["response"], "OK");
        }

        [Test]
        public void Relationships_Dict_Test() {
            _mockHttp.When(_testUrl + "relationships")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Relationships(new Dictionary<object, object>() { { "contentUri", "contentUrl" } });
            Assert.AreEqual(response.Content["response"], "OK");
        }

        [Test]
        public void Relationships_File_Test() {
            _mockHttp.When(_testUrl + "relationships")
                .Respond("application/json", "{'response': 'OK'}");

            RosetteFile f = new RosetteFile(_tmpFile);
            var response = _rosetteApi.Relationships(f);
            Assert.AreEqual(response.Content["response"], "OK");
        }

        //------------------------- Sentences ----------------------------------------

        [Test]
        public void Sentences_Content_Test() {
            _mockHttp.When(_testUrl + "sentences")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Sentences("content");
            Assert.AreEqual(response.Content["response"], "OK");
        }

        [Test]
        public void Sentences_Dict_Test() {
            _mockHttp.When(_testUrl + "sentences")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Sentences(new Dictionary<object, object>() { { "contentUri", "contentUrl" } });
            Assert.AreEqual(response.Content["response"], "OK");
        }

        [Test]
        public void Sentences_File_Test() {
            _mockHttp.When(_testUrl + "sentences")
                .Respond("application/json", "{'response': 'OK'}");

            RosetteFile f = new RosetteFile(_tmpFile);
            var response = _rosetteApi.Sentences(f);
            Assert.AreEqual(response.Content["response"], "OK");
        }

        //------------------------- Sentiment ----------------------------------------

        [Test]
        public void Sentiment_Content_Test() {
            _mockHttp.When(_testUrl + "sentiment")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Sentiment("content");
            Assert.AreEqual(response.Content["response"], "OK");
        }

        [Test]
        public void Sentiment_Dict_Test() {
            _mockHttp.When(_testUrl + "sentiment")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Sentiment(new Dictionary<object, object>() { { "contentUri", "contentUrl" } });
            Assert.AreEqual(response.Content["response"], "OK");
        }

        [Test]
        public void Sentiment_File_Test() {
            _mockHttp.When(_testUrl + "sentiment")
                .Respond("application/json", "{'response': 'OK'}");

            RosetteFile f = new RosetteFile(_tmpFile);
            var response = _rosetteApi.Sentiment(f);
            Assert.AreEqual(response.Content["response"], "OK");
        }

        //------------------------- Tokens ----------------------------------------

        [Test]
        public void Tokens_Content_Test() {
            _mockHttp.When(_testUrl + "tokens")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Tokens("content");
            Assert.AreEqual(response.Content["response"], "OK");
        }

        [Test]
        public void Tokens_Dict_Test() {
            _mockHttp.When(_testUrl + "tokens")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Tokens(new Dictionary<object, object>() { { "contentUri", "contentUrl" } });
            Assert.AreEqual(response.Content["response"], "OK");
        }

        [Test]
        public void Tokens_File_Test() {
            _mockHttp.When(_testUrl + "tokens")
                .Respond("application/json", "{'response': 'OK'}");

            RosetteFile f = new RosetteFile(_tmpFile);
            var response = _rosetteApi.Tokens(f);
            Assert.AreEqual(response.Content["response"], "OK");
        }

        //------------------------- Name Translation ----------------------------------------

        [Test]
        public void NameTranslation_Content_Test() {
            _mockHttp.When(_testUrl + "name-translation")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.NameTranslation("content");
            Assert.AreEqual(response.Content["response"], "OK");
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
            Assert.AreEqual(response.Content["response"], "OK");
        }

    }
}

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RichardSzalay.MockHttp;
using rosette_api;
using System.Collections.Specialized;
using System.IO.Compression;
using System.Net;

namespace rosette_apiUnitTests {
    /// <summary>
    /// Provides a concurrency test, non-unit.  Keep commented out for release.
    /// </summary>
    [TestFixture]
    public class ConcurrencyTest {
        private static int threads = 3;
        private static int calls = 5;
        private static int loops = 1;

        //[Test]
        public void TestConcurrency() {
            // To use the C# API, you must provide an API key
            string apikey = Environment.GetEnvironmentVariable("API_KEY");
            string alturl = string.Empty;

            // Block on the test, otherwise the threads will exit before completion when main exits
            while (loops-- > 0) {
                StartTest(apikey, alturl).GetAwaiter().GetResult();
            }
        }
        private static async Task StartTest(string apikey, string alturl) {
            var tasks = new List<Task>();
            CAPI api = string.IsNullOrEmpty(alturl) ? new CAPI(apikey) : new CAPI(apikey, alturl);
            foreach (int task in Enumerable.Range(0, threads)) {
                Console.WriteLine("Starting task {0}", task);
                tasks.Add(Task.Factory.StartNew(() => runLookup(task, api)));
            }
            await Task.WhenAll(tasks);
            Console.WriteLine("Test complete");
        }
        private static Task runLookup(int taskId, CAPI api) {
            string entities_text_data = @"Bill Murray will appear in new Ghostbusters film: Dr. Peter Venkman was spotted filming a cameo in Boston this… http://dlvr.it/BnsFfS";

            //string contentUri = "http://www.foxsports.com/olympics/story/chad-le-clos-showed-why-you-never-talk-trash-to-michael-phelps-080916";
            foreach (int call in Enumerable.Range(0, calls)) {
                Console.WriteLine("Task ID: {0} call {1}", taskId, call);
                try {
                    var result = api.Entity(content: entities_text_data);
                    Console.WriteLine("Concurrency: {0},Rresult: {1}", api.Concurrency, result);
                }
                catch (Exception ex) {
                    Console.WriteLine(ex);
                }
            }
            return Task.FromResult(0);
        }
    }

    [TestFixture]
    public class RosetteResponseTests {
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
            HttpResponseMessage message = new()
            {
                StatusCode = (HttpStatusCode)200,
                ReasonPhrase = "OK",
                Content = new StringContent(_testJson)
            };
            message.Headers.Add(_testHeaderKey, _testHeaderValue);
            RosetteResponse rr = new(message);
            Assert.That(rr.Headers[_testHeaderKey], Is.EqualTo(_testHeaderValue), "RosetteResponse: header mismatch");
        }

        [Test]
        public void RosetteResponse_ContentTest() {
            HttpResponseMessage message = new()
            {
                StatusCode = (HttpStatusCode)200,
                ReasonPhrase = "OK",
                Content = new StringContent(_testJson)
            };
            message.Headers.Add(_testHeaderKey, _testHeaderValue);
            RosetteResponse rr = new(message);
            Assert.That(rr.Content.Count, Is.EqualTo(_testItemCount), "RosetteResponse: header mismatch");
        }

        [Test]
        public void RosetteResponse_ContentAsJsonTest() {
            HttpResponseMessage message = new()
            {
                StatusCode = (HttpStatusCode)200,
                ReasonPhrase = "OK",
                Content = new StringContent(_testJson)
            };
            message.Headers.Add(_testHeaderKey, _testHeaderValue);
            RosetteResponse rr = new(message);
            Assert.That(rr.ContentAsJson, Is.EqualTo(_testJson), "RosetteResponse: json mismatch");
        }

        [Test]
        public void RosetteResponse_ExceptionTest() {
            HttpResponseMessage message = new()
            {
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
                Assert.That(ex.Code, Is.EqualTo(404), "RosetteResponse: Exception mismatch");
            }
        }
    }

    [TestFixture]
    public class RosetteExtensionsTests {
        [Test]
        public void MorphologyEndpointTest() {
            string expected = "han-readings";
            Assert.That(RosetteExtensions.MorphologyEndpoint(MorphologyFeature.hanReadings), Is.EqualTo(expected), "Morphology endpoint mismatch");
        }
    }

    [TestFixture]
    public class Rosette_errorTests : IDisposable {
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
        private string _testUrl = @"https://analytics.babelstreet.com/rest/v1/";

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
                Assert.That(ex.Code, Is.EqualTo(409));
                return;
            }
            catch (Exception) {
                Assert.Fail("RosetteException not thrown");
                return;
            }
        }
    }

    [TestFixture]
    public class Rosette_classTests {
        [Test]
        public void NameClassTest() {
            Name name = new("text", "language", "script", "entityType");
            Assert.That(name.text, Is.EqualTo("text"), "Name does not match");
            Assert.That(name.language, Is.EqualTo("language"), "Language does not match");
            Assert.That(name.script, Is.EqualTo("script"), "Script does not match");
            Assert.That(name.entityType, Is.EqualTo("entityType"), "EntityType does not match");
        }

        [Test]
        public void RosetteFileClassTest() {
            string tmpFile = Path.GetTempFileName();
            StreamWriter sw = File.AppendText(tmpFile);
            sw.WriteLine("Rosette API Unit Test");
            sw.Flush();
            sw.Close();

            RosetteFile f = new(tmpFile, "application/octet-stream", null);
            Assert.That(f.Filename, Is.Not.Null, "Filename is null");
            Assert.That(f.Filename, Is.EqualTo(tmpFile), "Filename does not match");
            Assert.That(f.ContentType, Is.EqualTo("application/octet-stream"), "ContentType does not match");
            Assert.That(f.Options, Is.Null, "Options does not match");

            byte[] b = f.getFileData();
            Assert.That(b.Count() > 0, Is.True, "File is empty");

            string content = f.getFileDataString();
            Assert.That(content.Length > 0, Is.True, "File is empty");

            MultipartContent multiPart = f.AsMultipart();
            Assert.That(multiPart.Headers.Count() > 0, Is.True, "Multipart not populated");
            f.Dispose();

            if (File.Exists(tmpFile)) {
                File.Delete(tmpFile);
            }
        }

        [Test]
        public void RosetteExceptionClassTest() {
            RosetteException ex = new("message", 1, "requestID", "file", "line");
            Assert.That(ex.Message, Is.EqualTo("message"), "Message does not match");
            Assert.That(ex.Code, Is.EqualTo(1), "Code does not match");
            Assert.That(ex.RequestID, Is.EqualTo("requestID"), "RequestID does not match");
            Assert.That(ex.File, Is.EqualTo("file"), "File does not match");
            Assert.That(ex.Line, Is.EqualTo("line"), "Line does not match");
        }

    }

    [TestFixture]
    public class Rosette_serializationTests {

        [Test]
        public void RecordFieldTypeSerializationTest() {
            string date = RecordFieldType.RniDate;
            string address = RecordFieldType.RniAddress;
            string name = RecordFieldType.RniName;
            string str = RecordFieldType.RniString;
            string number = RecordFieldType.RniNumber;
            string boolean = RecordFieldType.RniBoolean;
            string dateSerialized = JsonConvert.SerializeObject(date);
            string addressSerialized = JsonConvert.SerializeObject(address);
            string nameSerialized = JsonConvert.SerializeObject(name);
            string strSerialized = JsonConvert.SerializeObject(str);
            string numberSerialized = JsonConvert.SerializeObject(number);
            string booleanSerialized = JsonConvert.SerializeObject(boolean);

            Assert.That(dateSerialized, Is.EqualTo("\"rni_date\""), "RniDate does not deserialize to 'rni_date'");
            Assert.That(addressSerialized, Is.EqualTo("\"rni_address\""), "RniAddress does not deserialize to 'rni_address'");
            Assert.That(nameSerialized, Is.EqualTo("\"rni_name\""), "RniName does not deserialize to 'rni_name'");
            Assert.That(strSerialized, Is.EqualTo("\"rni_string\""), "RniString does not deserialize to 'rni_string'");
            Assert.That(numberSerialized, Is.EqualTo("\"rni_number\""), "RniNumber does not deserialize to 'rni_number'");
            Assert.That(booleanSerialized, Is.EqualTo("\"rni_boolean\""), "RniBoolean does not deserialize to 'rni_boolean'");
        }

        [Test]
        public void RecordSimilarityFieldSerializationTest() {
            RecordSimilarityField addressUnfielded = new UnfieldedAddressRecord("123 Roadlane Ave");
            string addressUnfieldedSerialized = JsonConvert.SerializeObject(addressUnfielded);

            RecordSimilarityField nameUnfielded = new UnfieldedNameRecord("Ethan R");
            string nameUnfieldedSerialized = JsonConvert.SerializeObject(nameUnfielded);

            RecordSimilarityField dateUnfielded = new UnfieldedDateRecord("1993-04-16");
            string dateUnfieldedSerialized = JsonConvert.SerializeObject(dateUnfielded);

            Assert.That(addressUnfieldedSerialized, Is.EqualTo("\"123 Roadlane Ave\""), "Unfielded Address does not serialize correctly");
            Assert.That(nameUnfieldedSerialized, Is.EqualTo("\"Ethan R\""), "Unfielded Name does not serialize correctly");
            Assert.That(dateUnfieldedSerialized, Is.EqualTo("\"1993-04-16\""), "Unfielded Date does not serialize correctly");

            FieldedAddressRecord addressFielded = new();
            addressFielded.HouseNumber = "123";
            addressFielded.Road = "Roadlane Ave";
            string addressFieldedSerialized = JsonConvert.SerializeObject(addressFielded, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            RecordSimilarityField nameFielded = new FieldedNameRecord("Ethan R");
            // keys in order: text, language, languageOfOrigin, script, entityType
            string nameFieldedSerialized = JsonConvert.SerializeObject(nameFielded, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            RecordSimilarityField dateFielded = new FieldedDateRecord("04161993", "MMddyyyy");
            string dateFieldedSerialized = JsonConvert.SerializeObject(dateFielded);

            Assert.That(addressFieldedSerialized, Is.EqualTo("{\"houseNumber\":\"123\",\"road\":\"Roadlane Ave\"}"), "Fielded Address does not serialize correctly");
            Assert.That(nameFieldedSerialized, Is.EqualTo("{\"text\":\"Ethan R\"}"), "Fielded Name does not serialize correctly");
            Assert.That(dateFieldedSerialized, Is.EqualTo("{\"format\":\"MMddyyyy\",\"date\":\"04161993\"}"), "Fielded Date does not serialize correctly");

            string jsonString = "{\"key\":\"value\",\"key2\":[\"value2\",\"value3\"]}";
            RecordSimilarityField unknownField = new UnknownFieldRecord(JToken.Parse(jsonString));
            string unknownFieldSerialized = JsonConvert.SerializeObject(unknownField);
            Assert.That(unknownFieldSerialized, Is.EqualTo(jsonString), "Unknown Field does not serialize correctly");

            RecordSimilarityField stringField = new StringRecord("software engineer");
            string stringFieldSerialized = JsonConvert.SerializeObject(stringField);
            Assert.That(stringFieldSerialized, Is.EqualTo("\"software engineer\""), "String Field does not serialize correctly");
            RecordSimilarityField numberField = new NumberRecord(47);
            string numberFieldSerialized = JsonConvert.SerializeObject(numberField);
            Assert.That(numberFieldSerialized, Is.EqualTo("47.0"), "Number Field does not serialize correctly");
            RecordSimilarityField booleanField = new BooleanRecord(false);
            string booleanFieldSerialized = JsonConvert.SerializeObject(booleanField);
            Assert.That(booleanFieldSerialized, Is.EqualTo("false"), "Boolean Field does not serialize correctly");
        }

        [Test]
        public void RecodSimilarityFullResponseDeserializationTest() {
            string jsonResponse = "{\"info\":[\"Threshold not specified, defaulting to 0.0\",\"IncludeExplainInfo not specified, defaulting to false\"],\"results\":[{\"score\":0.904213806305046,\"left\":{\"dob\":{\"date\":\"1993-04-16\"},\"primaryName\":{\"text\":\"Evan R\"}},\"right\":{\"dob\":{\"date\":\"1993-04-16\"},\"primaryName\":\"Ivan R\"},\"explainInfo\":{\"scoredFields\":{\"dob\":{\"weight\":0.2,\"calculatedWeight\":0.28571428571428575,\"rawScore\":1,\"finalScore\":0.28571428571428575,\"details\":{\"leftInput\":{\"originalString\":\"1993-04-16\",\"day\":16,\"month\":4,\"yearWithoutCentury\":93,\"century\":19,\"modifiedJulianDay\":49093,\"canonicalForm\":\"1993-04-16\",\"empty\":false},\"rightInput\":{\"originalString\":\"1993-04-16\",\"day\":16,\"month\":4,\"yearWithoutCentury\":93,\"century\":19,\"modifiedJulianDay\":49093,\"canonicalForm\":\"1993-04-16\",\"empty\":false},\"scoreTuples\":[{\"scoreInIsolation\":1,\"scoreInContext\":1,\"left\":\"1993-04-16\",\"right\":\"1993-04-16\",\"marked\":true,\"weight\":1,\"component\":\"EXACT_MATCH\"}],\"scoreAdjustments\":[{\"unbiasedScore\":1,\"score\":1,\"parameter\":\"dateFinalBias\"}],\"finalScore\":1,\"scores\":[{\"scoreInIsolation\":1,\"scoreInContext\":1,\"left\":\"1993-04-16\",\"right\":\"1993-04-16\",\"marked\":true,\"weight\":1,\"component\":\"EXACT_MATCH\"}],\"defaultInput\":{\"empty\":true},\"empty\":false}},\"primaryName\":{\"weight\":0.5,\"calculatedWeight\":0.7142857142857143,\"rawScore\":0.8658993288270642,\"finalScore\":0.6184995205907602,\"details\":{\"leftInput\":{\"data\":\"Evan R\",\"normalizedData\":\"evan r\",\"latnData\":\"evan r\",\"script\":\"Latn\",\"languageOfUse\":\"ENGLISH\",\"languageOfOrigin\":\"ENGLISH\",\"tokens\":[{\"token\":\"evan\",\"latnToken\":\"evan\",\"tokenWeight\":0.7213975215425366,\"bin\":3,\"biasedBin\":2.902736521062578,\"tokenType\":\"GIVEN\"},{\"token\":\"r\",\"latnToken\":\"r\",\"tokenWeight\":0.27860247845746355,\"bin\":8,\"biasedBin\":7.5161819937120935,\"tokenType\":\"UNKNOWN\"}],\"entityType\":\"PERSON\",\"empty\":false},\"rightInput\":{\"data\":\"Ivan R\",\"normalizedData\":\"ivan r\",\"latnData\":\"ivan r\",\"script\":\"Latn\",\"languageOfUse\":\"ENGLISH\",\"languageOfOrigin\":\"SPANISH\",\"tokens\":[{\"token\":\"ivan\",\"latnToken\":\"ivan\",\"tokenWeight\":0.7213975215425366,\"bin\":3,\"biasedBin\":2.902736521062578,\"tokenType\":\"UNKNOWN\"},{\"token\":\"r\",\"latnToken\":\"r\",\"tokenWeight\":0.27860247845746355,\"bin\":8,\"biasedBin\":7.5161819937120935,\"tokenType\":\"UNKNOWN\"}],\"entityType\":\"PERSON\",\"empty\":false},\"scoreTuples\":[{\"scoreInIsolation\":0.6599663295739067,\"scoreInContext\":0.6599663295739067,\"left\":\"evan\",\"right\":\"ivan\",\"marked\":true,\"reason\":\"HMM_MATCH\",\"leftMinTokenIndex\":0,\"leftMaxTokenIndex\":0,\"rightMinTokenIndex\":0,\"rightMaxTokenIndex\":0},{\"scoreInIsolation\":1,\"scoreInContext\":1,\"left\":\"r\",\"right\":\"r\",\"marked\":true,\"reason\":\"MATCH\",\"leftMinTokenIndex\":1,\"leftMaxTokenIndex\":1,\"rightMinTokenIndex\":1,\"rightMaxTokenIndex\":1}],\"scoreAdjustments\":[{\"unbiasedScore\":0.7290304644108475,\"score\":0.8658993288270642,\"parameter\":\"finalBias\"}],\"finalScore\":0.8658993288270642,\"defaultInput\":{\"empty\":true},\"empty\":false}}},\"leftOnlyFields\":[],\"rightOnlyFields\":[]}}],\"errorMessage\":\"dummy\"}";
            // create a HttpResponseMessage from this JSON
            HttpResponseMessage message = new()
            {
                StatusCode = (HttpStatusCode)200,
                ReasonPhrase = "OK",
                Content = new StringContent(jsonResponse)
            };
            RecordSimilarityResponse response = new(message);

            // Assert info messages
            Assert.That(response.Info, Is.Not.Null, "Info is null");
            Assert.That(response.Info.Count, Is.EqualTo(2), "Info count does not match");
            Assert.That(response.Info[0], Is.EqualTo("Threshold not specified, defaulting to 0.0"), "Info message 0 does not match");
            Assert.That(response.Info[1], Is.EqualTo("IncludeExplainInfo not specified, defaulting to false"), "Info message 1 does not match");

            // Assert error message
            Assert.That(response.ErrorMessage, Is.EqualTo("dummy"), "Error message does not match");

            // Assert result
            Assert.That(response.Results, Is.Not.Null, "Results is null");
            Assert.That(response.Results.Count, Is.EqualTo(1), "Results count does not match");
            RecordSimilarityResult result = response.Results[0];
            Assert.That(result.Score, Is.EqualTo(0.904213806305046), "Score does not match");
            Assert.That(result.Left, Is.Not.Null, "Left is null");
            Assert.That(result.Left.Count, Is.EqualTo(2), "Left count does not match");
            Assert.That(result.Right, Is.Not.Null, "Right is null");
            Assert.That(result.Right.Count, Is.EqualTo(2), "Right count does not match");

            // Assert explainInfo
            RecordSimilarityExplainInfo explainInfo = result.ExplainInfo;
            Assert.That(result.ExplainInfo, Is.Not.Null, "ExplainInfo is null");
            Assert.That(explainInfo.RightOnlyFields, Is.Not.Null, "RightOnlyFields is null");
            Assert.That(explainInfo.RightOnlyFields.Count, Is.Zero, "RightOnlyFields count does not match");
            Assert.That(explainInfo.LeftOnlyFields, Is.Not.Null, "LeftOnlyFields is null");
            Assert.That(explainInfo.LeftOnlyFields.Count, Is.Zero, "LeftOnlyFields count does not match");

            // Assert scoredFields is not null and has both keys
            Assert.That(explainInfo.ScoredFields, Is.Not.Null, "ScoredFields is null");
            Assert.That(explainInfo.ScoredFields.ContainsKey("dob"), Is.True, "ScoredFields does not contain key 'dob'");
            Assert.That(explainInfo.ScoredFields.ContainsKey("primaryName"), Is.True, "ScoredFields does not contain key 'primaryName'");
            // Assert dob
            RecordSimilarityFieldExplainInfo dob = explainInfo.ScoredFields["dob"];
            Assert.That(dob, Is.Not.Null, "dob is null");
            Assert.That(dob.Weight, Is.EqualTo(0.2), "dob weight does not match");
            Assert.That(dob.CalculatedWeight, Is.EqualTo(0.28571428571428575), "dob calculated weight does not match");
            Assert.That(dob.RawScore, Is.EqualTo(1), "dob raw score does not match");
            Assert.That(dob.FinalScore, Is.EqualTo(0.28571428571428575), "dob final score does not match");
            Assert.That(dob.Details, Is.Not.Null, "dob details is null");
        }

    }

    [TestFixture]
    public class Rosette_apiUnitTests : IDisposable {
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
        private static byte[] Compress(byte[] raw) {
            MemoryStream memory = new();
            using (GZipStream gzip = new(memory, CompressionMode.Compress, true)) {
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
            using (GZipStream stream = new(new MemoryStream(gzip), CompressionMode.Decompress)) {
                const int size = 4096;
                byte[] buffer = new byte[size];
                using (MemoryStream memory = new()) {
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
        private string _testUrl = @"https://analytics.babelstreet.com/rest/v1/";
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

        //------------------------- User-Agent Test ----------------------------------------
        [Test]
        public void UserAgentTest() {
            string uaString = string.Format("Babel-Street-Analytics-API-Csharp/{0}/{1}", CAPI.Version, Environment.Version.ToString());
            Assert.That(_rosetteApi.UserAgent, Is.EqualTo(uaString));
        }



        //------------------------- Simple Options Tests ----------------------------------------


        [Test]
        public void OptionsTest() {
            KeyValuePair<string, string> expected = new("test", "testValue");

            _rosetteApi.SetOption(expected.Key, expected.Value);

            Assert.That(_rosetteApi.GetOption(expected.Key), Is.EqualTo(expected.Value));
        }

        [Test]
        public void ClearOptionsTest() {
            _rosetteApi.SetOption("option1", "value1");
            _rosetteApi.SetOption("option2", "value2");

            _rosetteApi.ClearOptions();

            Assert.That(_rosetteApi.GetOption("option1"), Is.Null);
        }

        //------------------------- Simple Custom Header Tests ----------------------------------------


        [Test]
        public void CustomHeadersTestRosette() {
            KeyValuePair<string, string> expected = new("X-RosetteAPI-Test", "testValue");

            _rosetteApi.SetCustomHeaders(expected.Key, expected.Value);

            Assert.That(_rosetteApi.GetCustomHeaders()[expected.Key], Is.EqualTo(expected.Value));
        }

        [Test]
        public void CustomHeadersTestBabelStreet() {
            KeyValuePair<string, string> expected = new("X-BabelStreetAPI-Test", "testValue");

            _rosetteApi.SetCustomHeaders(expected.Key, expected.Value);

            Assert.That(_rosetteApi.GetCustomHeaders()[expected.Key], Is.EqualTo(expected.Value));
        }

        [Test]
        public void ClearHeadersTest() {
            _rosetteApi.SetCustomHeaders("X-RosetteAPI-Test", "testValue");

            _rosetteApi.ClearCustomHeaders();

            Assert.That(_rosetteApi.GetCustomHeaders(), Is.Empty);
        }

        [Test]
        public void CheckInvalidCustomHeader() {
            KeyValuePair<string, string> expected = new("Test", "testValue");

            try {
                _rosetteApi.SetCustomHeaders(expected.Key, expected.Value);
            }
            catch (RosetteException ex) {
                Assert.That(ex.Message, Is.EqualTo("Custom header name must begin with \"X-RosetteAPI-\" or \"X-BabelStreetAPI-\""));
                return;
            }
        }


        //------------------------- Simple URL Parameter Tests ----------------------------------------


        [Test]
        public void CustomUrlParametersTest() {
            NameValueCollection expected = new()
            {
                { "output", "rosette" }
            };
            _rosetteApi.SetUrlParameter("output", "rosette");

            Assert.That(_rosetteApi.GetUrlParameters()["output"], Is.EqualTo(expected["output"]));
        }

        [Test]
        public void ClearUrlParametersTest() {
            NameValueCollection expected = new()
            {
                { "output", "rosette" }
            };
            _rosetteApi.SetUrlParameter("output", "rosette");

            _rosetteApi.ClearUrlParameters();

            Assert.That(_rosetteApi.GetUrlParameters(), Is.Empty);
        }

        [Test]
        public void RemoveURLParametersTest() {
            NameValueCollection expected = new()
            {
                { "output", "rosette" }
            };
            _rosetteApi.RemoveUrlParameter("output");

            Assert.That(_rosetteApi.GetUrlParameters(), Is.Empty);
        }


        //------------------------- Get Calls (Info and Ping) ----------------------------------------

        [Test]
        public void InfoTest() {
            _mockHttp.When(_testUrl + "info")
                .Respond("application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Info();
            Assert.That(response.Content["response"], Is.EqualTo("OK"));
        }

        [Test]
        public void InfoTestFull()
        {
            Init();
            string name = "Rosette API";
            string version = "1.2.3";
            string buildNumber = null;
            string buildTime = null;
            string headersAsString = " { \"Content-Type\": \"application/json\", \"Date\": \"Thu, 11 Aug 2016 15:47:32 GMT\", \"Server\": \"openresty\", \"Strict-Transport-Security\": \"max-age=63072000; includeSubdomains; preload\", \"x-rosetteapi-app-id\": \"1409611723442\", \"x-rosetteapi-concurrency\": \"50\", \"x-rosetteapi-request-id\": \"d4176692-4f14-42d7-8c26-4b2d8f7ff049\", \"Content-Length\": \"72\", \"Connection\": \"Close\" }";
            Dictionary<string, object> content = new()
            {
                { "name", name },
                { "version", version },
                { "buildNumber", buildNumber },
                { "buildTime", buildTime }
            };
            Dictionary<string, string> responseHeaders = JsonConvert.DeserializeObject<Dictionary<string, string>>(headersAsString);
            HttpResponseMessage mockedMessage = MakeMockedMessage(responseHeaders, HttpStatusCode.OK, JsonConvert.SerializeObject(content));
            _mockHttp.When(_testUrl + "info").Respond(req => mockedMessage);
            InfoResponse expected = new(name, version, buildNumber, buildTime, responseHeaders, content);
            InfoResponse response = _rosetteApi.Info();
            Assert.That(response, Is.EqualTo(expected));
        }

        private HttpResponseMessage MakeMockedMessage(Dictionary<string, string> responseHeaders, HttpStatusCode statusCode, String content)
        {
            HttpResponseMessage mockedMessage = new(statusCode) {
                Content = new StringContent(content)
            };
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
            string headersAsString = " { \"Content-Type\": \"application/json\", \"Date\": \"Thu, 11 Aug 2016 15:47:32 GMT\", \"Server\": \"openresty\", \"Strict-Transport-Security\": \"max-age=63072000; includeSubdomains; preload\", \"x-rosetteapi-app-id\": \"1409611723442\", \"x-rosetteapi-concurrency\": \"50\", \"x-rosetteapi-request-id\": \"d4176692-4f14-42d7-8c26-4b2d8f7ff049\", \"Content-Length\": \"72\", \"Connection\": \"Close\" }";
            Dictionary<string, object> content = new()
            {
                { "message", message },
                { "time", time }
            };
            Dictionary<string, string> responseHeaders = JsonConvert.DeserializeObject<Dictionary<string, string>>(headersAsString);
            HttpResponseMessage mockedMessage = MakeMockedMessage(responseHeaders, HttpStatusCode.OK, JsonConvert.SerializeObject(content));
            _mockHttp.When(_testUrl + "ping").Respond(req => mockedMessage);
            PingResponse expected = new(message, time, responseHeaders, content);
            PingResponse response = _rosetteApi.Ping();
            Assert.That(response, Is.EqualTo(expected));
        }

        [Test]
        public void PingTest()
        {
            _mockHttp.When(_testUrl + "ping")
                .Respond("application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Ping();
            Assert.That(response.Content["response"], Is.EqualTo("OK"));
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
                Assert.That(ex.Message, Is.EqualTo("Must supply one of Content or ContentUri"));
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
                Assert.That(ex.Message, Is.EqualTo("Cannot supply both Content and ContentUri"));
                return;
            }
            Assert.Fail("Exception not thrown");
        }

        //------------------------- Address Similarity ----------------------------------------
        [Test]
        public void AddressSimilarityTestFull()
        {
            Init();
            double score = (double)0.9486632809417912;
            string headersAsString = " { \"Content-Type\": \"application/json\", \"Date\": \"Thu, 11 Aug 2016 15:47:32 GMT\", \"Server\": \"openresty\", \"Strict-Transport-Security\": \"max-age=63072000; includeSubdomains; preload\", \"x-rosetteapi-app-id\": \"1409611723442\", \"x-rosetteapi-concurrency\": \"50\", \"x-rosetteapi-request-id\": \"d4176692-4f14-42d7-8c26-4b2d8f7ff049\", \"Content-Length\": \"72\", \"Connection\": \"Close\" }";
            Dictionary<string, string> responseHeaders = JsonConvert.DeserializeObject<Dictionary<string, string>>(headersAsString);
            Dictionary<string, object> content = new()
            {
                { "score", score }
            };
            AddressSimilarityResponse expected = new(score, responseHeaders, content, null);
            String mockedContent = expected.ContentToString();
            HttpResponseMessage mockedMessage = MakeMockedMessage(responseHeaders, HttpStatusCode.OK, mockedContent);
            _mockHttp.When(_testUrl + "address-similarity").Respond(req => mockedMessage);
            AddressSimilarityResponse response = _rosetteApi.AddressSimilarity(new Address(city:"Cambridge"), new Address(city:"cambridge"));
            Assert.That(response, Is.EqualTo(expected));
        }

        [Test]
        public void AddressSimilarity_Content_Test() {
            _mockHttp.When(_testUrl + "address-similarity").Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");
            Address name1 = new("Address One");
            Address name2 = new("Address Two");
            var response = _rosetteApi.AddressSimilarity(name1, name2);
            Assert.That(response.Content["response"], Is.EqualTo("OK"));
        }

        [Test]
        public void AddressSimilarity_Dict_Test() {
            _mockHttp.When(_testUrl + "address-similarity")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.AddressSimilarity(new Dictionary<object, object>() { { "address1", "Address One" }, { "address2", "Address Two" } });
            Assert.That(response.Content["response"], Is.EqualTo("OK"));
        }

        //------------------------- Categories ----------------------------------------

        [Test]
        public void Categories_Content_Test() {
            _mockHttp.When(_testUrl + "categories")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");
            var response = _rosetteApi.Categories("content");
            Assert.That(response.Content["response"], Is.EqualTo("OK"));
        }

        [Test]
        public void CategoriesContentTestFull()
        {
            Init();
            JsonSerializer serializer = new();
            List<RosetteCategory> categories = [];
            RosetteCategory cat0 = new("ARTS_AND_ENTERTAINMENT", (decimal)0.23572849069656435, (decimal)0.12312312312312312);
            categories.Add(cat0);
            string headersAsString = " { \"Content-Type\": \"application/json\", \"Date\": \"Thu, 11 Aug 2016 15:47:32 GMT\", \"Server\": \"openresty\", \"Strict-Transport-Security\": \"max-age=63072000; includeSubdomains; preload\", \"x-rosetteapi-app-id\": \"1409611723442\", \"x-rosetteapi-concurrency\": \"50\", \"x-rosetteapi-request-id\": \"d4176692-4f14-42d7-8c26-4b2d8f7ff049\", \"Content-Length\": \"72\", \"Connection\": \"Close\" }";
            Dictionary<string, object> content = new()
            {
                { "categories", categories }
            };
            Dictionary<string, string> responseHeaders = serializer.Deserialize<Dictionary<string, string>>(new JsonTextReader(new StringReader(headersAsString)));
            String mockedContent = "{\"categories\": [ { \"label\": \"" + cat0.Label + "\", \"confidence\": " + cat0.Confidence + ", \"score\": " + cat0.Score + "} ] }";
            HttpResponseMessage mockedMessage = MakeMockedMessage(responseHeaders, HttpStatusCode.OK, mockedContent);
            _mockHttp.When(_testUrl + "categories").Respond(req => mockedMessage);
            CategoriesResponse expected = new(categories, responseHeaders, null, mockedContent);
            CategoriesResponse response = _rosetteApi.Categories("Sony Pictures is planning to shoot a good portion of the new \"\"Ghostbusters\"\" in Boston as well.");
            Assert.That(response, Is.EqualTo(expected));
        }

        [Test]
        public void Categories_Dict_Test() {
            _mockHttp.When(_testUrl + "categories")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Categories(new Dictionary<object, object>(){ {"contentUri", "contentUrl"} });
            Assert.That(response.Content["response"], Is.EqualTo("OK"));
        }

        [Test]
        public void Categories_File_Test() {
            _mockHttp.When(_testUrl + "categories")
                .Respond("application/json", "{'response': 'OK'}");

            RosetteFile f = new(_tmpFile);
            var response = _rosetteApi.Categories(f);
            Assert.That(response.Content["response"], Is.EqualTo("OK"));
        }


        //------------------------- Entity ----------------------------------------
        [Test]
        public void EntityTestFull()
        {
            Init();
            RosetteEntity e0 = new("Dan Akroyd", "Dan Akroyd", new EntityID("Q105221"), "PERSON", 2, 0.99, "X1", null, [new(0, 10), new(20,32)], .99, 1, null);
            RosetteEntity e1 = new("The Hollywood Reporter", "The Hollywood Reporter", new EntityID("Q61503"), "ORGANIZATION", 1, null, "X1", null, [new(15, 18)], null, null, null);
            RosetteEntity e2 = new("Dan Akroyd", "Dan Akroyd", new EntityID("Q105221"), "PERSON", 2, 0.99, "X1", null, [new(0, 10), new(20, 32)], .99, 0.0, null);
            List<RosetteEntity> entities = [e0, e1, e2];
            string headersAsString = " { \"Content-Type\": \"application/json\", \"Date\": \"Thu, 11 Aug 2016 15:47:32 GMT\", \"Server\": \"openresty\", \"Strict-Transport-Security\": \"max-age=63072000; includeSubdomains; preload\", \"x-rosetteapi-app-id\": \"1409611723442\", \"x-rosetteapi-concurrency\": \"50\", \"x-rosetteapi-request-id\": \"d4176692-4f14-42d7-8c26-4b2d8f7ff049\", \"Content-Length\": \"72\", \"Connection\": \"Close\" }";
            Dictionary<string, string> responseHeaders = JsonConvert.DeserializeObject<Dictionary<string, string>>(headersAsString);
            Dictionary<string, object> content = new()
            {
                { "entities", entities }
            };
            EntitiesResponse expected = new(entities, responseHeaders, content, null);
            String mockedContent = expected.ContentToString();
            HttpResponseMessage mockedMessage = MakeMockedMessage(responseHeaders, HttpStatusCode.OK, mockedContent);
            _mockHttp.When(_testUrl + "entities").Respond(req => mockedMessage);
            EntitiesResponse response = _rosetteApi.Entity("Original Ghostbuster Dan Aykroyd, who also co-wrote the 1984 Ghostbusters film, couldn’t be more pleased with the new all-female Ghostbusters cast, telling The Hollywood Reporter, “The Aykroyd family is delighted by this inheritance of the Ghostbusters torch by these most magnificent women in comedy.”");
            Assert.That(response, Is.EqualTo(expected));
        }

        [Test]
        public void EntityTestExtendedProperties() {
            // Entities response, based on Cloud defaults, with linkEntities,
            // includeDBpediaType, includeDBpediaTypes and includePermID set to true.

            Init();

            String e_type = "ORGANIZATION";
            String e_mention = "Toyota";
            String e_normalized = "Toyota";
            Nullable<int> e_count = 1;
            Nullable<double> e_confidence = null;
            List<MentionOffset> e_mentionOffsets = [new MentionOffset(0, 6)];
            EntityID e_entityID = new("Q53268");
            Nullable<double> e_linkingConfidence = 0.14286868;
            Nullable<double> e_salience = null;
            String e_dbpediaType = "Agent/Organisation";
            List<String> e_dbpediaTypes = ["Agent/Organisation"];
            String e_permId = "4295876746";

            RosetteEntity e = new(e_mention, e_normalized, e_entityID, e_type, e_count, e_confidence,
                e_dbpediaType, e_dbpediaTypes, e_mentionOffsets, e_linkingConfidence, e_salience, e_permId);
            List<RosetteEntity> entities = [e];
            Dictionary<string, object> content = new() { { "entities", entities } };

            string headersAsString = " { \"Content-Type\": \"application/json\", \"Date\": \"Thu, 11 Aug 2016 15:47:32 GMT\", \"Server\": \"openresty\", \"Strict-Transport-Security\": \"max-age=63072000; includeSubdomains; preload\", \"x-rosetteapi-app-id\": \"1409611723442\", \"x-rosetteapi-concurrency\": \"50\", \"x-rosetteapi-request-id\": \"d4176692-4f14-42d7-8c26-4b2d8f7ff049\", \"Content-Length\": \"72\", \"Connection\": \"Close\" }";
            Dictionary<string, string> responseHeaders = JsonConvert.DeserializeObject<Dictionary<string, string>>(headersAsString);

            EntitiesResponse expected = new(entities, responseHeaders, content, null);
            String mockedContent = expected.ContentToString();
            HttpResponseMessage mockedMessage = MakeMockedMessage(responseHeaders, HttpStatusCode.OK, mockedContent);
            _mockHttp.When(_testUrl + "entities").Respond(req => mockedMessage);
            EntitiesResponse response = _rosetteApi.Entity("Toyota");

            Assert.That(response, Is.EqualTo(expected));
            Assert.That(response.Entities[0].PermID, Is.EqualTo(expected.Entities[0].PermID));
            Assert.That(response.Entities[0].DBpediaType, Is.EqualTo(expected.Entities[0].DBpediaType));
            Assert.That(response.Entities[0].DBpediaTypes, Is.EqualTo(expected.Entities[0].DBpediaTypes));
        }

        [Test]
        public void Entity_Content_Test() {
            _mockHttp.When(_testUrl + "entities")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Entity("content");
            Assert.That(response.Content["response"], Is.EqualTo("OK"));
        }

        [Test]
        public void Entity_Dict_Test() {
            _mockHttp.When(_testUrl + "entities")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Entity(new Dictionary<object, object>() { { "contentUri", "contentUrl" } });
            Assert.That(response.Content["response"], Is.EqualTo("OK"));
        }

        [Test]
        public void Entity_File_Test() {
            _mockHttp.When(_testUrl + "entities")
                .Respond("application/json", "{'response': 'OK'}");

            RosetteFile f = new(_tmpFile);
            var response = _rosetteApi.Entity(f);
            Assert.That(response.Content["response"], Is.EqualTo("OK"));
        }

        [Test]
        public void EntityIDLinkNullOnSetToNull()
        {
            EntityID eid = new(null);
            Assert.That(eid.GetWikipedaURL(), Is.Null);
        }

        //------------------------- Event ----------------------------------------
        [Test]
        public void EventTestFull()
        {
            Init();
            RosetteEvent e0 = new("DATA-1275-Meet-Travel.TRAVEL", [
                new([new("key", "E1", "travel", null, null, null, 15, 21)],
                "Negative", null, [new("not", 11, 14)], 15, 21) ], 0.3333333333333333, "test");

            List<RosetteEvent> events = [e0];
            string headersAsString = " { \"Content-Type\": \"application/json\", \"Date\": \"Thu, 11 Aug 2016 15:47:32 GMT\", \"Server\": \"openresty\", \"Strict-Transport-Security\": \"max-age=63072000; includeSubdomains; preload\", \"x-rosetteapi-app-id\": \"1409611723442\", \"x-rosetteapi-concurrency\": \"50\", \"x-rosetteapi-request-id\": \"d4176692-4f14-42d7-8c26-4b2d8f7ff049\", \"Content-Length\": \"72\", \"Connection\": \"Close\" }";
            Dictionary<string, string> responseHeaders = JsonConvert.DeserializeObject<Dictionary<string, string>>(headersAsString);
            Dictionary<string, object> content = new()
            {
                { "events", events }
            };
            EventsResponse expected = new(events, responseHeaders, content, null);
            String mockedContent = expected.ContentToString();
            HttpResponseMessage mockedMessage = MakeMockedMessage(responseHeaders, HttpStatusCode.OK, mockedContent);
            _mockHttp.When(_testUrl + "events").Respond(req => mockedMessage);
            _rosetteApi.SetOption("negation", "BOTH");
            EventsResponse response = _rosetteApi.Event("Vivian went to Moscow.");
            Assert.That(response, Is.EqualTo(expected));
        }

        [Test]
        public void Event_Content_Test()
        {
            _mockHttp.When(_testUrl + "events")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Event("content");
            Assert.That(response.Content["response"], Is.EqualTo("OK"));
        }

        [Test]
        public void Event_Dict_Test()
        {
            _mockHttp.When(_testUrl + "events")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Event(new Dictionary<object, object>() { { "contentUri", "contentUrl" } });
            Assert.That(response.Content["response"], Is.EqualTo("OK"));
        }

        [Test]
        public void Event_File_Test()
        {
            _mockHttp.When(_testUrl + "events")
                .Respond("application/json", "{'response': 'OK'}");

            RosetteFile f = new(_tmpFile);
            var response = _rosetteApi.Event(f);
            Assert.That(response.Content["response"], Is.EqualTo("OK"));
        }

        //------------------------- Language ----------------------------------------

        [Test]
        public void LanguageTestFull()
        {
            Init();
            LanguageDetection lang0 = new("spa", (decimal)0.38719602327387076);
            LanguageDetection lang1 = new("eng", (decimal)0.32699986625091865);
            LanguageDetection lang2 = new("por", (decimal)0.05569054210624943);
            LanguageDetection lang3 = new("deu", (decimal)0.030069489878380328);
            LanguageDetection lang4 = new("zho", (decimal)0.23572849069656435);
            LanguageDetection lang5 = new("swe", (decimal)0.027734757034048835);
            LanguageDetection lang6 = new("ces", (decimal)0.02583105013400886);
            LanguageDetection lang7 = new("fin", (decimal)0.23572849069656435);
            LanguageDetection lang8 = new("fra", (decimal)0.023298946617300347);
            List<LanguageDetection> languageDetections = [lang0, lang1, lang2, lang3, lang4, lang5, lang6, lang7, lang8];
            string headersAsString = " { \"Content-Type\": \"application/json\", \"Date\": \"Thu, 11 Aug 2016 15:47:32 GMT\", \"Server\": \"openresty\", \"Strict-Transport-Security\": \"max-age=63072000; includeSubdomains; preload\", \"x-rosetteapi-app-id\": \"1409611723442\", \"x-rosetteapi-concurrency\": \"50\", \"x-rosetteapi-request-id\": \"d4176692-4f14-42d7-8c26-4b2d8f7ff049\", \"Content-Length\": \"72\", \"Connection\": \"Close\" }";
            Dictionary<string, string> responseHeaders = JsonConvert.DeserializeObject<Dictionary<string, string>>(headersAsString);
            Dictionary<string, object> content = new()
            {
                { "languageDetections", languageDetections }
            };
            LanguageIdentificationResponse expected = new(languageDetections, responseHeaders, content, null);
            String mockedContent = expected.ContentToString();
            HttpResponseMessage mockedMessage = MakeMockedMessage(responseHeaders, HttpStatusCode.OK, mockedContent);
            _mockHttp.When(_testUrl + "language").Respond(req => mockedMessage);
            LanguageIdentificationResponse response = _rosetteApi.Language("Por favor Señorita, says the man.");
            Assert.That(response, Is.EqualTo(expected));
        }

        [Test]
        public void Language_Content_Test() {
            _mockHttp.When(_testUrl + "language")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Language("content");
            Assert.That(response.Content["response"], Is.EqualTo("OK"));
        }

        [Test]
        public void Language_Dict_Test() {
            _mockHttp.When(_testUrl + "language")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Language(new Dictionary<object, object>() { { "contentUri", "contentUrl" } });
            Assert.That(response.Content["response"], Is.EqualTo("OK"));
        }

        [Test]
        public void Language_File_Test() {
            _mockHttp.When(_testUrl + "language")
                .Respond("application/json", "{'response': 'OK'}");

            RosetteFile f = new(_tmpFile);
            var response = _rosetteApi.Language(f);
            Assert.That(response.Content["response"], Is.EqualTo("OK"));
        }

        //------------------------- Morphology ----------------------------------------
        [Test]
        public void MorphologyTestFullComplete()
        {
            Init();
            MorphologyItem m0 = new("The", "DET", "the", [], []);
            MorphologyItem m1 = new("quick", "ADJ", "quick", [], []);
            MorphologyItem m2 = new("brown", "ADJ", "brown", [], []);
            MorphologyItem m3 = new("fox", "NOUN", "fox", [], []);
            MorphologyItem m4 = new("jumped", "VERB", "jump", [], []);
            MorphologyItem m5 = new(".", "PUNCT", ".", [], []);
            List<MorphologyItem> morphology = [m0, m1, m2, m3, m4, m5];
            string headersAsString = " { \"Content-Type\": \"application/json\", \"Date\": \"Thu, 11 Aug 2016 15:47:32 GMT\", \"Server\": \"openresty\", \"Strict-Transport-Security\": \"max-age=63072000; includeSubdomains; preload\", \"x-rosetteapi-app-id\": \"1409611723442\", \"x-rosetteapi-concurrency\": \"50\", \"x-rosetteapi-request-id\": \"d4176692-4f14-42d7-8c26-4b2d8f7ff049\", \"Content-Length\": \"72\", \"Connection\": \"Close\" }";
            Dictionary<string, string> responseHeaders = JsonConvert.DeserializeObject<Dictionary<string, string>>(headersAsString);
            Dictionary<string, object> content = new()
            {
                { "tokens", new List<string>(morphology.Select<MorphologyItem, string>((item) => item.Token)) },
                { "posTags", new List<string>(morphology.Select<MorphologyItem, string>((item) => item.PosTag)) },
                { "lemmas", new List<string>(morphology.Select<MorphologyItem, string>((item) => item.Lemma)) },
                { "compoundComponents", new List<List<string>>(morphology.Select<MorphologyItem, List<string>>((item) => item.CompoundComponents)) },
                { "hanReadings", new List<List<string>>(morphology.Select<MorphologyItem, List<string>>((item) => item.HanReadings)) }
            };
            MorphologyResponse expected = new(morphology, responseHeaders, content, null);
            String mockedContent = expected.ContentAsJson;
            HttpResponseMessage mockedMessage = MakeMockedMessage(responseHeaders, HttpStatusCode.OK, mockedContent);
            _mockHttp.When(_testUrl + "morphology/complete").Respond(req => mockedMessage);
            MorphologyResponse response = _rosetteApi.Morphology("The quick brown fox jumped.");
            Assert.That(response, Is.EqualTo(expected));
        }

        [Test]
        public void MorphologyTestFullLemmas()
        {
            Init();
            MorphologyItem m0 = new("The", null, "the", null, null);
            MorphologyItem m1 = new("quick", null, "quick", null, null);
            MorphologyItem m2 = new("brown", null, "brown", null, null);
            MorphologyItem m3 = new("fox", null, "fox", null, null);
            MorphologyItem m4 = new("jumped", null, "jump", null, null);
            MorphologyItem m5 = new(".", null, ".", null, null);
            List<MorphologyItem> morphology = [m0, m1, m2, m3, m4, m5];
            string headersAsString = " { \"Content-Type\": \"application/json\", \"Date\": \"Thu, 11 Aug 2016 15:47:32 GMT\", \"Server\": \"openresty\", \"Strict-Transport-Security\": \"max-age=63072000; includeSubdomains; preload\", \"x-rosetteapi-app-id\": \"1409611723442\", \"x-rosetteapi-concurrency\": \"50\", \"x-rosetteapi-request-id\": \"d4176692-4f14-42d7-8c26-4b2d8f7ff049\", \"Content-Length\": \"72\", \"Connection\": \"Close\" }";
            Dictionary<string, string> responseHeaders = JsonConvert.DeserializeObject<Dictionary<string, string>>(headersAsString);
            Dictionary<string, object> content = new()
            {
                { "tokens", new List<string>(morphology.Select<MorphologyItem, string>((item) => item.Token)) }
            };
            ;
            content.Add("lemmas", new List<string>(morphology.Select<MorphologyItem, string>((item) => item.Lemma)));
            MorphologyResponse expected = new(morphology, responseHeaders, content, null);
            String mockedContent = expected.ContentAsJson;
            HttpResponseMessage mockedMessage = MakeMockedMessage(responseHeaders, HttpStatusCode.OK, mockedContent);
            _mockHttp.When(_testUrl + "morphology/lemmas").Respond(req => mockedMessage);
            MorphologyResponse response = _rosetteApi.Morphology("The quick brown fox jumped.", feature: MorphologyFeature.lemmas);
            Assert.That(response, Is.EqualTo(expected));
        }

        [Test]
        public void MorphologyTestFullCompoundComponents()
        {
            Init();
            MorphologyItem m0 = new("Er", null, null, [], null);
            List<string> compoundComponents = ["Rechts", "Schutz", "Versicherungs", "Gesellschaft"];
            MorphologyItem m1 = new("Rechtsschutzversicherungsgesellschaft", null, null, compoundComponents, null);
            List<MorphologyItem> morphology = [m0, m1];
            string headersAsString = " { \"Content-Type\": \"application/json\", \"Date\": \"Thu, 11 Aug 2016 15:47:32 GMT\", \"Server\": \"openresty\", \"Strict-Transport-Security\": \"max-age=63072000; includeSubdomains; preload\", \"x-rosetteapi-app-id\": \"1409611723442\", \"x-rosetteapi-concurrency\": \"50\", \"x-rosetteapi-request-id\": \"d4176692-4f14-42d7-8c26-4b2d8f7ff049\", \"Content-Length\": \"72\", \"Connection\": \"Close\" }";
            Dictionary<string, string> responseHeaders = JsonConvert.DeserializeObject<Dictionary<string, string>>(headersAsString);
            Dictionary<string, object> content = new()
            {
                { "tokens", new List<string>(morphology.Select<MorphologyItem, string>((item) => item.Token)) }
            };
            ;
            content.Add("compoundComponents", new List<List<string>>(morphology.Select<MorphologyItem, List<string>>((item) => item.CompoundComponents)));
            MorphologyResponse expected = new(morphology, responseHeaders, content, null);
            String mockedContent = expected.ContentAsJson;
            HttpResponseMessage mockedMessage = MakeMockedMessage(responseHeaders, HttpStatusCode.OK, mockedContent);
            _mockHttp.When(_testUrl + "morphology/compound-components").Respond(req => mockedMessage);
            MorphologyResponse response = _rosetteApi.Morphology("Er Rechtsschutzversicherungsgesellschaft.", feature: MorphologyFeature.compoundComponents);
            Assert.That(response, Is.EqualTo(expected));
        }

        [Test]
        public void MorphologyTestFullHanReadings()
        {
            Init();
            List<string> h0 = ["Bei3-jing1-Da4-xue2"];
            List<string> h1 = null;
            List<string> h2 = ["zhu3-ren4"];
            MorphologyItem m0 = new("北京大学", null, null, null, h0);
            MorphologyItem m1 = new("生物系", null, null, null, h1);
            MorphologyItem m2 = new("主任", null, null, null, h2);
            List<MorphologyItem> morphology = [m0, m1, m2];
            string headersAsString = " { \"Content-Type\": \"application/json\", \"Date\": \"Thu, 11 Aug 2016 15:47:32 GMT\", \"Server\": \"openresty\", \"Strict-Transport-Security\": \"max-age=63072000; includeSubdomains; preload\", \"x-rosetteapi-app-id\": \"1409611723442\", \"x-rosetteapi-concurrency\": \"50\", \"x-rosetteapi-request-id\": \"d4176692-4f14-42d7-8c26-4b2d8f7ff049\", \"Content-Length\": \"72\", \"Connection\": \"Close\" }";
            Dictionary<string, string> responseHeaders = JsonConvert.DeserializeObject<Dictionary<string, string>>(headersAsString);
            Dictionary<string, object> content = new()
            {
                { "tokens", new List<string>(morphology.Select<MorphologyItem, string>((item) => item.Token)) }
            };
            ;
            content.Add("hanReadings", new List<List<string>>(morphology.Select<MorphologyItem, List<string>>((item) => item.HanReadings)));
            MorphologyResponse expected = new(morphology, responseHeaders, content, null);
            String mockedContent = expected.ContentAsJson;
            HttpResponseMessage mockedMessage = MakeMockedMessage(responseHeaders, HttpStatusCode.OK, mockedContent);
            _mockHttp.When(_testUrl + "morphology/han-readings").Respond(req => mockedMessage);
            MorphologyResponse response = _rosetteApi.Morphology("北京大学生物系主任.", feature: MorphologyFeature.hanReadings);
            Assert.That(response, Is.EqualTo(expected));
        }

        [Test]
        public void Morphology_Content_Test() {
            _mockHttp.When(_testUrl + "morphology/complete")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Morphology("content");
            Assert.That(response.Content["response"], Is.EqualTo("OK"));
        }

        [Test]
        public void Morphology_Dict_Test() {
            _mockHttp.When(_testUrl + "morphology/complete")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Morphology(new Dictionary<object, object>() { { "contentUri", "contentUrl" } });
            Assert.That(response.Content["response"], Is.EqualTo("OK"));
        }

        [Test]
        public void Morphology_File_Test() {
            _mockHttp.When(_testUrl + "morphology/complete")
                .Respond("application/json", "{'response': 'OK'}");

            RosetteFile f = new(_tmpFile);
            var response = _rosetteApi.Morphology(f);
            Assert.That(response.Content["response"], Is.EqualTo("OK"));
        }

        //------------------------- Name Similarity ----------------------------------------
        [Test]
        public void NameSimilarityTestFull()
        {
            Init();
            double score = (double)0.9486632809417912;
            string headersAsString = " { \"Content-Type\": \"application/json\", \"Date\": \"Thu, 11 Aug 2016 15:47:32 GMT\", \"Server\": \"openresty\", \"Strict-Transport-Security\": \"max-age=63072000; includeSubdomains; preload\", \"x-rosetteapi-app-id\": \"1409611723442\", \"x-rosetteapi-concurrency\": \"50\", \"x-rosetteapi-request-id\": \"d4176692-4f14-42d7-8c26-4b2d8f7ff049\", \"Content-Length\": \"72\", \"Connection\": \"Close\" }";
            Dictionary<string, string> responseHeaders = JsonConvert.DeserializeObject<Dictionary<string, string>>(headersAsString);
            Dictionary<string, object> content = new()
            {
                { "score", score }
            };
            NameSimilarityResponse expected = new(score, responseHeaders, content, null);
            String mockedContent = expected.ContentToString();
            HttpResponseMessage mockedMessage = MakeMockedMessage(responseHeaders, HttpStatusCode.OK, mockedContent);
            _mockHttp.When(_testUrl + "name-similarity").Respond(req => mockedMessage);
            NameSimilarityResponse response = _rosetteApi.NameSimilarity(new Name("Влади́мир Влади́мирович Пу́тин", "rus", null, "PERSON", Gender.Male), new Name("Vladmir Putin", "eng", null, "PERSON"));
            Assert.That(response, Is.EqualTo(expected));
        }

        [Test]
        public void NameSimilarity_Content_Test() {
            _mockHttp.When(_testUrl + "name-similarity").Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");
            Name name1 = new("Name One");
            Name name2 = new("Name Two");
            var response = _rosetteApi.NameSimilarity(name1, name2);
            Assert.That(response.Content["response"], Is.EqualTo("OK"));
        }

        [Test]
        public void NameSimilarity_Dict_Test() {
            _mockHttp.When(_testUrl + "name-similarity")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.NameSimilarity(new Dictionary<object, object>() { { "name1", "Name One" }, { "name2", "Name Two" } });
            Assert.That(response.Content["response"], Is.EqualTo("OK"));
        }

        //------------------------- Name Deduplication ----------------------------------------
        [Test]
        public void NameDeduplication_Content_Test() {
            _mockHttp.When(_testUrl + "name-deduplication").Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");
            List<string> dedup_names = ["John Smith", "Johnathon Smith", "Fred Jones"];
            List<Name> names = [.. dedup_names.Select(name => new Name(name, "eng", "Latn"))];
            float threshold = 0.75f;

            var response = _rosetteApi.NameDeduplication(names, threshold);
            Assert.That(response.Content["response"], Is.EqualTo("OK"));
        }

        [Test]
        public void NameDeduplication_Content_NoThreshold_Test() {
            _mockHttp.When(_testUrl + "name-deduplication").Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");
            List<string> dedup_names = ["John Smith", "Johnathon Smith", "Fred Jones"];
            List<Name> names = [.. dedup_names.Select(name => new Name(name, "eng", "Latn"))];

            var response = _rosetteApi.NameDeduplication(names);
            Assert.That(response.Content["response"], Is.EqualTo("OK"));
        }

        [Test]
        public void NameDeduplication_Dict_Test() {
            _mockHttp.When(_testUrl + "name-deduplication")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            List<string> dedup_names = ["John Smith", "Johnathon Smith", "Fred Jones"];
            Dictionary<object, object> dict = new() {
                { "names", dedup_names.Select(name => new Name(name, "eng", "Latn"))},
                { "threshold", 0.75f }};

            var response = _rosetteApi.NameDeduplication(dict);
            Assert.That(response.Content["response"], Is.EqualTo("OK"));
        }

        //------------------------- Record-Similarity ----------------------------------------
        private RecordSimilarityRequest CreateTestRecordSimilarityRequest(string name, string dob, string address, string job, string age, string retired) {
                // Creating the request object
                Dictionary<string, RecordSimilarityFieldInfo> fields = new()
                {
                    { name, new RecordSimilarityFieldInfo { Type = RecordFieldType.RniName, Weight = 0.5 } },
                    { dob, new RecordSimilarityFieldInfo { Type = RecordFieldType.RniDate, Weight = 0.2 } },
                    { address, new RecordSimilarityFieldInfo { Type = RecordFieldType.RniAddress, Weight = 0.5 } },
                    { job, new RecordSimilarityFieldInfo { Type = RecordFieldType.RniString, Weight = 0.2 } },
                    { age, new RecordSimilarityFieldInfo { Type = RecordFieldType.RniNumber, Weight = 0.4 } },
                    { retired, new RecordSimilarityFieldInfo { Type = RecordFieldType.RniBoolean, Weight = 0.05 } }
                };

                RecordSimilarityProperties properties = new() { Threshold = 0.7, IncludeExplainInfo = false };

                RecordSimilarityRecords records = new()
                {
                    Left =
                    [
                        new() {
                            { name, new FieldedNameRecord { Text = "Ethan R", Language = "eng", LanguageOfOrigin = "eng", Script = "Latn", EntityType = "PERSON"} },
                            { dob, new UnfieldedDateRecord { Date = "1993-04-16"} },
                            { address, new UnfieldedAddressRecord { Address = "123 Roadlane Ave"}},
                            { job, new StringRecord { Text = "software engineer"} },
                            { age, new NumberRecord { Number = 77 } },
                            { retired, new BooleanRecord { Boolean = true } }
                        }
                    ],
                    Right =
                    [
                        new() {
                            { name, new UnfieldedNameRecord { Text = "Ivan R"} },
                            { dob, new FieldedDateRecord { Date = "1993/04/16"} },
                            { address, new FieldedAddressRecord { HouseNumber = "234", Road = "Roadlane Ave"} },
                            { age, new NumberRecord { Number = 47 } },
                            { retired, new BooleanRecord { Boolean = false } }
                        }
                    ]
                };

                RecordSimilarityRequest request = new()
                {
                    Fields = fields,
                    Properties = properties,
                    Records = records
                };

                return request;

        }

        [Test]
        public void RecordSimilarity_Request_Test() {
            Init();
            _mockHttp.When(_testUrl + "record-similarity")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            // record field names
            string name = "name";
            string dob = "dob";
            string address = "dobAddress";
            string job = "job";
            string age = "age";
            string retired = "retired";

            // Creating the request object
            RecordSimilarityRequest request = CreateTestRecordSimilarityRequest(name, dob, address, job, age, retired);

            var response = _rosetteApi.RecordSimilarity(request);
            Assert.That(response.Content["response"], Is.EqualTo("OK"));
        }

        private JToken GetJTokenFromField(RecordSimilarityField field) {
            return JToken.FromObject(field);
        }

        [Test]
        public void RecordSimilarity_WithoutExplainInfo_Test() {
            Init();

            // record field names
            string name = "name";
            string dob = "dob";
            string address = "dobAddress";
            string job = "job";
            string age = "age";
            string retired = "retired";

            // Creating the request object
            RecordSimilarityRequest request = CreateTestRecordSimilarityRequest(name, dob, address, job, age, retired);
            //creating response headers
            string headersAsString = " { \"Content-Type\": \"application/json\", \"Date\": \"Thu, 11 Aug 2016 15:47:32 GMT\", \"Server\": \"openresty\", \"Strict-Transport-Security\": \"max-age=63072000; includeSubdomains; preload\", \"x-rosetteapi-app-id\": \"1409611723442\", \"x-rosetteapi-concurrency\": \"50\", \"x-rosetteapi-request-id\": \"d4176692-4f14-42d7-8c26-4b2d8f7ff049\", \"Content-Length\": \"72\", \"Connection\": \"Close\" }";
            Dictionary<string, string> responseHeaders = JsonConvert.DeserializeObject<Dictionary<string, string>>(headersAsString);

            UnfieldedAddressRecord addressUnfielded = new() { Address = "123 Roadlane Ave" };
            UnfieldedDateRecord dobUnfielded = new() { Date = "1993-04-16" };
            FieldedNameRecord nameFielded = new() { Text = "Ethan R", Language = "eng", LanguageOfOrigin = "eng", Script = "Latn", EntityType = "PERSON" };
            Dictionary<string, RecordSimilarityField> left = new()
            {
                {address, new UnknownFieldRecord(GetJTokenFromField(addressUnfielded)) },
                {dob, new UnknownFieldRecord(GetJTokenFromField(dobUnfielded)) },
                {name, new UnknownFieldRecord(GetJTokenFromField(nameFielded)) },
            };

            FieldedAddressRecord addressFielded = new() { HouseNumber = "234", Road = "Roadlane Ave" };
            FieldedDateRecord dobFielded = new() { Date = "1993/04/16" };
            UnfieldedNameRecord nameUnfielded = new() { Text = "Ivan R" };
            Dictionary<string, RecordSimilarityField> right = new()
            {
                {address, new UnknownFieldRecord(GetJTokenFromField(addressFielded)) },
                {dob, new UnknownFieldRecord(GetJTokenFromField(dobFielded)) },
                {name, new UnknownFieldRecord(GetJTokenFromField(nameUnfielded)) },
            };

            RecordSimilarityResult result = new(0.72, left, right, null, null, null);
            List<RecordSimilarityResult> results = [result];

            Dictionary<string, object> content = new()
            {
                { "results", results }
            };

            RecordSimilarityResponse expected =
                new(results, null, null, responseHeaders, content, null);

            String mockedContent = expected.ContentToString();
            HttpResponseMessage mockedMessage = MakeMockedMessage(responseHeaders, HttpStatusCode.OK, mockedContent);
            _mockHttp.When(_testUrl + "record-similarity").Respond(req => mockedMessage);
            RecordSimilarityResponse response = _rosetteApi.RecordSimilarity(request);

            Assert.That(response, Is.EqualTo(expected));
        }




        //------------------------- Transliteration ----------------------------------------
        [Test]
        public void Transliteration_Content_Test() {
            _mockHttp.When(_testUrl + "transliteration").Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");
            string transliteration_data = "haza ya7taj fakat ila an takoun ba3dh el-nousous allati na7n ymkn an tata7awal ila al-3arabizi.";
            string language = "ara";


            var response = _rosetteApi.Transliteration(transliteration_data, language);
            Assert.That(response.Content["response"], Is.EqualTo("OK"));
        }

        [Test]
        public void Transliteration_Dict_Test() {
            _mockHttp.When(_testUrl + "transliteration")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            Dictionary<object, object> dict = new() {
                { "content", "haza ya7taj fakat ila an takoun ba3dh el-nousous allati na7n ymkn an tata7awal ila al-3arabizi."},
                { "language", "ara" }
            };

            var response = _rosetteApi.Transliteration(dict);
            Assert.That(response.Content["response"], Is.EqualTo("OK"));
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
            List<string> locatives = [loc1];
            HashSet<string> modalities = ["assertion", "someOtherModality"];
            string headersAsString = " { \"Content-Type\": \"application/json\", \"Date\": \"Thu, 11 Aug 2016 15:47:32 GMT\", \"Server\": \"openresty\", \"Strict-Transport-Security\": \"max-age=63072000; includeSubdomains; preload\", \"x-rosetteapi-app-id\": \"1409611723442\", \"x-rosetteapi-concurrency\": \"50\", \"x-rosetteapi-request-id\": \"d4176692-4f14-42d7-8c26-4b2d8f7ff049\", \"Content-Length\": \"72\", \"Connection\": \"Close\" }";
            Dictionary<string, string> responseHeaders = JsonConvert.DeserializeObject<Dictionary<string, string>>(headersAsString);
            List<RosetteRelationship> relationships = [
                new RosetteRelationship(predicate, new Dictionary<int, string>() {{1, arg1}}, new Dictionary<int, string>() {{1, arg1ID}}, null, locatives, null, confidence, modalities)
            ];
            Dictionary<string, object> content = new()
            {
                { "relationships", relationships }
            };
            RelationshipsResponse expected = new(relationships, responseHeaders, content, null);
            String mockedContent = expected.ContentToString();
            HttpResponseMessage mockedMessage = MakeMockedMessage(responseHeaders, HttpStatusCode.OK, mockedContent);
            _mockHttp.When(_testUrl + "relationships").Respond(req => mockedMessage);
            RelationshipsResponse response = _rosetteApi.Relationships("The Ghostbusters movie was filmed in Boston.");
            Assert.That(response, Is.EqualTo(expected));
        }

        [Test]
        public void RelationshipsTestFullNoArgID()
        {
            Init();
            decimal confidence = (decimal)0.8541343114184464;
            string predicate = "be filmed";
            string arg1 = "The Ghostbusters movie";
            string loc1 = "in Boston";
            List<string> locatives = [loc1];
            HashSet<string> modalities = ["assertion", "someOtherModality"];
            string headersAsString = " { \"Content-Type\": \"application/json\", \"Date\": \"Thu, 11 Aug 2016 15:47:32 GMT\", \"Server\": \"openresty\", \"Strict-Transport-Security\": \"max-age=63072000; includeSubdomains; preload\", \"x-rosetteapi-app-id\": \"1409611723442\", \"x-rosetteapi-concurrency\": \"50\", \"x-rosetteapi-request-id\": \"d4176692-4f14-42d7-8c26-4b2d8f7ff049\", \"Content-Length\": \"72\", \"Connection\": \"Close\" }";
            Dictionary<string, string> responseHeaders = JsonConvert.DeserializeObject<Dictionary<string, string>>(headersAsString);
            List<RosetteRelationship> relationships = [
                new RosetteRelationship(predicate, new Dictionary<int, string>() {{1, arg1}}, [], null, locatives, null, confidence, modalities)
            ];
            Dictionary<string, object> content = new()
            {
                { "relationships", relationships }
            };
            RelationshipsResponse expected = new(relationships, responseHeaders, content, null);
            String mockedContent = expected.ContentToString();
            HttpResponseMessage mockedMessage = MakeMockedMessage(responseHeaders, HttpStatusCode.OK, mockedContent);
            _mockHttp.When(_testUrl + "relationships").Respond(req => mockedMessage);
            RelationshipsResponse response = _rosetteApi.Relationships("The Ghostbusters movie was filmed in Boston.");
            Assert.That(response, Is.EqualTo(expected));
        }

        [Test]
        public void Relationships_All_Constructors_Equal_Test()
        {
            Init();
            decimal confidence = (decimal)0.8541343114184464;
            string predicate = "be filmed";
            string arg1 = "The Ghostbusters movie";
            string loc1 = "in Boston";
            List<string> locatives = [loc1];
            RosetteRelationship relationshipFromOriginalConstructor = new(predicate, [arg1], null, locatives, null, confidence);
            RosetteRelationship relationshipFromDoubleDictConstructor = new(predicate, new Dictionary<int, string>() {{1, arg1}}, [], null, locatives, null, confidence, null);
            RosetteRelationship relationshipFromFullArgumentsConstructor = new(predicate, [new(1, arg1, null)], null, locatives, null, confidence, null);
            Assert.That(relationshipFromOriginalConstructor.Equals(relationshipFromDoubleDictConstructor) && relationshipFromDoubleDictConstructor.Equals(relationshipFromFullArgumentsConstructor) &&
                relationshipFromOriginalConstructor.ToString().Equals(relationshipFromDoubleDictConstructor.ToString()) && relationshipFromDoubleDictConstructor.ToString().Equals(relationshipFromFullArgumentsConstructor.ToString()));
        }

        [Test]
        public void Relationships_Content_Test() {
            _mockHttp.When(_testUrl + "relationships")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Relationships("content");
            Assert.That(response.Content["response"], Is.EqualTo("OK"));
        }

        [Test]
        public void Relationships_Dict_Test() {
            _mockHttp.When(_testUrl + "relationships")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Relationships(new Dictionary<object, object>() { { "contentUri", "contentUrl" } });
            Assert.That(response.Content["response"], Is.EqualTo("OK"));
        }

        [Test]
        public void Relationships_File_Test() {
            _mockHttp.When(_testUrl + "relationships")
                .Respond("application/json", "{'response': 'OK'}");

            RosetteFile f = new(_tmpFile);
            var response = _rosetteApi.Relationships(f);
            Assert.That(response.Content["response"], Is.EqualTo("OK"));
        }

        //------------------------- Semantic Vectors ----------------------------------------
        [Test]
        public void SemanticVectorsTestFull()
        {
            Init();
            List<double> vector = [0.02164695, 0.0032850206, 0.0038508752, -0.009704393, -0.0016203842];
            string headersAsString = " { \"Content-Type\": \"application/json\", \"Date\": \"Thu, 11 Aug 2016 15:47:32 GMT\", \"Server\": \"openresty\", \"Strict-Transport-Security\": \"max-age=63072000; includeSubdomains; preload\", \"x-rosetteapi-app-id\": \"1409611723442\", \"x-rosetteapi-concurrency\": \"50\", \"x-rosetteapi-request-id\": \"d4176692-4f14-42d7-8c26-4b2d8f7ff049\", \"Content-Length\": \"72\", \"Connection\": \"Close\" }";
            Dictionary<string, string> responseHeaders = JsonConvert.DeserializeObject<Dictionary<string, string>>(headersAsString);
            Dictionary<string, object> content = new()
            {
                { "documentEmbedding", vector },
                { "tokens", null },
                { "tokenEmbeddings", null }
            };
            SemanticVectorsResponse expected = new(vector, null, null, responseHeaders, content, null);
            String mockedContent = expected.ContentToString();
            HttpResponseMessage mockedMessage = MakeMockedMessage(responseHeaders, HttpStatusCode.OK, mockedContent);
            _mockHttp.When(_testUrl + "semantics/vector").Respond(req => mockedMessage);
            SemanticVectorsResponse response = _rosetteApi.SemanticVectors("The Ghostbusters movie was filmed in Boston.");
            Assert.That(response, Is.EqualTo(expected));
        }

        [Test]
        public void SemanticVectors_Content_Test()
        {
            _mockHttp.When(_testUrl + "semantics/vector")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.SemanticVectors("content");
            Assert.That(response.Content["response"], Is.EqualTo("OK"));
        }

        [Test]
        public void SemanticVectors_Dict_Test()
        {
            _mockHttp.When(_testUrl + "semantics/vector")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.SemanticVectors(new Dictionary<object, object>() { { "contentUri", "contentUrl" } });
            Assert.That(response.Content["response"], Is.EqualTo("OK"));
        }

        [Test]
        public void SemanticVectors_File_Test()
        {
            _mockHttp.When(_testUrl + "semantics/vector")
                .Respond("application/json", "{'response': 'OK'}");

            RosetteFile f = new(_tmpFile);
            var response = _rosetteApi.SemanticVectors(f);
            Assert.That(response.Content["response"], Is.EqualTo("OK"));
        }

        //------------------------- Similar Terms ----------------------------------------
        [Test]
        public void SimilarTermsTestFull()
        {
            Init();
            IDictionary<string, List<SimilarTerm>> terms = new Dictionary<string, List<SimilarTerm>>() {
                {"eng", new List<SimilarTerm>() {new("spy", 1.0m)}}
            };
            string headersAsString = " { \"Content-Type\": \"application/json\", \"Date\": \"Thu, 11 Aug 2016 15:47:32 GMT\", \"Server\": \"openresty\", \"Strict-Transport-Security\": \"max-age=63072000; includeSubdomains; preload\", \"x-rosetteapi-app-id\": \"1409611723442\", \"x-rosetteapi-concurrency\": \"50\", \"x-rosetteapi-request-id\": \"d4176692-4f14-42d7-8c26-4b2d8f7ff049\", \"Content-Length\": \"72\", \"Connection\": \"Close\" }";
            Dictionary<string, string> responseHeaders = JsonConvert.DeserializeObject<Dictionary<string, string>>(headersAsString);
            Dictionary<string, object> content = new() {
                {"similarTerms", terms}
            };
            SimilarTermsResponse expected = new(terms, responseHeaders, content, null);
            String mockedContent = expected.ContentToString();
            HttpResponseMessage mockedMessage = MakeMockedMessage(responseHeaders, HttpStatusCode.OK, mockedContent);
            _mockHttp.When(_testUrl + "semantics/similar").Respond(req => mockedMessage);
            SimilarTermsResponse response = _rosetteApi.SimilarTerms("spy");
            Assert.That(response, Is.EqualTo(expected));
        }

        [Test]
        public void SimilarTerms_Content_Test()
        {
            _mockHttp.When(_testUrl + "semantics/similar")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.SimilarTerms("content");
            Assert.That(response.Content["response"], Is.EqualTo("OK"));
        }

        [Test]
        public void SimilarTerms_Dict_Test()
        {
            _mockHttp.When(_testUrl + "semantics/similar")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.SimilarTerms(new Dictionary<object, object>() { { "contentUri", "contentUrl" } });
            Assert.That(response.Content["response"], Is.EqualTo("OK"));
        }

        [Test]
        public void SimilarTerms_File_Test()
        {
            _mockHttp.When(_testUrl + "semantics/similar")
                .Respond("application/json", "{'response': 'OK'}");

            RosetteFile f = new(_tmpFile);
            var response = _rosetteApi.SimilarTerms(f);
            Assert.That(response.Content["response"], Is.EqualTo("OK"));
        }

        //------------------------- Sentences ----------------------------------------

        [Test]
        public void SentencesTestFull()
        {
            Init();
            string s0 = "This land is your land. ";
            string s1 = "This land is my land\nFrom California to the New York island;\nFrom the red wood forest to the Gulf Stream waters\n\nThis land was made for you and Me.\n\n";
            string s2 = "As I was walking that ribbon of highway,\nI saw above me that endless skyway:\nI saw below me that golden valley:\nThis land was made for you and me.";
            List<string> sentences = [s0, s1, s2];
            string headersAsString = " { \"Content-Type\": \"application/json\", \"Date\": \"Thu, 11 Aug 2016 15:47:32 GMT\", \"Server\": \"openresty\", \"Strict-Transport-Security\": \"max-age=63072000; includeSubdomains; preload\", \"x-rosetteapi-app-id\": \"1409611723442\", \"x-rosetteapi-concurrency\": \"50\", \"x-rosetteapi-request-id\": \"d4176692-4f14-42d7-8c26-4b2d8f7ff049\", \"Content-Length\": \"72\", \"Connection\": \"Close\" }";
            Dictionary<string, string> responseHeaders = JsonConvert.DeserializeObject<Dictionary<string, string>>(headersAsString);
            Dictionary<string, object> content = new()
            {
                { "sentences", sentences }
            };
            SentenceTaggingResponse expected = new(sentences, responseHeaders, content, null);
            String mockedContent = expected.ContentToString();
            HttpResponseMessage mockedMessage = MakeMockedMessage(responseHeaders, HttpStatusCode.OK, mockedContent);
            _mockHttp.When(_testUrl + "sentences").Respond(req => mockedMessage);
            SentenceTaggingResponse response = _rosetteApi.Sentences("This land is your land. This land is my land\\nFrom California to the New York island;\\nFrom the red wood forest to the Gulf Stream waters\\n\\nThis land was made for you and Me.\\n\\nAs I was walking that ribbon of highway,\\nI saw above me that endless skyway:\\nI saw below me that golden valley:\\nThis land was made for you and me.");
            Assert.That(response, Is.EqualTo(expected));
        }

        [Test]
        public void Sentences_Content_Test() {
            _mockHttp.When(_testUrl + "sentences")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Sentences("content");
            Assert.That(response.Content["response"], Is.EqualTo("OK"));
        }

        [Test]
        public void Sentences_Dict_Test() {
            _mockHttp.When(_testUrl + "sentences")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Sentences(new Dictionary<object, object>() { { "contentUri", "contentUrl" } });
            Assert.That(response.Content["response"], Is.EqualTo("OK"));
        }

        [Test]
        public void Sentences_File_Test() {
            _mockHttp.When(_testUrl + "sentences")
                .Respond("application/json", "{'response': 'OK'}");

            RosetteFile f = new(_tmpFile);
            var response = _rosetteApi.Sentences(f);
            Assert.That(response.Content["response"], Is.EqualTo("OK"));
        }

        //------------------------- Sentiment ----------------------------------------
        [Test]
        public void SentimentTestFull()
        {
            Init();
            SentimentResponse.RosetteSentiment docSentiment = new("pos", (double)0.7962072011038756);
            RosetteSentimentEntity e0 = new("Dan Akroyd", "Dan Akroyd", new EntityID("Q105221"), "PERSON", 2, docSentiment, (double)0.5005508052749595, null, null, [new(0, 10), new(20, 32)], .99, 1, null);
            RosetteSentimentEntity e1 = new("The Hollywood Reporter", "The Hollywood Reporter", new EntityID("Q61503"), "ORGANIZATION", 1, docSentiment, (double)0.5338094035254866, null, null, [new(15, 18)], null, null, null);
            List<RosetteSentimentEntity> entities = [e0, e1];
            string headersAsString = " { \"Content-Type\": \"application/json\", \"Date\": \"Thu, 11 Aug 2016 15:47:32 GMT\", \"Server\": \"openresty\", \"Strict-Transport-Security\": \"max-age=63072000; includeSubdomains; preload\", \"x-rosetteapi-app-id\": \"1409611723442\", \"x-rosetteapi-concurrency\": \"50\", \"x-rosetteapi-request-id\": \"d4176692-4f14-42d7-8c26-4b2d8f7ff049\", \"Content-Length\": \"72\", \"Connection\": \"Close\" }";
            Dictionary<string, string> responseHeaders = JsonConvert.DeserializeObject<Dictionary<string, string>>(headersAsString);
            Dictionary<string, object> content = new()
            {
                { "document", docSentiment },
                { "entities", entities }
            };
            SentimentResponse expected = new(docSentiment, entities, responseHeaders, content, null);
            String mockedContent = expected.ContentToString();
            HttpResponseMessage mockedMessage = MakeMockedMessage(responseHeaders, HttpStatusCode.OK, mockedContent);
            _mockHttp.When(_testUrl + "sentiment").Respond(req => mockedMessage);
            SentimentResponse response = _rosetteApi.Sentiment("Original Ghostbuster Dan Aykroyd, who also co-wrote the 1984 Ghostbusters film, couldn’t be more pleased with the new all-female Ghostbusters cast, telling The Hollywood Reporter, “The Aykroyd family is delighted by this inheritance of the Ghostbusters torch by these most magnificent women in comedy.”");
            Assert.That(response, Is.EqualTo(expected));
        }

        [Test]
        public void Sentiment_Content_Test() {
            _mockHttp.When(_testUrl + "sentiment")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Sentiment("content");
            Assert.That(response.Content["response"], Is.EqualTo("OK"));
        }

        [Test]
        public void Sentiment_Dict_Test() {
            _mockHttp.When(_testUrl + "sentiment")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Sentiment(new Dictionary<object, object>() { { "contentUri", "contentUrl" } });
            Assert.That(response.Content["response"], Is.EqualTo("OK"));
        }

        [Test]
        public void Sentiment_File_Test() {
            _mockHttp.When(_testUrl + "sentiment")
                .Respond("application/json", "{'response': 'OK'}");

            RosetteFile f = new(_tmpFile);
            var response = _rosetteApi.Sentiment(f);
            Assert.That(response.Content["response"], Is.EqualTo("OK"));
        }

        //------------------------- Syntax Dependencies ------------------------------------
        private void SyntaxDependenciesTestFull()
        {
            Init();
            SyntaxDependenciesResponse.Dependency e0 = new("compound", 1, 0);
            SyntaxDependenciesResponse.Dependency e1 = new("nsubj", 3, 1);
            SyntaxDependenciesResponse.Dependency e2 = new("aux", 3, 2);
            SyntaxDependenciesResponse.Dependency e3 = new("root", -1, 3);
            SyntaxDependenciesResponse.Dependency e4 = new("punc", 3, 4);
            List<SyntaxDependenciesResponse.Dependency> dependencies = [e0, e1, e2, e3, e4];
            SyntaxDependenciesResponse.SentenceWithDependencies sentence = new(0, 4, dependencies);
            List<SyntaxDependenciesResponse.SentenceWithDependencies> sentences = [sentence];
            List<string> tokens = ["Sony", "Pictures", "is", "planning", "."];
            string headersAsString = " { \"Content-Type\": \"application/json\", \"Date\": \"Thu, 11 Aug 2016 15:47:32 GMT\", \"Server\": \"openresty\", \"Strict-Transport-Security\": \"max-age=63072000; includeSubdomains; preload\", \"x-rosetteapi-app-id\": \"1409611723442\", \"x-rosetteapi-concurrency\": \"50\", \"x-rosetteapi-request-id\": \"d4176692-4f14-42d7-8c26-4b2d8f7ff049\", \"Content-Length\": \"72\", \"Connection\": \"Close\" }";
            Dictionary<string, string> responseHeaders = JsonConvert.DeserializeObject<Dictionary<string, string>>(headersAsString);
            Dictionary<string, object> content = new()
            {
                { "sentences", sentences },
                { "tokens", tokens }
            };
            SyntaxDependenciesResponse expected = new(sentences, tokens, responseHeaders, content, null);
            String mockedContent = expected.ContentToString();
            HttpResponseMessage mockedMessage = MakeMockedMessage(responseHeaders, HttpStatusCode.OK, mockedContent);
            _mockHttp.When(_testUrl + "syntax/dependencies").Respond(req => mockedMessage);
            SyntaxDependenciesResponse response = _rosetteApi.SyntaxDependencies("Sony Pictures is planning.");
            Assert.That(response, Is.EqualTo(expected));
        }

        [Test]
        public void SyntaxDependencies_Content_Test()
        {
            _mockHttp.When(_testUrl + "syntax/dependencies")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            SyntaxDependenciesResponse response = _rosetteApi.SyntaxDependencies("content");
            Assert.That(response.Content["response"], Is.EqualTo("OK"));
        }

        [Test]
        public void SyntaxDependencies_Dict_Test()
        {
            _mockHttp.When(_testUrl + "syntax/dependencies")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            SyntaxDependenciesResponse response = _rosetteApi.SyntaxDependencies(new Dictionary<object, object>() { { "contentUri", "contentUrl" } });
            Assert.That(response.Content["response"], Is.EqualTo("OK"));
        }

        [Test]
        public void SyntaxDependencies_File_Test()
        {
            _mockHttp.When(_testUrl + "syntax/dependencies")
                .Respond("application/json", "{'response': 'OK'}");

            RosetteFile f = new(_tmpFile);
            SyntaxDependenciesResponse response = _rosetteApi.SyntaxDependencies(f);
            Assert.That(response.Content["response"], Is.EqualTo("OK"));
        }

        //------------------------- Tokens ----------------------------------------
        [Test]
        public void TokensTestFull()
        {
            Init();
            List<string> tokens = [
                "北京大学",
                "生物系",
                "主任",
                "办公室",
                "内部",
                "会议"
            ];
            string headersAsString = " { \"Content-Type\": \"application/json\", \"Date\": \"Thu, 11 Aug 2016 15:47:32 GMT\", \"Server\": \"openresty\", \"Strict-Transport-Security\": \"max-age=63072000; includeSubdomains; preload\", \"x-rosetteapi-app-id\": \"1409611723442\", \"x-rosetteapi-concurrency\": \"50\", \"x-rosetteapi-request-id\": \"d4176692-4f14-42d7-8c26-4b2d8f7ff049\", \"Content-Length\": \"72\", \"Connection\": \"Close\" }";
            Dictionary<string, string> responseHeaders = JsonConvert.DeserializeObject<Dictionary<string, string>>(headersAsString);
            Dictionary<string, object> content = new()
            {
                { "tokens", tokens }
            };
            TokenizationResponse expected = new(tokens, responseHeaders, content, null);
            String mockedContent = expected.ContentToString();
            HttpResponseMessage mockedMessage = MakeMockedMessage(responseHeaders, HttpStatusCode.OK, mockedContent);
            _mockHttp.When(_testUrl + "tokens").Respond(req => mockedMessage);
            TokenizationResponse response = _rosetteApi.Tokens("北京大学生物系主任办公室内部会议");
            Assert.That(response, Is.EqualTo(expected));
        }

        [Test]
        public void Tokens_Content_Test() {
            _mockHttp.When(_testUrl + "tokens")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Tokens("content");
            Assert.That(response.Content["response"], Is.EqualTo("OK"));
        }

        [Test]
        public void Tokens_Dict_Test() {
            _mockHttp.When(_testUrl + "tokens")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Tokens(new Dictionary<object, object>() { { "contentUri", "contentUrl" } });
            Assert.That(response.Content["response"], Is.EqualTo("OK"));
        }

        [Test]
        public void Tokens_File_Test() {
            _mockHttp.When(_testUrl + "tokens")
                .Respond("application/json", "{'response': 'OK'}");

            RosetteFile f = new(_tmpFile);
            var response = _rosetteApi.Tokens(f);
            Assert.That(response.Content["response"], Is.EqualTo("OK"));
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
            string headersAsString = " { \"Content-Type\": \"application/json\", \"Date\": \"Thu, 11 Aug 2016 15:47:32 GMT\", \"Server\": \"openresty\", \"Strict-Transport-Security\": \"max-age=63072000; includeSubdomains; preload\", \"x-rosetteapi-app-id\": \"1409611723442\", \"x-rosetteapi-concurrency\": \"50\", \"x-rosetteapi-request-id\": \"d4176692-4f14-42d7-8c26-4b2d8f7ff049\", \"Content-Length\": \"72\", \"Connection\": \"Close\" }";
            Dictionary<string, string> responseHeaders = JsonConvert.DeserializeObject<Dictionary<string, string>>(headersAsString);
            Dictionary<string, object> content = new()
            {
                { "translation", translation },
                { "targetLanguage", targetLanguage },
                { "targetScript", targetScript },
                { "targetScheme", targetScheme },
                { "confidence", confidence }
            };
            TranslateNamesResponse expected = new(translation, targetLanguage: targetLanguage, targetScript:targetScript, targetScheme:targetScheme, confidence: confidence, responseHeaders:responseHeaders, content: content);
            String mockedContent = expected.ContentToString();
            HttpResponseMessage mockedMessage = MakeMockedMessage(responseHeaders, HttpStatusCode.OK, mockedContent);
            _mockHttp.When(_testUrl + "name-translation").Respond(req => mockedMessage);
            Dictionary<object, object> input = new() {
                {"name", "معمر محمد أبو منيار القذاف"},
                {"targetLanguage", "eng"},
                {"targetScript", "Latn"}
            };
            TranslateNamesResponse response = _rosetteApi.NameTranslation(input);
            Assert.That(response, Is.EqualTo(expected));
        }

        [Test]
        public void NameTranslation_Content_Test() {
            _mockHttp.When(_testUrl + "name-translation")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.NameTranslation("content");
            Assert.That(response.Content["response"], Is.EqualTo("OK"));
        }

        [Test]
        public void NameTranslation_Dict_Test() {
            _mockHttp.When(_testUrl + "name-translation")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            Dictionary<object, object> sampleDict = new(){
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
            Assert.That(response.Content["response"], Is.EqualTo("OK"));
        }

        //------------------------- Topics ----------------------------------------
        [Test]
        public void TopicsTestFull() {
            Init();
            List<Concept> concepts = [
                new Concept("Unfinished Tales", null, "Q309019"),
                new Concept("Nicholas Hoult", null, "Q298347")
            ];
            List<KeyPhrase> keyPhrases = [
                new KeyPhrase("Scorpius Malfoy", null),
                new KeyPhrase("Nicholas Hoult", null)
            ];
            string headersAsString = " { \"Content-Type\": \"application/json\", \"Date\": \"Thu, 11 Aug 2016 15:47:32 GMT\", \"Server\": \"openresty\", \"Strict-Transport-Security\": \"max-age=63072000; includeSubdomains; preload\", \"x-rosetteapi-app-id\": \"1409611723442\", \"x-rosetteapi-concurrency\": \"50\", \"x-rosetteapi-request-id\": \"d4176692-4f14-42d7-8c26-4b2d8f7ff049\", \"Content-Length\": \"72\", \"Connection\": \"Close\" }";
            Dictionary<string, string> responseHeaders = JsonConvert.DeserializeObject<Dictionary<string, string>>(headersAsString);
            Dictionary<string, object> content = new()
            {
                { "concepts", concepts },
                { "keyphrases", keyPhrases }
            };
            TopicsResponse expected = new(concepts, keyPhrases, responseHeaders, content, null);
            String mockedContent = expected.ContentToString();
            HttpResponseMessage mockedMessage = MakeMockedMessage(responseHeaders, HttpStatusCode.OK, mockedContent);
            _mockHttp.When(_testUrl + "topics").Respond(req => mockedMessage);
            string testContent = @"Lily Collins is in talks to join Nicholas Hoult in Chernin Entertainment and Fox Searchlights J.R.R. Tolkien biopic Tolkien. Anthony Boyle, known for playing Scorpius Malfoy in the British play Harry Potter and the Cursed Child, also has signed on for the film centered on the famed author. In Tolkien, Hoult will play the author of the Hobbit and Lord of the Rings book series that were later adapted into two Hollywood trilogies from Peter Jackson. Dome Karukoski is directing the project.";
            TopicsResponse response = _rosetteApi.Topics(content: testContent);
            Assert.That(response, Is.EqualTo(expected));
        }

        [Test]
        public void Topics_Dict_Test() {
            _mockHttp.When(_testUrl + "topics")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Topics(new Dictionary<object, object>() { { "contentUri", "contentUrl" } });
            Assert.That(response.Content["response"], Is.EqualTo("OK"));
        }

        [Test]
        public void Topics_File_Test() {
            _mockHttp.When(_testUrl + "topics")
                .Respond("application/json", "{'response': 'OK'}");

            RosetteFile f = new(_tmpFile);
            var response = _rosetteApi.Topics(f);
            Assert.That(response.Content["response"], Is.EqualTo("OK"));
        }

    }

    [TestFixture]
    public class Rosette_UtilitiesTests {

        [Test]
        public void DictionaryEquals_with_null() {
            Dictionary<string, object> dict1 = null;
            Dictionary<string, object> dict2 = null;
            Assert.That(Utilities.DictionaryEquals(dict1, dict2));
        }

        [Test]
        public void DictionaryEquals_with_one_null() {
            Dictionary<string, object> dict1 = null;
            Dictionary<string, object> dict2 = new() {
                {"key1", "value1"}
            };
            Assert.That(Utilities.DictionaryEquals(dict1, dict2), Is.False);
        }

        [Test]
        public void DictionaryEquals_with_different_count() {
            Dictionary<string, object> dict1 = [];
            Dictionary<string, object> dict2 = new() {
                {"key1", "value1"}
            };
            Assert.That(Utilities.DictionaryEquals(dict1, dict2), Is.False);
        }

        [Test]
        public void DictionaryEquals_with_different_keys() {
            Dictionary<string, object> dict1 = new() {
                {"key1", "value1"}
            };
            Dictionary<string, object> dict2 = new() {
                {"key2", "value1"}
            };
            Assert.That(Utilities.DictionaryEquals(dict1, dict2), Is.False);
        }


        [Test]
        public void DictionaryEquals_with_different_values() {
            Dictionary<string, object> dict1 = new() {
                {"key1", "value1"}
            };
            Dictionary<string, object> dict2 = new() {
                {"key1", "value2"}
            };
            Assert.That(Utilities.DictionaryEquals(dict1, dict2), Is.False);
        }

        [Test]
        public void DictionaryEquals_with_same_order() {
            Dictionary<string, object> dict1 = new() {
                {"key1", "value1"},
                {"key2", "value2"},
                {"key3", "value3"}
            };
            Dictionary<string, object> dict2 = new() {
                {"key1", "value1"},
                {"key2", "value2"},
                {"key3", "value3"}
            };
            Assert.That(Utilities.DictionaryEquals(dict1, dict2));
        }

        [Test]
        public void DictionaryEquals_with_different_order() {
            Dictionary<string, object> dict1 = new() {
                {"key3", "value3"},
                {"key2", "value2"},
                {"key1", "value1"}
            };
            Dictionary<string, object> dict2 = new() {
                {"key1", "value1"},
                {"key2", "value2"},
                {"key3", "value3"}
            };
            Assert.That(Utilities.DictionaryEquals(dict1, dict2));
        }
    }
 }


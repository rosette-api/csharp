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

namespace rosette_apiUnitTests
{
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
            RosetteFile f = new RosetteFile("rosette_api.xml", "dataType", null);
            Assert.AreEqual("rosette_api.xml", f.getFilename(), "Filename does not match");
            Assert.AreEqual("dataType", f.getDataType(), "DataType does not match");
            Assert.IsNull(f.getOptions(), "Options does not match");

            byte[] b = f.getFileData();
            Assert.IsTrue(b.Count() > 0, "File is empty");

            string content = f.getFileDataString();
            Assert.IsTrue(content.Length > 0, "File is empty");
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

        /// <summary>
        /// Helper to add the checkVersion test, since it gets called for every API instance
        /// </summary>
        /// <param name="mockHttp"></param>
        private void addCheckVersion(MockHttpMessageHandler mockHttp) {
            string jsonResponse = string.Format("{{'response': 'OK', 'version': '{0}', 'versionChecked': true}}", CAPI.Version);

            mockHttp.When(_testUrl)
                .WithQueryString(string.Format("clientVersion={0}", CAPI.Version))
                .Respond("applciation/json", jsonResponse);
        }

        [SetUp]
        public void Init() {
            _mockHttp = new MockHttpMessageHandler();
            var client = new HttpClient(_mockHttp);

            string jsonResponse = string.Format("{{'response': 'OK', 'version': '{0}', 'versionChecked': true}}", CAPI.Version);

            _mockHttp.When(_testUrl + "info")
                .WithQueryString(string.Format("clientVersion={0}", CAPI.Version))
                .Respond("applciation/json", jsonResponse);

            _rosetteApi = new CAPI("userkey", null, 1, client);
        }

        //------------------------- Get Calls (Info and Ping) ----------------------------------------

        [Test]
        public void InfoTest() {
            _mockHttp.When(_testUrl + "info")
                .Respond("application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Info();
            Assert.AreEqual(response["response"], "OK");
        }

        [Test]
        public void PingTest() {
            _mockHttp.When(_testUrl + "ping")
                .Respond("application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Ping();
            Assert.AreEqual(response["response"], "OK");
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
            Assert.AreEqual(response["response"], "OK");
        }

        [Test]
        public void Categories_Dict_Test() {
            _mockHttp.When(_testUrl + "categories")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Categories(new Dictionary<object, object>(){ {"contentUri", "contentUrl"} });
            Assert.AreEqual(response["response"], "OK");
        }

        [Test]
        public void Categories_File_Test() {
            _mockHttp.When(_testUrl + "categories")
                .Respond("application/json", "{'response': 'OK'}");

            RosetteFile f = new RosetteFile("rosette_apiUnitTests.cs");
            var response = _rosetteApi.Categories(f);
            Assert.AreEqual(response["response"], "OK");
        }

        //------------------------- Entities Linked ----------------------------------------

        [Test]
        public void EntitiesLinked_Content_Test() {
            _mockHttp.When(_testUrl + "entities/linked")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.EntitiesLinked("content");
            Assert.AreEqual(response["response"], "OK");
        }

        [Test]
        public void EntitiesLinked_Dict_Test() {
            _mockHttp.When(_testUrl + "entities/linked")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.EntitiesLinked(new Dictionary<object, object>() { { "contentUri", "contentUrl" } });
            Assert.AreEqual(response["response"], "OK");
        }

        [Test]
        public void EntitiesLinked_File_Test() {
            _mockHttp.When(_testUrl + "entities/linked")
                .Respond("application/json", "{'response': 'OK'}");

            RosetteFile f = new RosetteFile("rosette_apiUnitTests.cs");
            var response = _rosetteApi.EntitiesLinked(f);
            Assert.AreEqual(response["response"], "OK");
        }

        //------------------------- Entity ----------------------------------------

        [Test]
        public void Entity_Content_Test() {
            _mockHttp.When(_testUrl + "entities")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Entity("content");
            Assert.AreEqual(response["response"], "OK");
        }

        [Test]
        public void Entity_Dict_Test() {
            _mockHttp.When(_testUrl + "entities")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Entity(new Dictionary<object, object>() { { "contentUri", "contentUrl" } });
            Assert.AreEqual(response["response"], "OK");
        }

        [Test]
        public void Entity_File_Test() {
            _mockHttp.When(_testUrl + "entities")
                .Respond("application/json", "{'response': 'OK'}");

            RosetteFile f = new RosetteFile("rosette_apiUnitTests.cs");
            var response = _rosetteApi.Entity(f);
            Assert.AreEqual(response["response"], "OK");
        }

        //------------------------- Language ----------------------------------------

        [Test]
        public void Language_Content_Test() {
            _mockHttp.When(_testUrl + "language")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Language("content");
            Assert.AreEqual(response["response"], "OK");
        }

        [Test]
        public void Language_Dict_Test() {
            _mockHttp.When(_testUrl + "language")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Language(new Dictionary<object, object>() { { "contentUri", "contentUrl" } });
            Assert.AreEqual(response["response"], "OK");
        }

        [Test]
        public void Language_File_Test() {
            _mockHttp.When(_testUrl + "language")
                .Respond("application/json", "{'response': 'OK'}");

            RosetteFile f = new RosetteFile("rosette_apiUnitTests.cs");
            var response = _rosetteApi.Language(f);
            Assert.AreEqual(response["response"], "OK");
        }

        //------------------------- Morphology ----------------------------------------

        [Test]
        public void Morphology_Content_Test() {
            _mockHttp.When(_testUrl + "morphology/complete")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Morphology("content");
            Assert.AreEqual(response["response"], "OK");
        }

        [Test]
        public void Morphology_Dict_Test() {
            _mockHttp.When(_testUrl + "morphology/complete")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Morphology(new Dictionary<object, object>() { { "contentUri", "contentUrl" } });
            Assert.AreEqual(response["response"], "OK");
        }

        [Test]
        public void Morphology_File_Test() {
            _mockHttp.When(_testUrl + "morphology/complete")
                .Respond("application/json", "{'response': 'OK'}");

            RosetteFile f = new RosetteFile("rosette_apiUnitTests.cs");
            var response = _rosetteApi.Morphology(f);
            Assert.AreEqual(response["response"], "OK");
        }

        //------------------------- Name Similarity ----------------------------------------

        [Test]
        public void NameSimilarity_Content_Test() {
            _mockHttp.When(_testUrl + "name-similarity")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            Name name1 = new Name("Name One");
            Name name2 = new Name("Name Two");
            var response = _rosetteApi.NameSimilarity(name1, name2);
            Assert.AreEqual(response["response"], "OK");
        }

        [Test]
        public void NameSimilarity_Dict_Test() {
            _mockHttp.When(_testUrl + "name-similarity")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.NameSimilarity(new Dictionary<object, object>() { { "name1", "Name One" }, {"name2", "Name Two"} });
            Assert.AreEqual(response["response"], "OK");
        }

        //------------------------- Relationships ----------------------------------------

        [Test]
        public void Relationships_Content_Test() {
            _mockHttp.When(_testUrl + "relationships")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Relationships("content");
            Assert.AreEqual(response["response"], "OK");
        }

        [Test]
        public void Relationships_Dict_Test() {
            _mockHttp.When(_testUrl + "relationships")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Relationships(new Dictionary<object, object>() { { "contentUri", "contentUrl" } });
            Assert.AreEqual(response["response"], "OK");
        }

        [Test]
        public void Relationships_File_Test() {
            _mockHttp.When(_testUrl + "relationships")
                .Respond("application/json", "{'response': 'OK'}");

            RosetteFile f = new RosetteFile("rosette_apiUnitTests.cs");
            var response = _rosetteApi.Relationships(f);
            Assert.AreEqual(response["response"], "OK");
        }

        //------------------------- Sentences ----------------------------------------

        [Test]
        public void Sentences_Content_Test() {
            _mockHttp.When(_testUrl + "sentences")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Sentences("content");
            Assert.AreEqual(response["response"], "OK");
        }

        [Test]
        public void Sentences_Dict_Test() {
            _mockHttp.When(_testUrl + "sentences")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Sentences(new Dictionary<object, object>() { { "contentUri", "contentUrl" } });
            Assert.AreEqual(response["response"], "OK");
        }

        [Test]
        public void Sentences_File_Test() {
            _mockHttp.When(_testUrl + "sentences")
                .Respond("application/json", "{'response': 'OK'}");

            RosetteFile f = new RosetteFile("rosette_apiUnitTests.cs");
            var response = _rosetteApi.Sentences(f);
            Assert.AreEqual(response["response"], "OK");
        }

        //------------------------- Sentiment ----------------------------------------

        [Test]
        public void Sentiment_Content_Test() {
            _mockHttp.When(_testUrl + "sentiment")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Sentiment("content");
            Assert.AreEqual(response["response"], "OK");
        }

        [Test]
        public void Sentiment_Dict_Test() {
            _mockHttp.When(_testUrl + "sentiment")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Sentiment(new Dictionary<object, object>() { { "contentUri", "contentUrl" } });
            Assert.AreEqual(response["response"], "OK");
        }

        [Test]
        public void Sentiment_File_Test() {
            _mockHttp.When(_testUrl + "sentiment")
                .Respond("application/json", "{'response': 'OK'}");

            RosetteFile f = new RosetteFile("rosette_apiUnitTests.cs");
            var response = _rosetteApi.Sentiment(f);
            Assert.AreEqual(response["response"], "OK");
        }

        //------------------------- Tokens ----------------------------------------

        [Test]
        public void Tokens_Content_Test() {
            _mockHttp.When(_testUrl + "tokens")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Tokens("content");
            Assert.AreEqual(response["response"], "OK");
        }

        [Test]
        public void Tokens_Dict_Test() {
            _mockHttp.When(_testUrl + "tokens")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.Tokens(new Dictionary<object, object>() { { "contentUri", "contentUrl" } });
            Assert.AreEqual(response["response"], "OK");
        }

        [Test]
        public void Tokens_File_Test() {
            _mockHttp.When(_testUrl + "tokens")
                .Respond("application/json", "{'response': 'OK'}");

            RosetteFile f = new RosetteFile("rosette_apiUnitTests.cs");
            var response = _rosetteApi.Tokens(f);
            Assert.AreEqual(response["response"], "OK");
        }

        //------------------------- Name Translation ----------------------------------------

        [Test]
        public void NameTranslation_Content_Test() {
            _mockHttp.When(_testUrl + "name-translation")
                .Respond(HttpStatusCode.OK, "application/json", "{'response': 'OK'}");

            var response = _rosetteApi.NameTranslation("content");
            Assert.AreEqual(response["response"], "OK");
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
            Assert.AreEqual(response["response"], "OK");
        }

    }
}

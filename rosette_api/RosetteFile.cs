using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

namespace rosette_api {
    /// <summary>RosetteFile Class
    /// <para>
    /// RosetteFile: Custom Datatype containing information about files for upload, and methods to read the files
    /// </para>
    /// </summary>
    public class RosetteFile : IDisposable {
        /// <summary>RosetteFile
        /// <para>
        /// RosetteFile: Custom Datatype containing information about files for upload, and methods to read the files
        /// </para>
        /// </summary>
        /// <param name="fileName">string: Path to the data file</param>
        /// <param name="contentType">(string, optional): Description of the content type of the data file. "text/plain" is used if unsure.</param>
        /// <param name="options">(string, optional): Json string to add extra information</param>
        public RosetteFile(string fileName, string contentType = "text/plain", string options = null) {
            Filename = fileName;
            ContentType = contentType;
            Options = options;
            _multiPartContent = null;
        }

        bool disposed = false;

        /// <summary>
        /// Dispose method for RosetteFile - cleans things up
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Virtual Dispose that is actually used by the class
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing) {
            if (disposed) {
                return;
            }

            if (disposing) {
                _multiPartContent.Dispose();
            }
            disposed = true;
        }

        /// <summary>Filename
        /// <para>
        /// getFilename: Get the filename
        /// </para>
        /// </summary>
        /// <returns>string: String of the filename</returns>
        public string Filename { get; private set; }


        /// <summary>ContentType
        /// <para>
        /// getDataType: Get the content type
        /// </para>
        /// </summary>
        /// <returns>string: String of the content type</returns>
        public string ContentType { get; private set; }

        /// <summary>Options
        /// <para>
        /// getOptions: Get the options in JSON format
        /// </para>
        /// </summary>
        /// <returns>string: String of the options</returns>
        public string Options { get; private set; }

        /// <summary>
        /// internal storage for the _multiPart object
        /// </summary>
        private MultipartFormDataContent _multiPartContent;

        /// <summary>getFileData
        /// <para>
        /// getFileData: Get the FileData in byte form
        /// </para>
        /// </summary>
        /// <returns>byte[]: Byte Array of the file data</returns>
        public byte[] getFileData() {
            byte[] bytes = null;
            try {
                bytes = File.ReadAllBytes(Filename);
            }
            catch (Exception e) {
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }
            return bytes;
        }

        /// <summary>getFileDataString
        /// <para>
        /// getFileDataString: Get the FileData in string form
        /// </para>
        /// </summary>
        /// <returns>string: String of the file data</returns>
        public string getFileDataString() {
            try {
                using (StreamReader ff = File.OpenText(Filename)) {
                    byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(ff.ReadToEnd());
                    MemoryStream stream = new MemoryStream(byteArray);
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    return reader.ReadToEnd();
                }
            }
            catch (Exception e) {
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }
            return null;
        }

        /// <summary>
        /// AsMultipart creates a multipart form data object containing the file contents, aka "content" and, optionally,
        /// the options, identified as "request".
        /// </summary>
        /// <returns>Returns the file data as a multipart form-data object</returns>
        public MultipartFormDataContent AsMultipart() {
            Stream formDataStream = new System.IO.MemoryStream();
            if (_multiPartContent != null) {
                _multiPartContent.Dispose();
            }
            _multiPartContent = new MultipartFormDataContent();

            FileStream fs = File.OpenRead(Filename);
            var streamContent = new StreamContent(fs);
            streamContent.Headers.Add("Content-Type", ContentType);
            streamContent.Headers.Add("Content-Disposition", "form-data; name=\"content\"; filename=\"" + Path.GetFileName(Filename) + "\"");
            _multiPartContent.Add(streamContent, "content", Path.GetFileName(Filename));

            if (!string.IsNullOrEmpty(Options)) {
                var stringContent = new StringContent(Options, Encoding.UTF8, "application/json");
                stringContent.Headers.Add("Content-Disposition", "form-data; name=\"request\"");
                _multiPartContent.Add(stringContent, "request");
            }

            return _multiPartContent;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using rosette_api;

namespace rosette_apiExamples
{
    class sentiment
    {
        /// <summary>
        /// Example code to call Rosette API to get a document's sentiment
        /// Requires Nuget Package:
        /// rosette_api
        /// </summary>
        static void Main(string[] args)
        {
            //To use the C# API, you must provide an API key
            string apikey = "Your API key";
            string alturl = string.Empty;

            //You may set the API key via command line argument:
            //sentiment yourapikeyhere
            if (args.Length != 0)
            {
                apikey = args[0];
                alturl = args.Length > 1 ? args[1] : string.Empty;
            }

            try
            {
                CAPI SentimentCAPI = string.IsNullOrEmpty(alturl) ? new CAPI(apikey) : new CAPI(apikey, alturl);
                var newFile = Path.GetTempFileName();
                StreamWriter sw = new StreamWriter(newFile);
                string sentiment_file_data = @"<html><head><title>New Ghostbusters Film</title></head><body><p>Original Ghostbuster Dan Aykroyd, who also co-wrote the 1984 Ghostbusters film, couldn’t be more pleased with the new all-female Ghostbusters cast, telling The Hollywood Reporter, “The Aykroyd family is delighted by this inheritance of the Ghostbusters torch by these most magnificent women in comedy.”</p></body></html>";
                sw.WriteLine(sentiment_file_data);
                sw.Flush();
                sw.Close();
                //Rosette API provides File upload options (shown here)
                //Simply create a new RosetteFile using the path to a file
                //The results of the API call will come back in the form of a Dictionary
                Dictionary<string, Object> SentimentResult = SentimentCAPI.Sentiment(new RosetteFile(newFile));
                Console.WriteLine(new JavaScriptSerializer().Serialize(SentimentResult));

                if (File.Exists(newFile)) {
                    File.Delete(newFile);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }
    }
}

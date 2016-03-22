using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using rosette_api;

namespace rosette_apiExamples
{
    class morphology_complete
    {
        /// <summary>
        /// Example code to call Rosette API to get the complete set of morphological analysis results for a piece of text.
        /// Requires Nuget Package:
        /// rosette_api
        /// </summary>
        static void Main(string[] args)
        {
            //To use the C# API, you must provide an API key
            string apikey = "Your API key";
            string alturl = string.Empty;

            //You may set the API key via command line argument:
            //morphology_complete yourapikeyhere
            if (args.Length != 0)
            {
                apikey = args[0];
                alturl = args.Length > 1 ? args[1] : string.Empty;
            } 
            try
            {
                CAPI MorphologyCAPI = string.IsNullOrEmpty(alturl) ? new CAPI(apikey) : new CAPI(apikey, alturl);
                string morphology_complete_data = @"The quick brown fox jumped over the lazy dog. Yes he did.";
                //The results of the API call will come back in the form of a Dictionary
                RosetteResponse response = MorphologyCAPI.Morphology(morphology_complete_data);
                foreach (KeyValuePair<string, string> h in response.Headers) {
                    Console.WriteLine(string.Format("{0}:{1}", h.Key, h.Value));
                }
                Console.WriteLine(response.ContentAsJson);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }
    }
}

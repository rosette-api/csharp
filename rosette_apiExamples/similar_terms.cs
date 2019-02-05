using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using rosette_api;

namespace rosette_apiExamples
{
    class similar_terms
    {
        /// <summary>
        /// Example code to call Rosette API to get an input's similar terms.
        /// Requires Nuget Package:
        /// rosette_api
        /// </summary>
        static void Main(string[] args)
        {
            //To use the C# API, you must provide an API key
            string apikey = "Your API key";
            string alturl = string.Empty;

            //You may set the API key via command line argument:
            //similar-terms yourapikeyhere
            if (args.Length != 0)
            {
                apikey = args[0];
                alturl = args.Length > 1 ? args[1] : string.Empty;
            }
            try
            {
                CAPI EmbeddingCAPI = string.IsNullOrEmpty(alturl) ? new CAPI(apikey) : new CAPI(apikey, alturl);
                string similar_terms_data = "spy";
                //The results of the API call will come back in the form of a Dictionary
                EmbeddingCAPI.SetOption("resultLanguages", new List<String>() {"spa", "deu", "jpn"});
                SimilarTermsResponse response = EmbeddingCAPI.SimilarTerms(similar_terms_data);
                foreach (KeyValuePair<string, string> h in response.Headers)
                {
                    Console.WriteLine(string.Format("{0}:{1}", h.Key, h.Value));
                }

                Console.WriteLine(response.ContentAsJson);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using rosette_api;

namespace rosette_apiExamples
{
    class tokens
    {
        /// <summary>
        /// Example code to call Rosette API to get tokens (words) in a piece of text.
        /// Requires Nuget Package:
        /// rosette_api
        /// </summary>
        static void Main(string[] args)
        {
            //To use the C# API, you must provide an API key
            string apikey = "Your API key";
            string alturl = string.Empty;

            //You may set the API key via command line argument:
            //tokens yourapikeyhere
            if (args.Length != 0)
            {
                apikey = args[0];
                alturl = args.Length > 1 ? args[1] : string.Empty;
            }
            try
            {
                CAPI TokensCAPI = string.IsNullOrEmpty(alturl) ? new CAPI(apikey) : new CAPI(apikey, alturl);
                string tokens_data = @"北京大学生物系主任办公室内部会议";
                //The results of the API call will come back in the form of a Dictionary
                TokenizationResponse response = TokensCAPI.Tokens(tokens_data, null, "sentence", null);
                foreach (KeyValuePair<string, string> h in response.Headers) {
                    Console.WriteLine(string.Format("{0}:{1}", h.Key, h.Value));
                }
                Console.WriteLine(response.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }
    }
}

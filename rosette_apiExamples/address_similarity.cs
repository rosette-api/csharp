using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using rosette_api;

namespace rosette_apiExamples
{
    class address_similarity
    {
        /// <summary>
        /// Example code to call Analytics API to get match score (similarity) for two addresses.
        /// Requires Nuget Package:
        /// rosette_api
        /// </summary>
        static void Main(string[] args)
        {
            //To use the C# API, you must provide an API key
            string apikey = "Your API key";
            string alturl = string.Empty;

            //You may set the API key via command line argument:
            //address_similarity yourapikeyhere
            if (args.Length != 0)
            {
                apikey = args[0];
                alturl = args.Length > 1 ? args[1] : string.Empty;
            } 
            try
            {
                CAPI cAPI = string.IsNullOrEmpty(alturl) ? new CAPI(apikey) : new CAPI(apikey, alturl);
                //The results of the API call will come back in the form of a Dictionary
                AddressSimilarityResponse response = cAPI.AddressSimilarity(new FieldedAddress(houseNumber:"1600", road:"Pennsylvania Ave N.W.", city:"Washington", state:"DC", postCode: "20500"), new UnfieldedAddress(address:"160 Pennsylvana Avenue, Washington, D.C., 20500"));
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

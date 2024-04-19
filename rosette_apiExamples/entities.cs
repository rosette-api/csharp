using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using rosette_api;

namespace rosette_apiExamples
{
    class entities
    {
        /// <summary>
        /// Example code to call Rosette API to get entities from a piece of text.
        /// Requires Nuget Package:
        /// rosette_api
        /// </summary>
        static void Main(string[] args)
        {
            //To use the C# API, you must provide an API key
            string apikey = "Your API key";
            string alturl = string.Empty;

            //You may set the API key via command line argument:
            //entities yourapikeyhere
            if (args.Length != 0)
            {
                apikey = args[0];
                alturl = args.Length > 1 ? args[1] : string.Empty;
            }
            try
            {
                CAPI EntitiesCAPI = string.IsNullOrEmpty(alturl) ? new CAPI(apikey) : new CAPI(apikey, alturl);
                string entities_text_data = @"The Securities and Exchange Commission today announced the leadership of the agency’s trial unit.  Bridget Fitzpatrick has been named Chief Litigation Counsel of the SEC and David Gottesman will continue to serve as the agency’s Deputy Chief Litigation Counsel. Since December 2016, Ms. Fitzpatrick and Mr. Gottesman have served as Co-Acting Chief Litigation Counsel.  In that role, they were jointly responsible for supervising the trial unit at the agency’s Washington D.C. headquarters as well as coordinating with litigators in the SEC’s 11 regional offices around the country.";
                //The results of the API call will come back in the form of a Dictionary
                EntitiesResponse response = EntitiesCAPI.Entity(entities_text_data);
                foreach (KeyValuePair<string, string> h in response.Headers) {
                    Console.WriteLine(string.Format("{0}:{1}", h.Key, h.Value));
                }
                // PrintContent() is a provided method to print the Dictionary to the console
                response.PrintContent();

                // Entities with full ADM
                EntitiesCAPI.SetUrlParameter("output", "rosette");

                // Within a document, there may be multiple references to a single entity.
                // indoc-coref server chains together all mentions to an entity.
                // Uncomment the next line to enable the entity extraction to use the indoc-coref server
                // EntitiesCAPI.SetOption("useIndocServer", true);

                response = EntitiesCAPI.Entity(entities_text_data);
                // response.Content contains the IDictionary results of the full ADM.
                // PrintContent() is a provided method to print the Dictionary to the console
                response.PrintContent();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }
    }
}

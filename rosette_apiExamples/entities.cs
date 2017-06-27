﻿using System;
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
                string entities_text_data = @"Bill Murray will appear in new Ghostbusters film: Dr. Peter Venkman was spotted filming a cameo in Boston this… http://dlvr.it/BnsFfS";
                //The results of the API call will come back in the form of a Dictionary
                EntitiesResponse response = EntitiesCAPI.Entity(entities_text_data, null, null, null, "social-media");
                foreach (KeyValuePair<string, string> h in response.Headers) {
                    Console.WriteLine(string.Format("{0}:{1}", h.Key, h.Value));
                }
                Console.WriteLine(response.ToString());

                // Entities with full ADM
                EntitiesCAPI.SetUrlParameter("output", "rosette");
                response = EntitiesCAPI.Entity(entities_text_data, null, null, null, "social-media");
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

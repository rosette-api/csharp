﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using rosette_api;

namespace rosette_apiExamples
{
    class categories
    {
        /// <summary>
        /// Example code to call Rosette API to get a document's (located at given URL) categories.
        /// Requires Nuget Package:
        /// rosette_api
        /// </summary>
        static void Main(string[] args)
        {
            //To use the C# API, you must provide an API key
            string apikey = "Your API key";
            string alturl = string.Empty;
            
            //You may set the API key via command line argument:
            //categories yourapikeyhere
            if (args.Length != 0)
            {
                apikey = args[0];
                alturl = args.Length > 1 ? args[1] : string.Empty;
            }
            try
            {
                CAPI CategoriesCAPI = string.IsNullOrEmpty(alturl) ? new CAPI(apikey) : new CAPI(apikey, alturl);
                string categories_text_data = @"Sony Pictures is planning to shoot a good portion of the new ""Ghostbusters"" in Boston as well.";
                //The results of the API call will come back in the form of a Dictionary
                CategoriesResponse response = CategoriesCAPI.Categories(categories_text_data,  null, null, null);
                Console.WriteLine(response.ContentAsJson);

                //Rosette API also supports Dictionary inputs
                //Simply instantiate a new dictionary object with the fields options as keys and inputs as values
                string categories_url_data = @"https://onlocationvacations.com/2015/03/05/the-new-ghostbusters-movie-begins-filming-in-boston-in-june/";
                response = CategoriesCAPI.Categories(new Dictionary<object, object>()
                {
                    {"contentUri", categories_url_data}

                });
                foreach (KeyValuePair<string, string> h in response.Headers) {
                    Console.WriteLine(string.Format("{0}:{1}", h.Key, h.Value));
                }
                Console.WriteLine(response.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}

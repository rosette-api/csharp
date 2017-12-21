﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using rosette_api;

namespace rosette_apiExamples
{
    class name_deduplication
    {
        /// <summary>
        /// Example code to call Rosette API to deduplication a list of names.
        /// Requires Nuget Package:
        /// rosette_api
        /// </summary>
        static void Main(string[] args)
        {
            //To use the C# API, you must provide an API key
            string apikey = "Your API key";
            string alturl = string.Empty;

            //You may set the API key via command line argument:
            //matched_name yourapikeyhere
            if (args.Length != 0)
            {
                apikey = args[0];
                alturl = args.Length > 1 ? args[1] : string.Empty;
            }
            try
            {
                CAPI rosetteApi = string.IsNullOrEmpty(alturl) ? new CAPI(apikey) : new CAPI(apikey, alturl);
                string name_dedupe_data = @"Alice Terry,Alice Thierry,Betty Grable,Betty Gable,Norma Shearer,Norm Shearer,Brigitte Helm,Bridget Helem,Judy Holliday,Julie Halliday";

                List<string> dedup_names = name_dedupe_data.Split(',').ToList<string>();
                List<Name> names = dedup_names.Select(name => new Name(name, "eng", "Latn")).ToList();
                float threshold = 0.75f;

                //The results of the API call will come back in the form of a Dictionary
                NameDeduplicationResponse response = rosetteApi.NameDeduplication(names, threshold);
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

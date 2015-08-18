﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using rosette_api;

namespace rosette_apiExamples
{
    class info
    {
        /// <summary>
        /// Example code to call Rosette API to get information such as version and build.
        /// Requires Nuget Package:
        /// rosette_api
        /// </summary>
        static void Main(string[] args)
        {
            //To use the C# API, you must provide an API key
            string apikey = "Your API key";

            //You may set the API key via command line argument:
            //info yourapikeyhere
            if (args.Length != 0)
            {
                apikey = args[0];
            } 
            try
            {
                CAPI InfoCAPI = new CAPI(apikey);
                //The results of the API call will come back in the form of a Dictionary
                Dictionary<string, Object> infoResult = InfoCAPI.Info();
                Console.WriteLine(new JavaScriptSerializer().Serialize(infoResult));
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }
    }
}
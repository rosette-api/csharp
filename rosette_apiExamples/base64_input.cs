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

            //You may set the API key via command line argument:
            //entities yourapikeyhere
            if (args.Length != 0)
            {
                apikey = args[0];
            }
            try
            {
                CAPI EntitiesCAPI = new CAPI(apikey);
                string entities_text_data = @"Bill Murray will appear in new Ghostbusters film: Dr. Peter Venkman was spotted filming a cameo in Boston this… http://dlvr.it/BnsFfS";
                var exampleBytes = System.Text.Encoding.UTF8.GetBytes(entities_text_data);
                String exampleText = System.Convert.ToBase64String(exampleBytes);
                //The results of the API call will come back in the form of a Dictionary
                Dictionary<string, Object> EntitiesResult = EntitiesCAPI.Entity(exampleText);
                Console.WriteLine(new JavaScriptSerializer().Serialize(EntitiesResult));
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }
    }
}

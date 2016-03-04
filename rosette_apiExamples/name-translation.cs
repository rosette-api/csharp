using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using rosette_api;

namespace rosette_apiExamples
{
    class translated_name
    {
        /// <summary>
        /// Example code to call Rosette API to translate a name from language to another.
        /// Requires Nuget Package:
        /// rosette_api
        /// </summary>
        static void Main(string[] args)
        {
            //To use the C# API, you must provide an API key
            string apikey = "Your API key";
            string alturl = string.Empty;

            //You may set the API key via command line argument:
            //translated_name yourapikeyhere
            if (args.Length != 0)
            {
                apikey = args[0];
                alturl = args.Length > 1 ? args[1] : string.Empty;
            }
            try
            {
                CAPI TranslatedNameCAPI = string.IsNullOrEmpty(alturl) ? new CAPI(apikey) : new CAPI(apikey, alturl);
                string translated_name_data = @"معمر محمد أبو منيار القذاف";
                //The results of the API call will come back in the form of a Dictionary
                Dictionary<string, Object> TranslatedNameResult = TranslatedNameCAPI.NameTranslation(translated_name_data, null, null, "eng", "Latn", "IC", null, "PERSON"); Console.WriteLine(new JavaScriptSerializer().Serialize(TranslatedNameResult));
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }
    }
}

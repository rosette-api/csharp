using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using rosette_api;

namespace rosette_apiExamples
{
    class record_similarity
    {
        /// <summary>
        /// Example code to call Rosette API to get record similarity scores between two list of records
        /// Requires Nuget Package:
        /// rosette_api
        /// </summary>
        static void Main(string[] args)
        {
            //To use the C# API, you must provide an API key
            string apikey = "Your API key";
            string alturl = string.Empty;

            //You may set the API key via command line argument:
            //record_similarity yourapikeyhere
            if (args.Length != 0)
            {
                apikey = args[0];
                alturl = args.Length > 1 ? args[1] : string.Empty;
            }
            try
            {
                CAPI RecordSimilarityCAPI = string.IsNullOrEmpty(alturl) ? new CAPI(apikey) : new CAPI(apikey, alturl);

                // record field names
                string primaryNameField = "primaryName";
                string dobField = "dob";
                string dob2Field = "dob2";
                string addrField = "addr";
                string dobHyphen = "1993-04-16";

                // Creating the request object
                Dictionary<string, RecordSimilarityFieldInfo> fields = new Dictionary<string, RecordSimilarityFieldInfo>
                {
                    { primaryNameField, new RecordSimilarityFieldInfo { Type = RecordFieldType.rni_name, Weight = 0.5 } },
                    { dobField, new RecordSimilarityFieldInfo { Type = RecordFieldType.rni_date, Weight = 0.2 } },
                    { dob2Field, new RecordSimilarityFieldInfo { Type = RecordFieldType.rni_date, Weight = 0.1 } },
                    { addrField, new RecordSimilarityFieldInfo { Type = RecordFieldType.rni_address, Weight = 0.5 } }
                };

                RecordSimilarityProperties properties = new RecordSimilarityProperties { Threshold = 0.7, IncludeExplainInfo = false };

                RecordSimilarityRecords records = new RecordSimilarityRecords {
                    Left = new List<Dictionary<string, RecordSimilarityField>>
                    {
                        new Dictionary<string, RecordSimilarityField>
                        {
                            { primaryNameField, new FieldedNameRecord { Text = "Ethan R", Language = "eng", LanguageOfOrigin = "eng", Script = "Latn", EntityType = "PERSON"} },
                            { dobField, new UnfieldedDateRecord { Date = dobHyphen} },
                            { dob2Field, new FieldedDateRecord { Date = "1993/04/16"} },
                            { addrField, new UnfieldedAddressRecord { Address = "123 Roadlane Ave"}}
                        },
                        new Dictionary<string, RecordSimilarityField>
                        {
                            { primaryNameField, new FieldedNameRecord { Text = "Evan R"} },
                            { dobField, new FieldedDateRecord { Date = dobHyphen} }
                        }
                    },
                    Right = new List<Dictionary<string, RecordSimilarityField>>
                    {
                        new Dictionary<string, RecordSimilarityField>
                        {
                            { primaryNameField, new FieldedNameRecord { Text = "Seth R", Language = "eng"} },
                            { dobField, new FieldedDateRecord { Date = dobHyphen} }
                        },
                        new Dictionary<string, RecordSimilarityField>
                        {
                            { primaryNameField, new UnfieldedNameRecord { Text = "Ivan R"} },
                            { dobField, new FieldedDateRecord { Date = dobHyphen} },
                            { dob2Field, new FieldedDateRecord { Date = "1993/04/16"} },
                            { addrField, new FieldedAddressRecord { Address = "123 Roadlane Ave"} }
                        }
                    }
                };

                RecordSimilarityRequest request = new RecordSimilarityRequest
                {
                    Fields = fields,
                    Properties = properties,
                    Records = records
                };

                //The results of the API call will come back in the form of a Dictionary
                RecordSimilarityResponse response = RecordSimilarityCAPI.RecordSimilarity(request);

                foreach (KeyValuePair<string, string> h in response.Headers) {
                    Console.WriteLine(string.Format("{0}:{1}", h.Key, h.Value));
                }

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
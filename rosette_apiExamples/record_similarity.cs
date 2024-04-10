using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using rosette_api;

namespace rosette_apiExamples
{
    class record_similarity
    {
        /// <summary>
        /// Example code to send Rosette API a ping to check its reachability.
        /// Requires Nuget Package:
        /// rosette_api
        /// </summary>
        static void Main(string[] args)
        {
            string primaryNameField = "primaryName";
            string dobField = "dob";
            string dob2Field = "dob2";
            string addrField = "addr";
            string dobHyphen = "1993-04-16";
            Dictionary<string, RecordSimilarityFieldInfo> fields = new Dictionary<string, RecordSimilarityFieldInfo>
            {
                { primaryNameField, new RecordSimilarityFieldInfo { Type = RecordFieldType.rni_name, Weight = 0.5 } },
                { dobField, new RecordSimilarityFieldInfo { Type = RecordFieldType.rni_date, Weight = 0.2 } },
                { dob2Field, new RecordSimilarityFieldInfo { Type = RecordFieldType.rni_date, Weight = 0.1 } },
                { addrField, new RecordSimilarityFieldInfo { Type = RecordFieldType.rni_address, Weight = 0.5 } }
            };
            RecordSimilarityProperties properties = new RecordSimilarityProperties { Threshold = 0.7, IncludeExplainInfo = true };
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
            //RecordSimlarityRequest
            RecordSimilarityRequest request = new RecordSimilarityRequest
            {
                Fields = fields,
                Properties = properties,
                Records = records
            };
            //create a CAPI
            CAPI RecordSimilarityCAPI = new CAPI("your_api_key", "http://172.17.0.1:8181/rest/v1");
            RecordSimilarityResponse result = RecordSimilarityCAPI.RecordSimilarity(request);
            //get the content of result
            Console.WriteLine("*****");
            foreach (KeyValuePair<string, RecordSimilarityFieldInfo> item in result.Fields)
            {
                Console.WriteLine($"Key: {item.Key}, Value: {item.Value}");
            }
            List<RecordSimilarityResult> recordSimilarityResults = result.Results;
            foreach (RecordSimilarityResult recordSimilarityResult in recordSimilarityResults)
            {
                Console.WriteLine("*****");
                Console.WriteLine("Score: " + recordSimilarityResult.Score);
                // print explain info
                if (recordSimilarityResult.ExplainInfo != null)
                {
                    
                    //print left only fields
                    Console.WriteLine("Left Only Fields: ");
                    foreach (string leftFields in recordSimilarityResult.ExplainInfo.LeftOnlyFields)
                    {
                        Console.WriteLine(string.Join(", ", leftFields));
                    }

                    //print right only fields
                    Console.WriteLine("Right Only Fields: ");
                    foreach (string rightFields in recordSimilarityResult.ExplainInfo.RightOnlyFields)
                    {
                        Console.WriteLine(string.Join(", ", rightFields));
                    }
                    //print scorefields
                    Console.WriteLine("Score Fields: ");
                    foreach (KeyValuePair<string, RecordSimilarityFieldExplainInfo> item in recordSimilarityResult.ExplainInfo.ScoredFields)
                    {
                        RecordSimilarityFieldExplainInfo recordSimilarityFieldExplainInfo = item.Value;
                        //Log the key and record similarity field explain info's properties
                        Console.WriteLine($"Key: {item.Key}, Value: | Weight: {recordSimilarityFieldExplainInfo.Weight}, CalculatedWeight: {recordSimilarityFieldExplainInfo.CalculatedWeight}, RawScore: {recordSimilarityFieldExplainInfo.RawScore}, FinalScore: {recordSimilarityFieldExplainInfo.FinalScore}, Details: {recordSimilarityFieldExplainInfo.Details}|");
                        
                    }

                }
                Console.WriteLine("Left Record: ");
                foreach (KeyValuePair<string, RecordSimilarityField> item in recordSimilarityResult.Left)
                {
                    Console.WriteLine($"Key: {item.Key}, ValueType: {item.Value.GetType()}, SerializedValue: {JsonConvert.SerializeObject(item.Value)}");
                }
                Console.WriteLine("Right Record: ");
                foreach (KeyValuePair<string, RecordSimilarityField> item in recordSimilarityResult.Right)
                {
                    Console.WriteLine($"Key: {item.Key}, ValueType: {item.Value.GetType()}, SerializedValue: {JsonConvert.SerializeObject(item.Value)}");
                }
            }
            Console.WriteLine("*****");
            // result.PrintContent();
        }
    }
}
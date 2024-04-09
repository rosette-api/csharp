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
    class ping
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
            RecordSimilarityRequest fieldInfo = new RecordSimilarityRequest
            {
                Fields = fields,
                Properties = properties,
                Records = records
            };
            // Serialize the object
            string json = JsonConvert.SerializeObject(fieldInfo, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore});
            Console.WriteLine(json);
        }
    }
}
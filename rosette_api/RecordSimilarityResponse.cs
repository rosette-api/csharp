using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;


namespace rosette_api {

    /// <summary>
    /// A class to represent the results from the Record Similarity endpoint of the Rosette API
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class RecordSimilarityResponse : RosetteResponse {
        private const string FIELDS = "fields";
        private const string RESULTS = "results";
        private const string ERROR_MESSAGE = "errorMessage";
        /// <summary>
        /// Gets or sets the the record similarity request's fields
        /// </summary>
        [JsonProperty(PropertyName = FIELDS)]
        public Dictionary<string, RecordSimilarityFieldInfo> Fields { get; set; }
        
        /// <summary>
        /// Gets or sets the the record similarity request's results
        /// </summary>
        [JsonProperty(PropertyName = RESULTS)]
        public List<RecordSimilarityResult> Results { get; set; }

        /// <summary>
        ///  Gets or sets the error message
        /// </summary>
        [JsonProperty(PropertyName = ERROR_MESSAGE)]
        public string ErrorMessage { get; set; }



        /// <summary>
        /// Creates a RecordSimilarityResponse from the given apiResults
        /// </summary>
        /// <param name="apiResults">The message from the API</param>
        public RecordSimilarityResponse(HttpResponseMessage apiResults) :base(apiResults)
        {
            this.Fields = this.ContentDictionary.ContainsKey(FIELDS) ? parseFields(this.ContentDictionary[FIELDS] as JObject) : null;
            this.Results = this.ContentDictionary.ContainsKey(RESULTS) ? parseResults(this.ContentDictionary[RESULTS] as JArray) : null;
        }

        private Dictionary<string, RecordSimilarityFieldInfo> parseFields(JObject fieldsJSON)
        {
            Dictionary<string, RecordSimilarityFieldInfo> fields = new Dictionary<string, RecordSimilarityFieldInfo>();
            // iterate over the keys in the fields object
            foreach (var property in fieldsJSON.Properties())
            {
                string key = property.Name;
                JObject field = fieldsJSON[key] as JObject;
                RecordFieldType type = (RecordFieldType)Enum.Parse(typeof(RecordFieldType), field["type"].Value<string>());
                double weight = field["weight"].ToObject<double>();
                RecordSimilarityFieldInfo filedInfo = new RecordSimilarityFieldInfo(type, weight); 
                fields.Add(key, filedInfo);
            }
            return fields;
        }

        private List<RecordSimilarityResult> parseResults(JArray resultsJSON)
        {
            List<RecordSimilarityResult> results = new List<RecordSimilarityResult>();
            foreach (JObject result in resultsJSON)
            {
                results.Add(new RecordSimilarityResult(result, this.Fields));
            }
            return results;
        }

        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns>True if equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is RecordSimilarityResponse)
            {
                RecordSimilarityResponse other = obj as RecordSimilarityResponse;
                List<bool> conditions = new List<bool>() {
                    this.Fields != null && other.Fields != null ? this.Fields.SequenceEqual(other.Fields) : this.Fields == other.Fields,
                    this.Results != null && other.Results != null ? this.Results.SequenceEqual(other.Results) : this.Results == other.Results,
                    this.ErrorMessage == other.ErrorMessage
                };
                return conditions.All(condition => condition);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Hashcode override
        /// </summary>
        /// <returns>The hashcode</returns>
        public override int GetHashCode()
        {
            int h0 = this.Fields != null ? this.Fields.Aggregate<KeyValuePair<string, RecordSimilarityFieldInfo>, int>(1, (seed, item) => seed ^ item.GetHashCode()) : 1;
            int h1 = this.Results != null ? this.Results.Aggregate<RecordSimilarityResult, int>(1, (seed, item) => seed ^ item.GetHashCode()) : 1;
            int h2 = this.ErrorMessage != null ? this.ErrorMessage.GetHashCode() : 1;
            int h3 = this.ResponseHeaders != null ? this.ResponseHeaders.GetHashCode() : 1;
            return h0 ^ h1 ^ h2;
        }
    }

    /// <summary>
    /// A class to represent the results from the Record Similarity endpoint of the Rosette API
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class RecordSimilarityResult {
        private const string SCORE = "score";
        private const string LEFT = "left";
        private const string RIGHT = "right";
        private const string EXPLAIN_INFO = "explainInfo";
        private const string ERROR = "error";

        /// <summary>
        /// Gets or sets the score of the record similarity
        /// </summary>
        [JsonProperty(PropertyName = SCORE)]
        public double Score { get; set; }

        /// <summary>
        /// Gets or sets the left record fields
        /// </summary>
        [JsonProperty(PropertyName = LEFT)]
        public Dictionary<string, RecordSimilarityField> Left { get; set; }

        /// <summary>
        /// Gets or sets the right record fields
        /// </summary>
        [JsonProperty(PropertyName = RIGHT)]
        public Dictionary<string, RecordSimilarityField> Right { get; set; }

        /// <summary>
        /// Gets or sets the explain info
        /// </summary>
        [JsonProperty(PropertyName = EXPLAIN_INFO)]
        public RecordSimilarityExplainInfo ExplainInfo { get; set; }

        /// <summary>
        /// Gets or sets the error message
        /// </summary>
        [JsonProperty(PropertyName = ERROR)]
        public string Error { get; set; }

        /// <summary>
        /// Creates a RecordSimilarityResult from the given score, left, right, explainInfo, and error
        /// </summary>
        public RecordSimilarityResult(double score, Dictionary<string, RecordSimilarityField> left, Dictionary<string, RecordSimilarityField> right, RecordSimilarityExplainInfo explainInfo, string error)
        {
            Score = score;
            Left = left;
            Right = right;
            ExplainInfo = explainInfo;
            Error = error;
        }

        public RecordSimilarityResult(JObject jsonResult, Dictionary<string, RecordSimilarityFieldInfo> fields) {
            Score = jsonResult[SCORE].ToObject<double>();
            Left = parseRecord(jsonResult[LEFT] as JObject, fields);
            Right = parseRecord(jsonResult[RIGHT] as JObject, fields);
            ExplainInfo = jsonResult[EXPLAIN_INFO] != null ? new RecordSimilarityExplainInfo(jsonResult[EXPLAIN_INFO] as JObject) : null;
            Error = jsonResult[ERROR] != null ? jsonResult[ERROR].Value<string>() : null;
        }

        private Dictionary<string, RecordSimilarityField> parseRecord(JObject record, Dictionary<string, RecordSimilarityFieldInfo> fields) {
            Dictionary<string, RecordSimilarityField> recordFields = new Dictionary<string, RecordSimilarityField>();
            foreach (JProperty property in record.Properties())
            {
                string fieldName = property.Name;
                JToken fieldValue = property.Value;
                if (fields.ContainsKey(fieldName))
                {
                    RecordSimilarityFieldInfo fieldInfo = fields[fieldName];
                    if (fieldInfo.GetType() == null ) {
                        throw new RosetteException("Unspecified field type for: " + fieldName);
                    }
                    RecordSimilarityField fieldData;

                    switch (fieldInfo.Type)
                    {
                        case RecordFieldType.rni_name:
                            fieldData = parseNameRecord(fieldValue);
                            break;
                        case RecordFieldType.rni_address:
                            fieldData = parseAddressRecord(fieldValue);
                            break;
                        case RecordFieldType.rni_date:
                            fieldData = parseDateRecord(fieldValue);
                            break;
                        default:
                            throw new RosetteException("Unsupported field type: " + fieldInfo.GetType());
                    }   
                    recordFields.Add(fieldName, fieldData);

                }
                else
                {
                    throw new RosetteException("Unsupported field name: " + fieldName + " not found in field mapping");
                }
            }
            return recordFields;
        }

        private RecordSimilarityField parseNameRecord(JToken nameRecord) {
            // check if nameRecord.Value is an object or a string
            if (nameRecord.Type == JTokenType.String)
            {
                return new UnfieldedNameRecord(nameRecord.Value<string>());
            } 
            else
            {
                // TODO Extract string literals (use the ones from the RecordSimilarityField classes)
                JObject nameRecordObj = nameRecord as JObject;
                string text = nameRecordObj["text"].Value<string>();
                string language = nameRecordObj["language"] != null ? nameRecordObj["language"].Value<string>() : null;
                string languageOfOrigin = nameRecordObj["languageOfOrigin"] != null ? nameRecordObj["languageOfOrigin"].Value<string>() : null;
                string script = nameRecordObj["script"] != null ? nameRecordObj["script"].Value<string>() : null;
                string entityType = nameRecordObj["entityType"] != null ? nameRecordObj["entityType"].Value<string>() : null;
                return new FieldedNameRecord(text, language, languageOfOrigin, script, entityType);
            }
        }

        private RecordSimilarityField parseAddressRecord(JToken addressRecord) {
            // check if addressRecord.Value is an object or a string
            if (addressRecord.Type == JTokenType.String)
            {
                return new UnfieldedAddressRecord(addressRecord.Value<string>());
            }
            else
            {
                // TODO Extract string literals (use the ones from the RecordSimilarityField classes)
                JObject addressRecordObj = addressRecord as JObject;
                string address = addressRecordObj["address"].Value<string>();
                return new FieldedAddressRecord(address);
            }
        }

        private RecordSimilarityField parseDateRecord(JToken dateRecord) {
            // check if dateRecord.Value is an object or a string
            if (dateRecord.Type == JTokenType.String)
            {
                return new UnfieldedDateRecord(dateRecord.Value<string>());
            }
            else
            {
                // TODO Extract string literals (use the ones from the RecordSimilarityField classes)
                JObject dateRecordObj = dateRecord as JObject;
                string date = dateRecordObj["date"].Value<string>();
                return new FieldedDateRecord(date);
            }
        }
    }


    /// <summary>
    /// A class to represent the explaininfo of a record similarity result
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class RecordSimilarityExplainInfo {

        private const string SCORED_FIELDS = "scoredFields";
        private const string LEFT_ONLY_FIELDS = "leftOnlyFields";
        private const string RIGHT_ONLY_FIELDS = "rightOnlyFields";

        /// <summary>
        /// Gets or sets the score fields
        /// </summary>
        [JsonProperty(PropertyName = SCORED_FIELDS)]
        public Dictionary<string, RecordSimilarityFieldExplainInfo> ScoredFields { get; set; }

        /// <summary>
        /// Gets or sets the left only fields
        /// </summary>
        [JsonProperty(PropertyName = LEFT_ONLY_FIELDS)]
        public List<string> LeftOnlyFields { get; set; }

        /// <summary>
        /// Gets or sets the right only fields
        /// </summary>
        [JsonProperty(PropertyName = RIGHT_ONLY_FIELDS)]
        public List<string> RightOnlyFields { get; set; }

        public RecordSimilarityExplainInfo(Dictionary<string, RecordSimilarityFieldExplainInfo> scoreFields, List<string> leftOnlyFields, List<string> rightOnlyFields) {
            ScoredFields = scoreFields;
            LeftOnlyFields = leftOnlyFields;
            RightOnlyFields = rightOnlyFields;
        }

        public RecordSimilarityExplainInfo(JObject jsonExplainInfo) {
            ScoredFields = jsonExplainInfo[SCORED_FIELDS] != null ? parseScoreFields(jsonExplainInfo[SCORED_FIELDS] as JObject): null;
            LeftOnlyFields = jsonExplainInfo[LEFT_ONLY_FIELDS] != null ? jsonExplainInfo[LEFT_ONLY_FIELDS].ToObject<List<string>>() : null;
            RightOnlyFields = jsonExplainInfo[RIGHT_ONLY_FIELDS] != null ? jsonExplainInfo[RIGHT_ONLY_FIELDS].ToObject<List<string>>() : null;
        }

        private Dictionary<string, RecordSimilarityFieldExplainInfo> parseScoreFields(JObject scoreFieldsJSON) {
            Dictionary<string, RecordSimilarityFieldExplainInfo> scoreFields = new Dictionary<string, RecordSimilarityFieldExplainInfo>();
            foreach (var property in scoreFieldsJSON.Properties())
            {
                string key = property.Name;
                RecordSimilarityFieldExplainInfo fieldInfo = new RecordSimilarityFieldExplainInfo(property.Value as JObject);
                scoreFields.Add(key, fieldInfo);
            }
            return scoreFields;
        }


    }

    /// <summary>
    /// A class to represent the explaininfo of a field in a record similarity result
    public class RecordSimilarityFieldExplainInfo {
        private const string WEIGHT = "weight";
        private const string CALCULATED_WEIGHT = "calculatedWeight";
        private const string RAW_SCORE = "rawScore";
        private const string FINAL_SCORE = "finalScore";

        private const string DETAILS = "details";

        /// <summary>
        /// Gets or sets the weight of the field
        /// </summary>
        [JsonProperty(PropertyName = WEIGHT)]
        public double Weight { get; set; }

        /// <summary>
        /// Gets or sets the calculated weight of the field
        /// </summary>
        [JsonProperty(PropertyName = CALCULATED_WEIGHT)]
        public double CalculatedWeight { get; set; }

        /// <summary>
        /// Gets or sets the raw score of the field
        /// </summary>
        [JsonProperty(PropertyName = RAW_SCORE)]
        public double RawScore { get; set; }

        /// <summary>
        /// Gets or sets the final score of the field
        /// </summary>
        [JsonProperty(PropertyName = FINAL_SCORE)]
        public double FinalScore { get; set; }

        /// <summary>
        /// Gets or sets the details of the field
        /// </summary>
        [JsonProperty(PropertyName = DETAILS)]
        public JObject Details { get; set; }

        public RecordSimilarityFieldExplainInfo(double weight, double calculatedWeight, double rawScore, double finalScore, JObject details) {
            Weight = weight;
            CalculatedWeight = calculatedWeight;
            RawScore = rawScore;
            FinalScore = finalScore;
            Details = details;
        }

        public RecordSimilarityFieldExplainInfo(JObject jsonExplainInfo) {
            Weight = jsonExplainInfo[WEIGHT].ToObject<double>();
            CalculatedWeight = jsonExplainInfo[CALCULATED_WEIGHT].ToObject<double>();
            RawScore = jsonExplainInfo[RAW_SCORE].ToObject<double>();
            FinalScore = jsonExplainInfo[FINAL_SCORE].ToObject<double>();
            Details = jsonExplainInfo[DETAILS] as JObject;
        }

    }
}
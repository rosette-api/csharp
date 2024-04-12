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
        /// Creates a RecordSimilarityResponse from the API's raw output
        /// </summary>
        /// <param name="apiResults">The message from the API</param>
        public RecordSimilarityResponse(HttpResponseMessage apiResults) :base(apiResults)
        {
            this.Fields = this.ContentDictionary.ContainsKey(FIELDS) ? parseFields(this.ContentDictionary[FIELDS] as JObject) : null;
            this.Results = this.ContentDictionary.ContainsKey(RESULTS) ? parseResults(this.ContentDictionary[RESULTS] as JArray) : null;
            this.ErrorMessage = this.ContentDictionary.ContainsKey(ERROR_MESSAGE) ? this.ContentDictionary[ERROR_MESSAGE].ToString() : null;
        }

        /// <summary>
        /// Creates an RecordSimilarityResponse from its components
        /// </summary>
        /// <param name="fields">The record fields</param>
        /// <param name="results">The list of record similarity responses</param>
        /// <param name="errorMessage">The error message of the response</param>
        /// <param name="responseHeaders">The response headers returned from the API</param>
        /// <param name="content">The content of the response in dictionary form</param>
        /// <param name="contentAsJson">The content of the response in JSON form</param>
        public RecordSimilarityResponse(
            Dictionary<string, RecordSimilarityFieldInfo> fields,
            List<RecordSimilarityResult> results,
            string errorMessage,
            Dictionary<string, string> responseHeaders, 
            Dictionary<string, object> content, 
            string contentAsJson
            ) : base(responseHeaders, content, contentAsJson)
        {
            Fields = fields;
            Results = results;
            ErrorMessage = errorMessage;
        }
        

        /// <summary>
        /// Parses the fields JObject into RecordSimilarityFieldInfo objects
        /// </summary>
        /// <param name="fieldsJSON">fields JObject from the response</param>
        /// <returns>RecordSimilarityFieldInfo objects</returns>
        private Dictionary<string, RecordSimilarityFieldInfo> parseFields(JObject fieldsJSON)
        {
            Dictionary<string, RecordSimilarityFieldInfo> fields = new Dictionary<string, RecordSimilarityFieldInfo>();
            // iterate over the keys in the fields object
            foreach (var property in fieldsJSON.Properties())
            {
                string key = property.Name;
                JObject field = fieldsJSON[key] as JObject;
                RecordFieldType type = (RecordFieldType)Enum.Parse(typeof(RecordFieldType), field["type"].Value<string>());
                double? weight = field["weight"].ToObject<double?>();
                RecordSimilarityFieldInfo fieldInfo = new RecordSimilarityFieldInfo(type, weight); 
                fields.Add(key, fieldInfo);
            }
            return fields;
        }

        /// <summary>
        /// Parses the results JArray into RecordSimilarityResult objects
        /// </summary>
        /// <param name="resultsJSON">Results JArray from the response</param>
        ///  <returns>RecordSimilarityResult objects</returns>
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
                    this.Fields != null && other.Fields != null ? Utilities.DictionaryEquals(this.Fields, other.Fields) : this.Fields == other.Fields,
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
        /// Gets or sets the left records
        /// </summary>
        [JsonProperty(PropertyName = LEFT)]
        public Dictionary<string, RecordSimilarityField> Left { get; set; }

        /// <summary>
        /// Gets or sets the right records
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
        /// <param name="score">The score of the record similarity</param>
        /// <param name="left">The left records</param>
        /// <param name="right">The right records</param>
        /// <param name="explainInfo">The explain info</param>
        /// <param name="error">The error message</param>
        public RecordSimilarityResult(double score, Dictionary<string, RecordSimilarityField> left, Dictionary<string, RecordSimilarityField> right, RecordSimilarityExplainInfo explainInfo, string error)
        {
            Score = score;
            Left = left;
            Right = right;
            ExplainInfo = explainInfo;
            Error = error;
        }

        /// <summary>
        /// Creates a RecordSimilarityResult from the given JObject and the fields of the response
        /// </summary>
        /// <param name="jsonResult">The raw Json Object of a result</param>
        /// <param name="fields">The fields of the response</param>
        public RecordSimilarityResult(JObject jsonResult, Dictionary<string, RecordSimilarityFieldInfo> fields) {
            Score = jsonResult[SCORE].ToObject<double>();
            Left = parseRecord(jsonResult[LEFT] as JObject, fields);
            Right = parseRecord(jsonResult[RIGHT] as JObject, fields);
            ExplainInfo = jsonResult[EXPLAIN_INFO] != null && !jsonResult[EXPLAIN_INFO].Type.Equals(JTokenType.Null) ? new RecordSimilarityExplainInfo(jsonResult[EXPLAIN_INFO] as JObject) : null;
            Error = jsonResult[ERROR] != null && !jsonResult[ERROR].Type.Equals(JTokenType.Null) ? jsonResult[ERROR].Value<string>() : null;
        }

        /// <summary>
        /// Parses a record JObject into a the correct record class
        /// </summary>
        /// <param name="record">The record JObject</param>
        /// <param name="fields">The fields of the response</param>
        /// <returns>RecordSimilarityField object</returns>
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

        /// <summary>
        /// Parses a name record JToken into a fielded or unfieled name record
        /// </summary>
        /// <param name="nameRecord">The name record JToken</param>
        /// <returns>NameField object</returns>
        private RecordSimilarityField parseNameRecord(JToken nameRecord) {
            // check if nameRecord.Value is an object or a string
            if (nameRecord.Type == JTokenType.String)
            {
                return new UnfieldedNameRecord(nameRecord.Value<string>());
            } 
            else
            {
                JObject nameRecordObj = nameRecord as JObject;
                string text = nameRecordObj[NameField.TEXT].Value<string>();
                string language = nameRecordObj[FieldedNameRecord.LANGUAGE] != null ? nameRecordObj[FieldedNameRecord.LANGUAGE].Value<string>() : null;
                string languageOfOrigin = nameRecordObj[FieldedNameRecord.LANGUAGE_OF_ORIGIN] != null ? nameRecordObj[FieldedNameRecord.LANGUAGE_OF_ORIGIN].Value<string>() : null;
                string script = nameRecordObj[FieldedNameRecord.SCRIPT] != null ? nameRecordObj[FieldedNameRecord.SCRIPT].Value<string>() : null;
                string entityType = nameRecordObj[FieldedNameRecord.ENTITY_TYPE] != null ? nameRecordObj[FieldedNameRecord.ENTITY_TYPE].Value<string>() : null;
                return new FieldedNameRecord(text, language, languageOfOrigin, script, entityType);
            }
        }

        /// <summary>
        /// Parses an address record JToken into a fielded or unfieled address record
        /// </summary>
        /// <param name="addressRecord">The address record JToken</param>
        /// <returns>AddressField object</returns>
        private RecordSimilarityField parseAddressRecord(JToken addressRecord) {
            // check if addressRecord.Value is an object or a string
            if (addressRecord.Type == JTokenType.String)
            {
                return new UnfieldedAddressRecord(addressRecord.Value<string>());
            }
            else
            {
                JObject addressRecordObj = addressRecord as JObject;
                string address = addressRecordObj[AddressField.ADDRESS].Value<string>();
                return new FieldedAddressRecord(address);
            }
        }

        /// <summary>
        /// Parses a date record JToken into a fielded or unfieled date record
        /// </summary>
        /// <param name="dateRecord">The date record JToken</param>
        /// <returns>DateField object</returns>
        private RecordSimilarityField parseDateRecord(JToken dateRecord) {
            // check if dateRecord.Value is an object or a string
            if (dateRecord.Type == JTokenType.String)
            {
                return new UnfieldedDateRecord(dateRecord.Value<string>());
            }
            else
            {
                JObject dateRecordObj = dateRecord as JObject;
                string date = dateRecordObj[DateField.DATE].Value<string>();
                return new FieldedDateRecord(date);
            }
        }

        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns>True if equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is RecordSimilarityResult)
            {
                RecordSimilarityResult other = obj as RecordSimilarityResult;
                List<bool> conditions = new List<bool>() {
                    this.Score == other.Score,
                    this.Left != null && other.Left != null ? Utilities.DictionaryEquals(this.Left, other.Left) : this.Left == other.Left,
                    this.Right != null && other.Right != null ? Utilities.DictionaryEquals(this.Right, other.Right) : this.Right == other.Right,
                    this.ExplainInfo != null && other.ExplainInfo != null ? this.ExplainInfo.Equals(other.ExplainInfo) : this.ExplainInfo == other.ExplainInfo,
                    this.Error == other.Error
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
            int h0 = this.Score.GetHashCode();
            int h1 = this.Left != null ? this.Left.Aggregate<KeyValuePair<string, RecordSimilarityField>, int>(1, (seed, item) => seed ^ item.GetHashCode()) : 1;
            int h2 = this.Right != null ? this.Right.Aggregate<KeyValuePair<string, RecordSimilarityField>, int>(1, (seed, item) => seed ^ item.GetHashCode()) : 1;
            int h3 = this.ExplainInfo != null ? this.ExplainInfo.GetHashCode() : 1;
            int h4 = this.Error != null ? this.Error.GetHashCode() : 1;
            return h0 ^ h1 ^ h2 ^ h3 ^ h4;
        }
    }


    /// <summary>
    /// A class to represent the explainInfo of a record similarity result
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

        /// <summary>
        /// Creates a RecordSimilarityExplainInfo from the given score fields, left only fields, and right only fields
        /// </summary>
        /// <param name="scoreFields">The score fields</param>
        /// <param name="leftOnlyFields">The left only fields</param>
        /// <param name="rightOnlyFields">The right only fields</param>

        public RecordSimilarityExplainInfo(Dictionary<string, RecordSimilarityFieldExplainInfo> scoreFields, List<string> leftOnlyFields, List<string> rightOnlyFields) {
            ScoredFields = scoreFields;
            LeftOnlyFields = leftOnlyFields;
            RightOnlyFields = rightOnlyFields;
        }

        /// <summary>
        /// Creates a RecordSimilarityExplainInfo from the response's explainInfo JObject
        /// </summary>
        /// <param name="jsonExplainInfo">The explainInfo JObject</param>
        public RecordSimilarityExplainInfo(JObject jsonExplainInfo) {
            ScoredFields = jsonExplainInfo[SCORED_FIELDS] != null && jsonExplainInfo[SCORED_FIELDS].Type != JTokenType.Null ? parseScoredFields(jsonExplainInfo[SCORED_FIELDS] as JObject): null;
            LeftOnlyFields = jsonExplainInfo[LEFT_ONLY_FIELDS] != null  && jsonExplainInfo[LEFT_ONLY_FIELDS].Type != JTokenType.Null ? jsonExplainInfo[LEFT_ONLY_FIELDS].ToObject<List<string>>() : null;
            RightOnlyFields = jsonExplainInfo[RIGHT_ONLY_FIELDS] != null && jsonExplainInfo[RIGHT_ONLY_FIELDS].Type != JTokenType.Null ? jsonExplainInfo[RIGHT_ONLY_FIELDS].ToObject<List<string>>() : null;  
        }

        /// <summary>
        /// Parses the scoredFields JObject into RecordSimilarityFieldExplainInfo objects
        /// </summary>
        /// <param name="scoredFieldsJSON">scoredFields JObject from the response</param>
        /// <returns>RecordSimilarityFieldExplainInfo objects</returns>
        private Dictionary<string, RecordSimilarityFieldExplainInfo> parseScoredFields(JObject scoredFieldsJSON) {
            Dictionary<string, RecordSimilarityFieldExplainInfo> scoredFields = new Dictionary<string, RecordSimilarityFieldExplainInfo>();
            foreach (var property in scoredFieldsJSON.Properties())
            {
                string key = property.Name;
                RecordSimilarityFieldExplainInfo fieldInfo = new RecordSimilarityFieldExplainInfo(property.Value as JObject);
                scoredFields.Add(key, fieldInfo);
            }
            return scoredFields;
        }

        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns>True if equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is RecordSimilarityExplainInfo)
            {
                RecordSimilarityExplainInfo other = obj as RecordSimilarityExplainInfo;
                List<bool> conditions = new List<bool>() {
                    this.ScoredFields != null && other.ScoredFields != null ? Utilities.DictionaryEquals(this.ScoredFields, other.ScoredFields) : this.ScoredFields == other.ScoredFields,
                    this.LeftOnlyFields != null && other.LeftOnlyFields != null ? this.LeftOnlyFields.SequenceEqual(other.LeftOnlyFields) : this.LeftOnlyFields == other.LeftOnlyFields,
                    this.RightOnlyFields != null && other.RightOnlyFields != null ? this.RightOnlyFields.SequenceEqual(other.RightOnlyFields) : this.RightOnlyFields == other.RightOnlyFields
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
            int h0 = this.ScoredFields != null ? this.ScoredFields.Aggregate<KeyValuePair<string, RecordSimilarityFieldExplainInfo>, int>(1, (seed, item) => seed ^ item.GetHashCode()) : 1;
            int h1 = this.LeftOnlyFields != null ? this.LeftOnlyFields.Aggregate<string, int>(1, (seed, item) => seed ^ item.GetHashCode()) : 1;
            int h2 = this.RightOnlyFields != null ? this.RightOnlyFields.Aggregate<string, int>(1, (seed, item) => seed ^ item.GetHashCode()) : 1;
            return h0 ^ h1 ^ h2;
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

        /// <summary>
        /// Creates a RecordSimilarityFieldExplainInfo from the given weight, calculatedWeight, rawScore, finalScore, and details
        /// </summary>
        /// <param name="weight">The weight of the field</param>
        /// <param name="calculatedWeight">The calculated weight of the field</param>
        /// <param name="rawScore">The raw score of the field</param>
        /// <param name="finalScore">The final score of the field</param>
        /// <param name="details">The details of the field</param>
        public RecordSimilarityFieldExplainInfo(double weight, double calculatedWeight, double rawScore, double finalScore, JObject details) {
            Weight = weight;
            CalculatedWeight = calculatedWeight;
            RawScore = rawScore;
            FinalScore = finalScore;
            Details = details;
        }

        /// <summary>
        /// Creates a RecordSimilarityFieldExplainInfo from the response's field explainInfo JObject
        /// </summary>
        /// <param name="jsonExplainInfo">The field explainInfo JObject</param>
        public RecordSimilarityFieldExplainInfo(JObject jsonExplainInfo) {
            Weight = jsonExplainInfo[WEIGHT].ToObject<double>();
            CalculatedWeight = jsonExplainInfo[CALCULATED_WEIGHT].ToObject<double>();
            RawScore = jsonExplainInfo[RAW_SCORE].ToObject<double>();
            FinalScore = jsonExplainInfo[FINAL_SCORE].ToObject<double>();
            Details = jsonExplainInfo[DETAILS] as JObject;
        }

        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns>True if equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is RecordSimilarityFieldExplainInfo)
            {
                RecordSimilarityFieldExplainInfo other = obj as RecordSimilarityFieldExplainInfo;
                List<bool> conditions = new List<bool>() {
                    this.Weight == other.Weight,
                    this.CalculatedWeight == other.CalculatedWeight,
                    this.RawScore == other.RawScore,
                    this.FinalScore == other.FinalScore,
                    this.Details != null && other.Details != null ? JToken.DeepEquals(this.Details, other.Details) : this.Details == other.Details
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
            int h0 = this.Weight.GetHashCode();
            int h1 = this.CalculatedWeight.GetHashCode();
            int h2 = this.RawScore.GetHashCode();
            int h3 = this.FinalScore.GetHashCode();
            int h4 = this.Details != null ? this.Details.GetHashCode() : 1;
            return h0 ^ h1 ^ h2 ^ h3 ^ h4;
        }

    }
}
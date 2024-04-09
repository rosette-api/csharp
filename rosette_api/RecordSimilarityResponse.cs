using System.Net.Http;
using Newtonsoft.Json;


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
        [JsonProperty(PropertyName = RIGHT_RECORD)]
        public Dictionary<string, RecordSimilarityField> Right { get; set; }

        /// <summary>
        /// Gets or sets the explain info
        /// </summary>
        [JsonProperty(PropertyName = EXPLANATION)]
        public RecordSimilarityExplainInfo ExplainInfo { get; set; }

        /// <summary>
        /// Gets or sets the error message
        /// </summary>
        [JsonProperty(PropertyName = ERROR)]
        public string Error { get; set; }

        /// <summary>
        /// Creates a RecordSimilarityResult from the given score, left, right, explainInfo, and error
        /// </summary>
        public RecordSimilarityResult(double score, Dictionary<string, RecordSimilarityField> left, Dictionary<string, RecordSimilarityField> right, RecordSimilarityExplanation explainInfo, string error)
        {
            Score = score;
            Left = left;
            Right = right;
            ExplainInfo = explainInfo;
            Error = error;
        }
    }


    /// <summary>
    /// A class to represent the explaininfo of a record similarity result
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class RecordSimilarityExplainInfo {

        private const string SCORE_FIELDS = "scoreFields";
        private const string LEFT_ONLY_FIELDS = "leftOnlyFields";
        private const string RIGHT_ONLY_FIELDS = "rightOnlyFields";

        /// <summary>
        /// Gets or sets the score fields
        /// </summary>
        [JsonProperty(PropertyName = SCORE_FIELDS)]
        public Dictionary<string, RecordSimilarityFieldExplainInfo> ScoreFields { get; set; }

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
        public JsonObject Details { get; set; }
    }
}
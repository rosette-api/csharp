using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;


namespace rosette_api
{
    
    /// <summary>
    /// Class for representing record similarity request properties
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class RecordSimilarityProperties
    {
        private const string THRESHOLD = "threshold";
        private const string INCLUDE_EXPLAIN_INFO = "includeExplainInfo";

        /// <summary>
        /// Gets or sets the the record similarity request's score treshold
        /// </summary>
        [JsonProperty(PropertyName = THRESHOLD)]
        public double Threshold { get; set; } = 0.0;


        /// <summary>
        /// Gets or sets the the record similarity request's include explain info parameter
        /// </summary>
        [JsonProperty(PropertyName = INCLUDE_EXPLAIN_INFO)]
        public bool IncludeExplainInfo { get; set; }

        /// <summary>
        /// No-args constructor
        /// </summary>
        public RecordSimilarityProperties() { }

        /// <summary>
        /// Includes explain info constructor. Sets the treshold to 0.0.
        /// </summary>
        /// <param name="includeExplainInfo">The include explain info parameter</param>
        public RecordSimilarityProperties(bool includeExplainInfo) {
            this.IncludeExplainInfo = includeExplainInfo;
            this.Threshold = 0.0;
        }

        /// <summary>
        /// Full constructor
        /// </summary>
        /// <param name="threshold">The score threshold</param>
        /// <param name="includeExplainInfo">The include explain info parameter</param>
        public RecordSimilarityProperties(double threshold, bool includeExplainInfo)
        {
            this.Threshold = threshold;
            this.IncludeExplainInfo = includeExplainInfo;
        }

        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns>True if equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is RecordSimilarityProperties)
            {
                RecordSimilarityProperties other = obj as RecordSimilarityProperties;
                List<bool> conditions = new List<bool>() {
                    this.Threshold == other.Threshold,
                    this.IncludeExplainInfo == other.IncludeExplainInfo
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
            int h0 = this.Threshold.GetHashCode();
            int h1 = this.IncludeExplainInfo.GetHashCode();
            return h0 ^ h1;
        }

        /// <summary>
        /// ToString override.
        /// </summary>
        /// <returns>This record similarity properties in JSON form</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
        
    }
    /// <summary>
    /// Class for representing record similarity request records lists
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class RecordSimilarityRecords
    {
        private const string LEFT = "left";
        private const string RIGHT = "right";  
        /// <summary>
        /// Gets or sets the the record similarity request's left records
        /// </summary>
        [JsonProperty(PropertyName = LEFT)]
        public List<Dictionary<string, RecordSimilarityField>> Left { get; set; }

        /// <summary>
        /// Gets or sets the the record similarity request's right records
        /// </summary>
        [JsonProperty(PropertyName = RIGHT)]
        public List<Dictionary<string, RecordSimilarityField>> Right { get; set; }

        /// <summary>
        /// No-args constructor
        /// </summary>
        public RecordSimilarityRecords() { }

        /// <summary>
        /// Full constructor
        /// </summary>
        /// <param name="left">The left records</param>
        /// <param name="right">The right records</param>
        public RecordSimilarityRecords(List<Dictionary<string, RecordSimilarityField>> left, List<Dictionary<string, RecordSimilarityField>> right)
        {
            this.Left = left;
            this.Right = right;
        }

        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns>True if equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is RecordSimilarityRecords)
            {
                RecordSimilarityRecords other = obj as RecordSimilarityRecords;
                List<bool> conditions = new List<bool>() {
                    this.Left != null && other.Left != null ? this.Left.SequenceEqual(other.Left) : this.Left == other.Left,
                    this.Right != null && other.Right != null ? this.Right.SequenceEqual(other.Right) : this.Right == other.Right
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
            int h0 = this.Left != null ? this.Left.GetHashCode() : 1;
            int h1 = this.Right != null ? this.Right.GetHashCode() : 1;
            return h0 ^ h1;
        }

        /// <summary>
        /// ToString override.
        /// </summary>
        /// <returns>This record similarity records in JSON form</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
    

    /// <summary>
    /// Request object to use with calls to the Record Similarity endpoint
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class RecordSimilarityRequest
    {
        private const string RECORDS = "records";
        private const string FIELDS = "fields";
        private const string PROPERTIES = "properties";

        /// <summary>
        /// Gets or sets the the record similarity request's fields
        /// </summary>
        [JsonProperty(PropertyName = FIELDS)]
        public Dictionary<string, RecordSimilarityFieldInfo> Fields { get; set; }

        /// <summary>
        /// Gets or sets the the record similarity request's properties
        /// </summary>
        [JsonProperty(PropertyName = PROPERTIES)]
        public RecordSimilarityProperties Properties { get; set; }

        /// <summary>
        /// Gets or sets the the record similarity request's records
        /// </summary>
        [JsonProperty(PropertyName = RECORDS)]
        public RecordSimilarityRecords Records { get; set; }

        /// <summary>
        /// No-args constructor
        /// </summary>
        public RecordSimilarityRequest() { }

        /// <summary>
        /// Full constructor
        /// </summary>
        /// <param name="fields">The fields to use in the request</param>
        /// <param name="properties">The properties to use in the request</param>
        /// <param name="records">The records to use in the request</param>
        public RecordSimilarityRequest(Dictionary<string, RecordSimilarityFieldInfo> fields, RecordSimilarityProperties properties, RecordSimilarityRecords records)
        {
            this.Fields = fields;
            this.Properties = properties;
            this.Records = records;
        }

        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns>True if equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is RecordSimilarityRequest)
            {
                RecordSimilarityRequest other = obj as RecordSimilarityRequest;
                List<bool> conditions = new List<bool>() {
                    this.Fields != null && other.Fields != null ? Utilities.DictionaryEquals(this.Fields, other.Fields) : this.Fields == other.Fields,
                    this.Properties != null && other.Properties != null ? this.Properties.Equals(other.Properties) : this.Properties == other.Properties,
                    this.Records != null && other.Records != null ? this.Records.Equals(other.Records) : this.Records == other.Records
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
            int h1 = this.Properties != null ? this.Properties.GetHashCode() : 1;
            int h2 = this.Records != null ? this.Records.GetHashCode() : 1;
            return h0 ^ h1 ^ h2;
        }

        /// <summary>
        /// ToString override.
        /// </summary>
        /// <returns>This record similarity request in JSON form</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
        
    }
}

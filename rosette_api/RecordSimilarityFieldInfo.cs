using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Linq;

namespace rosette_api {
    /// <summary>RecordFieldType enum
    /// <para>
    /// The possible record types that can be used in the Record Similarity endpoint
    /// </para>
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum RecordFieldType
    {
        
        rni_name, rni_date, rni_address
    }

    /// <summary>
    /// Class for representing record similarity field information
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class RecordSimilarityFieldInfo
    {
        private const string TYPE = "type";
        private const string WEIGHT = "weight";

        /// <summary>
        /// Gets or sets the record's field type
        /// </summary>
        [JsonProperty(PropertyName = TYPE)]
        public RecordFieldType Type { get; set; }
        
        /// <summary>
        /// Gets or sets the record's field weight
        /// </summary>
        [JsonProperty(PropertyName = WEIGHT)]
        public double? Weight { get; set; }


        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns>True if equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is RecordSimilarityFieldInfo)
            {
                RecordSimilarityFieldInfo other = obj as RecordSimilarityFieldInfo;
                List<bool> conditions = new List<bool>() {
                    this.Type == other.Type,
                    this.Weight == other.Weight
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
            int h0 = this.Type != null ? this.Type.GetHashCode() : 1;
            int h1 = this.Weight != null ? this.Weight.GetHashCode() : 1;
            return h0 ^ h1;
        }

        /// <summary>
        /// ToString override.
        /// </summary>
        /// <returns>This record similarity field info in JSON form</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
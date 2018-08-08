using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace rosette_api
{
    /// <summary>
    /// A class for representing an identified entity and its properties
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class RosetteEntity
    {
        /// <summary>
        /// The Location entity type
        /// </summary>
        public const string LOCATION = "LOCATION";
        /// <summary>
        /// The organization entity type
        /// </summary>
        public const string ORGANIZATION = "ORGANIZATION";
        /// <summary>
        /// The person entity type
        /// </summary>
        public const string PERSON = "PERSON";
        /// <summary>
        /// The product entity type
        /// </summary>
        public const string PRODUCT = "PRODUCT";
        /// <summary>
        /// The title entity type
        /// </summary>
        public const string TITLE = "TITLE";
        /// <summary>
        /// The nationality entity type
        /// </summary>
        public const string NATIONALITY = "NATIONALITY";
        /// <summary>
        /// The religion entity type
        /// </summary>
        public const string RELIGION = "RELIGION";
        /// <summary>
        /// The credit card number entity type
        /// </summary>
        public const string CREDIT_CARD_NUM = "IDENTIFIER:CREDIT_CARD_NUM";
        /// <summary>
        /// The email address entity type
        /// </summary>
        public const string EMAIL = "IDENTIFIER:EMAIL";
        /// <summary>
        /// The money entity type
        /// </summary>
        public const string MONEY = "IDENTIFIER:MONEY";
        /// <summary>
        /// The personal ID number entity type
        /// </summary>
        public const string PERSONAL_ID_NUM = "IDENTIFIER:PERSONAL_ID_NUM";
        /// <summary>
        /// The phone number entity type
        /// </summary>
        public const string PHONE_NUMBER = "IDENTIFIER:PHONE_NUMBER";
        /// <summary>
        /// The URL entity type
        /// </summary>
        public const string URL = "IDENTIFIER:URL";
        /// <summary>
        /// The date entity type
        /// </summary>
        public const string DATE = "TEMPORAL:DATE";
        /// <summary>
        /// The time entity type
        /// </summary>
        public const string TIME = "TEMPORAL:TIME";
        /// <summary>
        /// The distance entity type
        /// </summary>
        public const string DISTANCE = "IDENTIFIER:DISTANCE";
        /// <summary>
        /// The latitude-longitude entity type
        /// </summary>
        public const string LATITUDE_LONGITUDE = "IDENTIFIER:LATITUDE_LONGITUDE";

        /// <summary>
        /// Gets or sets the entity's name
        /// </summary>
        [JsonProperty("mention")]
        public String Mention { get; set; }

        /// <summary>
        /// Gets or sets the entity's name, normalized
        /// </summary>
        [JsonProperty("normalized")]
        public String NormalizedMention { get; set; }

        /// <summary>
        /// Gets or sets the entity's ID
        /// </summary>
        [JsonIgnore]
        public EntityID ID { get; set; }
        [JsonProperty("entityId")]
        private string EntityID { get { return ID.ToString(); } }

        /// <summary>
        /// Gets or sets the entity's type
        /// </summary>
        [JsonProperty("type")]
        public String EntityType { get; set; }

        /// <summary>
        /// Gets or sets the number of times this entity appeared in the input to the API
        /// </summary>
        [JsonProperty("count")]
        public Nullable<int> Count { get; set; }

        /// <summary>
        /// Gets or sets the confidence of the extracted entity
        /// </summary>
        [JsonProperty("confidence")]
        public Nullable<double> Confidence { get; set; }
        
        /// <summary>
        /// Gets or sets the dbpediaType of the extracted entity
        /// </summary>
        [JsonProperty("dbpediaType", NullValueHandling = NullValueHandling.Ignore)]
        public String DBpediaType { get; set; }

        /// <summary>
        /// Creates an entity
        /// </summary>
        /// <param name="mention">The mention of the entity</param>
        /// <param name="normalizedMention">The normalzied form of the mention</param>
        /// <param name="id">The mention's id</param>
        /// <param name="entityType">The entity type</param>
        /// <param name="count">The number of times this entity appeared in the input to the API</param>
        /// <param name="confidence">The confidence of this entity appeared in the input to the API</param>
        /// <param name="dbpediaType">The DBpedia type of the entity</param>
        public RosetteEntity(string mention, string normalizedMention, EntityID id, string entityType, int? count,
            double? confidence, string dbpediaType)
        {
            this.Mention = mention;
            this.NormalizedMention = normalizedMention;
            this.ID = id;
            this.Count = count;
            this.EntityType = entityType;
            this.Confidence = confidence;
            this.DBpediaType = dbpediaType;
        }

        /// <summary>
        /// Equals for RosetteEntity
        /// </summary>
        /// <param name="other">RosetteEntity</param>
        /// <returns></returns>
        protected bool Equals(RosetteEntity other)
        {
            return string.Equals(Mention, other.Mention) && string.Equals(NormalizedMention, other.NormalizedMention) && Equals(ID, other.ID) && string.Equals(EntityType, other.EntityType) && Count == other.Count && Confidence.Equals(other.Confidence) && string.Equals(DBpediaType, other.DBpediaType);
        }

        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj">The object to compare against</param>
        /// <returns>True if equal</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RosetteEntity) obj);
        }

        /// <summary>
        /// HashCode override
        /// </summary>
        /// <returns>The hashcode</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Mention != null ? Mention.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (NormalizedMention != null ? NormalizedMention.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ID != null ? ID.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (EntityType != null ? EntityType.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Count.GetHashCode();
                hashCode = (hashCode * 397) ^ Confidence.GetHashCode();
                hashCode = (hashCode * 397) ^ (DBpediaType != null ? DBpediaType.GetHashCode() : 0);
                return hashCode;
            }
        }

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns>This RosetteEntity in JSON form</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}

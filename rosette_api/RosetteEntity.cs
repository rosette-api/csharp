using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rosette_api
{
    /// <summary>
    /// A class for representing an identified entity and its properties
    /// </summary>
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
        public String Mention { get; set; }

        /// <summary>
        /// Gets or sets the entity's name, normalized
        /// </summary>
        public String NormalizedMention { get; set; }

        /// <summary>
        /// Gets or sets the entity's ID
        /// </summary>
        public EntityID ID { get; set; }

        /// <summary>
        /// Gets or sets the entity's type
        /// </summary>
        public String EntityType { get; set; }

        /// <summary>
        /// Gets or sets the number of times this entity appeared in the input to the API
        /// </summary>
        public Nullable<int> Count { get; set; }

        /// <summary>
        /// Creates an entity
        /// </summary>
        /// <param name="mention">The mention of the entity</param>
        /// <param name="normalizedMention">The normalzied form of the mention</param>
        /// <param name="id">The mention's id</param>
        /// <param name="entityType">The entity type</param>
        /// <param name="count">The number of times this entity appeared in the input to the API</param>
        public RosetteEntity(String mention, String normalizedMention, EntityID id, String entityType, Nullable<int> count)
        {
            this.Mention = mention;
            this.NormalizedMention = normalizedMention;
            this.ID = id;
            this.Count = count;
            this.EntityType = entityType;
        }

        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj">The object to compare against</param>
        /// <returns>True if equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is RosetteEntity)
            {
                RosetteEntity other = obj as RosetteEntity;
                List<bool> conditions = new List<bool>() {
                    this.ID != null && other.ID != null ? this.ID.Equals(other.ID) : this.ID == other.ID,
                    this.Count == other.Count,
                    this.EntityType == other.EntityType,
                    this.Mention == other.Mention,
                    this.NormalizedMention == other.NormalizedMention,
                    this.GetHashCode() == other.GetHashCode()
                };
                return conditions.All(condition => condition);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// HashCode override
        /// </summary>
        /// <returns>The hashcode</returns>
        public override int GetHashCode()
        {
            int h0 = this.NormalizedMention != null ? this.NormalizedMention.GetHashCode() : 1;
            int h1 = this.Mention != null ? this.Mention.GetHashCode() : 1;
            int h2 = this.ID != null ? this.ID.GetHashCode() : 1;
            int h3 = this.Count != null ? this.Count.GetHashCode() : 1;
            int h4 = this.EntityType != null ? this.EntityType.GetHashCode() : 1;
            return h0 ^ h1 ^ h2 ^ h3 ^ h4;
        }

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns>This RosetteEntity in JSON form</returns>
        public override string ToString()
        {
            string idString = this.ID != null ? new StringBuilder("\"").Append(this.ID.ToString()).Append("\"").ToString() : null;
            string entityTypeString = this.EntityType != null ? String.Format("\"{0}\"", this.EntityType) : null;
            string mentionString = this.Mention != null ? String.Format("\"{0}\"", this.Mention) : null;
            string normalizedString = this.NormalizedMention != null ? String.Format("\"{0}\"", this.NormalizedMention) : null;
            StringBuilder builder = new StringBuilder("{");
            builder.AppendFormat("\"type\": {0}, ", entityTypeString)
                .AppendFormat("\"mention\": {0}, ", mentionString)
                .AppendFormat("\"normalized\": {0}, ", normalizedString)
                .AppendFormat("\"count\": {0}, ", this.Count)
                .AppendFormat("\"entityId\": {0}", idString).Append("}");
            return builder.ToString();
        }
    }
}

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
        /// A class for representing a location, as identified by the Rosette API
        /// </summary>
        public class RosetteLocation : RosetteEntity
        {
            /// <summary>
            /// Creates an entity with a count of how many times it appeared in the text in which it was found
            /// </summary>
            /// <param name="mention">The mention of the entity</param>
            /// <param name="normalizedMention">The normalization of the mention</param>
            /// <param name="id">The entity ID, to compare it against other entities</param>
            /// <param name="entityType">The entity type</param>
            /// <param name="count">The number of times this entity was identified in the text in which it was found</param>
            public RosetteLocation(String mention, String normalizedMention, EntityID id, String entityType, int? count) : base(mention, normalizedMention, id, entityType, count) { }
        }

        /// <summary>
        /// A class for representing distances, as identified by the Rosette API
        /// </summary>
        public class RosetteDistance : RosetteEntity
        {
            /// <summary>
            /// Creates an entity with a count of how many times it appeared in the text in which it was found
            /// </summary>
            /// <param name="mention">The mention of the entity</param>
            /// <param name="normalizedMention">The normalization of the mention</param>
            /// <param name="id">The entity ID, to compare it against other entities</param>
            /// <param name="entityType">The entity type</param>
            /// <param name="count">The number of times this entity was identified in the text in which it was found</param>
            public RosetteDistance(String mention, String normalizedMention, EntityID id, String entityType, int? count) : base(mention, normalizedMention, id, entityType, count) { }
        }

        /// <summary>
        /// A class for representing a latitude-longitude coordinate pair, as identified by the Rosette API
        /// </summary>
        public class RosetteLatLong : RosetteEntity
        {
            /// <summary>
            /// Creates an entity with a count of how many times it appeared in the text in which it was found
            /// </summary>
            /// <param name="mention">The mention of the entity</param>
            /// <param name="normalizedMention">The normalization of the mention</param>
            /// <param name="id">The entity ID, to compare it against other entities</param>
            /// <param name="entityType">The entity type</param>
            /// <param name="count">The number of times this entity was identified in the text in which it was found</param>
            public RosetteLatLong(String mention, String normalizedMention, EntityID id, String entityType, int? count) : base(mention, normalizedMention, id, entityType, count) { }
        }

        /// <summary>
        /// A class to represent a person, as identified by the Rosette API
        /// </summary>
        public class RosettePerson : RosetteEntity
        {
            /// <summary>
            /// Creates an entity with a count of how many times it appeared in the text in which it was found
            /// </summary>
            /// <param name="mention">The mention of the entity</param>
            /// <param name="normalizedMention">The normalization of the mention</param>
            /// <param name="id">The entity ID, to compare it against other entities</param>
            /// <param name="entityType">The entity type</param>
            /// <param name="count">The number of times this entity was identified in the text in which it was found</param>
            public RosettePerson(String mention, String normalizedMention, EntityID id, String entityType, int? count) : base(mention, normalizedMention, id, entityType, count) { }
        }

        /// <summary>
        /// A class representing an organization, as identified by the Rosette API
        /// </summary>
        public class RosetteOrganization : RosetteEntity
        {
            /// <summary>
            /// Creates an entity with a count of how many times it appeared in the text in which it was found
            /// </summary>
            /// <param name="mention">The mention of the entity</param>
            /// <param name="normalizedMention">The normalization of the mention</param>
            /// <param name="id">The entity ID, to compare it against other entities</param>
            /// <param name="entityType">The entity type</param>
            /// <param name="count">The number of times this entity was identified in the text in which it was found</param>
            public RosetteOrganization(String mention, String normalizedMention, EntityID id, String entityType, int? count) : base(mention, normalizedMention, id, entityType, count) { }
        }

        /// <summary>
        /// A class representing a title (e.g. Mr, Ms.), as identified by the Rosette API
        /// </summary>
        public class RosetteTitle : RosetteEntity
        {
            /// <summary>
            /// Creates an entity with a count of how many times it appeared in the text in which it was found
            /// </summary>
            /// <param name="mention">The mention of the entity</param>
            /// <param name="normalizedMention">The normalization of the mention</param>
            /// <param name="id">The entity ID, to compare it against other entities</param>
            /// <param name="entityType">The entity type</param>
            /// <param name="count">The number of times this entity was identified in the text in which it was found</param>
            public RosetteTitle(String mention, String normalizedMention, EntityID id, String entityType, int? count) : base(mention, normalizedMention, id, entityType, count) { }
        }

        /// <summary>
        /// A class for representing credit card numbers, as identified by the Rosette API
        /// </summary>
        public class RosetteCreditCardNum : RosetteEntity
        {
            /// <summary>
            /// Creates an entity with a count of how many times it appeared in the text in which it was found
            /// </summary>
            /// <param name="mention">The mention of the entity</param>
            /// <param name="normalizedMention">The normalization of the mention</param>
            /// <param name="id">The entity ID, to compare it against other entities</param>
            /// <param name="entityType">The entity type</param>
            /// <param name="count">The number of times this entity was identified in the text in which it was found</param>
            public RosetteCreditCardNum(String mention, String normalizedMention, EntityID id, String entityType, int? count) : base(mention, normalizedMention, id, entityType, count) { }
        }

        /// <summary>
        /// A class to represent products, as identified by the Rosette API
        /// </summary>
        public class RosetteProduct : RosetteEntity
        {
            /// <summary>
            /// Creates an entity with a count of how many times it appeared in the text in which it was found
            /// </summary>
            /// <param name="mention">The mention of the entity</param>
            /// <param name="normalizedMention">The normalization of the mention</param>
            /// <param name="id">The entity ID, to compare it against other entities</param>
            /// <param name="entityType">The entity type</param>
            /// <param name="count">The number of times this entity was identified in the text in which it was found</param>
            public RosetteProduct(String mention, String normalizedMention, EntityID id, String entityType, int? count) : base(mention, normalizedMention, id, entityType, count) { }
        }

        /// <summary>
        /// A class representing nationalities, as identified by the Rosette API
        /// </summary>
        public class RosetteNationality : RosetteEntity
        {
            /// <summary>
            /// Creates an entity with a count of how many times it appeared in the text in which it was found
            /// </summary>
            /// <param name="mention">The mention of the entity</param>
            /// <param name="normalizedMention">The normalization of the mention</param>
            /// <param name="id">The entity ID, to compare it against other entities</param>
            /// <param name="entityType">The entity type</param>
            /// <param name="count">The number of times this entity was identified in the text in which it was found</param>
            public RosetteNationality(String mention, String normalizedMention, EntityID id, String entityType, int? count) : base(mention, normalizedMention, id, entityType, count) { }
        }

        /// <summary>
        /// A class for representing religions, as identified by the Rosette API
        /// </summary>
        public class RosetteReligion : RosetteEntity
        {
            /// <summary>
            /// Creates an entity with a count of how many times it appeared in the text in which it was found
            /// </summary>
            /// <param name="mention">The mention of the entity</param>
            /// <param name="normalizedMention">The normalization of the mention</param>
            /// <param name="id">The entity ID, to compare it against other entities</param>
            /// <param name="entityType">The entity type</param>
            /// <param name="count">The number of times this entity was identified in the text in which it was found</param>
            public RosetteReligion(String mention, String normalizedMention, EntityID id, String entityType, int? count) : base(mention, normalizedMention, id, entityType, count) { }
        }

        /// <summary>
        /// A class for representing email addresses, as identified by the Rosette API
        /// </summary>
        public class RosetteEmailAddress : RosetteEntity
        {
            /// <summary>
            /// Creates an entity with a count of how many times it appeared in the text in which it was found
            /// </summary>
            /// <param name="mention">The mention of the entity</param>
            /// <param name="normalizedMention">The normalization of the mention</param>
            /// <param name="id">The entity ID, to compare it against other entities</param>
            /// <param name="entityType">The entity type</param>
            /// <param name="count">The number of times this entity was identified in the text in which it was found</param>
            public RosetteEmailAddress(String mention, String normalizedMention, EntityID id, String entityType, int? count) : base(mention, normalizedMention, id, entityType, count) { }
        }

        /// <summary>
        /// A class for representing money, as identified by the RosetteAPI
        /// </summary>
        public class RosetteMoney : RosetteEntity
        {
            /// <summary>
            /// Creates an entity with a count of how many times it appeared in the text in which it was found
            /// </summary>
            /// <param name="mention">The mention of the entity</param>
            /// <param name="normalizedMention">The normalization of the mention</param>
            /// <param name="id">The entity ID, to compare it against other entities</param>
            /// <param name="entityType">The entity type</param>
            /// <param name="count">The number of times this entity was identified in the text in which it was found</param>
            public RosetteMoney(String mention, String normalizedMention, EntityID id, String entityType, int? count) : base(mention, normalizedMention, id, entityType, count) { }
        }

        /// <summary>
        /// A class for representing personal ID numbers, as identified by the Rosette API
        /// </summary>
        public class RosettePersonalIDNumber : RosetteEntity
        {
            /// <summary>
            /// Creates an entity with a count of how many times it appeared in the text in which it was found
            /// </summary>
            /// <param name="mention">The mention of the entity</param>
            /// <param name="normalizedMention">The normalization of the mention</param>
            /// <param name="id">The entity ID, to compare it against other entities</param>
            /// <param name="entityType">The entity type</param>
            /// <param name="count">The number of times this entity was identified in the text in which it was found</param>
            public RosettePersonalIDNumber(String mention, String normalizedMention, EntityID id, String entityType, int? count) : base(mention, normalizedMention, id, entityType, count) { }
        }

        /// <summary>
        /// A class for representing phone numbers, as identified by the Rosette API
        /// </summary>
        public class RosettePhoneNumber : RosetteEntity
        {
            /// <summary>
            /// Creates an entity with a count of how many times it appeared in the text in which it was found
            /// </summary>
            /// <param name="mention">The mention of the entity</param>
            /// <param name="normalizedMention">The normalization of the mention</param>
            /// <param name="id">The entity ID, to compare it against other entities</param>
            /// <param name="entityType">The entity type</param>
            /// <param name="count">The number of times this entity was identified in the text in which it was found</param>
            public RosettePhoneNumber(String mention, String normalizedMention, EntityID id, String entityType, int? count) : base(mention, normalizedMention, id, entityType, count) { }
        }

        /// <summary>
        /// A class for representing URLs, as identified by the Rosette API
        /// </summary>
        public class RosetteURL : RosetteEntity
        {
            /// <summary>
            /// Creates an entity with a count of how many times it appeared in the text in which it was found
            /// </summary>
            /// <param name="mention">The mention of the entity</param>
            /// <param name="normalizedMention">The normalization of the mention</param>
            /// <param name="id">The entity ID, to compare it against other entities</param>
            /// <param name="entityType">The entity type</param>
            /// <param name="count">The number of times this entity was identified in the text in which it was found</param>
            public RosetteURL(String mention, String normalizedMention, EntityID id, String entityType, int? count) : base(mention, normalizedMention, id, entityType, count) { }
        }

        /// <summary>
        /// A class for representing Dates, as identified by the Rosette API
        /// </summary>
        public class RosetteDate : RosetteEntity
        {
            /// <summary>
            /// Creates an entity with a count of how many times it appeared in the text in which it was found
            /// </summary>
            /// <param name="mention">The mention of the entity</param>
            /// <param name="normalizedMention">The normalization of the mention</param>
            /// <param name="id">The entity ID, to compare it against other entities</param>
            /// <param name="entityType">The entity type</param>
            /// <param name="count">The number of times this entity was identified in the text in which it was found</param>
            public RosetteDate(String mention, String normalizedMention, EntityID id, String entityType, int? count) : base(mention, normalizedMention, id, entityType, count) { }
        }

        /// <summary>
        /// A class for representing Times, as identified by the Rosette API
        /// </summary>
        public class RosetteTime : RosetteEntity
        {
            /// <summary>
            /// Creates an entity with a count of how many times it appeared in the text in which it was found
            /// </summary>
            /// <param name="mention">The mention of the entity</param>
            /// <param name="normalizedMention">The normalization of the mention</param>
            /// <param name="id">The entity ID, to compare it against other entities</param>
            /// <param name="entityType">The entity type</param>
            /// <param name="count">The number of times this entity was identified in the text in which it was found</param>
            public RosetteTime(String mention, String normalizedMention, EntityID id, String entityType, int? count) : base(mention, normalizedMention, id, entityType, count) { }
        }
    }
}

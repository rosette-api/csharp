using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Net.Http;

namespace rosette_api
{
    /// <summary>
    /// A class for representing responses from the API when the Entities endpoint has been called
    /// </summary>
    public class EntitiesResponse : RosetteResponse
    {
        /// <summary>
        /// The collection of identified entities
        /// </summary>
        public List<RosetteEntity> Entities;

        /// <summary>
        /// The response headers returned from the API
        /// </summary>
        public ResponseHeaders ResponseHeaders;

        internal String entitiesKey = "entities";
        internal String typeKey = "type";
        internal String mentionKey = "mention";
        internal String normalizedKey = "normalized";
        internal String entityIDKey = "entityID";
        internal String countKey = "count";

        /// <summary>
        /// Creates a CategoriesResponse from the API's raw output
        /// </summary>
        /// <param name="apiResult">The API's raw output</param>
        public EntitiesResponse(HttpResponseMessage apiResult) : base(apiResult)
        {
            List<RosetteEntity> entities = new List<RosetteEntity>();
            IEnumerable<Object> enumerableResults = this.Content.ContainsKey(entitiesKey) ? this.Content[entitiesKey] as IEnumerable<Object> : new List<Object>();
            foreach (Object result in enumerableResults)
            {
                Dictionary<string, object> dictResult = result as Dictionary<string, object>;
                String type = dictResult.ContainsKey(typeKey) ? (dictResult[typeKey] as String) : null;
                String mention = dictResult.ContainsKey(mentionKey) ? dictResult[mentionKey] as String : null;
                String entityIDStr = dictResult.ContainsKey(entityIDKey) ? dictResult[entityIDKey] as String : null;
                EntityID entityID = null;
                if (entityIDStr != null)
                {
                    if (entityIDStr.Substring(0, 1).Equals("Q", StringComparison.OrdinalIgnoreCase))
                    {
                        entityID = new QID(entityIDStr);
                    }
                    else
                    {
                        entityID = new TemporaryID(entityIDStr);
                    }
                }
                String normalized = dictResult.ContainsKey(normalizedKey) ? dictResult[normalizedKey] as String : null;
                Nullable<int> count = dictResult.ContainsKey(countKey) ? dictResult[countKey] as Nullable<int> : null;
                entities.Add(EntitiesResponse.CreateEntity(type, mention, normalized, entityID, count));
            }
            this.Entities = entities;
            this.ResponseHeaders = new ResponseHeaders(apiResult.Headers);
        }

        /// <summary>
        /// A factory method for creating entities
        /// </summary>
        /// <param name="type">The EntityType determines which type of RosetteEntity will be created</param>
        /// <param name="mention">The mention of the </param>
        /// <param name="normalizedMention">A normalized version of the mention</param>
        /// <param name="id">The EntityID of the mention in the text processed by the Rosette API</param>
        /// <param name="count">The count of the mention in the text processed by the Rosette API</param>
        /// <returns>A new RosetteEntity of the given type</returns>
        public static RosetteEntity CreateEntity(String type, String mention, String normalizedMention, EntityID id, Nullable<int> count)
        {
            switch (type)
            {
                case RosetteEntity.LOCATION: return new EntityExtractionEntity.RosetteLocation(mention, normalizedMention, id, type, count);
                case RosetteEntity.ORGANIZATION: return new EntityExtractionEntity.RosetteOrganization(mention, normalizedMention, id, type, count);
                case RosetteEntity.PERSON: return new EntityExtractionEntity.RosettePerson(mention, normalizedMention, id, type, count);
                case RosetteEntity.PRODUCT: return new EntityExtractionEntity.RosetteProduct(mention, normalizedMention, id, type, count);
                case RosetteEntity.TITLE: return new EntityExtractionEntity.RosetteTitle(mention, normalizedMention, id, type, count);
                case RosetteEntity.NATIONALITY: return new EntityExtractionEntity.RosetteNationality(mention, normalizedMention, id, type, count);
                case RosetteEntity.RELIGION: return new EntityExtractionEntity.RosetteReligion(mention, normalizedMention, id, type, count);
                case RosetteEntity.CREDIT_CARD_NUM: return new EntityExtractionEntity.RosetteCreditCardNum(mention, normalizedMention, id, type, count);
                case RosetteEntity.EMAIL: return new EntityExtractionEntity.RosetteEmailAddress(mention, normalizedMention, id, type, count);
                case RosetteEntity.MONEY: return new EntityExtractionEntity.RosetteMoney(mention, normalizedMention, id, type, count);
                case RosetteEntity.PERSONAL_ID_NUM: return new EntityExtractionEntity.RosettePersonalIDNumber(mention, normalizedMention, id, type, count);
                case RosetteEntity.PHONE_NUMBER: return new EntityExtractionEntity.RosettePhoneNumber(mention, normalizedMention, id, type, count);
                case RosetteEntity.URL: return new EntityExtractionEntity.RosetteURL(mention, normalizedMention, id, type, count);
                case RosetteEntity.DATE: return new EntityExtractionEntity.RosetteDate(mention, normalizedMention, id, type, count);
                case RosetteEntity.TIME: return new EntityExtractionEntity.RosetteTime(mention, normalizedMention, id, type, count);
                case RosetteEntity.DISTANCE: return new EntityExtractionEntity.RosetteDistance(mention, normalizedMention, id, type, count);
                case RosetteEntity.LATITUDE_LONGITUDE: return new EntityExtractionEntity.RosetteLatLong(mention, normalizedMention, id, type, count);
                default: return new EntityExtractionEntity(mention, normalizedMention, id, type, count);
            }
        }
    }

    /// <summary>
    /// A class to represent entities that have been identified in text
    /// </summary>
    public class EntityExtractionEntity : RosetteEntity
    {
        /// <summary>
        /// The entity's name, normalized
        /// </summary>
        public String NormalizedMention;

        /// <summary>
        /// The number of times this entity appeared in the input to the API
        /// </summary>
        public Nullable<int> Count;

        /// <summary>
        /// Creates an entity modeled after the entities extracted from the Rosette API's Entity Extraction endpoint
        /// </summary>
        /// <param name="mention">The mention of the entity</param>
        /// <param name="normalizedMention">The normalized version of the entity mention</param>
        /// <param name="id">The entity ID</param>
        /// <param name="entityType">The entity type</param>
        /// <param name="count">The count of how many times the entity appeared in the text</param>
        public EntityExtractionEntity(String mention, String normalizedMention, EntityID id, String entityType, Nullable<int> count)
            : base(mention, id, entityType)
        {
            this.NormalizedMention = normalizedMention;
            this.Count = count;
        }

        /// <summary>
        /// A class for representing a location, as identified by the Rosette API
        /// </summary>
        public class RosetteLocation : EntityExtractionEntity
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
        public class RosetteDistance : EntityExtractionEntity
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
        public class RosetteLatLong : EntityExtractionEntity
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
        public class RosettePerson : EntityExtractionEntity
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
        public class RosetteOrganization : EntityExtractionEntity
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
        public class RosetteTitle : EntityExtractionEntity
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
        public class RosetteCreditCardNum : EntityExtractionEntity
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
        public class RosetteProduct : EntityExtractionEntity
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
        public class RosetteNationality : EntityExtractionEntity
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
        public class RosetteReligion : EntityExtractionEntity
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
        public class RosetteEmailAddress : EntityExtractionEntity
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
        public class RosetteMoney : EntityExtractionEntity
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
        public class RosettePersonalIDNumber : EntityExtractionEntity
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
        public class RosettePhoneNumber : EntityExtractionEntity
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
        public class RosetteURL : EntityExtractionEntity
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
        public class RosetteDate : EntityExtractionEntity
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
        public class RosetteTime : EntityExtractionEntity
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

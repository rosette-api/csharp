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
        /// Gets or sets the collection of identified entities
        /// </summary>
        public List<RosetteEntity> Entities { get; set; }

        /// <summary>
        /// Gets or sets the response headers returned from the API
        /// </summary>
        public ResponseHeaders ResponseHeaders { get; set; }

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
            this.ResponseHeaders = new ResponseHeaders(this.Headers);
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
                case RosetteEntity.LOCATION: return new RosetteEntity.RosetteLocation(mention, normalizedMention, id, type, count);
                case RosetteEntity.ORGANIZATION: return new RosetteEntity.RosetteOrganization(mention, normalizedMention, id, type, count);
                case RosetteEntity.PERSON: return new RosetteEntity.RosettePerson(mention, normalizedMention, id, type, count);
                case RosetteEntity.PRODUCT: return new RosetteEntity.RosetteProduct(mention, normalizedMention, id, type, count);
                case RosetteEntity.TITLE: return new RosetteEntity.RosetteTitle(mention, normalizedMention, id, type, count);
                case RosetteEntity.NATIONALITY: return new RosetteEntity.RosetteNationality(mention, normalizedMention, id, type, count);
                case RosetteEntity.RELIGION: return new RosetteEntity.RosetteReligion(mention, normalizedMention, id, type, count);
                case RosetteEntity.CREDIT_CARD_NUM: return new RosetteEntity.RosetteCreditCardNum(mention, normalizedMention, id, type, count);
                case RosetteEntity.EMAIL: return new RosetteEntity.RosetteEmailAddress(mention, normalizedMention, id, type, count);
                case RosetteEntity.MONEY: return new RosetteEntity.RosetteMoney(mention, normalizedMention, id, type, count);
                case RosetteEntity.PERSONAL_ID_NUM: return new RosetteEntity.RosettePersonalIDNumber(mention, normalizedMention, id, type, count);
                case RosetteEntity.PHONE_NUMBER: return new RosetteEntity.RosettePhoneNumber(mention, normalizedMention, id, type, count);
                case RosetteEntity.URL: return new RosetteEntity.RosetteURL(mention, normalizedMention, id, type, count);
                case RosetteEntity.DATE: return new RosetteEntity.RosetteDate(mention, normalizedMention, id, type, count);
                case RosetteEntity.TIME: return new RosetteEntity.RosetteTime(mention, normalizedMention, id, type, count);
                case RosetteEntity.DISTANCE: return new RosetteEntity.RosetteDistance(mention, normalizedMention, id, type, count);
                case RosetteEntity.LATITUDE_LONGITUDE: return new RosetteEntity.RosetteLatLong(mention, normalizedMention, id, type, count);
                default: return new RosetteEntity(mention, normalizedMention, id, type, count);
            }
        }
    }
}

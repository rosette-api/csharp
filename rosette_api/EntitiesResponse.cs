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
        /// Gets the collection of identified entities
        /// </summary
        public List<RosetteEntity> Entities { get; private set; }

        /// <summary>
        /// Gets the response headers returned from the API
        /// </summary>
        public ResponseHeaders ResponseHeaders { get; private set; }

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
                entities.Add(new RosetteEntity(mention, normalized, entityID, type, count));
            }
            this.Entities = entities;
            this.ResponseHeaders = new ResponseHeaders(this.Headers);
        }
    }
}

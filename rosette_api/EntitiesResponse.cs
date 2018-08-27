﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http;
using System.Collections;
using System.Runtime.Serialization;

namespace rosette_api
{
    /// <summary>
    /// A class for representing responses from the API when the Entities endpoint has been called
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class EntitiesResponse : RosetteResponse
    {
        /// <summary>
        /// Gets or sets the collection of identified entities
        /// </summary>
        [JsonProperty(PropertyName = entitiesKey)]
        public List<RosetteEntity> Entities { get; set; }

        private const String entitiesKey = "entities";
        private const String typeKey = "type";
        private const String mentionKey = "mention";
        private const String normalizedKey = "normalized";
        private const String entityIDKey = "entityId";
        private const String countKey = "count";
        private const String confidenceKey = "confidence";
        private const String dbpediaTypeKey = "dbpediaType";

        /// <summary>
        /// Creates a CategoriesResponse from the API's raw output
        /// </summary>
        /// <param name="apiResult">The API's raw output</param>
        public EntitiesResponse(HttpResponseMessage apiResult) : base(apiResult)
        {
            List<RosetteEntity> entities = new List<RosetteEntity>();
            JArray enumerableResults = this.ContentDictionary.ContainsKey(entitiesKey) ? this.ContentDictionary[entitiesKey] as JArray : new JArray();
            foreach (JObject result in enumerableResults)
            {
                String type = result.Properties().Where((p) => String.Equals(p.Name, typeKey, StringComparison.OrdinalIgnoreCase)).Any() ? result[typeKey].ToString() : null;
                String mention = result.Properties().Where((p) => String.Equals(p.Name, mentionKey, StringComparison.OrdinalIgnoreCase)).Any() ? result[mentionKey].ToString() : null;
                String entityIDStr = result.Properties().Where((p) => String.Equals(p.Name, entityIDKey, StringComparison.OrdinalIgnoreCase)).Any() ? result[entityIDKey].ToString() : null;
                EntityID entityID = entityIDStr != null ? new EntityID(entityIDStr) : null;
                String normalized = result.Properties().Where((p) => String.Equals(p.Name, normalizedKey, StringComparison.OrdinalIgnoreCase)).Any() ? result[normalizedKey].ToString() : null;
                Nullable<int> count = result.Properties().Where((p) => String.Equals(p.Name, countKey)).Any() ? result[countKey].ToObject<int?>() : null;
                Nullable<double> confidence = result.Properties().Where((p) => String.Equals(p.Name, confidenceKey)).Any() ? result[confidenceKey].ToObject<double?>() : null;
                String dbpediaType = result.Properties().Where((p) => String.Equals(p.Name, dbpediaTypeKey, StringComparison.OrdinalIgnoreCase)).Any() ? result[dbpediaTypeKey].ToString() : null;
                entities.Add(new RosetteEntity(mention, normalized, entityID, type, count, confidence, dbpediaType));
            }
            this.Entities = entities;
        }

        /// <summary>
        /// Creates an EntitiesResponse from its components
        /// </summary>
        /// <param name="entities">The list of entities</param>
        /// <param name="responseHeaders">The response headers returned from the API</param>
        /// <param name="content">The content of the response in dictionary form</param>
        /// <param name="contentAsJson">The content of the response in JSON form</param>
        public EntitiesResponse(List<RosetteEntity> entities, Dictionary<string, string> responseHeaders, Dictionary<string, object> content, string contentAsJson)
            : base(responseHeaders, content, contentAsJson)
        {
            this.Entities = entities;
        }

        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj">The object to compare against</param>
        /// <returns>True if equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is EntitiesResponse)
            {
                EntitiesResponse other = obj as EntitiesResponse;
                List<bool> conditions = new List<bool>() {
                    this.Entities != null && other.Entities != null ? this.Entities.SequenceEqual(other.Entities) : this.Entities == other.Entities,
                    this.ResponseHeaders != null && other.ResponseHeaders != null ? this.ResponseHeaders.Equals(other.ResponseHeaders) : this.ResponseHeaders == other.ResponseHeaders,
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
        /// Hashcode override
        /// </summary>
        /// <returns>The hashcode</returns>
        public override int GetHashCode()
        {
            int h0 = this.Entities != null ? this.Entities.Aggregate<RosetteEntity, int>(1, (seed, item) => seed ^ item.GetHashCode()) : 1;
            int h1 = this.ResponseHeaders != null ? this.ResponseHeaders.GetHashCode() : 1;
            return h0 ^ h1;
        }
    }
}

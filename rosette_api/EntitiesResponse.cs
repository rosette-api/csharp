﻿using System;
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
        /// Gets the response headers returned from the API
        /// </summary>
        public ResponseHeaders ResponseHeaders { get; private set; }

        private const String entitiesKey = "entities";
        private const String typeKey = "type";
        private const String mentionKey = "mention";
        private const String normalizedKey = "normalized";
        private const String entityIDKey = "entityId";
        private const String countKey = "count";

        /// <summary>
        /// Creates a CategoriesResponse from the API's raw output
        /// </summary>
        /// <param name="apiResult">The API's raw output</param>
        public EntitiesResponse(HttpResponseMessage apiResult) : base(apiResult)
        {
            List<RosetteEntity> entities = new List<RosetteEntity>();
            IEnumerable<Object> enumerableResults = this.ContentDictionary.ContainsKey(entitiesKey) ? this.ContentDictionary[entitiesKey] as IEnumerable<Object> : new List<Object>();
            foreach (Object result in enumerableResults)
            {
                Dictionary<string, object> dictResult = result as Dictionary<string, object>;
                String type = dictResult.ContainsKey(typeKey) ? (dictResult[typeKey] as String) : null;
                String mention = dictResult.ContainsKey(mentionKey) ? dictResult[mentionKey] as String : null;
                String entityIDStr = dictResult.ContainsKey(entityIDKey) ? dictResult[entityIDKey] as String : null;
                EntityID entityID = entityIDStr != null ? new EntityID(entityIDStr) : null;
                String normalized = dictResult.ContainsKey(normalizedKey) ? dictResult[normalizedKey] as String : null;
                Nullable<int> count = dictResult.ContainsKey(countKey) ? dictResult[countKey] as Nullable<int> : null;
                entities.Add(new RosetteEntity(mention, normalized, entityID, type, count));
            }
            this.Entities = entities;
            this.ResponseHeaders = new ResponseHeaders(this.Headers);
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
            this.ResponseHeaders = new ResponseHeaders(responseHeaders);
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

        /// <summary>
        /// ToString override.
        /// </summary>
        /// <returns>This response in JSON form</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            string entitiesString = this.Entities != null ? new StringBuilder("[").Append(String.Join(", ", this.Entities)).Append("]").ToString() : null;
            string responseHeadersString = this.ResponseHeaders != null ? this.ResponseHeaders.ToString() : null;
            builder.Append("\"entities\": ").Append(entitiesString)
                .Append(", responseHeaders: ").Append(responseHeadersString).Append("}");
            return builder.ToString();
        }

        /// <summary>
        /// Gets the content in JSON form
        /// </summary>
        /// <returns>The content in JSON form</returns>
        public string ContentToString()
        {
            StringBuilder builder = new StringBuilder("{");            
            string entitiesString = this.Entities != null ? new StringBuilder("[").Append(String.Join(", ", this.Entities)).Append("]").ToString() : null;
            builder.Append("\"entities\": ").Append(entitiesString).Append("}");
            return builder.ToString();
        }
    }
}
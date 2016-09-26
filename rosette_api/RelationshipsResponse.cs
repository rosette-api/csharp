﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Collections;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace rosette_api
{
    /// <summary>
    /// A class to represent responses from the Relationships endpoint of the Rosette API
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class RelationshipsResponse : RosetteResponse
    {
        private const string relationshipsKey = "relationships";
        internal const string predicateKey = "predicate";
        internal const string argsKey = "arg";
        internal const string temporalsKey = "temporals";
        internal const string locativesKey = "locatives";
        internal const string adjunctsKey = "adjuncts";
        internal const string confidenceKey = "confidence";

        /// <summary>
        /// Gets or sets the relationships extracted by the Rosette API
        /// </summary>
        [JsonProperty(relationshipsKey)]
        public List<RosetteRelationship> Relationships { get; set; }

        /// <summary>
        /// Creates a RelationshipsResponse from the given apiResult
        /// </summary>
        /// <param name="apiResult">The message from the API</param>
        public RelationshipsResponse(HttpResponseMessage apiResult) :base(apiResult)
        {
            JArray relationshipResults = this.ContentDictionary.ContainsKey(relationshipsKey) ? this.ContentDictionary[relationshipsKey] as JArray : new JArray();
            List<RosetteRelationship> relationships = new List<RosetteRelationship>();
            foreach (JObject relationship in relationshipResults)
            {
                String predicate = relationship.Properties().Where((p) => p.Name == predicateKey).Any() ? relationship[predicateKey].ToString() : null;
                IEnumerable<JProperty> argProps = relationship.Properties().Where((p) => p.Name.Contains(argsKey));
                List<string> arguments = argProps.Any() ? argProps.Select<JProperty, string>((p) => p.Value.ToString().Trim()).ToList() : null;
                List<string> temporals = relationship.Properties().Where((p) => p.Name == temporalsKey).Any() ? relationship[temporalsKey].ToObject<List<string>>() : null;
                List<string> locatives = relationship.Properties().Where((p) => p.Name == locativesKey).Any() ? relationship[locativesKey].ToObject<List<string>>() : null;
                List<string> adjuncts = relationship.Properties().Where((p) => p.Name == adjunctsKey).Any() ? relationship[adjunctsKey].ToObject<List<string>>() : null;
                Nullable<decimal> confidence = relationship.Properties().Where((p) => p.Name == confidenceKey).Any() ? relationship[confidenceKey].ToObject<decimal?>() : new Nullable<decimal>();
                relationships.Add(new RosetteRelationship(predicate, arguments, temporals, locatives, adjuncts, confidence));
            }
            this.Relationships = relationships;
        }

        /// <summary>
        /// Creates a RelationshipsResponse from its components
        /// </summary>
        /// <param name="relationships">The relationships</param>
        /// <param name="responseHeaders">The response headers returned from the Rosette API</param>
        /// <param name="content">The content (the relationships) in dictionary form</param>
        /// <param name="contentAsJson">The content in JSON form</param>
        public RelationshipsResponse(List<RosetteRelationship> relationships, Dictionary<string, string> responseHeaders, Dictionary<string, object> content, string contentAsJson)
            : base(responseHeaders, content, contentAsJson)
        {
            this.Relationships = relationships;
        }

        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj">The object to compare against</param>
        /// <returns>True if equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is RelationshipsResponse)
            {
                RelationshipsResponse other = obj as RelationshipsResponse;
                List<bool> conditions = new List<bool>() {
                    this.Relationships != null && other.ResponseHeaders != null ? this.Relationships.SequenceEqual(other.Relationships) : this.Relationships == other.Relationships,
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
        /// HashCode override
        /// </summary>
        /// <returns>The hashcode</returns>
        public override int GetHashCode()
        {
            int h0 = this.Relationships != null ? this.Relationships.Aggregate<RosetteRelationship, int>(1, (seed, item) => seed ^ item.GetHashCode()) : 1;
            int h1 = this.ResponseHeaders != null ? this.ResponseHeaders.GetHashCode() : 1;
            return h0 ^ h1;
        }
    }

    /// <summary>
    /// A class to represent a relationship as identified by the Rosette API
    /// </summary>
    [JsonConverter(typeof(RelationshipConverter))]
    public class RosetteRelationship
    {
        internal const string ARGUMENTS = "arguments";        

        /// <summary>
        /// Gets or sets the main action or verb acting on the first argument, or the action connecting multiple arguments.
        /// </summary>
        [JsonProperty(RelationshipsResponse.predicateKey, NullValueHandling = NullValueHandling.Ignore)]
        public String Predicate { get; set; }

        /// <summary>
        /// Gets or sets the or more subjects in the relationship
        /// </summary>
        [JsonProperty(ARGUMENTS, NullValueHandling = NullValueHandling.Ignore)]
        public List<String> Arguments { get; set; }

        /// <summary>
        /// Gets or sets the time frame of the action.  May be empty.
        /// </summary>
        [JsonProperty(RelationshipsResponse.temporalsKey, NullValueHandling = NullValueHandling.Ignore)]
        public List<String> Temporals { get; set; }
        
        /// <summary>
        /// Gets or sets the location(s) of the action.  May be empty.
        /// </summary>
        [JsonProperty(RelationshipsResponse.locativesKey, NullValueHandling = NullValueHandling.Ignore)]
        public List<String> Locatives { get; set; }

        /// <summary>
        /// Gets or sets all other parts of the relationship.  May be empty.
        /// </summary>
        [JsonProperty(RelationshipsResponse.adjunctsKey, NullValueHandling = NullValueHandling.Ignore)]
        public List<String> Adjucts { get; set; }

        /// <summary>
        /// Gets a score for each relationship result, ranging from 0 to 1. You can use this measurement as a threshold to filter out undesired results.
        /// </summary>
        [JsonProperty(RelationshipsResponse.confidenceKey, NullValueHandling=NullValueHandling.Ignore)]
        public Nullable<decimal> Confidence { get; private set; }

        /// <summary>
        /// Creates a grammatical relationship
        /// </summary>
        /// <param name="predicate">The main action or verb acting on the first argument, or the action connecting multiple arguments.</param>
        /// <param name="arguments">One or more subjects in the relationship</param>
        /// <param name="temporals">Time frames of the action.  May be empty.</param>
        /// <param name="locatives">Locations of the action.  May be empty.</param>
        /// <param name="adjunts">All other parts of the relationship.  May be empty.</param>
        /// <param name="confidence">A score for each relationship result, ranging from 0 to 1. You can use this measurement as a threshold to filter out undesired results.</param>
        public RosetteRelationship(String predicate, List<String> arguments, List<string> temporals, List<string> locatives, List<string> adjunts, Nullable<decimal> confidence) {
            this.Predicate = predicate;
            this.Arguments = arguments;
            this.Temporals = temporals;
            this.Locatives = locatives;
            this.Adjucts = adjunts;
            this.Confidence = confidence;
        }

        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj">The object to compare against</param>
        /// <returns>True if equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is RosetteRelationship) {
                RosetteRelationship other = obj as RosetteRelationship;
                List<bool> conditions = new List<bool>() {
                    this.Adjucts != null && other.Adjucts != null ? this.Adjucts.SequenceEqual(other.Adjucts) : this.Adjucts == other.Adjucts,
                    this.Arguments != null && other.Arguments != null ? this.Arguments.SequenceEqual(other.Arguments) : this.Arguments == other.Arguments,
                    this.Confidence == other.Confidence,
                    this.Locatives != null && other.Locatives != null ? this.Locatives.SequenceEqual(other.Locatives) : this.Locatives == other.Locatives,
                    this.Predicate == other.Predicate,
                    this.Temporals != null && other.Temporals != null ? this.Temporals.SequenceEqual(other.Temporals) : this.Temporals == other.Temporals,
                    this.GetHashCode() == other.GetHashCode()
                };
                return conditions.All(condition => condition);
            } else {
                return false;
            }
        }

        /// <summary>
        /// HashCode override
        /// </summary>
        /// <returns>The HashCode</returns>
        public override int GetHashCode()
        {
            int h0 = this.Temporals != null ? this.Temporals.Aggregate<string, int>(1, (seed, item) => seed ^ item.GetHashCode()) : 1;
            int h1 = this.Predicate != null ? this.Predicate.GetHashCode() : 1;
            int h2 = this.Locatives != null ? this.Locatives.Aggregate<string, int>(1, (seed, item) => seed ^ item.GetHashCode()) : 1;
            int h3 = this.Confidence != null ? this.Confidence.GetHashCode() : 1;
            int h4 = this.Arguments != null ? this.Arguments.Aggregate<string, int>(1, (seed, item) => seed ^ item.GetHashCode()) : 1;
            int h5 = this.Adjucts != null ? this.Adjucts.Aggregate<string, int>(1, (seed, item) => seed ^ item.GetHashCode()) : 1;
            int hashcode = h0 ^ h1 ^ h2 ^ h3 ^ h4 ^ h4 ^ h5;
            return hashcode;
        }

        /// <summary>
        /// ToString override.  Writes this RosetteRelationship in JSON form
        /// </summary>
        /// <returns>The relationship as JSON</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    internal class RelationshipConverter : JsonConverter
    {

        public override bool CanConvert(Type objectType)
        {
            return objectType.Equals(typeof(RelationshipsResponse));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JsonSerializer relationshipSerializer = new JsonSerializer();
            relationshipSerializer.NullValueHandling = NullValueHandling.Ignore;
            relationshipSerializer.ContractResolver = new RelationshipContractResolver(value as RosetteRelationship);
            JsonObjectContract contract = (JsonObjectContract)relationshipSerializer.ContractResolver.ResolveContract(value.GetType());

            writer.WriteStartObject();
            foreach (JsonProperty property in contract.Properties)
            {
                if (!property.Ignored)
                {
                    writer.WritePropertyName(property.PropertyName);
                    relationshipSerializer.Serialize(writer, property.ValueProvider.GetValue(value));
                }
            }
            writer.WriteEndObject();
        }
    }

    internal class RelationshipContractResolver : DefaultContractResolver
    {
        private RosetteRelationship Relationship;

        internal RelationshipContractResolver(RosetteRelationship relationship) : base()
        {
            this.Relationship = relationship;
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);
            IList<JsonProperty> propertiesToRehandle = properties.Where(p => p.PropertyName ==  RosetteRelationship.ARGUMENTS).ToList();
            IList<JsonProperty> modifiedProperties = new List<JsonProperty>();
            int order = 0;
            foreach (JsonProperty jProperty in properties)
            {
                if (propertiesToRehandle.Contains(jProperty))
                {
                    IList<JsonProperty> newProperties = new List<JsonProperty>();
                    IList<string> argumentList = Relationship.Arguments;
                    for (int i = 1; i <= argumentList.Count; i++)
                    {
                        JsonProperty argProp = new JsonProperty();
                        argProp.Order = order;
                        argProp.PropertyName = RelationshipsResponse.argsKey + i;
                        argProp.PropertyType = typeof(string);
                        argProp.HasMemberAttribute = true;
                        argProp.ValueProvider = new StaticValueProvider(argumentList[i - 1]);
                        argProp.Ignored = argProp.ValueProvider.GetValue("Not needed") == null;
                        newProperties.Add(argProp);
                    }
                    foreach (JsonProperty newProperty in newProperties)
                    {
                        modifiedProperties.Insert(order++, newProperty);
                    }
                }
                else
                {
                    jProperty.Order = order;
                    jProperty.Ignored = jProperty.ValueProvider.GetValue(this.Relationship) == null;
                    modifiedProperties.Insert(order++, jProperty);
                }
            }
            return modifiedProperties;
        }
    }
}

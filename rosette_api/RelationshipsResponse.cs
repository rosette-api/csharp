using System;
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
        internal const string ARGPREFIX = "arg";
        internal const string ARG_ID_FORMAT = "arg{0}ID";
        internal const string temporalsKey = "temporals";
        internal const string locativesKey = "locatives";
        internal const string adjunctsKey = "adjuncts";
        internal const string confidenceKey = "confidence";
        internal const string MODALITIES = "modalities";

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
                IEnumerable<JProperty> allArgRelatedProps = relationship.Properties().Where((p) => p.Name.Contains(ARGPREFIX));
                IEnumerable<JProperty> argStringProps = allArgRelatedProps.Where((p) => !p.Name.Contains("ID"));
                IEnumerable<JProperty> argIDProps = allArgRelatedProps.Where((p) => p.Name.Contains("ID"));
                Dictionary<int, string> argStrings = argStringProps.Any() ? argStringProps.ToDictionary(p => Int32.Parse(p.Name.Substring(3, 1)), p => p.Value.ToString().Trim()) : null;
                Dictionary<int, string> argIDs = argIDProps.Any() ? argIDProps.ToDictionary(p => Int32.Parse(p.Name.Substring(3, 1)), p => p.Value.ToString().Trim()) : null;
                List<string> temporals = relationship.Properties().Where((p) => p.Name == temporalsKey).Any() ? relationship[temporalsKey].ToObject<List<string>>() : null;
                List<string> locatives = relationship.Properties().Where((p) => p.Name == locativesKey).Any() ? relationship[locativesKey].ToObject<List<string>>() : null;
                List<string> adjuncts = relationship.Properties().Where((p) => p.Name == adjunctsKey).Any() ? relationship[adjunctsKey].ToObject<List<string>>() : null;
                Nullable<decimal> confidence = relationship.Properties().Where((p) => p.Name == confidenceKey).Any() ? relationship[confidenceKey].ToObject<decimal?>() : new Nullable<decimal>();
                HashSet<string> modalities = relationship.Properties().Where((p) => p.Name == MODALITIES).Any() ? relationship[MODALITIES].ToObject<HashSet<string>>() : null;
                relationships.Add(new RosetteRelationship(predicate, argStrings, argIDs, temporals, locatives, adjuncts, confidence, modalities));
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
    [JsonObject(MemberSerialization=MemberSerialization.OptIn)]
    public class RosetteRelationship
    {
        internal const string ARGUMENTS = "arguments";        

        /// <summary>
        /// Gets or sets the main action or verb acting on the first argument, or the action connecting multiple arguments.
        /// </summary>
        [JsonProperty(RelationshipsResponse.predicateKey, NullValueHandling = NullValueHandling.Ignore)]
        public String Predicate { get; set; }

        /// <summary>
        /// Gets or sets the arguments in the relationship
        /// </summary>
        [Obsolete("This property has been made obsolete by the introduction of the ArgumentsFull property, which contains extra information about arguments.")]
        public List<String> Arguments { get; set; }

        /// <summary>
        /// Gets or sets the arguments in the relationship.  Arguments have extra content, such as IDs.
        /// </summary>
        [JsonProperty(ARGUMENTS, NullValueHandling = NullValueHandling.Ignore)]
        public List<Argument> ArgumentsFull { get; set; }

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
        /// Gets or sets the modalities of the relationship.
        /// </summary>
        [JsonProperty(RelationshipsResponse.MODALITIES, NullValueHandling = NullValueHandling.Ignore)]
        public HashSet<string> Modalities { get; set; }

        /// <summary>
        /// Creates a grammatical relationship.
        /// Temporary EntityIDs, ordered "T0", "T1"..., are added to the argument strings in an ArgumentsFull property.
        /// </summary>
        /// <param name="predicate">The main action or verb acting on the first argument, or the action connecting multiple arguments.</param>
        /// <param name="arguments">One or more arguments in the relationship</param>
        /// <param name="temporals">Time frames of the action.  May be empty.</param>
        /// <param name="locatives">Locations of the action.  May be empty.</param>
        /// <param name="adjunts">All other parts of the relationship.  May be empty.</param>
        /// <param name="confidence">A score for each relationship result, ranging from 0 to 1. You can use this measurement as a threshold to filter out undesired results.</param>
        public RosetteRelationship(String predicate, List<String> arguments, List<string> temporals, List<string> locatives, List<string> adjunts, Nullable<decimal> confidence) {
            this.Predicate = predicate;
#pragma warning disable 618
            this.Arguments = arguments;
            this.ArgumentsFull = new List<Argument>(this.Arguments.Select<string, Argument>((str, index) => new Argument(index + 1, str, null)));
#pragma warning restore 618
            this.Temporals = temporals;
            this.Locatives = locatives;
            this.Adjucts = adjunts;
            this.Confidence = confidence;
        }

        /// <summary>
        /// Creates a grammatical relationship
        /// </summary>
        /// <param name="predicate">The main action or verb acting on the first argument, or the action connecting multiple arguments.</param>
        /// <param name="arguments">The mention(s), mapped to the argument number, of one or more arguments in the relationship</param>
        /// <param name="IDs">The IDs of the arguments, mapped to the corresponding argument number.  The ID with key 1 should correspond to the argument with key 1.</param>
        /// <param name="temporals">Time frames of the action.  May be empty.</param>
        /// <param name="locatives">Locations of the action.  May be empty.</param>
        /// <param name="adjunts">All other parts of the relationship.  May be empty.</param>
        /// <param name="confidence">A score for each relationship result, ranging from 0 to 1. You can use this measurement as a threshold to filter out undesired results.</param>
        /// <param name="modalities">The modalities of the relationship.</param>
        public RosetteRelationship(string predicate, Dictionary<int, string> arguments, Dictionary<int, string> IDs, List<string> temporals, List<string> locatives, List<string> adjunts, Nullable<decimal> confidence, HashSet<string> modalities)
        {
            this.Predicate = predicate;
#pragma warning disable 618
            this.Arguments = arguments.Select(kvp => kvp.Value).ToList();
            this.ArgumentsFull = new List<Argument>(arguments.Select<KeyValuePair<int, string>, Argument>((kvp, index) => new Argument(kvp.Key, kvp.Value, IDs == null || !IDs.ContainsKey(kvp.Key) || IDs[kvp.Key] == null ? null : new EntityID(IDs[kvp.Key]))));
#pragma warning restore 618
            this.Temporals = temporals;
            this.Locatives = locatives;
            this.Adjucts = adjunts;
            this.Confidence = confidence;
            this.Modalities = modalities;
        }

        /// <summary>
        /// Creates a grammatical relationship
        /// </summary>
        /// <param name="predicate">The main action or verb acting on the first argument, or the action connecting multiple arguments.</param>
        /// <param name="arguments">The arguments in the relationship</param>
        /// <param name="temporals">Time frames of the action.  May be empty.</param>
        /// <param name="locatives">Locations of the action.  May be empty.</param>
        /// <param name="adjunts">All other parts of the relationship.  May be empty.</param>
        /// <param name="confidence">A score for each relationship result, ranging from 0 to 1. You can use this measurement as a threshold to filter out undesired results.</param>
        /// <param name="modalities">The modalities of the relationship.</param>
        public RosetteRelationship(string predicate, List<Argument> arguments, List<string> temporals, List<string> locatives, List<string> adjunts, Nullable<decimal> confidence, HashSet<string> modalities)
        {
            this.Predicate = predicate;
#pragma warning disable 618
            this.Arguments = arguments.Select(a => a.Mention).ToList();
            this.ArgumentsFull = arguments;
#pragma warning restore 618
            this.Temporals = temporals;
            this.Locatives = locatives;
            this.Adjucts = adjunts;
            this.Confidence = confidence;
            this.Modalities = modalities;
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
#pragma warning disable 618
                    this.Arguments != null && other.Arguments != null ? this.Arguments.SequenceEqual(other.Arguments) : this.Arguments == other.Arguments,
#pragma warning restore 618
                    this.ArgumentsFull != null && other.ArgumentsFull != null ? this.ArgumentsFull.SequenceEqual(other.ArgumentsFull) : this.ArgumentsFull == other.ArgumentsFull,
                    this.Confidence == other.Confidence,
                    this.Locatives != null && other.Locatives != null ? this.Locatives.SequenceEqual(other.Locatives) : this.Locatives == other.Locatives,
                    this.Predicate == other.Predicate,
                    this.Temporals != null && other.Temporals != null ? this.Temporals.SequenceEqual(other.Temporals) : this.Temporals == other.Temporals,
                    this.Modalities != null && other.Modalities != null ? this.Modalities.SetEquals(other.Modalities) : this.Modalities == other.Modalities,
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
#pragma warning disable 618
            int h4 = this.Arguments != null ? this.Arguments.Aggregate<string, int>(1, (seed, item) => seed ^ item.GetHashCode()) : 1;
#pragma warning restore 618
            int h5 = this.ArgumentsFull != null ? this.ArgumentsFull.Aggregate<Argument, int>(1, (seed, item) => seed ^ item.GetHashCode()) : 1;            
            int h6 = this.Adjucts != null ? this.Adjucts.Aggregate<string, int>(1, (seed, item) => seed ^ item.GetHashCode()) : 1;
            int h7 = this.Modalities != null ? this.Modalities.Aggregate<string, int>(1, (seed, item) => seed ^ item.GetHashCode()) : 1;
            int hashcode = h0 ^ h1 ^ h2 ^ h3 ^ h4 ^ h4 ^ h5 ^ h6 ^ h7;
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

    #region JSONConvertersForRelationships
    internal class RelationshipConverter : JsonConverter
    {

        public override bool CanConvert(Type objectType)
        {
            return objectType.Equals(typeof(RosetteRelationship));
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

        internal RelationshipContractResolver(RosetteRelationship relationship)
            : base()
        {
            this.Relationship = relationship;
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization).Where(p => p.HasMemberAttribute).ToList();
            IList<JsonProperty> propertiesToRehandle = properties.Where(p => p.PropertyName == RosetteRelationship.ARGUMENTS).ToList();
            IList<JsonProperty> modifiedProperties = new List<JsonProperty>();
            int order = 0;
            foreach (JsonProperty jProperty in properties)
            {
                if (propertiesToRehandle.Contains(jProperty))
                {
                    IList<JsonProperty> newProperties = new List<JsonProperty>();
                    IList<Argument> argumentList = Relationship.ArgumentsFull;
                    foreach (Argument argument in argumentList)
                    {
                        JsonProperty argStringProp = this.CreateNewCustomProperty(order, RelationshipsResponse.ARGPREFIX + argument.Position, typeof(string), argument.Mention);
                        modifiedProperties.Insert(order++, argStringProp);
                        if (argument.ID != null && argument.ID.ID != null)
                        {
                            JsonProperty argIDProp = this.CreateNewCustomProperty(order, String.Format(RelationshipsResponse.ARG_ID_FORMAT, argument.Position), typeof(string), argument.ID.ID);
                            modifiedProperties.Insert(order++, argIDProp);
                        }
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

        private JsonProperty CreateNewCustomProperty(int order, string name, Type valueType, object value)
        {
            JsonProperty argProp = new JsonProperty();
            argProp.Order = order;
            argProp.PropertyName = name;
            argProp.PropertyType = valueType;
            argProp.HasMemberAttribute = true;
            argProp.ValueProvider = new StaticValueProvider(value);
            argProp.Ignored = argProp.ValueProvider.GetValue("Not needed") == null;
            return argProp;
        }
    }
    #endregion JSONConvertersForRelationships

    /// <summary>
    /// Represents an argument of a relationship
    /// </summary>
    [JsonConverter(typeof(ArgumentConverter))]
    [JsonObject(MemberSerialization=MemberSerialization.OptIn)]
    public class Argument
    {
        internal const string ARGUMENT = "arg{0}";
        internal const string ARGID = "arg{0}ID";

        /// <summary>
        /// Gets or sets the argument's text
        /// </summary>
        [JsonProperty(ARGUMENT, NullValueHandling = NullValueHandling.Ignore)]
        public string Mention { get; set; }

        /// <summary>
        /// Private field for getters and setters of the Position attribute
        /// </summary>
        private int _position;

        /// <summary>
        /// Gets or sets the argument's position in relation to other arguments.  Limited to 1+.
        /// </summary>
        public int Position { 
            get { return _position; }
            set 
            { 
                if (value <= 0) 
                { 
                    throw new ArgumentOutOfRangeException("Value must be greater than zero."); 
                } 
                else 
                { 
                    _position = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the argument's ID
        /// </summary>
        [JsonProperty(ARGID, NullValueHandling = NullValueHandling.Ignore)]
        public EntityID ID { get; set; }

        /// <summary>
        /// Creates an argument from its position and components.
        /// </summary>
        /// <param name="position">The position of the argument in relation to other arguments.</param>
        /// <param name="argument">The mention of the argument.</param>
        /// <param name="id">The id of the argument.</param>
        public Argument(int position, string argument, EntityID id) {
            this.Mention = argument;
            this.Position = position;
            this.ID = id;
        }

        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj">The object to compare against</param>
        /// <returns>True if equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is Argument) {
                Argument other = obj as Argument;
                List<bool> conditions = new List<bool>() {
                    this.Mention == other.Mention,
                    this.Position == other.Position,
                    this.ID != null ? this.ID.Equals(other.ID) : other.ID == null,
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
            int h0 = this.Mention != null ? this.Mention.GetHashCode() : 1;
            int h1 = this.ID != null ? this.ID.GetHashCode() : 1;
            int h2 = this.Position.GetHashCode();
            int hashcode = h0 ^ h1 ^ h2;
            return hashcode;
        }

        /// <summary>
        /// ToString override.  Writes this Argument in JSON form
        /// </summary>
        /// <returns>The relationship as JSON</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    #region JSONConvertersForArguments
    internal class ArgumentConverter : JsonConverter
    {

        public override bool CanConvert(Type objectType)
        {
            return objectType.Equals(typeof(Argument));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JsonSerializer argumentSerializer = new JsonSerializer();
            argumentSerializer.NullValueHandling = NullValueHandling.Ignore;
            argumentSerializer.ContractResolver = new ArgumentContractResolver(value as Argument);
            JsonObjectContract contract = (JsonObjectContract)argumentSerializer.ContractResolver.ResolveContract(value.GetType());

            writer.WriteStartObject();
            foreach (JsonProperty property in contract.Properties)
            {
                if (!property.Ignored)
                {
                    writer.WritePropertyName(property.PropertyName);
                    argumentSerializer.Serialize(writer, property.ValueProvider.GetValue(value));
                }
            }
            writer.WriteEndObject();
        }
    }

    internal class ArgumentContractResolver : DefaultContractResolver
    {
        private Argument Argument;

        internal ArgumentContractResolver(Argument argument) : base()
        {
            this.Argument = argument;
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);
            IList<JsonProperty> propertiesToRehandle = properties.Where(p => p.PropertyName == Argument.ARGUMENT || p.PropertyName == Argument.ARGID).ToList();
            IList<JsonProperty> modifiedProperties = new List<JsonProperty>();
            foreach (JsonProperty jProperty in properties)
            {
                if (propertiesToRehandle.Contains(jProperty))
                {
                    jProperty.PropertyName = String.Format(jProperty.PropertyName, Argument.Position);
                }
                modifiedProperties.Add(jProperty);
            }
            return modifiedProperties;
        }
    }
    #endregion JSONConvertersforObjects
}

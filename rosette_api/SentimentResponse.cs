using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Collections;
using Newtonsoft.Json.Converters;

namespace rosette_api
{
    /// <summary>
    /// A class for representing responses from the sentiment analysis endpoint of the Rosette API
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class SentimentResponse : RosetteResponse
    {
        private const string docKey = "document";
        private const string entitiesKey = "entities";
        internal const string labelKey = "label";
        internal const string confidenceKey = "confidence";
        internal const string mentionKey = "mention";
        internal const string normalizedMentionKey = "normalized";
        internal const string countKey = "count";
        internal const string typeKey = "type";
        internal const string entityIDKey = "entityId";
        internal const string sentimentKey = "sentiment";

        /// <summary>
        /// Gets or sets the document-level sentiment identified by the Rosette API
        /// </summary>
        [JsonProperty(docKey)]
        public RosetteSentiment DocSentiment { get; set; }
        /// <summary>
        /// Gets or sets the entities identified by the Rosette API with sentiment
        /// </summary>
        [JsonProperty(entitiesKey)]
        public List<RosetteSentimentEntity> EntitySentiments { get; set; }

        /// <summary>
        /// Creates a SentimentResponse from the given apiResult
        /// </summary>
        /// <param name="apiResult">The message from the API</param>
        public SentimentResponse(HttpResponseMessage apiResult)
            : base(apiResult)
        {
            List<RosetteSentimentEntity> entitySentiments = new List<RosetteSentimentEntity>();
            JObject docResult = this.ContentDictionary.ContainsKey(docKey) ? this.ContentDictionary[docKey] as JObject : new JObject();
            string docSentiment = docResult.Properties().Where((p) => p.Name == labelKey).Any() ? docResult[labelKey].ToString() : null;
            decimal? docSentimentConfidence = docResult.Properties().Where((p) => p.Name == confidenceKey).Any() ? docResult[confidenceKey].ToObject<decimal?>() : new decimal?();
            this.DocSentiment = docSentiment != null && docSentimentConfidence != null ? new RosetteSentiment(docSentiment, docSentimentConfidence) : null;
            JArray enumerableResults = this.ContentDictionary.ContainsKey(entitiesKey) ? this.ContentDictionary[entitiesKey] as JArray : new JArray();
            foreach (JObject result in enumerableResults)
            {
                string type = result.Properties().Where((p) => p.Name == typeKey).Any() ? result[typeKey].ToString() : null;
                string mention = result.Properties().Where((p) => p.Name == mentionKey).Any() ? result[mentionKey].ToString() : null;
                string normalizedMention = result.Properties().Where((p) => p.Name == normalizedMentionKey).Any() ? result[normalizedMentionKey].ToString() : null;
                string entityIDStr = result.Properties().Where((p) => p.Name == entityIDKey).Any() ? result[entityIDKey].ToString() : null;
                EntityID entityID = entityIDStr != null ? new EntityID(entityIDStr) : null;
                Nullable<int> count = result.Properties().Where((p) => p.Name == countKey).Any() ? result[countKey].ToObject<int?>() : null;
                string sentiment = null;
                Nullable<double> confidence = null;
                if (result.Properties().Where((p) => p.Name == sentimentKey).Any())
                {
                    JObject entitySentiment = result[sentimentKey].ToObject<JObject>();
                    sentiment = entitySentiment.Properties().Where((p) => p.Name == labelKey).Any() ? entitySentiment[labelKey].ToString() : null;
                    confidence = entitySentiment.Properties().Where((p) => p.Name == confidenceKey).Any() ? entitySentiment[confidenceKey].ToObject<decimal?>() : new decimal?();
                }
                entitySentiments.Add(new RosetteSentimentEntity(mention, normalizedMention, entityID, type, count, sentiment, confidence));
            }
            this.EntitySentiments = entitySentiments;
        }

        /// <summary>
        /// Creates a SentimentResponse from its components
        /// </summary>
        /// <param name="docSentiment">The sentiment of the entire document/input text</param>
        /// <param name="entitySentiments">The entities, each with a sentiment, found in the input text</param>
        /// <param name="responseHeaders">The response headers returned from the API</param>
        /// <param name="content">The content (the doc and entity sentiments) in Dictionary form</param>
        /// <param name="contentAsJson">The content in JSON form</param>
        public SentimentResponse(RosetteSentiment docSentiment, List<RosetteSentimentEntity> entitySentiments, Dictionary<string, string> responseHeaders, Dictionary<string, object> content, string contentAsJson)
            : base(responseHeaders, content, contentAsJson)
        {
            this.DocSentiment = docSentiment;
            this.EntitySentiments = entitySentiments;
        }

        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj">The object to compare against</param>
        /// <returns>True if equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is SentimentResponse)
            {
                SentimentResponse other = obj as SentimentResponse;
                List<bool> conditions = new List<bool>() {
                    this.DocSentiment != null && other.DocSentiment != null ? this.DocSentiment.Equals(other.DocSentiment) : this.DocSentiment == other.DocSentiment,
                    this.EntitySentiments != null && other.EntitySentiments != null ? this.EntitySentiments.SequenceEqual(other.EntitySentiments) : this.EntitySentiments == other.EntitySentiments,
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
            int h0 = this.ResponseHeaders != null ? this.ResponseHeaders.GetHashCode() : 1;
            int h1 = this.EntitySentiments != null ? this.EntitySentiments.Aggregate<RosetteSentimentEntity, int>(1, (seed, item) => seed ^ item.GetHashCode()) : 1;
            int h2 = this.DocSentiment != null ? this.DocSentiment.GetHashCode() : 1;
            return h0 ^ h1 ^ h2;
        }

        /// <summary>
        /// A class for representing sentiments
        /// </summary>
        [JsonObject(MemberSerialization.OptOut)]
        public class RosetteSentiment
        {
            /// <summary>
            /// The enumeration of possible sentiment labels
            /// </summary>
            public enum SentimentLabel
            {
                /// <summary>
                /// The sentiment is positive
                /// </summary>
                [JsonProperty("pos")]
                pos,
                /// <summary>
                /// The sentiment is neutral
                /// </summary>
                [JsonProperty("neu")]
                neu,
                /// <summary>
                /// The sentiment is negative
                /// </summary>
                [JsonProperty("neg")]
                neg
            };
            /// <summary>
            /// On a scale of 0-1, the confidence in the Label's correctness.
            /// </summary>
            [JsonProperty(confidenceKey)]
            public Nullable<decimal> Confidence;
            /// <summary>
            /// The label indicating the sentiment
            /// </summary>
            [JsonProperty(labelKey)]
            [JsonConverter(typeof(StringEnumConverter))]
            public SentimentLabel Label;

            /// <summary>
            /// A constructor for creating RosetteSentiments
            /// </summary>
            /// <param name="sentiment">The sentiment label: "pos", "neu", or "neg"</param>
            /// <param name="confidence">An indicator of confidence in the label being correct.  A range from 0-1.</param>
            public RosetteSentiment(String sentiment, Nullable<decimal> confidence)
            {
                switch (sentiment)
                {
                    case "pos": this.Label = SentimentLabel.pos; break;
                    case "neu": this.Label = SentimentLabel.neu; break;
                    case "neg": this.Label = SentimentLabel.neg; break;
                    default: this.Label = SentimentLabel.neu; break;
                }
                this.Confidence = confidence;
            }

            /// <summary>
            /// Equals override
            /// </summary>
            /// <param name="obj">The object to compare against</param>
            /// <returns>True if equal</returns>
            public override bool Equals(object obj)
            {
                if (obj is RosetteSentiment)
                {
                    RosetteSentiment other = obj as RosetteSentiment;
                    List<bool> conditions = new List<bool>() {
                        this.Confidence == other.Confidence,
                        this.Label.Equals(other.Label),
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
                int h0 = this.Confidence != null ? this.Confidence.GetHashCode() : 1;
                int h1 = this.Label.GetHashCode();
                return h0 ^ h1;
            }

            /// <summary>
            /// ToString override.
            /// </summary>
            /// <returns>Writes this RosetteSentiment in JSON form</returns>
            public override string ToString()
            {
                return JsonConvert.SerializeObject(this);
            }
        }
    }

    /// <summary>
    /// A class to represent an entity returned by the Sentiment Analysis endpoint of the Rosette API
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class RosetteSentimentEntity : RosetteEntity
    {
        /// <summary>
        /// The sentiment label of this entity
        /// </summary>
        [JsonProperty(SentimentResponse.sentimentKey)]
        public SentimentResponse.RosetteSentiment Sentiment { get; set; }

        /// <summary>
        /// Creates an entity that has a sentiment associated with it
        /// </summary>
        /// <param name="mention">The mention of the entity</param>
        /// <param name="normalizedMention">The normalized mention of the entity</param>
        /// <param name="id">The contextual ID of the entity to compare it against other entities</param>
        /// <param name="entityType">The entity type</param>
        /// <param name="count">The number of times the entity appeared in the text</param>
        /// <param name="sentiment">The contextual sentiment of the entity</param>
        /// <param name="confidence">The confidence that the sentiment was correctly identified</param>
        public RosetteSentimentEntity(String mention, String normalizedMention, EntityID id, String entityType, Nullable<int> count, String sentiment, Nullable<double> confidence) : base(mention, normalizedMention, id, entityType, count)
        {
            this.Sentiment = new SentimentResponse.RosetteSentiment(sentiment, confidence);
        }

        /// <summary>
        /// Is this RosetteSentimentEntity the same as the other entity?
        /// </summary>
        /// <param name="other">The other entity</param>
        /// <returns>True if the entities are equal (sentiment is ignored).</returns>
        public bool EntityEquals(RosetteEntity other)
        {
           List<bool> conditions = new List<bool>() {
               this.Mention == other.Mention &&
               this.NormalizedMention != null && other.NormalizedMention != null ? this.NormalizedMention.Equals(other.NormalizedMention) : this.NormalizedMention == other.NormalizedMention,
               this.Mention != null && other.Mention != null ? this.Mention.Equals(other.Mention) : this.Mention == other.Mention,
               this.ID != null && other.ID != null ? this.ID.Equals(other.ID) : this.ID == other.ID,
               this.EntityType != null && other.EntityType != null ? this.EntityType.Equals(other.EntityType) : this.EntityType == other.EntityType,
               this.Count != null && other.Count != null ? this.Count.Equals(other.Count) : this.Count == other.Count,
               this.Confidence != null && other.Confidence != null ? this.Confidence.Equals(other.Confidence) : this.Confidence == other.Confidence
           };
           return conditions.All(condition => condition);
        }

        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj">The object to compare against</param>
        /// <returns>True if equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is RosetteSentimentEntity)
            {
                RosetteSentimentEntity other = obj as RosetteSentimentEntity;
                List<bool> conditions = new List<bool>() {
                    this.Sentiment != null && other.Sentiment != null ? this.Sentiment.Equals(other.Sentiment) : this.Sentiment == other.Sentiment,
                    this.EntityEquals(other),
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
            int h0 = this.Count != null ? this.Count.GetHashCode() : 1;
            int h1 = this.EntityType != null ? this.EntityType.GetHashCode() : 1;
            int h2 = this.ID != null ? this.ID.GetHashCode() : 1;
            int h3 = this.Mention != null ? this.Mention.GetHashCode() : 1;
            int h4 = this.NormalizedMention != null ? this.NormalizedMention.GetHashCode() : 1;
            int h5 = this.Sentiment != null ? this.Sentiment.GetHashCode() : 1;
            int h6 = this.Confidence != null ? this.Confidence.GetHashCode() : 1;
            return h0 ^ h1 ^ h2 ^ h3 ^ h4 ^ h5 ^ h6;
        }

        /// <summary>
        /// ToString override.
        /// </summary>
        /// <returns>This RosetteSentimentEntity in JSON form</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}

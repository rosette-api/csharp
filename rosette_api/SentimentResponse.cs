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
    /// A class for representing responses from the sentiment analysis endpoint of the Analytics API
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class SentimentResponse : RosetteResponse, IEquatable<SentimentResponse>
    {
        private const string docKey = "document";
        private const string entitiesKey = "entities";
        internal const string labelKey = "label";
        internal const string confidenceKey = "confidence";
        internal const string mentionKey = "mention";
        internal const string normalizedMentionKey = "normalized";
        internal const string dbpediaTypeKey = "dbpediaType";
        internal const string dbpediaTypesKey = "dbpediaTypes";
        internal const string countKey = "count";
        internal const string typeKey = "type";
        internal const string entityIDKey = "entityId";
        internal const string sentimentKey = "sentiment";
        internal const String mentionOffsetsKey = "mentionOffsets";
        internal const String linkingConfidenceKey = "linkingConfidence";
        internal const String salienceKey = "salience";
        internal const String permIdKey = "permId";

        /// <summary>
        /// Gets or sets the document-level sentiment identified by the Analytics API
        /// </summary>
        [JsonProperty(docKey)]
        public RosetteSentiment DocSentiment { get; set; }
        /// <summary>
        /// Gets or sets the entities identified by the Analytics API with sentiment
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
            double? docSentimentConfidence = docResult.Properties().Where((p) => p.Name == confidenceKey).Any() ? docResult[confidenceKey].ToObject<double?>() : new double?();
            this.DocSentiment = docSentiment != null && docSentimentConfidence != null ? new RosetteSentiment(docSentiment, docSentimentConfidence) : null;
            JArray enumerableResults = this.ContentDictionary.ContainsKey(entitiesKey) ? this.ContentDictionary[entitiesKey] as JArray : new JArray();
            foreach (JObject result in enumerableResults)
            {
                string type = result.Properties().Where((p) => p.Name == typeKey).Any() ? result[typeKey].ToString() : null;
                string mention = result.Properties().Where((p) => p.Name == mentionKey).Any() ? result[mentionKey].ToString() : null;
                string normalizedMention = result.Properties().Where((p) => p.Name == normalizedMentionKey).Any() ? result[normalizedMentionKey].ToString() : null;
                string entityIDStr = result.Properties().Where((p) => p.Name == entityIDKey).Any() ? result[entityIDKey].ToString() : null;
                EntityID entityID = entityIDStr != null ? new EntityID(entityIDStr) : null;
                string dbpediaType = result.Properties().Where((p) => p.Name == dbpediaTypeKey).Any() ? result[dbpediaTypeKey].ToString() : null;
                List<String> dbpediaTypes = result.Properties().Where((permIdKey) => permIdKey.Name == dbpediaTypesKey).Any() ? result[dbpediaTypesKey].ToObject<List<String>>() : null;
                Nullable<int> count = result.Properties().Where((p) => p.Name == countKey).Any() ? result[countKey].ToObject<int?>() : null;
                Nullable<double> confidence = result.Properties().Where((p) => String.Equals(p.Name, confidenceKey)).Any() ? result[confidenceKey].ToObject<double?>() : null;
                JArray mentionOffsetsArr = result.Properties().Where((p) => p.Name == mentionOffsetsKey).Any() ? result[mentionOffsetsKey] as JArray : null;
                List<MentionOffset> mentionOffsets = mentionOffsetsArr != null ? mentionOffsetsArr.ToObject<List<MentionOffset>>() : null;
                Nullable<double> linkingConfidence = result.Properties().Where((p) => p.Name == linkingConfidenceKey).Any() ? result[linkingConfidenceKey].ToObject<double?>() : null;
                Nullable<double> salience = result.Properties().Where((p) => p.Name == salienceKey).Any() ? result[salienceKey].ToObject<double?>() : null;
                String permId = result.Properties().Where((p) => p.Name == permIdKey).Any() ? result[permIdKey].ToString() : null;
                RosetteSentiment sentiment = null;
                if (result.Properties().Where((p) => p.Name == sentimentKey).Any()) {
                    sentiment = result[sentimentKey].ToObject<RosetteSentiment>();
                }
                entitySentiments.Add(new RosetteSentimentEntity(mention, normalizedMention, entityID, type, count, sentiment, confidence, dbpediaType, dbpediaTypes, mentionOffsets, linkingConfidence, salience, permId));
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
        /// <param name="other">The other to compare against</param>
        /// <returns>True if equal</returns>
        public bool Equals(SentimentResponse other)
        {
            List<bool> conditions = new List<bool>() {
                this.DocSentiment != null && other.DocSentiment != null ? this.DocSentiment.Equals(other.DocSentiment) : this.DocSentiment == other.DocSentiment,
                this.EntitySentiments != null && other.EntitySentiments != null ? this.EntitySentiments.SequenceEqual(other.EntitySentiments) : this.EntitySentiments == other.EntitySentiments,
                this.ResponseHeaders != null && other.ResponseHeaders != null ? this.ResponseHeaders.Equals(other.ResponseHeaders) : this.ResponseHeaders == other.ResponseHeaders,
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
            if (obj is SentimentResponse)
            {
                return Equals(obj is SentimentResponse);
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
        public class RosetteSentiment : IEquatable<RosetteSentiment>
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
            public Nullable<double> Confidence;
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
            [JsonConstructor]
            public RosetteSentiment(String sentiment, Nullable<double> confidence)
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
            /// A constructor for creating RosetteSentiments
            /// </summary>
            /// <param name="sentimentLabel">The sentiment label: "pos", "neu", or "neg"</param>
            /// <param name="confidence">An indicator of confidence in the label being correct.  A range from 0-1.</param>
            public RosetteSentiment(SentimentLabel sentimentLabel, Nullable<double> confidence)
            {
                this.Label = sentimentLabel;
                this.Confidence = confidence;
            }
            /// <summary>
            /// Equals override
            /// </summary>
            /// <param name="other">The object to compare against</param>
            /// <returns>True if equal</returns>

            public bool Equals(RosetteSentiment other)
            {
                List<bool> conditions = new List<bool>() {
                    this.Confidence == other.Confidence,
                    this.Label.Equals(other.Label),
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
                if (obj is RosetteSentiment)
                {
                    return Equals(obj as RosetteSentiment);
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
    /// A class to represent an entity returned by the Sentiment Analysis endpoint of the Analytics API
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class RosetteSentimentEntity : RosetteEntity, IEquatable<RosetteSentimentEntity>
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
        /// <param name="dbpediaType">The DBpedia type of the entity</param>
        /// <param name="dbpediaTypes">A list of DBpedia types of the entitiy</param>
        /// <param name="mentionOffsets">The mention offsets of the entity</param>
        /// <param name="linkingConfidence">The linking confidence of the entity</param>
        /// <param name="salience">The salience of the entity</param>
        /// <param name="permId">The Thomson Reuters Permanent Identifier of the entity</param>
        public RosetteSentimentEntity(string mention,
                                      string normalizedMention,
                                      EntityID id,
                                      string entityType,
                                      int? count,
                                      SentimentResponse.RosetteSentiment sentiment,
                                      double? confidence,
                                      string dbpediaType,
                                      List<String> dbpediaTypes,
                                      List<MentionOffset> mentionOffsets,
                                      double? linkingConfidence,
                                      double? salience,
                                      String permId
                                     ) : base(mention,
                                              normalizedMention,
                                              id,
                                              entityType,
                                              count,
                                              confidence,
                                              dbpediaType,
                                              dbpediaTypes,
                                              mentionOffsets,
                                              linkingConfidence,
                                              salience,
                                              permId
                                             )
        {
            this.Sentiment = new SentimentResponse.RosetteSentiment(sentiment.Label, sentiment.Confidence);
        }

        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="other">The other to compare against</param>
        /// <returns>True if equal</returns>
        public bool Equals(RosetteSentimentEntity other)
        {
            if (other == null) {
                return false;
            }
            List<bool> conditions = new List<bool>() {
                this.Sentiment != null ? this.Sentiment.Equals(other.Sentiment) : other.Sentiment == null,
                base.Equals(other)
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
                return Equals(obj as RosetteEntity);
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
            var hashCode = base.GetHashCode();
            hashCode = (hashCode * 397) ^ (Sentiment != null ? Sentiment.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (Confidence != null ? Confidence.GetHashCode() : 0);
            return hashCode;
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

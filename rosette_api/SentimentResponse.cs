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
    /// A class for representing responses from the sentiment analysis endpoint of the Rosette API
    /// </summary>
    public class SentimentResponse : RosetteResponse
    {
        private const string docKey = "document";
        private const string entitiesKey = "entities";
        private const string labelKey = "label";
        private const string confidenceKey = "confidence";
        private const string mentionKey = "mention";
        private const string normalizedMentionKey = "normalizedMention";
        private const string countKey = "count";
        private const string typeKey = "type";
        private const string entityIDKey = "entityId";

        /// <summary>
        /// Gets or sets the document-level sentiment identified by the Rosette API
        /// </summary>
        public RosetteSentiment DocSentiment { get; set; }
        /// <summary>
        /// Gets or sets the entities identified by the Rosette API with sentiment
        /// </summary>
        public List<RosetteSentimentEntity> EntitySentiments { get; set; }

        /// <summary>
        /// Gets the response headers returned from the API
        /// </summary>
        public ResponseHeaders ResponseHeaders { get; private set; }

        /// <summary>
        /// Creates a SentimentResponse from the given apiResult
        /// </summary>
        /// <param name="apiResult">The message from the API</param>
        public SentimentResponse(HttpResponseMessage apiResult)
            : base(apiResult)
        {
            List<RosetteSentimentEntity> entitySentiments = new List<RosetteSentimentEntity>();
            Dictionary<string, object> docResult = this.Content.ContainsKey(docKey) ? this.Content[docKey] as Dictionary<string, object> : new Dictionary<string, object>();
            if (docResult.ContainsKey(labelKey) && docResult.ContainsKey(confidenceKey))
            {
                this.DocSentiment = new RosetteSentiment(docResult[labelKey] as String, docResult[confidenceKey] as Nullable<double>);
            }
            IEnumerable<Object> enumerableResults = this.Content.ContainsKey(entitiesKey) ? this.Content[entitiesKey] as IEnumerable<Object> : new List<object>();
            foreach (Object result in enumerableResults)
            {
                Dictionary<string, object> dictResult = result as Dictionary<string, object>;
                String type = dictResult.ContainsKey(typeKey) ? (dictResult[typeKey] as String) : null;
                String mention = dictResult.ContainsKey(mentionKey) ? dictResult[mentionKey] as String : null;
                String normalizedMention = dictResult.ContainsKey(normalizedMentionKey) ? dictResult[normalizedMentionKey] as String : null;
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
                Nullable<int> count = dictResult.ContainsKey(countKey) ? dictResult[countKey] as Nullable<int> : null;
                String sentiment = dictResult.ContainsKey(labelKey) ? dictResult[labelKey] as String : null;
                Nullable<double> confidence = dictResult.ContainsKey(confidenceKey) ? dictResult[confidenceKey] as Nullable<double> : null;
                entitySentiments.Add(new RosetteSentimentEntity(mention, normalizedMention, entityID, type, count, sentiment, confidence));
            }
            this.EntitySentiments = entitySentiments;
            this.ResponseHeaders = new ResponseHeaders(this.Headers);
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
                    this.DocSentiment.Equals(other.DocSentiment),
                    this.EntitySentiments.SequenceEqual(other.EntitySentiments),
                    this.ResponseHeaders.Equals(other.ResponseHeaders)
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
            return this.ResponseHeaders.GetHashCode() ^ this.EntitySentiments.GetHashCode() ^ this.DocSentiment.GetHashCode();
        }

        /// <summary>
        /// A class for representing sentiments
        /// </summary>
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
                pos,
                /// <summary>
                /// The sentiment is neutral
                /// </summary>
                /// 
                neu,
                /// <summary>
                /// The sentiment is negative
                /// </summary>
                neg
            };
            /// <summary>
            /// On a scale of 0-1, the confidence in the Label's correctness.
            /// </summary>
            public Nullable<double> Confidence;
            /// <summary>
            /// The label indicating the sentiment
            /// </summary>
            public SentimentLabel Label;

            /// <summary>
            /// A constructor for creating RosetteSentiments
            /// </summary>
            /// <param name="sentiment">The sentiment label: "pos", "neu", or "neg"</param>
            /// <param name="confidence">An indicator of confidence in the label being correct.  A range from 0-1.</param>
            public RosetteSentiment(String sentiment, Nullable<double> confidence)
            {
                switch (sentiment)
                {
                    case "pos": this.Label = SentimentLabel.pos; break;
                    case "neu": this.Label = SentimentLabel.neu; break;
                    case "neg": this.Label = SentimentLabel.neg; break;
                    default: throw new ArgumentException("The sentiment label returned by the API has been changed.  The binding needs to be updated.", "sentiment");
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
                        this.Label == other.Label
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
                return this.Confidence.GetHashCode() ^ this.Label.GetHashCode();
            }
        }
    }

    /// <summary>
    /// A class to represent an entity returned by the Sentiment Analysis endpoint of the Rosette API
    /// </summary>
    public class RosetteSentimentEntity : RosetteEntity
    {
        /// <summary>
        /// The sentiment label of this entity
        /// </summary>
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
            return this.Mention == other.Mention &&
                this.NormalizedMention == other.NormalizedMention &&
                this.ID == other.ID &&
                this.EntityType == other.EntityType &&
                this.Count == other.Count;
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
                    this.Sentiment.Equals(other.Sentiment),
                    this.NormalizedMention.Equals(other.NormalizedMention),
                    this.Mention.Equals(other.Mention),
                    this.ID.Equals(other.ID),
                    this.EntityType.Equals(other.EntityType),
                    this.Count.Equals(other.Count)
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
            return this.Count.GetHashCode() ^ this.EntityType.GetHashCode() ^ this.ID.GetHashCode() ^ this.Mention.GetHashCode() ^ this.NormalizedMention.GetHashCode() ^ this.Sentiment.GetHashCode();
        }
    }
}

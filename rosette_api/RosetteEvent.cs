using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using static System.Collections.Specialized.BitVector32;

namespace rosette_api
{
    /// <summary>
    /// A class for representing an identified event and its properties
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class RosetteEvent : IEquatable<RosetteEvent>
    {

        /// <summary>
        /// Gets or sets the event type of the extracted event
        /// </summary>
        [JsonProperty("eventType")]
        public String EventType { get; set; }

        /// <summary>
        /// Gets or sets the list of mentions of the extracted event
        /// </summary>
        [JsonProperty("mentions")]
        public List<EventMention> Mentions { get; set; }

        /// <summary>
        /// Gets or sets the confidence of the extracted event
        /// </summary>
        [JsonProperty("confidence")]
        public Nullable<double> Confidence { get; set; }

        /// <summary>
        /// Gets or sets the workspace ID of the extracted event
        /// </summary>
        [JsonProperty("workspaceId")]
        public String workspaceId { get; set; }

        /// <summary>
        /// Creates an event
        /// </summary>
        /// <param name="eventType">The description of the event type</param>
        /// <param name="mentions">The list of event mentions</param>
        /// <param name="id">The confidence this was a correctly labeled event</param>
        /// <param name="workspaceId">The specific workspace ID that was used for analysis</param>
        public RosetteEvent(String eventType, List<EventMention> mentions, Nullable<double> confidence, string workspaceId)
        {
            this.EventType = eventType;
            this.Mentions = mentions;
            this.Confidence = confidence;
            this.workspaceId = workspaceId;
        }

        /// <summary>
        /// Equals for RosetteEvent
        /// </summary>
        /// <param name="other">RosetteEvent</param>
        /// <returns></returns>
        public bool Equals(RosetteEvent other)
        {
            return string.Equals(EventType, other.EventType)
                && Mentions.SequenceEqual(other.Mentions)
                && Confidence.Equals(other.Confidence)
                && string.Equals(workspaceId, other.workspaceId);
        }

        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj">The object to compare against</param>
        /// <returns>True if equal</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals(obj as RosetteEvent);
        }

        /// <summary>
        /// HashCode override
        /// </summary>
        /// <returns>The hashcode</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (EventType != null ? EventType.GetHashCode() : 0);
                hashCode = (hashCode * 401) ^ (Mentions != null ? Mentions.GetHashCode() : 0);
                hashCode = (hashCode * 401) ^ (Confidence != null ? Confidence.GetHashCode() : 0);
                hashCode = (hashCode * 401) ^ (workspaceId != null ? workspaceId.GetHashCode() : 0);
                return hashCode;
            }
        }

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns>This RosetteEvent in JSON form</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}


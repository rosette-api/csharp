using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

namespace rosette_api
{   /// <summary>
    /// A class for representing an identified mention of an event
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class EventMention : MentionOffset
    {

        /// <summary>
        /// Gets or sets the role type of the extracted event mention
        /// </summary>
        [JsonProperty("roles")]
        public List<EventRole> Roles { get; set; }

        /// <summary>
        /// Gets or sets the polarity of the extracted event mention
        /// </summary>
        [JsonProperty("polarity")]
        public String Polarity { get; set; }

        /// <summary>
        /// Gets or sets the confidence of the extracted event mention
        /// </summary>
        [JsonProperty("confidence")]
        public Nullable<double> Confidence { get; set; }

        /// <summary>
        /// Gets or sets the list of negation cues of the extracted event mention
        /// </summary>
        [JsonProperty("negationCues")]
        public List<NegationCue> NegationCues { get; set; }

        /// <summary>
        /// Creates an event mention
        /// </summary>
        /// <param name="roles">The list of detected roles in the event mention type</param>
        /// <param name="polarity">The polarity of the event mention</param>
        /// <param name="confidence">The confidence this was a correctly labeled event mention</param>
        /// <param name="negationCues">The list of detected negation cues in the event mention type</param>
        public EventMention(List<EventRole> roles, String polarity, double? confidence, List<NegationCue> negationCues, int startOffset, int endOffset) : base(startOffset, endOffset)
        {
            this.Roles = roles;
            this.Polarity = polarity;
            this.Confidence = confidence;
            this.NegationCues = negationCues;
        }

        /// <summary>
        /// Equals for EventMention
        /// </summary>
        /// <param name="other">Other EventMention</param>
        /// <returns>bool</returns>
        public bool Equals(EventMention other)
        {
            return Roles.SequenceEqual(other.Roles)
                && string.Equals(Polarity, other.Polarity)
                && Confidence.Equals(other.Confidence)
                && NegationCues.SequenceEqual(other.NegationCues);
        }

        /// <summary>
        /// Override for Equals
        /// </summary>
        /// <param name="obj">The object to compare against</param>
        /// <returns>bool</returns>
        public override bool Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals(obj as EventMention);
        }

        /// <summary>
        /// Override for GetHashCode specific to EventMention
        /// </summary>
        /// <returns>int</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Roles != null ? Roles.GetHashCode() : 0);
                hashCode = (hashCode * 401) ^ (Polarity != null ? Polarity.GetHashCode() : 0);
                hashCode = (hashCode * 401) ^ (Confidence != null ? Confidence.GetHashCode() : 0);
                hashCode = (hashCode * 401) ^ (NegationCues != null ? NegationCues.GetHashCode() : 0);
                return hashCode;
            }
        }

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns>This EventMention in JSON form</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    /// <summary>
    /// A class for representing an event role 
    /// </summary>
    public class EventRole : MentionOffset
    {
        /// <summary>
        /// Gets or sets the name of the event role
        /// </summary>
        [JsonProperty("name")]
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the ID of the event role
        /// </summary>
        [JsonProperty("id")]
        public String Id { get; set; }

        /// <summary>
        /// Gets or sets the dataspan of the event role
        /// </summary>
        [JsonProperty("dataSpan")]
        public String DataSpan { get; set; }

        /// <summary>
        /// Gets or sets the confidence of event role
        /// </summary>
        [JsonProperty("confidence")]
        public Nullable<double> Confidence { get; set; }

        /// <summary>
        /// Gets or sets the role type of the event role
        /// </summary>
        [JsonProperty("roleType")]
        public String RoleType { get; set; }

        /// <summary>
        /// Gets or sets the extractor name of the event role
        /// </summary>
        [JsonProperty("extractorName")]
        public String ExtractorName { get; set; }

        /// <summary>
        /// Creates an event role
        /// </summary>
        /// <param name="name">The name of the event role</param>
        /// <param name="id">The event role's id</param>
        /// <param name="dataSpan">The event role's dataspan</param>
        /// <param name="confidence">The confidence this was a correctly labeled event role</param>
        /// <param name="roleType">The event role's type</param>
        /// <param name="extractorName">The name of the event role's extractor</param>
        public EventRole(String name, String id, String dataSpan, double? confidence, string roleType, string extractorName, int startOffset, int endOffset) : base(startOffset, endOffset)
        {
            this.Name = name;
            this.Id = id;
            this.DataSpan = dataSpan;
            this.Confidence = confidence;
            this.RoleType = roleType;
            this.ExtractorName = extractorName;
        }

        /// <summary>
        /// Equals for EventRole
        /// </summary>
        /// <param name="other">Other EventRole</param>
        /// <returns>bool</returns>
        public bool Equals(EventRole other)
        {
            return string.Equals(Name, other.Name)
                && string.Equals(Id, other.Id)
                && string.Equals(DataSpan, other.DataSpan)
                && Confidence.Equals(other.Confidence)
                && string.Equals(RoleType, other.RoleType)
                && string.Equals(ExtractorName, other.ExtractorName);
        }

        /// <summary>
        /// Override for Equals
        /// </summary>
        /// <param name="obj">The object to compare against</param>
        /// <returns>bool</returns>
        public override bool Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals(obj as EventRole);
        }

        /// <summary>
        /// Override for GetHashCode specific to EventRole
        /// </summary>
        /// <returns>int</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 401) ^ (Id != null ? Id.GetHashCode() : 0);
                hashCode = (hashCode * 401) ^ (DataSpan != null ? DataSpan.GetHashCode() : 0);
                hashCode = (hashCode * 401) ^ (Confidence != null ? Confidence.GetHashCode() : 0);
                hashCode = (hashCode * 401) ^ (RoleType != null ? RoleType.GetHashCode() : 0);
                hashCode = (hashCode * 401) ^ (ExtractorName != null ? ExtractorName.GetHashCode() : 0);
                return hashCode;
            }
        }

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns>This EventRole in JSON form</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    /// <summary>
    /// A class for representing a negation cue
    /// </summary>
    public class NegationCue : MentionOffset
    {
        /// <summary>
        /// Gets or sets the dataspan of the negation cue
        /// </summary>
        [JsonProperty("dataSpan")]
        public String DataSpan { get; set; }

        /// <summary>
        /// Creates a negation cue
        /// </summary>
        /// <param name="dataSpan">The negation cue's dataspan</param>
        public NegationCue(String dataSpan, int startOffset, int endOffset) : base(startOffset, endOffset)
        {
            this.DataSpan = dataSpan;
        }

        /// <summary>
        /// Equals for NegationCue
        /// </summary>
        /// <param name="other">Other NegationCue</param>
        /// <returns>bool</returns>
        public bool Equals(NegationCue other)
        {
            return string.Equals(DataSpan, other.DataSpan);
        }

        /// <summary>
        /// Override for Equals
        /// </summary>
        /// <param name="obj">The object to compare against</param>
        /// <returns>bool</returns>
        public override bool Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals(obj as NegationCue);
        }

        /// <summary>
        /// Override for GetHashCode specific to NegationCue
        /// </summary>
        /// <returns>int</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (DataSpan != null ? DataSpan.GetHashCode() : 0);
                return hashCode;
            }
        }

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns>This NegationCue in JSON form</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}


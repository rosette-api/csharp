using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace rosette_api
{
    [JsonObject(MemberSerialization.OptOut)]
    public class EventMention : MentionOffset
    {

        /// <summary>
        /// Gets or sets the entity's type
        /// </summary>
        [JsonProperty("roles")]
        public List<EventRole> Roles { get; set; }

        /// <summary>
        /// Gets or sets the number of times this entity appeared in the input to the API
        /// </summary>
        [JsonProperty("polarity")]
        public String Polarity { get; set; }

        /// <summary>
        /// Gets or sets the confidence of the extracted entity
        /// </summary>
        [JsonProperty("confidence")]
        public Nullable<double> Confidence { get; set; }

        /// <summary>
        /// Gets or sets the confidence of the extracted entity
        /// </summary>
        [JsonProperty("negationCues")]
        public List<NegationCue> NegationCues { get; set; }

        public EventMention(List<EventRole> roles, String polarity, double? confidence, List<NegationCue> negationCues, int startOffset, int endOffset) : base (startOffset, endOffset)
		{
            this.Roles = roles;
            this.Polarity = polarity;
            this.Confidence = confidence;
            this.NegationCues = negationCues;
        }
    }

    public class EventRole : MentionOffset
    {
        /// <summary>
        /// Gets or sets the entity's type
        /// </summary>
        [JsonProperty("name")]
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the number of times this entity appeared in the input to the API
        /// </summary>
        [JsonProperty("id")]
        public String Id { get; set; }

        /// <summary>
        /// Gets or sets the confidence of the extracted entity
        /// </summary>
        [JsonProperty("dataSpan")]
        public String DataSpan { get; set; }

        /// <summary>
        /// Gets or sets the confidence of the extracted entity
        /// </summary>
        [JsonProperty("confidence")]
        public Nullable<double> Confidence { get; set; }

        public EventRole(String name, String id, String dataSpan, double? confidence, int startOffset, int endOffset) : base(startOffset, endOffset)
        {
            this.Name = name;
            this.Id = id;
            this.DataSpan = dataSpan;
            this.Confidence = confidence;
        }

    }

    public class NegationCue : MentionOffset
    {
        /// <summary>
        /// Gets or sets the confidence of the extracted entity
        /// </summary>
        [JsonProperty("dataSpan")]
        public String DataSpan { get; set; }

        public NegationCue(String name, String id, String dataSpan, double? confidence, int startOffset, int endOffset) : base(startOffset, endOffset)
        {
            this.DataSpan = dataSpan;
        }
    }
}


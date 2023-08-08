using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace rosette_api
{   //TODO HASHCODE AND EQUALS
    //TODO FIX COMMENTS
    [JsonObject(MemberSerialization.OptOut)]
    public class EventMention : MentionOffset
    {

        /// <summary>
        /// Gets or sets 
        /// </summary>
        [JsonProperty("roles")]
        public List<EventRole> Roles { get; set; }

        /// <summary>
        /// Gets or sets 
        /// </summary>
        [JsonProperty("polarity")]
        public String Polarity { get; set; }

        /// <summary>
        /// Gets or sets
        /// </summary>
        [JsonProperty("confidence")]
        public Nullable<double> Confidence { get; set; }

        /// <summary>
        /// Gets or sets the list of negation cues
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
    //TODO HASHCODE AND EQUALS
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

        public EventRole(String name, String id, String dataSpan, double? confidence, int startOffset, int endOffset) : base(startOffset, endOffset)
        {
            this.Name = name;
            this.Id = id;
            this.DataSpan = dataSpan;
            this.Confidence = confidence;
        }

    }
    //TODO HASHCODE AND EQUALS
    public class NegationCue : MentionOffset
    {
        /// <summary>
        /// Gets or sets the dataspan of the negation cue
        /// </summary>
        [JsonProperty("dataSpan")]
        public String DataSpan { get; set; }

        public NegationCue(String dataSpan, int startOffset, int endOffset) : base(startOffset, endOffset)
        {
            this.DataSpan = dataSpan;
        }
    }
}


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
        public List<NegationCues> NegationCues { get; set; }

        public EventMention(int startOffsett, int endOffsett)
		{

		}
	}

    public class NegationCues
    {
    }

    public class EventRole
    {
    }
}


using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace rosette_api
{
    [JsonObject(MemberSerialization.OptOut)]
    public class RosetteEvent : IEquatable<RosetteEvent>
    {

        /// <summary>
        /// Gets or sets the 
        /// </summary>
        [JsonProperty("eventType")]
        public String EventType { get; set; }

        /// <summary>
        /// Gets or sets the
        /// </summary>
        [JsonProperty("normalized")]
        public List<String> NormalizedMention { get; set; }

        /// <summary>
        /// Gets or sets 
        /// </summary>
        [JsonIgnore]
        public EntityID ID { get; set; }
        [JsonProperty("entityId")]
        private string EntityID { get { return ID.ToString(); } }



        public RosetteEvent()
		{
		}
	}
}


using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace rosette_api
{
    /// <summary>
    /// A class for representing responses from the API when the Events (/events) endpoint has been called
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class EventsResponse : RosetteResponse
	{
        /// <summary>
        /// Gets or sets the collection of identified events
        /// </summary>
        [JsonProperty(PropertyName = eventsKey)]
        public List<RosetteEvent> Events { get; set; }

        private const String eventTypeKey = "eventType";
        private const String mentionsKey = "mentions";
        private const String confidenceKey = "confidence";
        private const String workspaceIdKey = "workspaceId";
        private const String eventsKey = "events";

        /// <summary>
        /// Creates an EntitiesResponse from the API's raw output
        /// </summary>
        /// <param name="apiResult">The API's raw output</param>
        public EventsResponse(HttpResponseMessage apiResult)
            : base(apiResult)
        {
            List<RosetteEvent> events = new List<RosetteEvent>();
            JArray enumerableResults = this.ContentDictionary.ContainsKey(eventsKey) ? this.ContentDictionary[eventsKey] as JArray : new JArray();
            foreach (JObject result in enumerableResults)
            {
                String eventType = result.Properties().Where<JProperty>((p) => p.Name == eventTypeKey).Any() ? result[eventTypeKey].ToString() : null;
                JArray mentionsArr = result.Properties().Where((p) => String.Equals(p.Name, mentionsKey, StringComparison.OrdinalIgnoreCase)).Any() ? result[mentionsKey] as JArray : null;
                List<EventMention> mentions = mentionsArr != null ? mentionsArr.ToObject<List<EventMention>>() : null;
                Nullable<double> confidence = result.Properties().Where((p) => String.Equals(p.Name, confidenceKey)).Any() ? result[confidenceKey].ToObject<double?>() : null;
                String workspaceId = result.Properties().Where<JProperty>((p) => p.Name == workspaceIdKey).Any() ? result[workspaceIdKey].ToString() : null;
                events.Add(new RosetteEvent(eventType, mentions, confidence, workspaceId));
            }
            this.Events = events;
        }
        
        /// <summary>
        /// Constructs a Event Response from a list of RosetteEvents, a collection of response headers, and the response content in dictionary/JSON form
        /// </summary>
        /// <param name="events">The list of events/param>
        /// <param name="responseHeaders">The response headers from the API</param>
        /// <param name="content">The content of the response (i.e. the event list)</param>
        /// <param name="contentAsJson">The content as a JSON string</param>
        public EventsResponse(List<RosetteEvent> events, Dictionary<string, string> responseHeaders, Dictionary<string, object> content, String contentAsJson)
            : base(responseHeaders, content, contentAsJson)
        {
            this.Events = events;
        }

        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj">The object to compare against</param>
        /// <returns>True if equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is EventsResponse)
            {
                EventsResponse other = obj as EventsResponse;
                List<bool> conditions = new List<bool>() {
                    this.Events != null && other.Events != null ? this.Events.SequenceEqual(other.Events) : ReferenceEquals(this.Events, other.Events),
                    this.ResponseHeaders != null && other.ResponseHeaders != null ? this.ResponseHeaders.Equals(other.ResponseHeaders) : this.ResponseHeaders == other.ResponseHeaders,
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
            int h0 = this.Events != null ? this.Events.Aggregate<RosetteEvent, int>(1, (seed, item) => seed ^ item.GetHashCode()) : 1;
            int h1 = this.ResponseHeaders != null ? this.ResponseHeaders.GetHashCode() : 1;
            return h0 ^ h1;
        }


    }
}


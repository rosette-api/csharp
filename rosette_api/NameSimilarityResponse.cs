using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net.Http;

namespace rosette_api
{
    /// <summary>
    /// A class to represent the results from the Name Similarity endpoint of the Analytics API
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class NameSimilarityResponse : RosetteResponse
    {
        private const string scoreKey = "score";

        /// <summary>
        /// The score, on a range of 0-1, of how closely the names match
        /// </summary>
        [JsonProperty(scoreKey)]
        public Nullable<double> Score;

        /// <summary>
        /// Creates a NameSimilarityResponse from the given apiResults
        /// </summary>
        /// <param name="apiResults">The message from the API</param>
        public NameSimilarityResponse(HttpResponseMessage apiResults) :base(apiResults)
        {
            if (this.ContentDictionary.ContainsKey(scoreKey))
            {
                this.Score = this.ContentDictionary[scoreKey] as double?;
            }
        }

        /// <summary>
        /// Creates a NameSimilarityResponse from its headers
        /// </summary>
        /// <param name="score">The name simiarity score: 0-1</param>
        /// <param name="responseHeaders">The response headers from the API</param>
        /// <param name="content">The content of the response (the score) in dictionary form</param>
        /// <param name="contentAsJSON">The content in JSON</param>
        public NameSimilarityResponse(double? score, Dictionary<string, string> responseHeaders, Dictionary<string, object> content, string contentAsJSON) : base(responseHeaders, content, contentAsJSON)
        {
            this.Score = score;
        }

        /// <summary>
        /// Equals override.
        /// </summary>
        /// <param name="obj">The object to compare against</param>
        /// <returns>True if equal.</returns>
        public override bool Equals(object obj)
        {
            if (obj is NameSimilarityResponse)
            {
                NameSimilarityResponse other = obj as NameSimilarityResponse;
                return this.Score == other.Score && this.ResponseHeaders.Equals(other.ResponseHeaders);
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
            return this.Score.GetHashCode() ^ this.ResponseHeaders.GetHashCode();
        }
    }
}

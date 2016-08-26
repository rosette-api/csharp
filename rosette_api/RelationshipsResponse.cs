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
    /// A class to represent responses from the Relationships endpoint of the Rosette API
    /// </summary>
    public class RelationshipsResponse : RosetteResponse
    {
        private const string relationshipsKey = "relationships";
        private const string predicateKey = "predicate";
        private const string argsKey = "arg";
        private const string temporalsKey = "temporals";
        private const string locativesKey = "locatives";
        private const string adjunctsKey = "adjuncts";
        private const string confidenceKey = "confidence";

        /// <summary>
        /// Gets or sets the relationships extracted by the Rosette API
        /// </summary>
        public List<RosetteRelationship> Relationships { get; set; }

        /// <summary>
        /// Gets the response headers returned from the API
        /// </summary>
        public ResponseHeaders ResponseHeaders { get; private set; }

        /// <summary>
        /// Creates a RelationshipsResponse from the given apiResult
        /// </summary>
        /// <param name="apiResult">The message from the API</param>
        public RelationshipsResponse(HttpResponseMessage apiResult) :base(apiResult)
        {
            Dictionary<string, object>[] relationshipResults = this.Content.ContainsKey(relationshipsKey) ? this.Content[relationshipsKey] as Dictionary<string, object>[] : new Dictionary<string, object>[0];
            List<RosetteRelationship> relationships = new List<RosetteRelationship>();
            foreach (Dictionary<string, object> relationship in relationshipResults)
            {
                String predicate = relationship.ContainsKey(predicateKey) ? relationship[predicateKey] as String : null;
                List<String> arguments = new List<string>();
                if (relationship.Any(kvp => kvp.Key.Contains(argsKey))) { 
                    arguments.AddRange(relationship.Where(kvp => kvp.Key.Contains(argsKey)).Select(kvp => kvp.Value as String)); 
                }
                List<string> temporals = new List<string>();
                if (relationship.ContainsKey(temporalsKey))
                {
                    temporals.AddRange(relationship[temporalsKey] as String[]);
                }
                List<string> locatives = new List<string>();
                if (relationship.ContainsKey(locativesKey))
                {
                    locatives.AddRange(relationship[locativesKey] as String[]);
                }
                List<string> adjuncts = new List<string>();
                if (relationship.ContainsKey(adjunctsKey))
                {
                    adjuncts.AddRange(relationship[adjunctsKey] as String[]);
                }
                Nullable<double> confidence = relationship.ContainsKey(confidenceKey) ?  relationship[confidenceKey] as Nullable<double> : new Nullable<double>();
                relationships.Add(new RosetteRelationship(predicate, arguments, temporals, locatives, adjuncts, confidence));
            }
            this.Relationships = relationships;
            this.ResponseHeaders = new ResponseHeaders(this.Headers);
        }

        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj">The object to compare against</param>
        /// <returns>True if equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is RelationshipsResponse)
            {
                RelationshipsResponse other = obj as RelationshipsResponse;
                List<bool> conditions = new List<bool>() {
                    this.Relationships.Count == other.Relationships.Count &! this.Relationships.Except(other.Relationships).Any(),
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
        /// HashCode override
        /// </summary>
        /// <returns>The hashcode</returns>
        public override int GetHashCode()
        {
            return this.Relationships.GetHashCode() ^ this.ResponseHeaders.GetHashCode();
        }
    }

    /// <summary>
    /// A class to represent a relationship as identified by the Rosette API
    /// </summary>
    public class RosetteRelationship
    {
        /// <summary>
        /// Gets or sets the main action or verb acting on the first argument, or the action connecting multiple arguments.
        /// </summary>
        public String Predicate { get; set; }

        /// <summary>
        /// Gets or sets the or more subjects in the relationship
        /// </summary>
        public List<String> Arguments { get; set; }

        /// <summary>
        /// Gets or sets the time frame of the action.  May be empty.
        /// </summary>
        public List<String> Temporals { get; set; }

        /// <summary>
        /// Gets or sets the location(s) of the action.  May be empty.
        /// </summary>
        public List<String> Locatives { get; set; }

        /// <summary>
        /// Gets or sets all other parts of the relationship.  May be empty.
        /// </summary>
        public List<String> Adjucts { get; set; }

        /// <summary>
        /// Gets a score for each relationship result, ranging from 0 to 1. You can use this measurement as a threshold to filter out undesired results.
        /// </summary>
        public Nullable<double> Confidence { get; private set; }

        /// <summary>
        /// Creates a grammatical relationship
        /// </summary>
        /// <param name="predicate">The main action or verb acting on the first argument, or the action connecting multiple arguments.</param>
        /// <param name="arguments">One or more subjects in the relationship</param>
        /// <param name="temporals">Time frames of the action.  May be empty.</param>
        /// <param name="locatives">Locations of the action.  May be empty.</param>
        /// <param name="adjunts">All other parts of the relationship.  May be empty.</param>
        /// <param name="confidence">A score for each relationship result, ranging from 0 to 1. You can use this measurement as a threshold to filter out undesired results.</param>
        public RosetteRelationship(String predicate, List<String> arguments, List<string> temporals, List<string> locatives, List<string> adjunts, Nullable<double> confidence) {
            this.Predicate = predicate;
            this.Arguments = arguments;
            this.Temporals = temporals;
            this.Locatives = locatives;
            this.Adjucts = adjunts;
            this.Confidence = confidence;
        }

        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj">The object to compare against</param>
        /// <returns>True if equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is RosetteRelationship) {
                RosetteRelationship other = obj as RosetteRelationship;
                List<bool> conditions = new List<bool>() {
                    this.Adjucts.SequenceEqual(other.Adjucts),
                    this.Arguments.SequenceEqual(other.Arguments),
                    this.Confidence == other.Confidence,
                    this.Locatives.SequenceEqual(other.Locatives),
                    this.Predicate == other.Predicate,
                    this.Temporals.SequenceEqual(other.Temporals)
                };
                return conditions.All(condition => condition);
            } else {
                return false;
            }
        }

        /// <summary>
        /// HashCode override
        /// </summary>
        /// <returns>The HashCode</returns>
        public override int GetHashCode()
        {
            return this.Temporals.GetHashCode() ^ this.Predicate.GetHashCode() ^ this.Locatives.GetHashCode() ^ this.Confidence.GetHashCode() ^ this.Arguments.GetHashCode() ^ this.Adjucts.GetHashCode();
        }
    }
}

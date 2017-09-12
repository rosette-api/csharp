using System;
using Newtonsoft.Json;

namespace rosette_api {
    /// <summary>
    /// Concepts found in a document
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class Concept : IEquatable<Concept> {
        private string phrase;
        private double? salience;
        private string conceptId;

        /// <summary>
        /// ctor of Concept
        /// </summary>
        /// <param name="phrase">phrase text of the concept</param>
        /// <param name="salience">concept salience</param>
        /// <param name="conceptId">ID of the concept</param>
        public Concept(string phrase, double? salience, string conceptId) {
            Phrase = phrase;
            Salience = salience;
            ConceptId = conceptId;
        }

        /// <summary>
        /// Concept phrase
        /// </summary>
        [JsonProperty("phrase")]
        public string Phrase {
            get { return phrase; }
            private set { phrase = value; }
        }

        /// <summary>
        /// Concept salience
        /// </summary>
        [JsonProperty("salience")]
        public double? Salience {
            get { return salience; }
            private set { salience = value; }
        }

        /// <summary>
        /// ID of concept
        /// </summary>
        [JsonProperty("conceptId")]
        public string ConceptId {
            get { return conceptId; }
            private set { conceptId = value; }
        }

        /// <summary>
        /// Equals for Concepts
        /// </summary>
        /// <param name="other">Other concept to check for equality</param>
        /// <returns>bool</returns>
        public bool Equals(Concept other) {
            if (other == null) {
                return false;
            }

            return (phrase == other.Phrase && (salience == null ? true : salience == other.Salience) && conceptId == other.ConceptId);
        }

        /// <summary>
        /// Override Equals
        /// </summary>
        /// <param name="obj">Object to check for equality</param>
        /// <returns>bool</returns>
        public override bool Equals(Object obj) {
            if (obj == null) {
                return false;
            }

            Concept conceptObj = obj as Concept;
            if (conceptObj == null) {
                return false;
            }
            else {
                return Equals(conceptObj);
            }
        }

        /// <summary>
        /// Override GetHashCode specific to Concept
        /// </summary>
        /// <returns>int</returns>
        public override int GetHashCode() {
            return phrase.GetHashCode() ^ (salience == null ? 1 : salience.GetHashCode()) ^ conceptId.GetHashCode();
        }
        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns>This RosetteEntity in JSON form</returns>
        public override string ToString() {
            return JsonConvert.SerializeObject(this);
        }
    }
}

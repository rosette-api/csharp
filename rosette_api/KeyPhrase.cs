using System;
using Newtonsoft.Json;

namespace rosette_api {
    /// <summary>
    /// Key phrases found in document
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class KeyPhrase : IEquatable<KeyPhrase> {
        private string phrase;
        private double? salience;

        /// <summary>
        /// ctor of KeyPhrase
        /// </summary>
        /// <param name="phrase">phrase text of the concept</param>
        /// <param name="salience">concept salience</param>
        public KeyPhrase(string phrase, double? salience) {
            Phrase = phrase;
            Salience = salience;
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
        /// Equality
        /// </summary>
        /// <param name="other">KeyPhrase to compare for equality</param>
        /// <returns>bool</returns>
        public bool Equals(KeyPhrase other) {
            if (other == null) {
                return false;
            }

            return (phrase == other.Phrase && salience == other.Salience);
        }

        /// <summary>
        /// Override for Equals
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
        /// Override for GetHashCode specific to KeyPhrase
        /// </summary>
        /// <returns>bool</returns>
        public override int GetHashCode() {
            return phrase.GetHashCode() ^ (salience == null ? 1 : salience.GetHashCode());
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

using System;
using Newtonsoft.Json;

namespace rosette_api {
    /// <summary>
    /// Offsets of a piece of text in the document
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class MentionOffset : IEquatable<MentionOffset> {

		/// <summary>
		/// ctor of MentionOffset
		/// </summary>
		/// <param name="startOffset">offset of the start of text</param>
		/// <param name="endOffset">offset of the end of text</param>
		public MentionOffset(int startOffset, int endOffset) {
            StartOffset = startOffset;
            EndOffset = endOffset;
        }

        /// <summary>
        /// Start offset
        /// </summary>
        [JsonProperty("startOffset")]
        public int StartOffset { get; set; }

        /// <summary>
        /// End offset
        /// </summary>
        [JsonProperty("endOffset")]
        public int EndOffset { get; set; }

        /// <summary>
        /// Equality
        /// </summary>
        /// <param name="other">Offset to compare for equality</param>
        /// <returns>bool</returns>
        public bool Equals(MentionOffset other) {
			if (other == null) {
			    return false;
			}
            return StartOffset.Equals(other.StartOffset) && EndOffset.Equals(other.EndOffset);
        }

        /// <summary>
        /// Override for Equals
        /// </summary>
        /// <param name="obj">Object to check for equality</param>
        /// <returns>bool</returns>
        public override bool Equals(Object obj) {
            if (Object.ReferenceEquals(obj, null)) {
                return false;
            }

            MentionOffset mentionOffsetObj = obj as MentionOffset;
            if (mentionOffsetObj == null) {
                return false;
            } else {
                return Equals(mentionOffsetObj);
            }
        }

        /// <summary>
        /// Override for GetHashCode specific to MentionOffset
        /// </summary>
        /// <returns>int</returns>
        public override int GetHashCode() {
            return StartOffset.GetHashCode() ^ EndOffset.GetHashCode();
        }

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns>This MentionOffset in JSON form</returns>
        public override string ToString() {
            return JsonConvert.SerializeObject(this);
        }
    }
}

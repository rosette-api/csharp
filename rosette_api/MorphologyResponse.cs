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
    /// A class for representing responses from the Morphology endpoint of the API
    /// </summary>
    public class MorphologyResponse : RosetteResponse
    {
        private const string tokenKey = "tokens";
        private const string hanReadingsKey = "hanReadings";
        private const string compoundComponentsKey = "compoundComponents";
        private const string posTagsKey = "posTags";
        private const string lemmasKey = "lemmas";
        private const string responseHeadersKey = "responseHeaders";

        /// <summary>
        /// Gets or sets the output of the Rosette API's morphology endpoint, grouped by tokens in order of appearance
        /// </summary>
        public List<MorphologyItem> Items { get; set; }

        /// <summary>
        /// Gets the response headers returned from the API
        /// </summary>
        public ResponseHeaders ResponseHeaders { get; private set; }

        /// <summary>
        /// Creates a MorphologyResponse from the given apiResponse
        /// </summary>
        /// <param name="apiResponse">The message from the API</param>
        public MorphologyResponse(HttpResponseMessage apiResponse) : base(apiResponse)
        {
            List<string> tokens = this.Content.ContainsKey(tokenKey) ? this.Content[tokenKey] as List<string>: new List<String>();
            int tokenCount = tokens.Count();
            string[] lemmas = this.Content.ContainsKey(lemmasKey) ? this.Content[lemmasKey] as string[] : new String[tokenCount];
            string[] posTags = this.Content.ContainsKey(posTagsKey) ? this.Content[posTagsKey] as string[] : new String[tokenCount];
            List<string>[] compoundComponentsArr = this.Content.ContainsKey(compoundComponentsKey) ? this.Content[compoundComponentsKey] as List<string>[] : new List<string>[tokenCount];
            List<string>[] hanReadingsArr = this.Content.ContainsKey(hanReadingsKey) ? this.Content[hanReadingsKey] as List<string>[] : new List<string>[tokenCount];

            List<MorphologyItem> items = new List<MorphologyItem>();
            for (int obj = 0; obj < tokenCount; obj++) {
                items.Add(new MorphologyItem(tokens[obj], lemmas[obj], posTags[obj], compoundComponentsArr[obj], hanReadingsArr[obj]));
            }
            this.Items = items;
            this.ResponseHeaders = new ResponseHeaders(this.Headers);
        }

        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj">The object to compare against</param>
        /// <returns>True if equal</returns>
        public override bool Equals(Object obj)
        {
            if (obj is MorphologyResponse)
            {
                MorphologyResponse other = obj as MorphologyResponse;
                List<bool> conditions = new List<bool>() {
                    this.Items.SequenceEqual(other.Items),
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
            return this.Items.GetHashCode() ^ this.ResponseHeaders.GetHashCode();
        }
    }

    /// <summary>
    /// A class for representing responses from the Morphology endpoint of the Rosette API
    /// </summary>
    public class MorphologyItem
    {
        /// <summary>
        /// Gets or sets he token on which this morphology item is based
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// If enabled, the part of speech of the token
        /// </summary>
        public string PosTag { get; set; }
        /// <summary>
        /// If enabled, the lemma of the token
        /// </summary>
        public string Lemma { get; set; }
        /// <summary>
        /// If enabled and present, the Han readings of the token
        /// </summary>
        public List<string> HanReadings { get; set; }
        /// <summary>
        /// If enabled and present, the compound components of the token
        /// </summary>
        public List<string> CompoundComponents { get; set; }

        /// <summary>
        /// Creates a Morphology Item that holds morphology details associated with a given token
        /// </summary>
        /// <param name="token">The token on which this Morphology Item is based</param>
        /// <param name="posTag">Optional:  The part of speech of the token</param>
        /// <param name="lemma">Optional:  The lemma of the token</param>
        /// <param name="hanReadings">Optional:  The Han readings of the token</param>
        /// <param name="compoundComponents">Optional:  The compound components of the token</param>
        public MorphologyItem(string token, string posTag = null, string lemma = null, List<string> hanReadings = null, List<string> compoundComponents = null) {
            this.Token = token;
            this.PosTag = posTag;
            this.Lemma = lemma;
            this.HanReadings = hanReadings;
            this.CompoundComponents = compoundComponents;
        }

        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj">The object to compare against</param>
        /// <returns>True if equal</returns>
        public override bool Equals(Object obj)
        {
            if (obj is MorphologyItem)
            {
                MorphologyItem other = obj as MorphologyItem;
                List<bool> conditions = new List<bool>() {
                    this.Token == other.Token,
                    this.PosTag == other.PosTag,
                    this.Lemma == other.Lemma,
                    this.HanReadings == other.HanReadings,
                    this.CompoundComponents == other.CompoundComponents
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
            return this.Token.GetHashCode() ^ this.Lemma.GetHashCode() ^ this.PosTag.GetHashCode() ^ this.HanReadings.GetHashCode() ^ this.CompoundComponents.GetHashCode();
        }
    }
}

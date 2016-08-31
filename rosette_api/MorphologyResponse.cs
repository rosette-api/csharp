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
            object[] tokenObjArr = this.ContentDictionary.ContainsKey(tokenKey) ? this.ContentDictionary[tokenKey] as object[] : new object[0];
            List<string> tokens = tokenObjArr.ToList().ConvertAll<string>(o => o != null ? o.ToString() : null);
            int tokenCount = tokens != null ? tokens.Count() : 0;
            object[] lemmaObjArr = this.ContentDictionary.ContainsKey(lemmasKey) ? this.ContentDictionary[lemmasKey] as object[] : new object[tokenCount];
            List<string> lemmas = lemmaObjArr.ToList().ConvertAll<string>(o => o != null ? o.ToString() : null);
            object[] posTagObjArr = this.ContentDictionary.ContainsKey(posTagsKey) ? this.ContentDictionary[posTagsKey] as object[] : new object[tokenCount];
            List<string> posTags = posTagObjArr.ToList().ConvertAll<string>(o => o != null ? o.ToString() : null);
            object[] compoundComponentsObjArr = this.ContentDictionary.ContainsKey(compoundComponentsKey) ? this.ContentDictionary[compoundComponentsKey] as object[] : new object[tokenCount];
            List<List<string>> compoundComponentsArr = compoundComponentsObjArr.ToList()
                .ConvertAll<List<string>>(o => o != null ? (o as object[]).ToList().ConvertAll<string>(i => i != null ? i.ToString() : null) : null);
            object[] hanReadingsObjArr = this.ContentDictionary.ContainsKey(hanReadingsKey) ? this.ContentDictionary[hanReadingsKey] as object[] : new object[tokenCount];
            List<List<string>> hanReadingsArr = hanReadingsObjArr.ToList()
                .ConvertAll<List<string>>(o => o != null ? (o as object[]).ToList().ConvertAll<string>(i => i != null ? i.ToString() : null) : null);
            if (compoundComponentsArr == null) { compoundComponentsArr = new List<string>[tokenCount].ToList(); }
            if (hanReadingsArr == null) { hanReadingsArr = new List<string>[tokenCount].ToList(); }
            List<MorphologyItem> items = new List<MorphologyItem>();
            for (int obj = 0; obj < tokenCount; obj++) {
                items.Add(new MorphologyItem(tokens[obj], posTags[obj], lemmas[obj], compoundComponentsArr[obj], hanReadingsArr[obj]));
            }
            this.Items = items;
            this.ResponseHeaders = new ResponseHeaders(this.Headers);
        }

        /// <summary>
        /// Creates a MorphologyResponse from its components
        /// </summary>
        /// <param name="items">The list of morphology items</param>
        /// <param name="responseHeaders">The response headers returned from the API</param>
        /// <param name="content">The content of the response in Dictionary form</param>
        /// <param name="contentAsJson">The content form API </param>
        public MorphologyResponse(List<MorphologyItem> items, Dictionary<string, string> responseHeaders, Dictionary<string, object> content, string contentAsJson)
            : base(responseHeaders, content, contentAsJson)
        {
            this.Items = items;
            this.ResponseHeaders = new ResponseHeaders(responseHeaders);
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
                    this.Items != null && other.Items != null ? this.Items.SequenceEqual(other.Items) : this.Items == other.Items,
                    this.ResponseHeaders != null && other.ResponseHeaders != null ? this.ResponseHeaders.Equals(other.ResponseHeaders) : this.ResponseHeaders == other.ResponseHeaders,
                    this.GetHashCode() == other.GetHashCode()
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
            int h0 = this.Items != null ? this.Items.Aggregate<MorphologyItem, int>(1, (seed, item) => seed ^ item.GetHashCode()) : 1;
            int h1 = this.ResponseHeaders != null ? this.ResponseHeaders.GetHashCode() : 1;
            return h0 ^ h1;
        }

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns>The response in JSON form</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder("{");
            string itemsString = this.Items != null ? String.Format("[{0}]", String.Join<MorphologyItem>(", ", this.Items)) : null;
            if (this.Items != null) { builder.AppendFormat("\"{0}\": {1}", "items", itemsString); }
            if (this.ResponseHeaders != null) { builder.AppendFormat("\"{0}\": \"{1}", responseHeadersKey, this.ResponseHeaders).Append("\", "); }
            if (builder.Length > 2) { builder.Remove(builder.Length - 2, 2); }
            builder.Append("}");
            return builder.ToString();
        }

        /// <summary>
        /// Gets the content of the response in JSON form
        /// </summary>
        /// <returns>The content in JSON form</returns>
        public string ContentToString()
        {
            StringBuilder builder = new StringBuilder("{");
            string itemsString = this.Items != null ? String.Format("[{0}]", String.Join<MorphologyItem>(", ", this.Items)) : null;
            if (this.Items != null) { builder.AppendFormat("\"{0}\": {1}", "items", itemsString); }
            builder.Append("}");
            return builder.ToString();
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
                    this.HanReadings != null && other.HanReadings != null ? this.HanReadings.SequenceEqual(other.HanReadings) : this.HanReadings == other.HanReadings,
                    this.CompoundComponents != null && other.CompoundComponents != null ? this.CompoundComponents.SequenceEqual(other.CompoundComponents) : this.CompoundComponents == other.CompoundComponents,
                    this.GetHashCode() == other.GetHashCode()
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
            int h0 = this.Token != null ? this.Token.GetHashCode() : 1;
            int h1 = this.Lemma != null ? this.Lemma.GetHashCode() : 1;
            int h2 = this.PosTag != null ? this.PosTag.GetHashCode() : 1;
            int h3 = this.HanReadings != null ? this.HanReadings.Aggregate<string, int>(1, (seed, item) => item.GetHashCode()) : 1;
            int h4 = this.CompoundComponents != null ? this.CompoundComponents.Aggregate<string, int>(1, (seed, item) => item.GetHashCode()) : 1;
            return h0 ^ h1 ^ h2 ^ h3 ^ h4;
        }

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns>The morphology item in JSON form</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder("{");
            if (this.Token != null) { builder.Append("\"token\": \"").Append(this.Token).Append("\", "); }
            if (this.PosTag != null) { builder.Append("\"posTag\": \"").Append(this.PosTag).Append("\", "); }
            if (this.Lemma != null) { builder.Append("\"lemma\": \"").Append(this.Lemma).Append("\", "); }
            if (this.CompoundComponents != null) { builder.AppendFormat("\"compoundComponents\": [{0}]", String.Join<string>(", ", this.CompoundComponents)).Append(", "); }
            if (this.HanReadings != null) { builder.AppendFormat("\"hanReadings\": [{0}]", String.Join<string>(", ", this.HanReadings)).Append(", "); }
            if (builder.Length > 2) { builder.Remove(builder.Length - 2, 2); }
            builder.Append("}");
            return builder.ToString();
        }
    }
}

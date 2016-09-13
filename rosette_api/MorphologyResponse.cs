using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Collections;

namespace rosette_api
{
    /// <summary>
    /// A class for representing responses from the Morphology endpoint of the API
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
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
        [JsonProperty("items")]
        public List<MorphologyItem> Items { get; set; }

        /// <summary>
        /// Creates a MorphologyResponse from the given apiResponse
        /// </summary>
        /// <param name="apiResponse">The message from the API</param>
        public MorphologyResponse(HttpResponseMessage apiResponse) : base(apiResponse)
        {
            JArray tokensArr = this.ContentDictionary.ContainsKey(tokenKey) ? this.ContentDictionary[tokenKey] as JArray : null;
            List<string> tokens = tokensArr != null ? new List<string>(tokensArr.Select<JToken, string>((jToken) => jToken != null ? jToken.ToString() : null)) : null;
            int tokenCount = tokens != null ? tokens.Count : 0;
            JArray lemmasArr = this.ContentDictionary.ContainsKey(lemmasKey) ? this.ContentDictionary[lemmasKey] as JArray : null;
            List<string> lemmas = lemmasArr != null ? new List<string>(lemmasArr.Select<JToken, string>((jToken) => jToken != null ? jToken.ToString() : null)) : null;
            JArray posTagsArr = this.ContentDictionary.ContainsKey(posTagsKey) ? this.ContentDictionary[posTagsKey] as JArray : null;
            List<string> posTags = posTagsArr != null ? new List<string>(posTagsArr.Select<JToken, string>((jToken) => jToken != null ? jToken.ToString() : null)) : null;
            JArray compoundComponentsArr = this.ContentDictionary.ContainsKey(compoundComponentsKey) ? this.ContentDictionary[compoundComponentsKey] as JArray : null;
            List<List<string>> compoundComponents = compoundComponentsArr != null ? new List<List<string>>(compoundComponentsArr.Select<JToken, List<string>>((jToken) => jToken != null ? jToken.ToObject<List<string>>() : null)) : null;
            JArray hanReadingsArr = this.ContentDictionary.ContainsKey(hanReadingsKey) ? this.ContentDictionary[hanReadingsKey] as JArray : null;
            List<List<string>> hanReadings = hanReadingsArr != null ? new List<List<string>>(hanReadingsArr.Select<JToken, List<string>>((jToken) => jToken != null ? jToken.ToObject<List<string>>() : null)) : null;
            this.Items = this.MakeMorphologyItems(tokens, lemmas, posTags, compoundComponents, hanReadings);
        }

        /// <summary>
        /// Creates a list of morphology items from JsonToken arrays of equal length that each contain a type of component to the morphology item
        /// </summary>
        /// <param name="tokens">The token list upon which all else in the morphology item is based</param>
        /// <param name="lemmas">The list of corresponding lemmas</param>
        /// <param name="posTags">The list of corresponding posTags</param>
        /// <param name="compoundComponentsArr">The list of corresponding compound components</param>
        /// <param name="hanReadingsArr">The list of corresponding Han readings</param>
        /// <returns>A composite list of morphology items</returns>
        private List<MorphologyItem> MakeMorphologyItems(List<string> tokens, List<string> lemmas, List<string> posTags, List<List<string>> compoundComponentsArr, List<List<string>> hanReadingsArr) 
        {
            int tokenCount = tokens != null ? tokens.Count : 0;
            List<MorphologyItem> items = new List<MorphologyItem>();
            for (int obj = 0; obj < tokenCount; obj++)
            {
                string token = tokens != null && tokens[obj] != null ? tokens[obj].ToString() : null;
                string posTag = posTags != null && posTags[obj] != null ? posTags[obj].ToString() : null;
                string lemma = lemmas != null && lemmas[obj] != null ? lemmas[obj].ToString() : null;
                List<string> compoundComponents = compoundComponentsArr != null && compoundComponentsArr[obj] != null ? compoundComponentsArr[obj] : null;
                List<string> hanReadings = hanReadingsArr != null && hanReadingsArr[obj] != null ? hanReadingsArr[obj] : null;
                items.Add(new MorphologyItem(token, posTag, lemma, compoundComponents, hanReadings));
            }
            return items;
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
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// Gets the content of the response in JSON form
        /// </summary>
        /// <returns>The content in JSON form</returns>
        public override string ContentToString()
        {
            return JsonConvert.SerializeObject(this.Items);
        }
    }

    /// <summary>
    /// A class for representing responses from the Morphology endpoint of the Rosette API
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class MorphologyItem
    {
        /// <summary>
        /// Gets or sets he token on which this morphology item is based
        /// </summary>
        [JsonProperty("token")]
        public string Token { get; set; }
        /// <summary>
        /// If enabled, the part of speech of the token
        /// </summary>
        [JsonProperty("posTag")]
        public string PosTag { get; set; }
        /// <summary>
        /// If enabled, the lemma of the token
        /// </summary>
        [JsonProperty("lemma")]
        public string Lemma { get; set; }
        /// <summary>
        /// If enabled and present, the Han readings of the token
        /// </summary>
        [JsonProperty("hanReadings")]
        public List<string> HanReadings { get; set; }
        /// <summary>
        /// If enabled and present, the compound components of the token
        /// </summary>
        [JsonProperty("compoundComponents")]
        public List<string> CompoundComponents { get; set; }

        /// <summary>
        /// Creates a Morphology Item that holds morphology details associated with a given token
        /// </summary>
        /// <param name="token">The token on which this Morphology Item is based</param>
        /// <param name="posTag">Optional:  The part of speech of the token</param>
        /// <param name="lemma">Optional:  The lemma of the token</param>
        /// <param name="compoundComponents">Optional:  The compound components of the token</param>
        /// <param name="hanReadings">Optional:  The Han readings of the token</param>
        public MorphologyItem(string token, string posTag = null, string lemma = null, List<string> compoundComponents = null, List<string> hanReadings = null)
        {
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
            return JsonConvert.SerializeObject(this);
        }
    }
}

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
        internal static string tokenKey = "tokens";
        internal static string hanReadingsKey = "hanReadings";
        internal static string compoundComponentsKey = "compoundComponents";
        internal static string posTagsKey = "posTags";
        internal static string lemmasKey = "lemmas";
        internal static string responseHeadersKey = "responseHeaders";

        /// <summary>
        /// The output of the Rosette API's morphology endpoint, grouped by tokens in order of appearance
        /// </summary>
        public List<MorphologyItem> Items;

        /// <summary>
        /// The response headers returned from the API
        /// </summary>
        public ResponseHeaders ResponseHeaders;

        /// <summary>
        /// Creates a MorphologyResponse from the given apiResponse
        /// </summary>
        /// <param name="apiResponse">The message from the API</param>
        public MorphologyResponse(HttpResponseMessage apiResponse) : base(apiResponse)
        {
            List<string> tokens = this.Content.ContainsKey(tokenKey) ? this.Content[tokenKey] as List<string>: new List<String>();
            int tokenCount = tokens.Count();
            string[] lemmas = new String[tokenCount];
            if (this.Content.ContainsKey(lemmasKey))
            {
                lemmas = this.Content[lemmasKey] as string[];
            }
            string[] posTags = new String[tokenCount];
            if (this.Content.ContainsKey(posTagsKey))
            {
                posTags = this.Content[posTagsKey] as string[];
            }
            List<string>[] compoundComponentsArr = new List<string>[tokenCount];
            if (this.Content.ContainsKey(compoundComponentsKey))
            {
                compoundComponentsArr = this.Content[compoundComponentsKey] as List<string>[];
            }
            List<string>[] hanReadingsArr = new List<string>[tokenCount];
            if (this.Content.ContainsKey(hanReadingsKey))
            {
                hanReadingsArr = this.Content[hanReadingsKey] as List<string>[];
            }

            List<MorphologyItem> items = new List<MorphologyItem>();
            for (int obj = 0; obj < tokenCount; obj++) {
                items.Add(new MorphologyItem(tokens[obj], lemmas[obj], posTags[obj], compoundComponentsArr[obj], hanReadingsArr[obj]));
            }
            this.Items = items;
            this.ResponseHeaders = new ResponseHeaders(this.Headers);
        }
    }

    /// <summary>
    /// A class for representing responses from the Morphology endpoint of the Rosette API
    /// </summary>
    public class MorphologyItem
    {
        /// <summary>
        /// The token on which this morphology item is based
        /// </summary>
        public Optional<string> Token;
        /// <summary>
        /// If enabled, the part of speech of the token
        /// </summary>
        public Optional<string> PosTag;
        /// <summary>
        /// If enabled, the lemma of the token
        /// </summary>
        public Optional<string> Lemma;
        /// <summary>
        /// If enabled and present, the Han readings of the token
        /// </summary>
        public Optional<List<string>> HanReadings;
        /// <summary>
        /// If enabled and present, the compound components of the token
        /// </summary>
        public Optional<List<string>> CompoundComponents;

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
            this.HanReadings = new Optional<List<string>>(hanReadings);
            this.CompoundComponents = new Optional<List<string>>(compoundComponents);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace rosette_api
{
    /// <summary>
    /// An interface for common methods of EntityID classes
    /// </summary>
    public interface EntityID
    {
        /// <summary>
        /// Gets or sets the ID of this EntityID
        /// </summary>
        /// <returns>The ID of this EntityID, represented as a string</returns>
        String ID { get; set; }
    }

    /// <summary>
    /// A class to represent EntityIDs that are local and not universally recognized
    /// </summary>
    public class TemporaryID : EntityID
    {
        /// <summary>
        /// The ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Creates an entity ID using the given string
        /// </summary>
        /// <param name="id">The ID value of the new Entity ID</param>
        public TemporaryID(String id)
        {
            this.ID = id;
        }
    }

    /// <summary>
    /// A class to represent EntityIDs that are linkable to Wikidata
    /// </summary>
    public class QID : EntityID
    {
        /// <summary>
        /// The QID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// The URL to the Wikidata for this QID
        /// </summary>
        public string Link;
        
        /// <summary>
        /// Creates an Entity ID that is linkable to Wikipedia from the given id string
        /// </summary>
        /// <param name="id">The ID value of the new Entity ID</param>
        /// <exception cref="ArgumentException">Throws an invalid argument exception if the id param is not a valid Wikidata QID.</exception>
        public QID(String id)
        {
            String siteLink = ("https://www.wikidata.org/w/api.php?action=wbgetentities&ids=" + id + "&sitefilter=enwiki&props=sitelinks&format=json");
            String jsonStr;
            try
            {
                jsonStr = MakeRequest(siteLink);
            }
            catch
            {
                throw new ArgumentException("A QID object could not be created because the given ID is not a valid Wikidata QID.", "id");
            }
            int lengthOfTitle = jsonStr.IndexOf("\"badges\":") - jsonStr.IndexOf("\"title\":") - 11;
            if (lengthOfTitle > 0)
            {
                String title = jsonStr.Substring(jsonStr.IndexOf("\"title\":") + 9, lengthOfTitle);
                title = title.Replace(" ", "_");
                this.Link = "https://en.wikipedia.org/wiki/" + (title);
            }
            this.ID = id;
        }

        private static String MakeRequest(string requestUrl)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
                request.Method = "GET";
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    Stream stream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    String responseString = reader.ReadToEnd();
                    return responseString;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}

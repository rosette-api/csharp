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
    public class EntityID
    {
        /// <summary>
        /// Gets or sets the ID of this EntityID
        /// </summary>
        /// <returns>The ID of this EntityID, represented as a string</returns>
        public string ID { get; set; }

        /// <summary>
        /// Creates an entity ID using the given string
        /// </summary>
        /// <param name="id">The ID value of the new Entity ID</param>
        public EntityID(string id)
        {
            this.ID = id;
        }

        /// <summary>
        /// Creates a link to the Wikipedia article for this EntityID's ID if it is a QID.  If the ID is not a QID, this method returns null.
        /// </summary>
        /// <returns>The valid Wikipedia link or null.</returns>
        public string GetWikipedaURL()
        {
            String siteLink = ("https://www.wikidata.org/w/api.php?action=wbgetentities&ids=" + ID + "&sitefilter=enwiki&props=sitelinks&format=json");
            try
            {
                String jsonStr = MakeRequest(siteLink);
                int lengthOfTitle = jsonStr.IndexOf("\"badges\":") - jsonStr.IndexOf("\"title\":") - 11;
                String title = jsonStr.Substring(jsonStr.IndexOf("\"title\":") + 9, lengthOfTitle);
                title = title.Replace(" ", "_");
                string link = "https://en.wikipedia.org/wiki/" + (title);
                return link;
            }
            catch
            {
                return null;
            }
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

        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj">The object to compare against</param>
        /// <returns>True if equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is EntityID) {
                EntityID other = obj as EntityID;
                List<bool> conditions = new List<bool>() {
                    this.ID != null && other.ID != null ? this.ID.Equals(other.ID) : this.ID == other.ID,
                    this.GetHashCode() == other.GetHashCode()
                };
                return conditions.All(condition => condition);
            } else {
                return false;
            }
        }

        /// <summary>
        /// GetHashCode override
        /// </summary>
        /// <returns>The hashcode</returns>
        public override int GetHashCode()
        {
            return this.ID != null ? this.ID.GetHashCode() : 1;
        }

        /// <summary>
        /// ToString override.
        /// </summary>
        /// <returns>This EntityID in JSON form</returns>
        public override string ToString()
        {
            return this.ID;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace rosette_api
{
    /// <summary>
    /// Custom resolver for JSON serialization
    /// </summary>
    public class RosetteResponseContractResolver : DefaultContractResolver
    {
        /// <summary>
        /// Generates the list of JSON properties
        /// </summary>
        /// <param name="type">type to process</param>
        /// <param name="memberSerialization">member serialization</param>
        /// <returns>IList of JsonProperty</returns>
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var list = base.CreateProperties(type, memberSerialization);
            return this.DontIgnorePropertiesWithMemberAttributes(list);
        }

        /// <summary>
        /// Filter to remove properties that have not been specifically included
        /// </summary>
        /// <param name="list">JsonProperty list</param>
        /// <returns>IList of Json Property</returns>
        protected IList<JsonProperty> DontIgnorePropertiesWithMemberAttributes(IList<JsonProperty> list)
        {
            foreach (var prop in list)
            {
                prop.Ignored = !prop.HasMemberAttribute; // Ignore properties that have not been specifically included
            }
            return list;
        }
    }
}

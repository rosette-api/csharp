using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace rosette_api
{
    public class RosetteResponseContractResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var list = base.CreateProperties(type, memberSerialization);
            return this.DontIgnorePropertiesWithMemberAttributes(list);
        }

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

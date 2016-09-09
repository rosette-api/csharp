using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace rosette_api
{
    internal class IEnumerableExpanderContractResolver<T> : IContractResolver
    {
        public new static readonly ConditionalContractResolver Instance = new ConditionalContractResolver();

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> property = base.CreateProperties(typeof(T), memberSerialization);
            return property;
        }

        public JsonContract ResolveContract(Type type)
        {
            return new JsonArrayContract(type).ItemConverter.;
        }
    }
}

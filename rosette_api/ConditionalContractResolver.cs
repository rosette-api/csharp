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
    internal class ConditionalContractResolver : DefaultContractResolver
    {
        internal Predicate<JsonProperty> Condition;
        internal Func<JsonProperty, int, IEnumerable<JsonProperty>> PropertyCreationAction;

        internal ConditionalContractResolver(Predicate<JsonProperty> condition, Func<JsonProperty, int, IEnumerable<JsonProperty>> propertyCreationAction)
        {
            this.Condition = condition;
            this.PropertyCreationAction = propertyCreationAction;
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);
            IList<JsonProperty> propertiesToRehandle = properties.Where(p => this.Condition(p)).ToList();

            int order = 0;
            foreach (JsonProperty jProperty in properties)
            {
                if (propertiesToRehandle.Contains(jProperty))
                {
                    IEnumerable<JsonProperty> newProperties = this.PropertyCreationAction(jProperty, order);
                    foreach (JsonProperty newProperty in newProperties)
                    {
                        properties.Insert(order++, newProperty);
                    }
                }
                else
                {
                    jProperty.Order = order++;
                }
            }
            return properties;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;

namespace rosette_api
{
    /// <summary>
    /// JSON value provider that always returns a static value
    /// </summary>
    internal class StaticValueProvider : IValueProvider
    {
        private readonly object _staticValue;

        public StaticValueProvider(object staticValue)
        {
            _staticValue = staticValue;
        }

        public void SetValue(object target, object value)
        {
            throw new NotSupportedException();
        }

        public object GetValue(object target)
        {
            return _staticValue;
        }
    }
}

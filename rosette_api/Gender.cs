using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace rosette_api
{
    /// <summary>
    /// Possible gender options for the Name objects
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Gender
    {
        Female,
        NonBinary,
        Male
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace rosette_api {
    /// <summary>
    /// Extension classes specific to the Rosette binding
    /// </summary>
    public static class RosetteExtensions {
        /// <summary>
        /// Extension method to convert camelCase feature to hyphenated string endpoint
        /// </summary>
        /// <param name="feature">MorphologyFeature</param>
        /// <returns>hyphenated string</returns>
        public static string MorphologyEndpoint(this MorphologyFeature feature) {
            return Regex.Replace(feature.ToString(), @"([a-z])([A-Z])", "$1-$2").ToLower();
        }
    }
}

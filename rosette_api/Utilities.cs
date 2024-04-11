using System.Collections.Generic;
using System;

namespace rosette_api {
    /// <summary>
    /// Utilities class
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Compares two dictionaries for equality
        /// </summary>
        public static bool DictionaryEquals<TKey, TValue>(this IDictionary<TKey, TValue> first, IDictionary<TKey, TValue> second)
        {
            if (first == null && second == null)
            {
                return true;
            }
            if (first == null || second == null)
            {
                return false;
            }
            if (first.Count != second.Count)
            {
                return false;
            }
            foreach (var kvp in first)
            {
                if (!second.TryGetValue(kvp.Key, out TValue secondValue))
                {
                    return false;
                }
                if (!kvp.Value.Equals(secondValue))
                {
                    return false;
                }
            }
            return true;
        }
    } 
}
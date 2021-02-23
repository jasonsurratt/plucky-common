using System.Collections.Generic;

namespace Plucky.Common
{
    public static class IDictionaryExtensions
    {
        public static void AddAll<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IReadOnlyDictionary<TKey, TValue> from)
        {
            foreach (var kvp in from)
            {
                dictionary[kvp.Key] = kvp.Value;
            }
        }


        public static V GetOrDefault<K, V>(this IReadOnlyDictionary<K, V> dict, K key)
        {
            if (dict.TryGetValue(key, out V value))
            {
                return value;
            }
            return default;
        }
    }
}

using System;
using System.Collections.Generic;

namespace AnAusAutomat.Toolbox.Extensions
{
    public static class DictionaryExtensions
    {
        public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default(TValue))
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return dictionary.TryGetValue(key, out TValue value) ? value : defaultValue;
        }
    }
}
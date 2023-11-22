using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Caching.Memory;

namespace Cortside.HealthMonitor.WebApi {
    /// <summary>
    /// MemoryCache extensions
    /// </summary>
    public static class MemoryCacheExtensions {
        private static readonly Func<MemoryCache, object> GetEntriesCollection = Delegate.CreateDelegate(
            typeof(Func<MemoryCache, object>),
            typeof(MemoryCache)
                .GetField("_coherentState", BindingFlags.Instance | BindingFlags.NonPublic)
                .FieldType
                .GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetGetMethod(true),
            throwOnBindFailure: true) as Func<MemoryCache, object>;

        /// <summary>
        /// Get keys
        /// </summary>
        /// <param name="memoryCache"></param>
        /// <returns></returns>
        public static IEnumerable GetKeys(this IMemoryCache memoryCache) {
            var fieldInfo = typeof(MemoryCache).GetField("_coherentState", BindingFlags.Instance | BindingFlags.NonPublic);
            var propertyInfo = fieldInfo.FieldType.GetProperty("EntriesCollection", BindingFlags.Instance | BindingFlags.NonPublic);
            var value = fieldInfo.GetValue(memoryCache);
            var dict = propertyInfo.GetValue(value);
            var cacheEntries = dict as IDictionary;

            return cacheEntries.Keys;
        }

        /// <summary>
        /// Get keys
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="memoryCache"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetKeys<T>(this IMemoryCache memoryCache) =>
            GetKeys(memoryCache).OfType<T>();
    }
}

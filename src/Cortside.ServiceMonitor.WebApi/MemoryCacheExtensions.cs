using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Caching.Memory;

namespace Cortside.ServiceMonitor.WebApi {

    /// <summary>
    /// MemoryCache extensions
    /// </summary>
    public static class MemoryCacheExtensions {
        private static readonly Func<MemoryCache, object> GetEntriesCollection = Delegate.CreateDelegate(
            typeof(Func<MemoryCache, object>),
            typeof(MemoryCache).GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true),
            throwOnBindFailure: true) as Func<MemoryCache, object>;

        /// <summary>
        /// Get keys
        /// </summary>
        /// <param name="memoryCache"></param>
        /// <returns></returns>
        public static IEnumerable GetKeys(this IMemoryCache memoryCache) =>
            ((IDictionary)GetEntriesCollection((MemoryCache)memoryCache)).Keys;

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Caching;

namespace UFIDA.U8.Portal.ZT.VouchPlugIns.Tools
{
    public class CacheHelper
    {
        private static readonly MemoryCache objCache = MemoryCache.Default;

        public static object GetCache(string cacheKey)
        {
            return objCache.Get(cacheKey);
        }
        public static void SetCache(string key, object obj, long timestamp)
        {
            var policy = new CacheItemPolicy
            {
                AbsoluteExpiration = Tool.ConvertFromUnixTimestamp(timestamp)
            };

            objCache.Set(key, obj, policy);
        }
        public static bool CacheKeyContains(string cacheKey)
        {
            return objCache.Get(cacheKey) != null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrismAop.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CacheAttribute : Attribute
    {
        public string CacheKey { get; }
        public int Expiration { get; set; }

        /// <summary>
        /// 缓存特性标记
        /// </summary>
        /// <param name="cacheKey">缓存key，默认为空</param>
        /// <param name="expiration">过期时间，单位为分钟，默认60分钟，传0不过期</param>
        public CacheAttribute(string cacheKey = "", int expiration = 60)
        {
            CacheKey = cacheKey;
            Expiration = expiration;
        }

        public override string ToString() => $"{{ CacheKey: {CacheKey ?? "null"}, Expiration: {Expiration} minute}}";
    }
}

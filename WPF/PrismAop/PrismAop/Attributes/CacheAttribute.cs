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
        public string? CacheKey { get; }
        public long Expiration { get; }

        public CacheAttribute(string? cacheKey = null, long expiration = 0)
        {
            CacheKey = cacheKey;
            Expiration = expiration;
        }

        public override string ToString() => $"{{ CacheKey: {CacheKey ?? "null"}, Expiration: {Expiration} }}";
    }
}

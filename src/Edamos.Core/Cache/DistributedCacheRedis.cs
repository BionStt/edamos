using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Options;

namespace Edamos.Core.Cache
{
    public class DistributedCacheRedis<T> : RedisCache, IDistributedCache<T>
    {
        public DistributedCacheRedis(IOptions<RedisCacheOptions> optionsAccessor) : base(optionsAccessor)
        {
        }
    }
}
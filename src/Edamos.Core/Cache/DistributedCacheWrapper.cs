using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace Edamos.Core.Cache
{
    public class DistributedCacheWrapper<TService,TObject> where TObject : class
    {
        private static readonly DistributedCacheEntryOptions DefaultExpirationOptions =
            new DistributedCacheEntryOptions {AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)};
        private readonly IDistributedCache<TObject> _distributedCache;

        public DistributedCacheWrapper(IDistributedCache<TObject> distributedCache)
        {
            _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
        }

        protected async Task<TObject> GetOrSetDistributedCache(
            CacheUsage cacheUsage, 
            string key,
            DistributedCacheEntryOptions distributedCacheEntryOptions, 
            Func<Task<TObject>> factory)
        {
            if ((cacheUsage & CacheUsage.Distributed) == CacheUsage.Distributed)
            {
                if (distributedCacheEntryOptions != null)
                {
                    // Guard for incerrect expiration usage
                    if (!distributedCacheEntryOptions.AbsoluteExpirationRelativeToNow.HasValue)
                    {
                        distributedCacheEntryOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);
                    }
                }

                string fullKey = BuildFullKey(key);
                byte[] data = await this._distributedCache.GetAsync(fullKey);

                TObject result;
                if (data != null)
                {                    
                    try
                    {
                        result = this.Deserialize(data);
                        return result;
                    }
                    catch
                    {
                        // return new cache entry if previous one if corrupted
                    }
                }

                result = await factory();

                try
                {
                    data = this.Serialize(result);
                    await this._distributedCache.SetAsync(fullKey, data,
                        distributedCacheEntryOptions ?? DefaultExpirationOptions);
                }
                catch
                {
                    // Ignore cache exceptions
                }

                return result;
            }

            return await factory();
        }

        private string BuildFullKey(string key)
        {
            //TODO: optimize full key building
            return typeof(TService).FullName.GetHashCode() + "-" + typeof(TObject).FullName.GetHashCode() + "-" + key;
        }

        protected virtual byte[] Serialize(TObject value)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(TObject));
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (GZipStream gZip = new GZipStream(memoryStream, CompressionLevel.Fastest, true))
                {
                    serializer.WriteObject(gZip, value);
                }

                memoryStream.Seek(0, SeekOrigin.Begin);

                return memoryStream.ToArray();
            }            
        }

        protected virtual TObject Deserialize(byte[] data)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(TObject));
            using (MemoryStream memoryStream = new MemoryStream(data))
            {
                using (GZipStream gZip = new GZipStream(memoryStream, CompressionMode.Decompress, true))
                {
                    TObject result = serializer.ReadObject(gZip) as TObject;

                    return result;
                }                
            }
        }
    }
}
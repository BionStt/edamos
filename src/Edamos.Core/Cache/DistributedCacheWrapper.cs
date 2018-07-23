//using System;
//using System.IO;
//using System.IO.Compression;
//using System.Linq;
//using System.Runtime.Serialization.Json;
//using System.Threading.Tasks;
//using App.Metrics;
//using App.Metrics.Timer;
//using Microsoft.Extensions.Caching.Distributed;
//using Microsoft.Extensions.Caching.Memory;

//namespace Edamos.Core.Cache
//{
//    public class DistributedCacheWrapper<TService,TObject> where TObject : class
//    {
//        private static readonly DistributedCacheEntryOptions DefaultExpirationOptions =
//            new DistributedCacheEntryOptions {AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)};

//        private readonly MetricTags _metricsTags = new MetricTags(new[] {"ctype", "srv", "obj"},
//            new[] {"dst", typeof(TService).Name.Split('.').Last(), typeof(TObject).Name.Split('.').Last()});

//        private readonly IDistributedCache<TObject> _distributedCache;
//        private readonly IMetrics _metrics;

//        public DistributedCacheWrapper(IDistributedCache<TObject> distributedCache, IMetrics metrics)
//        {
//            _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
//            _metrics = metrics ?? throw new ArgumentNullException(nameof(metrics));
//        }

//        protected async Task<TResult> GetOrSetDistributedCache<TResult>(
//            CacheUsage cacheUsage, 
//            string key,
//            DistributedCacheEntryOptions distributedCacheEntryOptions, 
//            Func<Task<TResult>> factory) where TResult : class 
//        {
//            if ((cacheUsage & CacheUsage.Distributed) == CacheUsage.Distributed)
//            {
//                if (distributedCacheEntryOptions != null)
//                {
//                    // Guard for incerrect expiration usage
//                    if (!distributedCacheEntryOptions.AbsoluteExpirationRelativeToNow.HasValue)
//                    {
//                        distributedCacheEntryOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);
//                    }
//                }

//                TResult result;
//                byte[] data;
//                string fullKey = BuildFullKey<TResult>(key);
//                using (var timer = this._metrics.Measure.Timer.Time(CacheMetrics.ReadTimer, this._metricsTags))
//                {                                        
//                    data = await this._distributedCache.GetAsync(fullKey);

//                    if (data != null)
//                    {                        
//                        try
//                        {
//                            result = this.Deserialize<TResult>(data);
//                            timer.TrackUserValue("hit");
//                            return result;
//                        }
//                        catch
//                        {
//                            // return new cache entry if previous one if corrupted
//                            timer.TrackUserValue("exc");
//                        }
//                    }
//                    else
//                    {
//                        timer.TrackUserValue("miss");
//                    }
//                }

//                using (var timer = this._metrics.Measure.Timer.Time(CacheMetrics.FactoryTimer, this._metricsTags))
//                {
//                    result = await factory();
//                    timer.TrackUserValue("ok");
//                }

//                using (var timer = this._metrics.Measure.Timer.Time(CacheMetrics.WriteTimer, this._metricsTags))
//                {
//                    try
//                    {
//                        data = this.Serialize(result);
//                        await this._distributedCache.SetAsync(fullKey, data,
//                            distributedCacheEntryOptions ?? DefaultExpirationOptions);
//                        timer.TrackUserValue("ok");
//                    }
//                    catch
//                    {
//                        // Ignore cache exceptions
//                        timer.TrackUserValue("exc");
//                    }
//                }

//                return result;
//            }

//            return await factory();
//        }

        

//        private string BuildFullKey<TResult>(string key)
//        {
//            //TODO: optimize full key building
//            return typeof(TService).FullName.GetHashCode() + "-" + 
//                   typeof(TObject).FullName.GetHashCode() + "-" +
//                   typeof(TResult).FullName.GetHashCode() + "-" + 
//                   key;
//        }

//        protected virtual byte[] Serialize<TResult>(TResult value)
//        {
//            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(TResult));
//            using (MemoryStream memoryStream = new MemoryStream())
//            {
//                using (GZipStream gZip = new GZipStream(memoryStream, CompressionLevel.Fastest, true))
//                {
//                    serializer.WriteObject(gZip, value);
//                }

//                memoryStream.Seek(0, SeekOrigin.Begin);

//                return memoryStream.ToArray();
//            }            
//        }

//        protected virtual TResult Deserialize<TResult>(byte[] data) where TResult : class 
//        {
//            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(TResult));
//            using (MemoryStream memoryStream = new MemoryStream(data))
//            {
//                using (GZipStream gZip = new GZipStream(memoryStream, CompressionMode.Decompress, true))
//                {
//                    TResult result = serializer.ReadObject(gZip) as TResult;

//                    return result;
//                }                
//            }
//        }
//    }
//}
using Microsoft.Extensions.Caching.Distributed;

namespace Edamos.Core.Cache
{
    public interface IDistributedCache<T> : IDistributedCache
    {
        
    }
}
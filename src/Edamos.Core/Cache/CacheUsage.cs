using System;

namespace Edamos.Core.Cache
{
    [Flags]
    public enum CacheUsage
    {
        None = 0,
        Distributed = 1,
        Memory = 2,
        All = 3
    }
}
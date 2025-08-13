using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.BuildingBlocks.Application.Behaviors
{
    // Cache edilebilir request'ler için marker interface
    public interface ICacheableRequest
    {
        //Cache key'i için benzersiz bir değer döndürür
        string CacheKey { get; }

        // Cache süresi (dakika)
        TimeSpan CacheDuration { get; }
    }
}

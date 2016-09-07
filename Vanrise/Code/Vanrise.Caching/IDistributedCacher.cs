using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Caching
{
    public interface IDistributedCacher 
    {
        Q GetOrCreateObject<T, Q>(string cacheName, Func<CachedObjectCreationHandler<Q>> getObjectCreationHandler)where T : BaseCacheManager;
    }
}

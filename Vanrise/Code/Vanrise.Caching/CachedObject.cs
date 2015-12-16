using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Caching
{
    public class CachedObject
    {
        public CachedObject(Object obj)
        {
            this.Object = obj;
        }

        public string CacheName { get; set; }

        public Object Object { get; set; }

        public CacheObjectSize ApproximateSize { get; set; }

        public DateTime LastAccessedTime { get; set; }
    }
}

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
            this.CreatedTime = DateTime.Now;
        }

        public Object CacheName { get; set; }

        public Object Object { get; set; }

        public CacheObjectSize ApproximateSize { get; set; }

        public DateTime LastAccessedTime { get; set; }

        public DateTime CreatedTime { get; set; }
    }
}

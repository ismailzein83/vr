using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Caching
{
    public static class RuntimeCacheFactory
    {
        static ObjectFactory s_runtimeObjectFactory = new ObjectFactory(Assembly.Load("Vanrise.Caching.Runtime"));
        public static IDistributedCacher GetDistributedCacher()
        {
            return s_runtimeObjectFactory.CreateObjectFromType<IDistributedCacher>();
        }
    }
}

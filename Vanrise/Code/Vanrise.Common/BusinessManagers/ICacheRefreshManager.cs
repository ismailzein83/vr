using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common
{
    public interface ICacheRefreshManager : IBEManager
    {
        bool ShouldRefreshCacheManager(string cacheTypeName, ref object updateHandle);
        void TriggerCacheExpiration(string cacheTypeName);
    }
}

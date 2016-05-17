using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Business
{
    public class BELookupRuleDefinitionManager
    {
        internal class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            protected override bool ShouldSetCacheExpired()
            {
                throw new NotImplementedException();
            }
        }
    }
}

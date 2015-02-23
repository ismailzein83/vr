using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Caching;

namespace TOne.BusinessEntity.Business
{
    public class PricingGenerator 
    {
        TOneCacheManager _cacheManager;

        public PricingGenerator(TOneCacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

    }
}

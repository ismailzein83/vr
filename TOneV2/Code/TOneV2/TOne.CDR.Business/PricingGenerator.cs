using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;
using TOne.Caching;
using TOne.CDR.Entities;

namespace TOne.CDR.Business
{
    public class PricingGenerator
    {

        TOneCacheManager _cacheManager;

        public PricingGenerator(TOneCacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        public T GetRepricing<T>(BillingCDRMain main) where T : BillingCDRPricingBase, new()
        {
            T pricing = new T();

            
            return pricing;
        }
    }
}

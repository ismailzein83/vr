using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Business
{
    public class PricingEntityCacheManager : Vanrise.Caching.BaseCacheManager<PricingEntity>
    {
        DateTime? _chargingPolicyCacheLastCheck;
        DateTime? _packageCacheLastCheck;
        protected override bool ShouldSetCacheExpired(PricingEntity parameter)
        {
            switch (parameter)
            {
                case PricingEntity.ChargingPolicy: return Vanrise.Caching.CacheManagerFactory.GetCacheManager<ChargingPolicyManager.CacheManager>().IsCacheExpired(ref _chargingPolicyCacheLastCheck);
                case PricingEntity.Package: return Vanrise.Caching.CacheManagerFactory.GetCacheManager<PackageManager.CacheManager>().IsCacheExpired(ref _packageCacheLastCheck);
                default: throw new Exception(String.Format("PricingEntity '{0}' not implemented", parameter));
            }
        }
    }
}

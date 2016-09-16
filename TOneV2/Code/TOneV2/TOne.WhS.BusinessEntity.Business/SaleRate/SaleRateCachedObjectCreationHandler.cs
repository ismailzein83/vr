using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Caching;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SaleRateCachedObjectCreationHandler : CachedObjectCreationHandler<List<SaleRate>>
    {
        public SalePriceListOwnerType _ownerType { get; set; }
        public int _ownerId { get; set; }
        public DateTime _effectiveOn { get; set; }

        public SaleRateCachedObjectCreationHandler(Entities.SalePriceListOwnerType ownerType, int ownerId, DateTime effectiveOn)
        {
            _ownerType = ownerType;
            _ownerId = ownerId;
            _effectiveOn = effectiveOn;
        }
        public override List<SaleRate> CreateObject()
        {
            var cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<SaleRateCacheManager>();
            ISaleRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleRateDataManager>();
            return dataManager.GetEffectiveSaleRates(_ownerType, _ownerId, _effectiveOn).Select(rate => cacheManager.CacheAndGetRate(rate)).ToList();
        }
    }
}
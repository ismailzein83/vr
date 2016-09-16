using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Caching;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SupplierRateCachedObjectCreationHandler : CachedObjectCreationHandler<List<SupplierRate>>
    {
        public int _supplierId { get; set; }
        public DateTime _effectiveOn { get; set; }

        public SupplierRateCachedObjectCreationHandler(int supplierId, DateTime effectiveOn)
        {
            _supplierId = supplierId;
            _effectiveOn = effectiveOn;
        }
        public override List<SupplierRate> CreateObject()
        {
            var cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<SupplierRateCacheManager>();
            ISupplierRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierRateDataManager>();
            return dataManager.GetEffectiveSupplierRates(_supplierId, _effectiveOn).Select(rate => cacheManager.CacheAndGetRate(rate)).ToList();
        }
    }
}
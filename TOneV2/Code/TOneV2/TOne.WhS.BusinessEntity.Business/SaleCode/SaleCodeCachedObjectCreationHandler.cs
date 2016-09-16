using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Caching;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SaleCodeCachedObjectCreationHandler : CachedObjectCreationHandler<List<SaleCode>>
    {
        public int _sellingNumberPlanId { get; set; }
        public DateTime _effectiveOn { get; set; }

        public SaleCodeCachedObjectCreationHandler(int sellingNumberPlanId, DateTime effectiveOn)
        {
            _sellingNumberPlanId = sellingNumberPlanId;
            _effectiveOn = effectiveOn;
        }
        public override List<SaleCode> CreateObject()
        {
            var cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<SaleCodeCacheManager>();
            SaleCodeManager saleCodeManager = new SaleCodeManager();
            return saleCodeManager.GetSellingNumberPlanSaleCodes(_sellingNumberPlanId, _effectiveOn).Select(code => cacheManager.CacheAndGetCode(code)).ToList();
        }
    }
}

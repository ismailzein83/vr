using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Caching;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SupplierCodeCachedObjectCreationHandler : CachedObjectCreationHandler<List<SupplierCode>>
    {
        public int _supplierId { get; set; }
        public DateTime _effectiveOn { get; set; }

        public SupplierCodeCachedObjectCreationHandler(int supplierId, DateTime effectiveOn)
        {
            _supplierId = supplierId;
            _effectiveOn = effectiveOn;
        }
        public override List<SupplierCode> CreateObject()
        {
            var cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<SupplierCodeCacheManager>();
            SupplierCodeManager supplierCodeManager = new SupplierCodeManager();
            return supplierCodeManager.GetSupplierCodes(_supplierId, _effectiveOn).Select(code => cacheManager.CacheAndGetCode(code)).ToList();
        }
    }
}
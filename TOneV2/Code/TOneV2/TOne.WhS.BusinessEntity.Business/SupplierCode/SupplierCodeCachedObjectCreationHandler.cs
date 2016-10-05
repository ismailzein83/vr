//using System;
//using System.Collections.Generic;
//using System.Linq;
//using TOne.WhS.BusinessEntity.Entities;
//using Vanrise.Caching;

//namespace TOne.WhS.BusinessEntity.Business
//{
//    public class SupplierCodeCachedObjectCreationHandler : CachedObjectCreationHandler<List<SupplierCode>>
//    {
//        public DateTime _effectiveOn { get; set; }

//        public SupplierCodeCachedObjectCreationHandler(DateTime effectiveOn)
//        {
//            _effectiveOn = effectiveOn;
//        }
//        public override List<SupplierCode> CreateObject()
//        {
//            var cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<SupplierCodeCacheManager>();
//            SupplierCodeManager supplierCodeManager = new SupplierCodeManager();
//            //Dictionary<int, List<SupplierCode>> codesBySupplier = cacheManager.GetOrCreateObject("SupplierCodeCachedObjectCreationHandler",
//            //    () =>
//            //    {
//            //    });
//            return supplierCodeManager.GetSupplierCodes(_effectiveOn).Select(code => cacheManager.CacheAndGetCode(code)).ToList();
//        }
//    }
//}
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using TOne.WhS.BusinessEntity.Data;
//using TOne.WhS.BusinessEntity.Entities;
//using Vanrise.Caching;

//namespace TOne.WhS.BusinessEntity.Business
//{
//    public class SupplierRateCachedObjectCreationHandler : CachedObjectCreationHandler<List<SupplierRate>>
//    {
//        public DateTime _effectiveOn { get; set; }

//        public SupplierRateCachedObjectCreationHandler(DateTime effectiveOn)
//        {
//            _effectiveOn = effectiveOn;
//        }
//        public override List<SupplierRate> CreateObject()
//        {
//            var cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<SupplierRateCacheManager>();
//            ISupplierRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierRateDataManager>();
//            return dataManager.GetEffectiveSupplierRates(_effectiveOn).Select(rate => cacheManager.CacheAndGetRate(rate)).ToList();
//        }
//    }

//    //public class SupplierRateCachedObjectCreationHandler : CachedObjectCreationHandler<List<SupplierRate>>
//    //{
//    //    public DateTime _effectiveOn { get; set; }

//    //    public int _supplierId { get; set; }

//    //    public SupplierRateCachedObjectCreationHandler(int supplierId, DateTime effectiveOn)
//    //    {
//    //        _supplierId = supplierId;
//    //        _effectiveOn = effectiveOn;
//    //    }
//    //    public override List<SupplierRate> CreateObject()
//    //    {
//    //        var cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<SupplierRateCacheManager>();
//    //        Dictionary<int, List<SupplierRate>> ratesBySuppliers = cacheManager.GetOrCreateObject("SupplierRateCachedObjectCreationHandler",
//    //            () =>
//    //            {
//    //                ISupplierRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierRateDataManager>();
//    //                Dictionary<int, List<SupplierRate>> rslt = new Dictionary<int, List<SupplierRate>>();
//    //                var supplierRates = dataManager.GetEffectiveSupplierRates(_effectiveOn).Select(rate => cacheManager.CacheAndGetRate(rate)).ToList(); ;
//    //                var priceLists = new SupplierPriceListManager().GetCachedPriceLists();
//    //                foreach (var rate in supplierRates)
//    //                {
//    //                    var priceList = priceLists.GetRecord(rate.PriceListId);
//    //                    rslt.GetOrCreateItem(priceList.SupplierId).Add(rate);
//    //                }
//    //                return rslt;
//    //            });
//    //        return ratesBySuppliers.GetRecord(_supplierId);
//    //    }
//    //}
//}
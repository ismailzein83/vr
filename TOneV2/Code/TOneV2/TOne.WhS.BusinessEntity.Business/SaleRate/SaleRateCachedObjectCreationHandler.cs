//using System;
//using System.Collections.Generic;
//using System.Linq;
//using TOne.WhS.BusinessEntity.Data;
//using TOne.WhS.BusinessEntity.Entities;
//using Vanrise.Caching;

//namespace TOne.WhS.BusinessEntity.Business
//{
//    public class SaleRateCachedObjectCreationHandler : CachedObjectCreationHandler<List<SaleRate>>
//    {
//        public DateTime _effectiveOn { get; set; }

//        public SaleRateCachedObjectCreationHandler(DateTime effectiveOn)
//        {
//            _effectiveOn = effectiveOn;
//        }
//        public override List<SaleRate> CreateObject()
//        {
//            var cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<SaleRateCacheManager>();
//            ISaleRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleRateDataManager>();
//            return dataManager.GetEffectiveSaleRates(_effectiveOn).Select(rate => cacheManager.CacheAndGetRate(rate)).ToList();
//        }
//    }

//    //public class SaleRateCachedObjectCreationHandler : CachedObjectCreationHandler<List<SaleRate>>
//    //{
//    //    public DateTime _effectiveOn { get; set; }

//    //    public Entities.SalePriceListOwnerType _ownerType { get; set; }

//    //    public int _ownerId { get; set; }

//    //    public SaleRateCachedObjectCreationHandler(Entities.SalePriceListOwnerType ownerType, int ownerId, DateTime effectiveOn)
//    //    {
//    //        _effectiveOn = effectiveOn;
//    //        _ownerType = ownerType;
//    //        _ownerId = ownerId;
//    //    }
//    //    public override List<SaleRate> CreateObject()
//    //    {
//    //        var cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<SaleRateCacheManager>();
//    //        Dictionary<Entities.SalePriceListOwnerType, Dictionary<int, List<SaleRate>>> ratesByOwner = cacheManager.GetOrCreateObject("SaleRateCachedObjectCreationHandler",
//    //            () =>
//    //            {
//    //                ISaleRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleRateDataManager>();
//    //                var saleRates = dataManager.GetEffectiveSaleRates(_effectiveOn).Select(rate => cacheManager.CacheAndGetRate(rate)).ToList();
//    //                Dictionary<Entities.SalePriceListOwnerType, Dictionary<int, List<SaleRate>>> rslt = new Dictionary<SalePriceListOwnerType, Dictionary<int, List<SaleRate>>>();
//    //                rslt.Add(Entities.SalePriceListOwnerType.Customer, new Dictionary<int, List<SaleRate>>());
//    //                rslt.Add(Entities.SalePriceListOwnerType.SellingProduct, new Dictionary<int, List<SaleRate>>());
//    //                var priceLists = new SalePriceListManager().GetCachedSalePriceLists();

//    //                foreach (var rate in saleRates)
//    //                {
//    //                    var priceList = priceLists.GetRecord(rate.PriceListId);
//    //                    rslt[priceList.OwnerType].GetOrCreateItem(priceList.OwnerId).Add(rate);
//    //                }

//    //                return rslt;
//    //            }
//    //           );
//    //        return ratesByOwner[_ownerType].GetRecord(_ownerId);
//    //    }
//    //}
//}
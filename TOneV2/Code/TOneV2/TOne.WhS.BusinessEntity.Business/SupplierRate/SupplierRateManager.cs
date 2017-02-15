using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;
using Vanrise.Common.Business;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SupplierRateManager
    {

        #region Public Methods
        public IDataRetrievalResult<SupplierRateDetail> GetFilteredSupplierRates(DataRetrievalInput<BaseSupplierRateQueryHandler> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new SupplierRateRequestHandler());
        }
        public List<SupplierRate> GetSupplierRatesEffectiveAfter(int supplierId, DateTime minimumDate)
        {
            ISupplierRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierRateDataManager>();
            return dataManager.GetSupplierRates(supplierId, minimumDate);
        }
        //public List<SupplierRate> GetRates(DateTime? effectiveOn, bool isEffectiveInFuture, IEnumerable<RoutingSupplierInfo> supplierInfos)
        //{
        //    ISupplierRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierRateDataManager>();
        //    return dataManager.GetEffectiveSupplierRatesBySuppliers(effectiveOn, isEffectiveInFuture, supplierInfos);
        //}

        public SupplierZoneRate GetCachedSupplierZoneRate(int supplierId, long supplierZoneId, DateTime effectiveOn)
        {
            SupplierZoneRateLocator supplierZoneRateLocator = new SupplierZoneRateLocator(new SupplierRateReadWithCache(effectiveOn));
            return supplierZoneRateLocator.GetSupplierZoneRate(supplierId, supplierZoneId, effectiveOn);
        }
        public RateChangeType GetSupplierRateChange(DateTime fromTime, DateTime tillTime, long rateId)
        {
            Dictionary<long, SupplierRate> supplierRates = GetCachedSupplierRatesInBetweenPeriod(fromTime, tillTime);
            SupplierRate rate;
            if (supplierRates != null)
            {
                supplierRates.TryGetValue(rateId, out rate);
                if (rate != null) return rate.RateChange;
            }
            else supplierRates = new Dictionary<long, SupplierRate>();
            rate = GetSupplierRateByRateId(rateId);
            if (rate != null)
            {
                supplierRates[rate.SupplierRateId] = rate;
                return rate.RateChange;
            }
            throw new ArgumentNullException(String.Format("CostRate: {0}", rateId));
        }

        private SupplierRate GetSupplierRateByRateId(long rateId)
        {
            ISupplierRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierRateDataManager>();
            return dataManager.GetSupplierRateById(rateId);
        }
        private struct GetCachedSupplierInBetweenPeriodCacheName
        {
            public DateTime FromTime { get; set; }
            public DateTime ToTime { get; set; }
        }
        public Dictionary<long, SupplierRate> GetCachedSupplierRatesInBetweenPeriod(DateTime fromTime, DateTime tillTime)
        {
            var cacheName = new GetCachedSupplierInBetweenPeriodCacheName { FromTime = fromTime, ToTime = tillTime };
            var cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<SupplierRateCacheManager>();
            return cacheManager.GetOrCreateObject(cacheName,
               () =>
               {
                   ISupplierRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierRateDataManager>();
                   IEnumerable<SupplierRate> saleRates = dataManager.GetSupplierRatesInBetweenPeriod(fromTime, tillTime);
                   return saleRates.ToDictionary(cn => cn.SupplierRateId, cn => cacheManager.CacheAndGetRate(cn));
               });
        }
        public long ReserveIDRange(int numberOfIDs)
        {
            long startingId;
            IDManager.Instance.ReserveIDRange(GetSupplierRateType(), numberOfIDs, out startingId);
            return startingId;
        }

        public int GetSupplierRateTypeId()
        {
            return Vanrise.Common.Business.TypeManager.Instance.GetTypeId(this.GetSupplierRateType());
        }

        public Type GetSupplierRateType()
        {
            return this.GetType();
        }

        public int GetCurrencyId(SupplierRate supplierRate)
        {
            if (supplierRate == null)
                throw new ArgumentNullException("supplierRate");
            if (supplierRate.CurrencyId.HasValue)
                return supplierRate.CurrencyId.Value;
            var supplierPriceListManager = new SupplierPriceListManager();
            SupplierPriceList supplierPriceList = supplierPriceListManager.GetPriceList(supplierRate.PriceListId);
            if (supplierPriceList == null)
                throw new NullReferenceException(String.Format("supplierPriceList (Id: {0}) does not exist for supplierRate (Id: {1})", supplierRate.SupplierRateId, supplierPriceList.PriceListId));
            return supplierPriceList.CurrencyId;
        }


        #endregion

        #region Private Members

        private string GetCurrencyName(int? currencyId)
        {
            if (currencyId != null)
            {
                CurrencyManager manager = new CurrencyManager();
                Currency currency = manager.GetCurrency(currencyId.Value);

                if (currency != null)
                    return currency.Name;
            }

            return "Currency Not Found";
        }
        private string GetSupplierZoneName(long zoneId)
        {
            SupplierZoneManager manager = new SupplierZoneManager();
            SupplierZone suplierZone = manager.GetSupplierZone(zoneId);

            if (suplierZone != null)
                return suplierZone.Name;

            return "Zone Not Found";
        }
        #endregion

        #region Mappers
        private SupplierRateDetail SupplierRateDetailMapper(SupplierRate supplierRate)
        {
            CurrencyManager currencyManager = new CurrencyManager();
            SupplierPriceListManager manager = new SupplierPriceListManager();
            SupplierPriceList priceList = manager.GetPriceList(supplierRate.PriceListId);
            supplierRate.PriceListFileId = priceList.FileId;

            int currencyId = supplierRate.CurrencyId ?? priceList.CurrencyId;

            return new SupplierRateDetail
            {
                Entity = supplierRate,
                CurrencyName = currencyManager.GetCurrencySymbol(currencyId),
                SupplierZoneName = this.GetSupplierZoneName(supplierRate.ZoneId),
            };
        }
        #endregion

        #region Private Classes

        private class SupplierRateRequestHandler : BigDataRequestHandler<BaseSupplierRateQueryHandler, SupplierRate, SupplierRateDetail>
        {
            public override SupplierRateDetail EntityDetailMapper(SupplierRate entity)
            {
                SupplierRateManager manager = new SupplierRateManager();
                return manager.SupplierRateDetailMapper(entity);
            }

            public override IEnumerable<SupplierRate> RetrieveAllData(DataRetrievalInput<BaseSupplierRateQueryHandler> input)
            {
                return input.Query.GetFilteredSupplierRates();
            }
        }

        #endregion
    }
}

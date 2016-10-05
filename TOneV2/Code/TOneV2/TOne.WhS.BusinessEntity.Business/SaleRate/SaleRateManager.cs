using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SaleRateManager : ISaleEntityZoneRateManager
    {
        #region Public Methods
        public List<SaleRate> GetRates(DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            throw new NotImplementedException();
        }

        public List<SaleRate> GetSaleRatesEffectiveAfter(int sellingNumberPlanId, DateTime minimumDate)
        {
            ISaleRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleRateDataManager>();
            return dataManager.GetSaleRatesEffectiveAfter(sellingNumberPlanId, minimumDate);
        }

        public IEnumerable<SaleRate> GetSaleRatesEffectiveAfter(SalePriceListOwnerType ownerType, int ownerId, DateTime minimumDate)
        {
            ISaleRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleRateDataManager>();
            return dataManager.GetSaleRatesEffectiveAfter(ownerType, ownerId, minimumDate);
        }

        public Vanrise.Entities.IDataRetrievalResult<SaleRateDetail> GetFilteredSaleRates(Vanrise.Entities.DataRetrievalInput<SaleRateQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new SaleRateRequestHandler());
        }
        public RateChangeType GetSaleRateChange(DateTime fromTime, DateTime tillTime, long rateId)
        {
            Dictionary<long, SaleRate> saleRates = GetCachedSaleRatesInBetweenPeriod(fromTime, tillTime);
            SaleRate rate;
            if (saleRates != null)
            {
                saleRates.TryGetValue(rateId, out rate);
                if (rate != null) return rate.RateChange;
            }
            else saleRates = new Dictionary<long, SaleRate>();
            rate = GetSaleRateByRateId(rateId);
            if (rate != null)
            {
                saleRates[rate.SaleRateId] = rate;
                return rate.RateChange;
            }
            throw new ArgumentNullException(String.Format("SaleRate: {0}", rateId));
        }
        private SaleRate GetSaleRateByRateId(long rateId)
        {
            ISaleRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleRateDataManager>();
            return dataManager.GetSaleRateById(rateId);
        }
        public Dictionary<long, SaleRate> GetCachedSaleRatesInBetweenPeriod(DateTime fromTime, DateTime tillTime)
        {
            var cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<SaleRateCacheManager>();
            return cacheManager.GetOrCreateObject("GetSaleRatesInBetweenPeriod",
               () =>
               {
                   ISaleRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleRateDataManager>();
                   IEnumerable<SaleRate> saleRates = dataManager.GetSaleRatesInBetweenPeriod(fromTime, tillTime);
                   return saleRates.ToDictionary(cn => cn.SaleRateId, cn => cacheManager.CacheAndGetRate(cn));
               });
        }

        public SaleEntityZoneRate GetCachedCustomerZoneRate(int customerId, long saleZoneId, DateTime effectiveOn)
        {
            CustomerSellingProductManager customerSellingProductManager = new CustomerSellingProductManager();
            CustomerSellingProduct customerSellingProduct = customerSellingProductManager.GetEffectiveSellingProduct(customerId, effectiveOn, false);
            if (customerSellingProduct == null)
                return null;
            SaleEntityZoneRateLocator customerZoneRateLocator = new SaleEntityZoneRateLocator(new SaleRateReadWithCache(effectiveOn));
            return customerZoneRateLocator.GetCustomerZoneRate(customerId, customerSellingProduct.SellingProductId, saleZoneId);
        }

        public IEnumerable<SaleRate> GetExistingRatesByZoneIds(SalePriceListOwnerType ownerType, int ownerId, IEnumerable<long> zoneIds, DateTime minEED)
        {
            ISaleRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleRateDataManager>();
            return dataManager.GetExistingRatesByZoneIds(ownerType, ownerId, zoneIds, minEED);
        }

        public int GetSaleRateTypeId()
        {
            return Vanrise.Common.Business.TypeManager.Instance.GetTypeId(this.GetSaleRateType());
        }

        public Type GetSaleRateType()
        {
            return this.GetType();
        }

        public long ReserveIdRange(int numberOfIds)
        {
            long startingId;
            IDManager.Instance.ReserveIDRange(GetSaleRateType(), numberOfIds, out startingId);
            return startingId;
        }

        public int GetCurrencyId(SaleRate saleRate)
        {
            if (saleRate == null)
                throw new ArgumentNullException("saleRate");
            if (saleRate.CurrencyId.HasValue)
                return saleRate.CurrencyId.Value;
            var salePriceListManager = new SalePriceListManager();
            SalePriceList salePriceList = salePriceListManager.GetPriceList(saleRate.PriceListId);
            if (salePriceList == null)
                throw new NullReferenceException(String.Format("salePriceList (Id: {0}) does not exist for saleRate (Id: {1})", saleRate.SaleRateId, salePriceList.PriceListId));
            return salePriceList.CurrencyId;
        }

        #endregion

        #region Private Classes

        private class SaleRateRequestHandler : BigDataRequestHandler<SaleRateQuery, SaleRateDetail, SaleRateDetail>
        {
            #region Fields

            private SaleZoneManager _saleZoneManager = new SaleZoneManager();
            private CurrencyManager _currencyManager = new CurrencyManager();
            private Vanrise.Common.Business.RateTypeManager _rateTypeManager = new Vanrise.Common.Business.RateTypeManager();
            private CurrencyExchangeRateManager _currencyExchangeRateManager = new CurrencyExchangeRateManager();
            private SaleRateManager _saleRateManager = new SaleRateManager();

            #endregion

            public override IEnumerable<SaleRateDetail> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<SaleRateQuery> input)
            {
                var saleZoneManager = new SaleZoneManager();
                IEnumerable<SaleZone> saleZones = (input.Query.SellingNumberPlanId.HasValue) ?
                    saleZoneManager.GetSaleZonesByOwner(input.Query.OwnerType, input.Query.OwnerId, input.Query.SellingNumberPlanId.Value, input.Query.EffectiveOn, false) :
                    saleZoneManager.GetSaleZonesByOwner(input.Query.OwnerType, input.Query.OwnerId, input.Query.EffectiveOn, false);

                List<SaleRateDetail> ratesFormatted = new List<SaleRateDetail>();
                DateTime? rateConversionEffectiveDate = null;
                if (input.Query.CurrencyId.HasValue)
                    rateConversionEffectiveDate = input.Query.EffectiveOn;

                var rateLocator = new SaleEntityZoneRateLocator(new SaleRateReadWithCache(input.Query.EffectiveOn));

               
                if (saleZones != null)
                {
                    var filteredSaleZone = saleZones.FindAllRecords(sz => input.Query.ZonesIds == null || input.Query.ZonesIds.Contains(sz.SaleZoneId));
                    if (input.Query.OwnerType == SalePriceListOwnerType.SellingProduct)
                    {
                        foreach (SaleZone saleZone in filteredSaleZone)
                        {
                            SaleEntityZoneRate rate = rateLocator.GetSellingProductZoneRate(input.Query.OwnerId, saleZone.SaleZoneId);
                            if (rate != null)
                            {
                                SaleRateDetail saleRateDetail = SaleRateDetailMapper(rate.Rate, input.Query.CurrencyId, rateConversionEffectiveDate, false);
                                saleRateDetail.OtherRates = (rate.RatesByRateType != null) ? rate.RatesByRateType.Values.MapRecords((itm) => SaleOtherRateDetailMapper(itm, input.Query.CurrencyId, rateConversionEffectiveDate, false)).ToList() : null;
                                ratesFormatted.Add(saleRateDetail);
                            }

                        }
                    }
                    else
                    {
                        int? sellingProductId = null;

                        var customerSellingProductManager = new CustomerSellingProductManager();
                        sellingProductId = customerSellingProductManager.GetEffectiveSellingProductId(input.Query.OwnerId, input.Query.EffectiveOn, false);
                        if (!sellingProductId.HasValue)
                            throw new DataIntegrityValidationException(string.Format("Customer with Id {0} is not assigned to any selling product", input.Query.OwnerId));

                        foreach (SaleZone saleZone in filteredSaleZone)
                        {
                            SaleEntityZoneRate rate = rateLocator.GetCustomerZoneRate(input.Query.OwnerId, sellingProductId.Value, saleZone.SaleZoneId);
                            if (rate != null)
                            {
                                SaleRateDetail saleRateDetail = SaleRateDetailMapper(rate.Rate, input.Query.CurrencyId, rateConversionEffectiveDate, rate.Source == SalePriceListOwnerType.SellingProduct);
                                if (rate.RatesByRateType != null)
                                {
                                    if (rate.SourcesByRateType == null)
                                        throw new NullReferenceException("SourcesByRateType");

                                    saleRateDetail.OtherRates = new List<SaleOtherRateDetail>();
                                    foreach (var otherRate in rate.RatesByRateType)
                                    {
                                        SalePriceListOwnerType otherRateSource;
                                        if (!rate.SourcesByRateType.TryGetValue(otherRate.Key, out otherRateSource))
                                            throw new NullReferenceException("otherRateSource");

                                        saleRateDetail.OtherRates.Add(SaleOtherRateDetailMapper(otherRate.Value, input.Query.CurrencyId, rateConversionEffectiveDate, otherRateSource == SalePriceListOwnerType.SellingProduct));
                                    }
                                }
                                ratesFormatted.Add(saleRateDetail);
                            }
                        }
                    }
                }            
                return ratesFormatted;
            }



            //protected override BigResult<SaleRateDetail> AllRecordsToBigResult(DataRetrievalInput<SaleRateQuery> input, IEnumerable<SaleRate> allRecords)
            //{
            //    DateTime? rateConversionEffectiveDate = null;
            //    if (input.Query.CurrencyId.HasValue)
            //        rateConversionEffectiveDate = (input.Query.EffectiveOn.HasValue) ? input.Query.EffectiveOn.Value : DateTime.Now;
            //    return allRecords.ToBigResult(input, null, x => ConvertRateToCurrency(SaleRateDetailMapper(x), input.Query.CurrencyId, rateConversionEffectiveDate));
            //}
            protected SaleRateDetail SaleRateDetailMapper(SaleRate entity, int? targetCurrencyId, DateTime? rateConversionEffectiveDate, bool isRateInherited)
            {
                SaleRateDetail saleRateDetail = new SaleRateDetail();

                saleRateDetail.Entity = entity;
                saleRateDetail.ZoneName = _saleZoneManager.GetSaleZoneName(entity.ZoneId);
                saleRateDetail.IsRateInherited = isRateInherited;
                saleRateDetail.ConvertedRate = GetConvertedRate(entity, targetCurrencyId, rateConversionEffectiveDate);
                saleRateDetail.CurrencyName = GetCurrencyName(entity, targetCurrencyId);

                return saleRateDetail;
            }

            protected SaleOtherRateDetail SaleOtherRateDetailMapper(SaleRate entity, int? targetCurrencyId, DateTime? rateConversionEffectiveDate, bool isRateInherited)
            {
                SaleOtherRateDetail saleOtherRateDetail = new SaleOtherRateDetail();

                saleOtherRateDetail.Entity = entity;
                saleOtherRateDetail.ZoneName = _saleZoneManager.GetSaleZoneName(entity.ZoneId);
                saleOtherRateDetail.IsRateInherited = isRateInherited;
                saleOtherRateDetail.ConvertedRate = GetConvertedRate(entity, targetCurrencyId, rateConversionEffectiveDate);
                saleOtherRateDetail.CurrencyName = GetCurrencyName(entity, targetCurrencyId);
                
                if (entity.RateTypeId.HasValue)
                    saleOtherRateDetail.RateTypeName = _rateTypeManager.GetRateTypeName(entity.RateTypeId.Value);

                return saleOtherRateDetail;
            }
            
            public override SaleRateDetail EntityDetailMapper(SaleRateDetail entity)
            {
                //SaleRateDetail saleRateDetail = new SaleRateDetail();

                //saleRateDetail.Entity = entity;
                //saleRateDetail.ZoneName = _saleZoneManager.GetSaleZoneName(entity.ZoneId);

                //if (entity.RateTypeId.HasValue)
                //    saleRateDetail.RateTypeName = _rateTypeManager.GetRateTypeName(entity.RateTypeId.Value);

                return entity ;
            }

            private Decimal GetConvertedRate(SaleRate saleRate, int? targetCurrencyId, DateTime? rateConversionEffectiveDate)
            {
                int currencyId = _saleRateManager.GetCurrencyId(saleRate);
                Decimal convertedRate = 0;

                if (targetCurrencyId.HasValue)
                    convertedRate = _currencyExchangeRateManager.ConvertValueToCurrency(saleRate.NormalRate, currencyId, targetCurrencyId.Value, rateConversionEffectiveDate.Value);
                else
                    convertedRate = saleRate.NormalRate;
                
                return convertedRate;
            }

            private string GetCurrencyName(SaleRate saleRate, int? targetCurrencyId)
            {
                int currencyId = targetCurrencyId.HasValue ? targetCurrencyId.Value : _saleRateManager.GetCurrencyId(saleRate);
                return _currencyManager.GetCurrencySymbol(currencyId);
            }
        }

        #endregion
    }
}

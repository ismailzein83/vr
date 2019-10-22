using System;
using System.Collections.Generic;
using System.Linq;
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

        public DateTime? GetNextOpenOrCloseTime(DateTime effectiveDate)
        {
            ISaleRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleRateDataManager>();
            return dataManager.GetNextOpenOrCloseTime(effectiveDate);
        }

        public object GetMaximumTimeStamp()
        {
            ISaleRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleRateDataManager>();
            return dataManager.GetMaximumTimeStamp();
        }

        public List<SaleRate> GetRates(DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            throw new NotImplementedException();
        }

        public List<SaleRate> GetSaleRatesEffectiveAfter(int sellingNumberPlanId, DateTime minimumDate)
        {
            ISaleRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleRateDataManager>();
            List<SaleRate> allSaleRates = dataManager.GetSaleRatesEffectiveAfter(sellingNumberPlanId, minimumDate);

            List<SaleRate> saleRates = new List<SaleRate>();
            SalePriceListManager salePriceListManager = new SalePriceListManager();

            foreach (SaleRate item in allSaleRates)
            {
                if (!salePriceListManager.IsSalePriceListDeleted(item.PriceListId))
                    saleRates.Add(item);
            }

            return saleRates;
        }

        public IEnumerable<SaleRate> GetSaleRatesEffectiveAfter(SalePriceListOwnerType ownerType, int ownerId, DateTime minimumDate)
        {
            ISaleRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleRateDataManager>();
            return dataManager.GetSaleRatesEffectiveAfter(ownerType, ownerId, minimumDate);
        }

        public Vanrise.Entities.IDataRetrievalResult<SaleRateDetail> GetFilteredSaleRates(Vanrise.Entities.DataRetrievalInput<SaleRateQuery> input)
        {
            VRActionLogger.Current.LogGetFilteredAction(SaleRateLoggableEntity.Instance, input);
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
            else return RateChangeType.RateNotAvailable;
        }

        private SaleRate GetSaleRateByRateId(long rateId)
        {
            ISaleRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleRateDataManager>();
            return dataManager.GetSaleRateById(rateId);
        }

        private struct GetCachedSaleRatesInBetweenPeriodCacheName
        {
            public DateTime FromTime { get; set; }
            public DateTime ToTime { get; set; }
        }

        public Dictionary<long, SaleRate> GetCachedSaleRatesInBetweenPeriod(DateTime fromTime, DateTime tillTime)
        {
            var cacheName = new GetCachedSaleRatesInBetweenPeriodCacheName { FromTime = fromTime, ToTime = tillTime };
            var cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<SaleRateCacheManager>();
            return cacheManager.GetOrCreateObject(cacheName,
               () =>
               {
                   ISaleRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleRateDataManager>();
                   IEnumerable<SaleRate> saleRates = dataManager.GetSaleRatesInBetweenPeriod(fromTime, tillTime);
                   return saleRates.ToDictionary(cn => cn.SaleRateId, cn => cacheManager.CacheAndGetRate(cn));
               });
        }

        public SaleEntityZoneRate GetCachedCustomerZoneRate(int customerId, long saleZoneId, DateTime effectiveOn)
        {
            if (!new SaleZoneManager().IsSaleZoneSoldToCustomer(customerId, saleZoneId, effectiveOn))
                return null;

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

        public string GetSaleRateSourceId(SaleRate saleRate)
        {
            if (saleRate == null)
                return null;
            return saleRate.SourceId;
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
                throw new NullReferenceException(String.Format("salePriceList (Id: {0}) does not exist for saleRate (Id: {1})", saleRate.SaleRateId, saleRate.PriceListId));
            return salePriceList.CurrencyId;
        }

        public IEnumerable<SaleRate> GetZoneRateBySellingProduct(int sellingProductId, List<long> zoneIds, DateTime effectiveOn)
        {
            var dataManager = BEDataManagerFactory.GetDataManager<ISaleRateDataManager>();
            return dataManager.GetSaleRatesEffectiveAfterByOwnerAndZones(SalePriceListOwnerType.SellingProduct, sellingProductId, zoneIds, effectiveOn);
        }

        public OverlappedRatesByZone GetCustomerOverlappedRatesByZone(int customerId, IEnumerable<long> zoneIds, DateTime effectiveOn)
        {
            if (zoneIds == null || zoneIds.Count() == 0)
                return null;

            var dataManager = BEDataManagerFactory.GetDataManager<ISaleRateDataManager>();
            IEnumerable<SaleRate> rates = dataManager.GetSaleRatesEffectiveAfterByOwnerAndZones(SalePriceListOwnerType.Customer, customerId, zoneIds, effectiveOn);

            if (rates == null || rates.Count() == 0)
                return null;

            var overlappedRatesByZone = new OverlappedRatesByZone();

            foreach (SaleRate rate in rates)
            {
                ZoneOverlappedRates zoneOverlappedRates;

                if (!overlappedRatesByZone.TryGetValue(rate.ZoneId, out zoneOverlappedRates))
                {
                    zoneOverlappedRates = new ZoneOverlappedRates()
                    {
                        NormalRates = new List<SaleRate>(),
                        OtherRatesByType = new Dictionary<int, List<SaleRate>>()
                    };
                    overlappedRatesByZone.Add(rate.ZoneId, zoneOverlappedRates);
                }

                if (rate.RateTypeId.HasValue)
                {
                    List<SaleRate> otherRatesByType;
                    if (!zoneOverlappedRates.OtherRatesByType.TryGetValue(rate.RateTypeId.Value, out otherRatesByType))
                    {
                        otherRatesByType = new List<SaleRate>();
                        zoneOverlappedRates.OtherRatesByType.Add(rate.RateTypeId.Value, otherRatesByType);
                    }
                    otherRatesByType.Add(rate);
                }
                else
                    zoneOverlappedRates.NormalRates.Add(rate);
            }

            return overlappedRatesByZone;
        }

        public void ProcessBaseRatesByZone(int customerId, BaseRatesByZone baseRatesByZone, IEnumerable<CustomerCountry2> soldCountries)
        {
            if (baseRatesByZone.Count == 0)
                return;

            var saleZoneManager = new SaleZoneManager();

            IEnumerable<long> zoneIds = baseRatesByZone.Values.MapRecords(x => x.ZoneId);
            DateTime minimumBED = baseRatesByZone.GetMinimumBED();

            OverlappedRatesByZone overlappedRatesByZone = new SaleRateManager().GetCustomerOverlappedRatesByZone(customerId, zoneIds, minimumBED);
            Dictionary<int, DateTime> sellDatesByCountry = StructureSellDatesByCountry(soldCountries);

            foreach (BaseRates zoneBaseRates in baseRatesByZone.Values)
            {
                ZoneOverlappedRates zoneOverlappedRates = overlappedRatesByZone.GetRecord(zoneBaseRates.ZoneId);
                DateTime soldOn = sellDatesByCountry.GetRecord(zoneBaseRates.CountryId);

                if (zoneBaseRates.BaseNormalRate != null)
                {
                    DateTime baseNormalRateBED = saleZoneManager.GetCustomerInheritedZoneRateBED(null, zoneOverlappedRates, zoneBaseRates.BaseNormalRate.BED, zoneBaseRates.BaseNormalRate.EED, soldOn);
                    zoneBaseRates.Entity.SetNormalRateBED(baseNormalRateBED);
                }

                if (zoneBaseRates.BaseOtherRates.Count > 0)
                {
                    foreach (BaseRate baseOtherRate in zoneBaseRates.BaseOtherRates.Values)
                    {
                        DateTime baseOtherRateBED = saleZoneManager.GetCustomerInheritedZoneRateBED(baseOtherRate.RateTypeId.Value, zoneOverlappedRates, baseOtherRate.BED, baseOtherRate.EED, soldOn);
                        zoneBaseRates.Entity.SetOtherRateBED(baseOtherRate.RateTypeId.Value, baseOtherRateBED);
                    }
                }
            }
        }

        public IEnumerable<SaleRate> GetZoneRatesBySellingProduct(IEnumerable<long> saleZoneIds, int sellingProductId)
        {
            if (saleZoneIds == null || saleZoneIds.Count() == 0)
                throw new Vanrise.Entities.MissingArgumentValidationException("saleZoneIds");

            throw new NotImplementedException();
        }

        public Dictionary<int, List<SaleRate>> GetZoneRatesBySellingProducts(IEnumerable<long> saleZoneIds, IEnumerable<int> sellingProductIds)
        {
            if (saleZoneIds == null || saleZoneIds.Count() == 0)
                throw new Vanrise.Entities.MissingArgumentValidationException("saleZoneIds");

            if (sellingProductIds == null || sellingProductIds.Count() == 0)
                throw new Vanrise.Entities.MissingArgumentValidationException("sellingProductIds");

            ISaleRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleRateDataManager>();
            IEnumerable<SaleRate> saleRates = dataManager.GetZoneRatesBySellingProducts(sellingProductIds, saleZoneIds);

            if (saleRates == null || saleRates.Count() == 0)
                return null;

            var saleRatesBySellingProductId = new Dictionary<int, List<SaleRate>>();
            Dictionary<int, SalePriceList> salePriceListsById = new SalePriceListManager().GetCachedSalePriceLists();

            foreach (SaleRate saleRate in saleRates.OrderBy(x => x.BED))
            {
                SalePriceList salePriceList = salePriceListsById.GetRecord(saleRate.PriceListId);

                if (salePriceList == null || salePriceList.OwnerType != SalePriceListOwnerType.SellingProduct || !sellingProductIds.Contains(salePriceList.OwnerId))
                    continue;

                List<SaleRate> sellingProductSaleRates;

                if (!saleRatesBySellingProductId.TryGetValue(salePriceList.OwnerId, out sellingProductSaleRates))
                {
                    sellingProductSaleRates = new List<SaleRate>();
                    saleRatesBySellingProductId.Add(salePriceList.OwnerId, sellingProductSaleRates);
                }

                sellingProductSaleRates.Add(saleRate);
            }

            return saleRatesBySellingProductId;
        }

        public IEnumerable<SaleRate> GetAllSaleRatesByOwner(SalePriceListOwnerType ownerType, int ownerId, IEnumerable<long> saleZoneIds, bool getNormalRates, bool getOtherRates)
        {
            ISaleRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleRateDataManager>();
            return dataManager.GetAllSaleRatesByOwner(ownerType, ownerId, saleZoneIds, getNormalRates, getOtherRates);
        }

        public IEnumerable<SaleRate> GetAllSaleRatesBySellingProductAndCustomer(IEnumerable<long> saleZoneIds, int sellingProductId, int customerId, bool getNormalRates, bool getOtherRates)
        {
            ISaleRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleRateDataManager>();
            return dataManager.GetAllSaleRatesBySellingProductAndCustomer(saleZoneIds, sellingProductId, customerId, getNormalRates, getOtherRates);
        }

        public IEnumerable<SaleRate> GetSaleRatesEffectiveAfterByOwnersAndZones(SalePriceListOwnerType ownerType, IEnumerable<int> ownerIds, IEnumerable<long> zoneIds, DateTime minimumDate)
        {
            var dataManager = BEDataManagerFactory.GetDataManager<ISaleRateDataManager>();
            return dataManager.GetSaleRatesEffectiveAfterByOwnersAndZones(ownerType, ownerIds, zoneIds, minimumDate);
        }

        #endregion

        #region Private Classes

        private class SaleRateRequestHandler : BigDataRequestHandler<SaleRateQuery, SaleRateDetail, SaleRateDetail>
        {
            #region Fields

            private SaleZoneManager _saleZoneManager;
            private CurrencyManager _currencyManager;
            private Vanrise.Common.Business.RateTypeManager _rateTypeManager;
            private CurrencyExchangeRateManager _currencyExchangeRateManager;
            private SaleRateManager _saleRateManager;
            private Vanrise.Common.Business.ConfigManager _configManager;

            #endregion

            public SaleRateRequestHandler()
            {
                _saleZoneManager = new SaleZoneManager();
                _currencyManager = new CurrencyManager();
                _rateTypeManager = new Vanrise.Common.Business.RateTypeManager();
                _currencyExchangeRateManager = new CurrencyExchangeRateManager();
                _saleRateManager = new SaleRateManager();
                _configManager = new Vanrise.Common.Business.ConfigManager();
            }

            public override IEnumerable<SaleRateDetail> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<SaleRateQuery> input)
            {
                IEnumerable<long> saleZoneIds;
                IEnumerable<SaleZone> saleZones = GetFilteredOwnerSaleZones(input.Query.SellingNumberPlanId, input.Query.OwnerType, input.Query.OwnerId, input.Query.EffectiveOn, input.Query.CountriesIds, input.Query.SaleZoneName, out saleZoneIds);

                if (saleZones == null || saleZones.Count() == 0)
                    return null;

                if (!input.Query.CurrencyId.HasValue)
                    throw new Vanrise.Entities.MissingArgumentValidationException("input.Query.CurrencyId");
                int currencyId = input.Query.IsSystemCurrency ? new Vanrise.Common.Business.ConfigManager().GetSystemCurrencyId() : input.Query.CurrencyId.Value;

                int longPrecisionValue = new Vanrise.Common.Business.GeneralSettingsManager().GetLongPrecisionValue();
                if (input.Query.OwnerType == SalePriceListOwnerType.SellingProduct)
                    return GetSellingProductZoneRates(input.Query.OwnerId, saleZones, saleZoneIds, input.Query.EffectiveOn, currencyId, input.Query.IsSystemCurrency, longPrecisionValue);
                else
                    return GetCustomerSaleZoneRates(input.Query.OwnerId, saleZones, saleZoneIds, input.Query.EffectiveOn, currencyId, input.Query.IsSystemCurrency, longPrecisionValue);
            }

            public override SaleRateDetail EntityDetailMapper(SaleRateDetail entity)
            {
                return entity;
            }

            #region Private / Protected Methods

            private IEnumerable<SaleZone> GetFilteredOwnerSaleZones(int? sellingNumberPlanId, SalePriceListOwnerType ownerType, int ownerId, DateTime effectiveOn, IEnumerable<int> countriesIds, string saleZoneName, out IEnumerable<long> filteredSaleZoneIds)
            {
                var saleZoneManager = new SaleZoneManager();
                filteredSaleZoneIds = null;

                IEnumerable<SaleZone> saleZones =
                    (sellingNumberPlanId.HasValue) ? saleZoneManager.GetSaleZonesByOwner(ownerType, ownerId, sellingNumberPlanId.Value, effectiveOn, false) : saleZoneManager.GetSaleZonesByOwner(ownerType, ownerId, effectiveOn, false);

                if (saleZones == null || saleZones.Count() == 0)
                    return null;

                IEnumerable<SaleZone> filteredSaleZones = saleZones.FindAllRecords(x =>
                    (countriesIds == null || countriesIds.Contains(x.CountryId))
                    && (saleZoneName == null || x.Name.ToLower().Contains(saleZoneName.ToLower()))
                    );

                if (filteredSaleZones == null || filteredSaleZones.Count() == 0)
                    return null;

                filteredSaleZoneIds = filteredSaleZones.MapRecords(x => x.SaleZoneId);
                return filteredSaleZones;
            }

            private IEnumerable<SaleRateDetail> GetSellingProductZoneRates(int sellingProductId, IEnumerable<SaleZone> saleZones, IEnumerable<long> saleZoneIds, DateTime effectiveOn, int currencyId, bool isSystemCurrency, int longPrecisionValue)
            {
                var saleRates = new List<SaleRateDetail>();

                IEnumerable<int> sellingProductIds = new List<int>() { sellingProductId };
                var productZoneRateHistoryLocator = new ProductZoneRateHistoryLocator(new ProductZoneRateHistoryReader(sellingProductIds, saleZoneIds, true, false));

                foreach (SaleZone saleZone in saleZones)
                {
                    SaleRateHistoryRecord saleRateHistoryRecord = productZoneRateHistoryLocator.GetProductZoneRateHistoryRecord(sellingProductId, saleZone.Name, null, currencyId, effectiveOn);

                    if (saleRateHistoryRecord != null)
                    {
                        SaleRateDetail saleRate = GetSaleRateDetail(SalePriceListOwnerType.SellingProduct, saleZone, saleRateHistoryRecord, null, isSystemCurrency, longPrecisionValue);
                        saleRates.Add(saleRate);
                    }
                }

                return saleRates;
            }

            private IEnumerable<SaleRateDetail> GetCustomerSaleZoneRates(int customerId, IEnumerable<SaleZone> saleZones, IEnumerable<long> saleZoneIds, DateTime effectiveOn, int currencyId, bool isSystemCurrency, int longPrecisionValue)
            {
                var saleRates = new List<SaleRateDetail>();

                int sellingProductId = new CarrierAccountManager().GetSellingProductId(customerId);
                var customerZoneRateHistoryLocator = new CustomerZoneRateHistoryLocator(new CustomerZoneRateHistoryReader(new List<int>() { customerId }, new List<int>() { sellingProductId }, saleZoneIds, true, false));

                var salePriceListManager = new SalePriceListManager();

                foreach (SaleZone saleZone in saleZones)
                {
                    SaleRateHistoryRecord saleRateHistoryRecord = customerZoneRateHistoryLocator.GetCustomerZoneRateHistoryRecord(customerId, sellingProductId, saleZone.Name, null, saleZone.CountryId, effectiveOn, currencyId, longPrecisionValue);

                    if (saleRateHistoryRecord != null)
                    {
                        SaleRateDetail saleRate = GetSaleRateDetail(SalePriceListOwnerType.Customer, saleZone, saleRateHistoryRecord, salePriceListManager, isSystemCurrency, longPrecisionValue);
                        saleRates.Add(saleRate);
                    }
                }

                return saleRates;
            }

            private SaleRateDetail GetSaleRateDetail(SalePriceListOwnerType ownerType, SaleZone saleZone, SaleRateHistoryRecord saleRateHistoryRecord, SalePriceListManager salePriceListManager, bool isSystemCurrency, int longPrecisionValue)
            {
                var saleRateDetail = new SaleRateDetail();

                saleRateDetail.Entity = new SaleRate()
                {
                    SaleRateId = saleRateHistoryRecord.SaleRateId,
                    RateTypeId = null,
                    ZoneId = saleZone.SaleZoneId,
                    PriceListId = saleRateHistoryRecord.PriceListId,
                    CurrencyId = saleRateHistoryRecord.CurrencyId, // CustomerZoneRateHistoryLocator gets the pricelist's currency if the rate's currency was not found
                    Rate = saleRateHistoryRecord.Rate,
                    BED = saleRateHistoryRecord.BED,
                    EED = saleRateHistoryRecord.EED,
                    SourceId = saleRateHistoryRecord.SourceId,
                    RateChange = saleRateHistoryRecord.ChangeType,
                    Note = saleRateHistoryRecord.Note
                };

                saleRateDetail.ZoneName = saleZone.Name;
                saleRateDetail.CountryId = saleZone.CountryId;
                saleRateDetail.RateTypeName = null;
                int displayedCurrencyId = (isSystemCurrency) ? _configManager.GetSystemCurrencyId() : saleRateHistoryRecord.CurrencyId;
                saleRateDetail.DisplayedCurrency = _currencyManager.GetCurrencySymbol(displayedCurrencyId);

                saleRateDetail.DisplayedRate = (isSystemCurrency) ? saleRateHistoryRecord.ConvertedRate : saleRateHistoryRecord.Rate;
                saleRateDetail.DisplayedRate = decimal.Round(saleRateDetail.DisplayedRate, longPrecisionValue);
                saleRateDetail.IsRateInherited = saleRateHistoryRecord.SellingProductId.HasValue;

                if (ownerType == SalePriceListOwnerType.Customer && !saleRateDetail.IsRateInherited)
                {
                    SalePriceList salePriceList = salePriceListManager.GetPriceList(saleRateDetail.Entity.PriceListId);
                    if (salePriceList == null)
                        throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("Pricelist of rate '{0}' of zone '{1}' was not found", saleRateDetail.Entity.SaleRateId, saleRateDetail.ZoneName));
                    saleRateDetail.PriceListFileId = salePriceList.FileId;
                }

                return saleRateDetail;
            }

            protected override ResultProcessingHandler<SaleRateDetail> GetResultProcessingHandler(DataRetrievalInput<SaleRateQuery> input, BigResult<SaleRateDetail> bigResult)
            {
                return new ResultProcessingHandler<SaleRateDetail>
                {
                    ExportExcelHandler = new SaleRateDetailExportExcelHandler(input.Query)
                };
            }

            #endregion
        }

        private class SaleRateLoggableEntity : VRLoggableEntityBase
        {
            public static SaleRateLoggableEntity Instance = new SaleRateLoggableEntity();

            private SaleRateLoggableEntity()
            {

            }

            static SaleRateManager s_saleRateManager = new SaleRateManager();

            public override string EntityUniqueName
            {
                get { return "WhS_BusinessEntity_SaleRate"; }
            }

            public override string ModuleName
            {
                get { return "Business Entity"; }
            }

            public override string EntityDisplayName
            {
                get { return "Sale Rate"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "WhS_BusinessEntity_SaleRate_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                SaleRate saleRate = context.Object.CastWithValidate<SaleRate>("context.Object");
                return saleRate.SaleRateId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                return null;
            }
        }

        #endregion

        #region Private Methods

        private Dictionary<int, DateTime> StructureSellDatesByCountry(IEnumerable<CustomerCountry2> soldCountries)
        {
            if (soldCountries == null || soldCountries.Count() == 0)
                return null;

            var sellDatesByCountry = new Dictionary<int, DateTime>();

            foreach (CustomerCountry2 soldCountry in soldCountries)
            {
                if (!sellDatesByCountry.ContainsKey(soldCountry.CountryId))
                    sellDatesByCountry.Add(soldCountry.CountryId, soldCountry.BED);
            }

            return sellDatesByCountry;
        }

        private class SaleRateDetailExportExcelHandler : ExcelExportHandler<SaleRateDetail>
        {
            SaleRateQuery query;
            public SaleRateDetailExportExcelHandler(SaleRateQuery query)
            {
                this.query = query;
            }
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<SaleRateDetail> context)
            {
                IEnumerable<int> rateTypeIds = null;
                List<RateItem> rateItems = GetRateItems(context);
                bool isCustomer = query.OwnerType == SalePriceListOwnerType.Customer;

                var rateTypeManager = new RateTypeManager();

                var sheet = new ExportExcelSheet
                {
                    SheetName = "Sales Rates",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
                };
                if (query.ColumnsToShow != null && query.ColumnsToShow.Count > 0)
                {
                    if (query.ColumnsToShow.Contains("Zone"))
                        sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Zone" });

                    sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Codes" });
                    sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Code Group" });
                    sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Country" });

                    if (query.ColumnsToShow.Contains("Rate"))
                        sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Rate", CellType = ExcelCellType.Number, NumberType = NumberType.LongDecimal });
                    if (query.ColumnsToShow.Contains("RateChange"))
                        sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Rate Change" });
                    if (isCustomer && query.ColumnsToShow.Contains("RateInherited"))
                        sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Rate Inherited" });
                    if (query.ColumnsToShow.Contains("PricelistId"))
                        sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Pricelist Id" });
                    if (query.ColumnsToShow.Contains("Currency"))
                        sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Currency" });
                    if (query.ColumnsToShow.Contains("Rate BED"))
                        sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "BED", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.Date });
                    if (query.ColumnsToShow.Contains("Rate EED"))
                        sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "EED", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.Date });

                    sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Services" });
                    if (isCustomer)
                    {
                        rateTypeIds = Helper.GetRateTypeIds(query.OwnerId, DateTime.Now);
                        foreach (var rateTypeId in rateTypeIds)
                        {
                            var rateType = rateTypeManager.GetRateType(rateTypeId);
                            sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = rateType.Name });
                        }
                        if (query.ColumnsToShow.Contains("Note"))
                            sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Note" });
                    }

                    sheet.Rows = new List<ExportExcelRow>();

                    foreach (var record in rateItems)
                    {
                        var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                        if (query.ColumnsToShow.Contains("Zone"))
                            row.Cells.Add(new ExportExcelCell { Value = record.ZoneName });

                        row.Cells.Add(new ExportExcelCell { Value = record.Codes });
                        row.Cells.Add(new ExportExcelCell { Value = record.CodeGroupsId });
                        row.Cells.Add(new ExportExcelCell { Value = record.CountryNames });

                        if (query.ColumnsToShow.Contains("Rate"))
                            row.Cells.Add(new ExportExcelCell { Value = record.Rate });
                        if (query.ColumnsToShow.Contains("RateChange"))
                            row.Cells.Add(new ExportExcelCell { Value = record.ChangeDescription });
                        if (isCustomer && query.ColumnsToShow.Contains("RateInherited"))
                            row.Cells.Add(new ExportExcelCell { Value = record.RateInherited });
                        if (query.ColumnsToShow.Contains("PricelistId"))
                            row.Cells.Add(new ExportExcelCell { Value = record.PriceListId });
                        if (query.ColumnsToShow.Contains("Currency"))
                            row.Cells.Add(new ExportExcelCell { Value = record.CurrencySymbol });
                        if (query.ColumnsToShow.Contains("Rate BED"))
                            row.Cells.Add(new ExportExcelCell { Value = record.RateBED });
                        if (query.ColumnsToShow.Contains("Rate EED"))
                            row.Cells.Add(new ExportExcelCell { Value = record.RateEED });

                        row.Cells.Add(new ExportExcelCell { Value = record.ServicesSymbol });
                        if (isCustomer)
                        {
                            if (rateTypeIds != null)
                                foreach (var rateTypeId in rateTypeIds)
                                {
                                    if (record.RatesByRateType != null && record.RatesByRateType.TryGetValue(rateTypeId, out var otherRateHistory))
                                    {
                                        row.Cells.Add(new ExportExcelCell { Value = otherRateHistory.ConvertedRate });
                                    }
                                    else row.Cells.Add(new ExportExcelCell { Value = string.Empty });
                                }
                            if (query.ColumnsToShow.Contains("Note"))
                                row.Cells.Add(new ExportExcelCell { Value = record.Note });
                        }
                        sheet.Rows.Add(row);
                    }
                }

                else
                {
                    sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Zone" });
                    sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Codes" });
                    sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Code Group" });
                    sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Country" });
                    sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Rate", CellType = ExcelCellType.Number, NumberType = NumberType.LongDecimal });
                    sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Rate Change" });
                    if (isCustomer) sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Rate Inherited" });
                    sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Pricelist Id" });
                    sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Currency" });
                    sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Rate BED", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.Date });
                    sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Rate EED", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.Date });
                    sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Services" });
                    if (isCustomer)
                    {
                        if (rateTypeIds != null)
                            foreach (var rateTypeId in rateTypeIds)
                            {
                                var rateType = rateTypeManager.GetRateType(rateTypeId);
                                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = rateType.Name });
                            }
                        sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Note" });
                    }
                    sheet.Rows = new List<ExportExcelRow>();

                    foreach (var record in rateItems)
                    {
                        var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                        row.Cells.Add(new ExportExcelCell { Value = record.ZoneName });
                        row.Cells.Add(new ExportExcelCell { Value = record.Codes });
                        row.Cells.Add(new ExportExcelCell { Value = record.CodeGroupsId });
                        row.Cells.Add(new ExportExcelCell { Value = record.CountryNames });
                        row.Cells.Add(new ExportExcelCell { Value = record.Rate });
                        row.Cells.Add(new ExportExcelCell { Value = record.ChangeDescription });
                        if (isCustomer) row.Cells.Add(new ExportExcelCell { Value = record.RateInherited });
                        row.Cells.Add(new ExportExcelCell { Value = record.PriceListId });
                        row.Cells.Add(new ExportExcelCell { Value = record.CurrencySymbol });
                        row.Cells.Add(new ExportExcelCell { Value = record.RateBED });
                        row.Cells.Add(new ExportExcelCell { Value = record.RateEED });
                        row.Cells.Add(new ExportExcelCell { Value = record.ServicesSymbol });
                        if (isCustomer)
                        {
                            if (rateTypeIds != null)
                                foreach (var rateTypeId in rateTypeIds)
                                {
                                    if (record.RatesByRateType != null && record.RatesByRateType.TryGetValue(rateTypeId, out var otherRateHistory))
                                    {
                                        row.Cells.Add(new ExportExcelCell { Value = otherRateHistory.ConvertedRate });
                                    }
                                    else row.Cells.Add(new ExportExcelCell { Value = record.Rate });
                                }
                            row.Cells.Add(new ExportExcelCell { Value = record.Note });
                        }
                        sheet.Rows.Add(row);
                    }
                }
                context.MainSheet = sheet;
            }
            private List<RateItem> GetRateItems(IConvertResultToExcelDataContext<SaleRateDetail> context)
            {
                if (context.BigResult?.Data == null)
                    return null;

                var rateItems = new List<RateItem>();
                int longPrecisionValue = new GeneralSettingsManager().GetLongPrecisionValue();

                var saleZoneIds = context.BigResult.Data.Select(item => item.Entity.ZoneId).ToList();
                var saleCodes = new SaleCodeManager().GetSaleCodesByZoneIDs(saleZoneIds, query.EffectiveOn);
                SaleEntityZoneRoutingProductLocator routingProductLocator;
                CustomerZoneRateHistoryLocator customerZoneRateHistoryLocator = null;

                if (query.OwnerType == SalePriceListOwnerType.Customer)
                {
                    int sellingProductId = new CarrierAccountManager().GetSellingProductId(query.OwnerId);
                    var customerIds = new List<RoutingCustomerInfo> { new RoutingCustomerInfo { CustomerId = query.OwnerId } };
                    routingProductLocator = new SaleEntityZoneRoutingProductLocator(new SaleEntityRoutingProductReadAllNoCache(customerIds, query.EffectiveOn, false));
                    customerZoneRateHistoryLocator = new CustomerZoneRateHistoryLocator(new CustomerZoneRateHistoryReader(new List<int> { query.OwnerId }, new List<int> { sellingProductId }, saleZoneIds, false, true));
                }
                else
                {
                    routingProductLocator = new SaleEntityZoneRoutingProductLocator(new SaleEntityRoutingProductReadWithCache(query.EffectiveOn));
                }

                Dictionary<long, List<SaleCode>> saleCodesByZoneId = StructureSaleCodesByZoneId(saleCodes);

                foreach (var saleRateDetail in context.BigResult.Data)
                {
                    if (saleRateDetail.Entity == null)
                        continue;

                    long zoneId = saleRateDetail.Entity.ZoneId;

                    if (!saleCodesByZoneId.TryGetValue(zoneId, out saleCodes))
                        continue;

                    var ratesByRateType = new Dictionary<int, SaleRateHistoryRecord>();
                    var rateTypeIds = Helper.GetRateTypeIds(query.OwnerId, saleRateDetail.Entity.ZoneId, DateTime.Now);

                    SaleEntityZoneRoutingProduct currentRoutingProduct = routingProductLocator.GetCustomerZoneRoutingProduct(query.OwnerId, sellingProductId, zoneId);
                    var servicesIds = new RoutingProductManager().GetZoneServiceIds(currentRoutingProduct.RoutingProductId, zoneId);
                    string servicesSymbol = new ZoneServiceConfigManager().GetZoneServicesNames(servicesIds.ToList());

                    if (query.OwnerType == SalePriceListOwnerType.Customer)
                    {
                        foreach (var rateTypeId in rateTypeIds)
                        {
                            var otherRateHistory = customerZoneRateHistoryLocator.GetCustomerZoneRateHistoryRecord(
                                query.OwnerId, sellingProductId, saleRateDetail.ZoneName, rateTypeId, saleRateDetail.CountryId, query.EffectiveOn,
                                query.IsSystemCurrency ? query.CurrencyId.Value : saleRateDetail.Entity.CurrencyId.Value, longPrecisionValue);

                            if (otherRateHistory != null)
                                ratesByRateType.Add(rateTypeId, otherRateHistory);
                        }
                    }
                    if (query.ByCode)
                    {
                        var ratesByCode = GetRateItemByCode(saleRateDetail, saleCodes, ratesByRateType, servicesSymbol);
                        if (ratesByCode.Count > 0)
                            rateItems.AddRange(ratesByCode);
                    }
                    else
                    {
                        RateItem rateItem = GetRateItemByZone(saleRateDetail, saleCodes, ratesByRateType, servicesSymbol);
                        rateItems.Add(rateItem);
                    }
                }
                return rateItems;
            }
            private List<RateItem> GetRateItemByCode(SaleRateDetail saleRateDetail, IEnumerable<SaleCode> saleCodes, Dictionary<int, SaleRateHistoryRecord> ratesByRateType, string servicesSymbol)
            {
                var rateItems = new List<RateItem>();

                foreach (var saleCode in saleCodes)
                {
                    RateItem rateItem = new RateItem
                    {
                        ZoneId = saleRateDetail.Entity.ZoneId,
                        Codes = saleCode.Code,
                        Rate = saleRateDetail.DisplayedRate,
                        RateBED = saleRateDetail.Entity.BED,
                        RateEED = saleRateDetail.Entity.EED,
                        ZoneName = saleRateDetail.ZoneName,
                        PriceListFileId = saleRateDetail.PriceListFileId,
                        CurrencySymbol = saleRateDetail.DisplayedCurrency,
                        RateInherited = saleRateDetail.IsRateInherited ? "Inherited" : "Explicit",
                        ChangeDescription = Utilities.GetEnumDescription(saleRateDetail.Entity.RateChange),
                        RatesByRateType = ratesByRateType.Count > 0 ? ratesByRateType : null,
                        Note = saleRateDetail.Entity.Note,
                        PriceListId = saleRateDetail.Entity.PriceListId
                    };
                    var codeGroup = new CodeGroupManager().GetCodeGroup(saleCode.CodeGroupId);
                    if (codeGroup != null)
                    {
                        rateItem.CodeGroupsId = codeGroup.Code;
                        rateItem.CountryNames = new CountryManager().GetCountryName(codeGroup.CountryId);
                    }
                    rateItem.ServicesSymbol = servicesSymbol;
                    rateItems.Add(rateItem);
                }
                return rateItems;
            }
            private RateItem GetRateItemByZone(SaleRateDetail saleRateDetail, IEnumerable<SaleCode> saleCodes, Dictionary<int, SaleRateHistoryRecord> ratesByRateType, string servicesSymbol)
            {
                RateItem rateItem = new RateItem
                {
                    ZoneId = saleRateDetail.Entity.ZoneId,
                    Rate = saleRateDetail.DisplayedRate,
                    RateBED = saleRateDetail.Entity.BED,
                    RateEED = saleRateDetail.Entity.EED,
                    ZoneName = saleRateDetail.ZoneName,
                    CurrencySymbol = saleRateDetail.DisplayedCurrency,
                    PriceListFileId = saleRateDetail.PriceListFileId,
                    PriceListId = saleRateDetail.Entity.PriceListId,
                    RateInherited = saleRateDetail.IsRateInherited ? "Inherited" : "Explicit",
                    ChangeDescription = Utilities.GetEnumDescription(saleRateDetail.Entity.RateChange),
                    RatesByRateType = ratesByRateType.Count > 0 ? ratesByRateType : null,
                    Note = saleRateDetail.Entity.Note
                };
                List<string> codeValues = new List<string>();
                HashSet<int> codeGroupsId = new HashSet<int>();
                HashSet<string> countryNames = new HashSet<string>();
                foreach (var saleCode in saleCodes)
                {
                    codeValues.Add(saleCode.Code);
                    codeGroupsId.Add(saleCode.CodeGroupId);
                    var codeGroup = new CodeGroupManager().GetCodeGroup(saleCode.CodeGroupId);
                    string countryName = new CountryManager().GetCountryName(codeGroup.CountryId);
                    countryNames.Add(countryName);
                }
                if (codeValues.Count > 0)
                    rateItem.Codes = string.Join(",", codeValues);

                if (codeGroupsId.Count > 0)
                    rateItem.CodeGroupsId = string.Join(",", codeGroupsId);

                if (countryNames.Count > 0)
                    rateItem.CountryNames = string.Join(",", countryNames);

                rateItem.ServicesSymbol = servicesSymbol;
                return rateItem;
            }
            private Dictionary<long, List<SaleCode>> StructureSaleCodesByZoneId(List<SaleCode> saleCodes)
            {
                var saleCodesByZoneId = new Dictionary<long, List<SaleCode>>();
                foreach (var saleCode in saleCodes)
                {
                    List<SaleCode> codes = saleCodesByZoneId.GetOrCreateItem(saleCode.ZoneId);
                    codes.Add(saleCode);
                }
                return saleCodesByZoneId;
            }
        }
        public class RateItem
        {
            public long ZoneId { get; set; }
            public decimal Rate { get; set; }
            public string Codes { get; set; }
            public string ZoneName { get; set; }
            public DateTime RateBED { get; set; }
            public DateTime? RateEED { get; set; }
            public string CodeGroupsId { get; set; }
            public string CountryNames { get; set; }
            public string RateInherited { get; set; }
            public long? PriceListFileId { get; set; }
            public long? PriceListId { get; set; }
            public string ServicesSymbol { get; set; }
            public string CurrencySymbol { get; set; }
            public string ChangeDescription { get; set; }
            public Dictionary<int, SaleRateHistoryRecord> RatesByRateType { get; set; }
            public string Note { get; set; }
        }
        #endregion
    }
}

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
            throw new ArgumentNullException(String.Format("SaleRate: {0}", rateId));
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
            if (!IsCountrySellToCustomer(customerId, saleZoneId, effectiveOn))
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

                if (input.Query.OwnerType == SalePriceListOwnerType.SellingProduct)
                    return GetSellingProductZoneRates(input.Query.OwnerId, saleZones, saleZoneIds, input.Query.EffectiveOn, currencyId, input.Query.IsSystemCurrency);
                else
                    return GetCustomerSaleZoneRates(input.Query.OwnerId, saleZones, saleZoneIds, input.Query.EffectiveOn, currencyId, input.Query.IsSystemCurrency);
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

            private IEnumerable<SaleRateDetail> GetSellingProductZoneRates(int sellingProductId, IEnumerable<SaleZone> saleZones, IEnumerable<long> saleZoneIds, DateTime effectiveOn, int currencyId, bool isSystemCurrency)
            {
                var saleRates = new List<SaleRateDetail>();
                var sellingProductZoneRateHistoryLocator = new SellingProductZoneRateHistoryLocator(new SellingProductZoneRateHistoryReader(sellingProductId, saleZoneIds, true, false));

                foreach (SaleZone saleZone in saleZones)
                {
                    SaleRateHistoryRecord saleRateHistoryRecord = sellingProductZoneRateHistoryLocator.GetSaleRateHistoryRecord(saleZone.Name, null, currencyId, effectiveOn);

                    if (saleRateHistoryRecord != null)
                    {
                        SaleRateDetail saleRate = GetSaleRateDetail(SalePriceListOwnerType.SellingProduct, saleZone, saleRateHistoryRecord, null, isSystemCurrency);
                        saleRates.Add(saleRate);
                    }
                }

                return saleRates;
            }

            private IEnumerable<SaleRateDetail> GetCustomerSaleZoneRates(int customerId, IEnumerable<SaleZone> saleZones, IEnumerable<long> saleZoneIds, DateTime effectiveOn, int currencyId, bool isSystemCurrency)
            {
                var saleRates = new List<SaleRateDetail>();
                var customerZoneRateHistoryLocator = new CustomerZoneRateHistoryLocator(customerId, new CustomerZoneRateHistoryReader(customerId, saleZoneIds, true, false));
                var salePriceListManager = new SalePriceListManager();

                int? sellingProductId = new CustomerSellingProductManager().GetEffectiveSellingProductId(customerId, effectiveOn, false);
                if (!sellingProductId.HasValue) return null;

                foreach (SaleZone saleZone in saleZones)
                {
                    SaleRateHistoryRecord saleRateHistoryRecord = customerZoneRateHistoryLocator.GetSaleRateHistoryRecord(saleZone.Name, saleZone.CountryId, null, currencyId, effectiveOn);

                    if (saleRateHistoryRecord != null)
                    {
                        SaleRateDetail saleRate = GetSaleRateDetail(SalePriceListOwnerType.Customer, saleZone, saleRateHistoryRecord, salePriceListManager, isSystemCurrency);
                        saleRates.Add(saleRate);
                    }
                }

                return saleRates;
            }

            private SaleRateDetail GetSaleRateDetail(SalePriceListOwnerType ownerType, SaleZone saleZone, SaleRateHistoryRecord saleRateHistoryRecord, SalePriceListManager salePriceListManager, bool isSystemCurrency)
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
                    RateChange = saleRateHistoryRecord.ChangeType
                };

                saleRateDetail.ZoneName = saleZone.Name;
                saleRateDetail.CountryId = saleZone.CountryId;
                saleRateDetail.RateTypeName = null;
                int displayedCurrencyId = (isSystemCurrency) ? _configManager.GetSystemCurrencyId() : saleRateHistoryRecord.CurrencyId;
                saleRateDetail.DisplayedCurrency = _currencyManager.GetCurrencySymbol(displayedCurrencyId);
                saleRateDetail.DisplayedRate = (isSystemCurrency) ? saleRateHistoryRecord.ConvertedRate : saleRateHistoryRecord.Rate;
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
                    ExportExcelHandler = new SaleRateDetailExportExcelHandler()
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
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<SaleRateDetail> context)
            {
                var sheet = new ExportExcelSheet()
                {
                    SheetName = "Sales Rates",
                    Header = new ExportExcelHeader() { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Zone" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Rate" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Rate Change" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Rate Inherited" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Currency" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "BED", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.Date });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "EED", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.Date });

                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    foreach (var record in context.BigResult.Data)
                    {
                        if (record.Entity != null)
                        {
                            var row = new ExportExcelRow() { Cells = new List<ExportExcelCell>() };
                            row.Cells.Add(new ExportExcelCell() { Value = record.ZoneName });
                            row.Cells.Add(new ExportExcelCell() { Value = record.DisplayedRate });
                            row.Cells.Add(new ExportExcelCell() { Value = Vanrise.Common.Utilities.GetEnumDescription(record.Entity.RateChange) });
                            row.Cells.Add(new ExportExcelCell() { Value = string.Format("{0}", record.IsRateInherited == true ? "Inherited" : "Explicit") });
                            row.Cells.Add(new ExportExcelCell() { Value = record.DisplayedCurrency });
                            row.Cells.Add(new ExportExcelCell() { Value = record.Entity.BED });
                            row.Cells.Add(new ExportExcelCell() { Value = record.Entity.EED });
                            sheet.Rows.Add(row);
                        }
                    }
                }

                context.MainSheet = sheet;
            }
        }

        private bool IsCountrySellToCustomer(int customerId, long saleZoneId, DateTime effectiveOn)
        {
            int? saleZoneCountryId = new SaleZoneManager().GetSaleZoneCountryId(saleZoneId);
            if (!saleZoneCountryId.HasValue)
                throw new NullReferenceException(string.Format("saleZoneCountryId of saleZoneId: {0}", saleZoneId));

            CustomerCountry2 customerCountry = new CustomerCountryManager().GetCustomerCountry(customerId, saleZoneCountryId.Value, effectiveOn, false);
            if (customerCountry == null)
                return false;

            return true;
        }

        #endregion
    }
}

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
    public class SupplierRateManager
    {

        #region Public Methods
        public IDataRetrievalResult<SupplierRateDetail> GetFilteredSupplierRates(DataRetrievalInput<BaseSupplierRateQueryHandler> input)
        {
            VRActionLogger.Current.LogGetFilteredAction(SupplierRateLoggableEntity.Instance, input);
            return BigDataManager.Instance.RetrieveData(input, new SupplierRateRequestHandler());
        }
        public Dictionary<long, SupplierRate> GetSupplierRateByZoneId(List<long> supplierZoneIds, DateTime BeginDate, DateTime? EndDate)
        {
            var supplierRateByZoneId = new Dictionary<long, SupplierRate>();
            ISupplierRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierRateDataManager>();
            var supplierRates = dataManager.GetSupplierRates(supplierZoneIds, BeginDate, EndDate);
            foreach (var supplierRate in supplierRates)
            {
                if (!supplierRateByZoneId.ContainsKey(supplierRate.ZoneId))
                    supplierRateByZoneId.Add(supplierRate.ZoneId, supplierRate);
            }
            return supplierRateByZoneId;
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

        public DateTime? GetNextOpenOrCloseTime(DateTime effectiveDate)
        {
            ISupplierRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierRateDataManager>();
            return dataManager.GetNextOpenOrCloseTime(effectiveDate);
        }

        public object GetMaximumTimeStamp()
        {
            ISupplierRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierRateDataManager>();
            return dataManager.GetMaximumTimeStamp();
        }

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
            else return RateChangeType.RateNotAvailable;
        }

        public Dictionary<long, SupplierRate> GetSupplierRates(HashSet<long> supplierRateIds)
        {
            if (supplierRateIds == null || supplierRateIds.Count == 0)
                return null;

            ISupplierRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierRateDataManager>();
            List<SupplierRate> supplierRates = dataManager.GetSupplierRates(supplierRateIds);
            if (supplierRates == null)
                return null;

            return supplierRates.ToDictionary(itm => itm.SupplierRateId, itm => itm);
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
                throw new NullReferenceException(String.Format("supplierPriceList (Id: {0}) does not exist for supplierRate (Id: {1})", supplierRate.SupplierRateId, supplierRate.PriceListId));
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

        #endregion

        #region Private Classes

        private class SupplierRateRequestHandler : BigDataRequestHandler<BaseSupplierRateQueryHandler, SupplierRate, SupplierRateDetail>
        {
            private SupplierPriceListManager _supplierPriceListManager;

            private SupplierRateManager _supplierRateManager;
            private RateTypeManager _rateTypeManager;
            private CurrencyExchangeRateManager _currencyExchangeRateManager;
            private CurrencyManager _currencyManager;


            public SupplierRateRequestHandler()
            {
                _supplierPriceListManager = new SupplierPriceListManager();
                _supplierRateManager = new SupplierRateManager();
                _rateTypeManager = new RateTypeManager();
                _currencyExchangeRateManager = new CurrencyExchangeRateManager();
                _currencyManager = new CurrencyManager();
            }
            public override SupplierRateDetail EntityDetailMapper(SupplierRate entity)
            {
                throw new NotImplementedException();
            }
            private SupplierRateDetail SupplierRateDetailMapper(SupplierRate supplierRate, int? systemCurrencyId, DateTime effectiveOn)
            {
                SupplierPriceList priceList = _supplierPriceListManager.GetPriceList(supplierRate.PriceListId);
                supplierRate.PriceListFileId = priceList.FileId;

                int currencyId = supplierRate.CurrencyId ?? priceList.CurrencyId;
                int currencyValueId = systemCurrencyId != null ? systemCurrencyId.Value : currencyId;

                return new SupplierRateDetail
                {
                    Entity = supplierRate,
                    SupplierId = priceList.SupplierId,
                    SupplierZoneName = _supplierRateManager.GetSupplierZoneName(supplierRate.ZoneId),
                    DisplayedCurrency = _currencyManager.GetCurrencySymbol(currencyValueId),
                    DisplayedRate = (systemCurrencyId != null) ? _currencyExchangeRateManager.ConvertValueToCurrency(supplierRate.Rate, currencyId, currencyValueId, effectiveOn) : supplierRate.Rate,
                };
            }
            public override IEnumerable<SupplierRate> RetrieveAllData(DataRetrievalInput<BaseSupplierRateQueryHandler> input)
            {
                return input.Query.GetFilteredSupplierRates();
            }


            protected override BigResult<SupplierRateDetail> AllRecordsToBigResult(DataRetrievalInput<BaseSupplierRateQueryHandler> input, IEnumerable<SupplierRate> allRecords)
            {
                int? systemCurrencyId = (input.Query.IsSystemCurrency) ? (int?)new Vanrise.Common.Business.ConfigManager().GetSystemCurrencyId() : null;
                var effectiveOn = input.Query.EffectiveOn;
                return allRecords.ToBigResult(input, null, (entity) => SupplierRateDetailMapper(entity, systemCurrencyId, effectiveOn));
            }
            protected override ResultProcessingHandler<SupplierRateDetail> GetResultProcessingHandler(DataRetrievalInput<BaseSupplierRateQueryHandler> input, BigResult<SupplierRateDetail> bigResult)
            {
                return new ResultProcessingHandler<SupplierRateDetail>
                {
                    ExportExcelHandler = new SupplierRateExcelExportHandler(input.Query)
                };
            }
        }

        private class SupplierRateExcelExportHandler : ExcelExportHandler<SupplierRateDetail>
        {
            private BaseSupplierRateQueryHandler _query;
            public SupplierRateExcelExportHandler(BaseSupplierRateQueryHandler baseSupplierRateQueryHandler)
            {
                _query = baseSupplierRateQueryHandler;
            }
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<SupplierRateDetail> context)
            {
                _query.ThrowIfNull("SupplierRateQueryHandler");

                context.MainSheet = _query is SupplierRateQueryHandler
                    ? GetRateSheet(context)
                    : GetRateSheetHistory(context);
            }

            private ExportExcelSheet GetRateSheetHistory(IConvertResultToExcelDataContext<SupplierRateDetail> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet
                {
                    SheetName = "Supplier Rates",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
                };
                    sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "ID" });
                    sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Zone" });
                    sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Normal Rate", CellType = ExcelCellType.Number, NumberType = NumberType.LongDecimal });
                    sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Rate Change" });
                    sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Currency" });
                    sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "BED", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.Date });
                    sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "EED", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.Date });

                    sheet.Rows = new List<ExportExcelRow>();
                    if (context.BigResult?.Data != null)
                    {
                        foreach (var record in context.BigResult.Data)
                        {
                            if (record.Entity != null)
                            {
                                var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                                sheet.Rows.Add(row);
                                row.Cells.Add(new ExportExcelCell { Value = record.Entity.SupplierRateId });
                                row.Cells.Add(new ExportExcelCell { Value = record.SupplierZoneName });
                                row.Cells.Add(new ExportExcelCell { Value = record.DisplayedRate });
                                row.Cells.Add(new ExportExcelCell { Value = Vanrise.Common.Utilities.GetEnumDescription(record.Entity.RateChange) });
                                row.Cells.Add(new ExportExcelCell { Value = record.DisplayedCurrency });
                                row.Cells.Add(new ExportExcelCell { Value = record.Entity.BED });
                                row.Cells.Add(new ExportExcelCell { Value = record.Entity.EED });
                            }
                        }
                    }
                return sheet;
            }
            private ExportExcelSheet GetRateSheet(IConvertResultToExcelDataContext<SupplierRateDetail> context)
            {
                SupplierRateQueryHandler supplierRateQuery = (SupplierRateQueryHandler)_query;
                List<SupplierRateItem> supplierRateItems = GetRateItems(context);

                var rateTypeManager = new RateTypeManager();
                context.ThrowIfContentLengthExceeded = true;
                ExportExcelSheet sheet = new ExportExcelSheet
                {
                    SheetName = "Supplier Rates",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
                };
                if (supplierRateQuery.Query.ColumnsToShow!=null && supplierRateQuery.Query.ColumnsToShow.Count>0)
                {
                    if (supplierRateQuery.Query.ColumnsToShow.Contains("Zone"))
                        sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Zone" });

                    if (supplierRateQuery.Query.ColumnsToShow.Contains("Code"))
                        sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Code" });

                    if (supplierRateQuery.Query.ColumnsToShow.Contains("CodeGroup"))
                        sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "CodeGroup" });

                    if (supplierRateQuery.Query.ColumnsToShow.Contains("Country"))
                        sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Country" });

                    if (supplierRateQuery.Query.ColumnsToShow.Contains("Normal Rate"))
                        sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Normal Rate", CellType = ExcelCellType.Number, NumberType = NumberType.LongDecimal });

                    if (supplierRateQuery.Query.ColumnsToShow.Contains("Rate Change"))
                        sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Rate Change" });

                    if (supplierRateQuery.Query.ColumnsToShow.Contains("Currency"))
                        sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Currency" });

                    if (supplierRateQuery.Query.ColumnsToShow.Contains("Rate BE"))
                        sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Rate BED", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.Date });

                    if (supplierRateQuery.Query.ColumnsToShow.Contains("Rate EED"))
                        sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Rate EED", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.Date });

                    if (supplierRateQuery.Query.ColumnsToShow.Contains("Services"))
                        sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Services" });

                    //var rateTypes = rateTypeManager.GetAllRateTypes();
                    //foreach (var rateTypeInfo in rateTypes)
                    //{
                    //    sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = rateTypeInfo.Name });
                    //}
                    sheet.Rows = new List<ExportExcelRow>();

                    foreach (var supplierRateItem in supplierRateItems)
                    {
                        var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                        sheet.Rows.Add(row);
                        if (supplierRateQuery.Query.ColumnsToShow.Contains("Zone"))
                            row.Cells.Add(new ExportExcelCell { Value = supplierRateItem.ZoneName });

                        if (supplierRateQuery.Query.ColumnsToShow.Contains("Code"))
                            row.Cells.Add(new ExportExcelCell { Value = supplierRateItem.Codes });

                        if (supplierRateQuery.Query.ColumnsToShow.Contains("CodeGroup"))
                            row.Cells.Add(new ExportExcelCell { Value = supplierRateItem.CodeGroupsId });

                        if (supplierRateQuery.Query.ColumnsToShow.Contains("Country"))
                            row.Cells.Add(new ExportExcelCell { Value = supplierRateItem.CountryNames });

                        if (supplierRateQuery.Query.ColumnsToShow.Contains("Normal Rate"))
                            row.Cells.Add(new ExportExcelCell { Value = supplierRateItem.Rate });

                        if (supplierRateQuery.Query.ColumnsToShow.Contains("Rate Change"))
                            row.Cells.Add(new ExportExcelCell { Value = supplierRateItem.ChangeDescription });

                        if (supplierRateQuery.Query.ColumnsToShow.Contains("Currency"))
                            row.Cells.Add(new ExportExcelCell { Value = supplierRateItem.CurrencySymbol });

                        if (supplierRateQuery.Query.ColumnsToShow.Contains("Rate BE"))
                            row.Cells.Add(new ExportExcelCell { Value = supplierRateItem.RateBED });

                        if (supplierRateQuery.Query.ColumnsToShow.Contains("Rate EED"))
                            row.Cells.Add(new ExportExcelCell { Value = supplierRateItem.RateEED });

                        if (supplierRateQuery.Query.ColumnsToShow.Contains("Services"))
                            row.Cells.Add(new ExportExcelCell { Value = supplierRateItem.ServicesSymbol });

                        //foreach (var rateTypeInfo in rateTypes)
                        //{
                        //    var ratesByRateType = supplierRateItem.RatesByRateType;
                        //    if (ratesByRateType != null && ratesByRateType.Any() && ratesByRateType.TryGetValue(rateTypeInfo.RateTypeId, out var supplierOtherRate))
                        //    {
                        //        row.Cells.Add(new ExportExcelCell { Value = supplierOtherRate.Rate });
                        //    }
                        //    else row.Cells.Add(new ExportExcelCell { Value = string.Empty });
                        //}
                    }
                }
                else
                {
                    sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Zone" });
                    sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Code" });
                    sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "CodeGroup" });
                    sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Country" });
                    sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Normal Rate", CellType = ExcelCellType.Number, NumberType = NumberType.LongDecimal });
                    sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Rate Change" });
                    sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Currency" });
                    sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Rate BED", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.Date });
                    sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Rate EED", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.Date });
                    sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Services" });

                    var rateTypes = rateTypeManager.GetAllRateTypes();
                    foreach (var rateTypeInfo in rateTypes)
                    {
                        sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = rateTypeInfo.Name });
                    }
                    sheet.Rows = new List<ExportExcelRow>();

                    foreach (var supplierRateItem in supplierRateItems)
                    {
                        var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                        sheet.Rows.Add(row);
                        row.Cells.Add(new ExportExcelCell { Value = supplierRateItem.ZoneName });
                        row.Cells.Add(new ExportExcelCell { Value = supplierRateItem.Codes });
                        row.Cells.Add(new ExportExcelCell { Value = supplierRateItem.CodeGroupsId });
                        row.Cells.Add(new ExportExcelCell { Value = supplierRateItem.CountryNames });
                        row.Cells.Add(new ExportExcelCell { Value = supplierRateItem.Rate });
                        row.Cells.Add(new ExportExcelCell { Value = supplierRateItem.ChangeDescription });
                        row.Cells.Add(new ExportExcelCell { Value = supplierRateItem.CurrencySymbol });
                        row.Cells.Add(new ExportExcelCell { Value = supplierRateItem.RateBED });
                        row.Cells.Add(new ExportExcelCell { Value = supplierRateItem.RateEED });
                        row.Cells.Add(new ExportExcelCell { Value = supplierRateItem.ServicesSymbol });

                        foreach (var rateTypeInfo in rateTypes)
                        {
                            var ratesByRateType = supplierRateItem.RatesByRateType;
                            if (ratesByRateType != null && ratesByRateType.Any() && ratesByRateType.TryGetValue(rateTypeInfo.RateTypeId, out var supplierOtherRate))
                            {
                                row.Cells.Add(new ExportExcelCell { Value = supplierOtherRate.Rate });
                            }
                            else row.Cells.Add(new ExportExcelCell { Value = string.Empty });
                        }
                    }
                }
                return sheet;
            }
            private List<SupplierRateItem> GetRateItems(IConvertResultToExcelDataContext<SupplierRateDetail> context)
            {
                if (context.BigResult?.Data == null)
                    return null;

                var rateQueryHandler = _query as SupplierRateQueryHandler;
                var supplierZoneIds = new List<long>();
                var supplierRatesDetail = new List<SupplierRateDetail>();
                foreach (var record in context.BigResult.Data)
                {
                    if (record.Entity != null)
                    {
                        supplierZoneIds.Add(record.Entity.ZoneId);
                        supplierRatesDetail.Add(record);
                    }
                }

                var supplierOtherRates = new SupplierOtherRateManager().GetSupplierOtherRates(supplierZoneIds, _query.EffectiveOn);
                OtherRateByZoneId supplierOtherRateByZoneId = StructureOtherRateByZoneId(supplierOtherRates);

                var supplierCodes = new SupplierCodeManager().GetSupplierCodesByZoneIds(supplierZoneIds, _query.EffectiveOn);
                Dictionary<long, List<SupplierCode>> supplierCodesByZoneId = StructureSupplierCodeByZoneId(supplierCodes);

                SupplierZoneServiceLocator zoneServiceLocator = new SupplierZoneServiceLocator(new SupplierZoneServiceReadAllWithCache(_query.EffectiveOn));

                var rateItems = new List<SupplierRateItem>();
                foreach (var supplierRateDetail in supplierRatesDetail)
                {
                    long zoneId = supplierRateDetail.Entity.ZoneId;
                    if (!supplierCodesByZoneId.TryGetValue(zoneId, out var zoneSupplierCodes))
                        continue;

                    var supplierZoneServices = zoneServiceLocator.GetSupplierZoneServices(rateQueryHandler.Query.SupplierId, zoneId, _query.EffectiveOn);
                    string zoneServicesName = string.Empty;
                    if (supplierZoneServices?.Services != null)
                        zoneServicesName = new ZoneServiceConfigManager().GetZoneServicesNames(supplierZoneServices.Services.Select(item => item.ServiceId));

                    supplierOtherRateByZoneId.TryGetValue(zoneId, out var zoneOtherRates);

                    if (rateQueryHandler.Query.ByCode)
                    {
                        var rateItemsByCode = GetRateItemsByCode(supplierRateDetail, zoneSupplierCodes, zoneServicesName, zoneOtherRates);
                        rateItems.AddRange(rateItemsByCode);
                    }
                    else
                    {
                        SupplierRateItem rateItem = GetRateItemByZone(supplierRateDetail, zoneSupplierCodes, zoneServicesName, zoneOtherRates);
                        rateItems.Add(rateItem);
                    }
                }
                return rateItems;
            }
            private List<SupplierRateItem> GetRateItemsByCode(SupplierRateDetail supplierRate, List<SupplierCode> supplierCodes, string zoneServicesName, Dictionary<int, SupplierOtherRate> ratesByRateType)
            {
                var countryManager = new CountryManager();
                var rateItems = new List<SupplierRateItem>();
                foreach (var zoneSupplierCode in supplierCodes)
                {
                    long zoneId = supplierRate.Entity.ZoneId;

                    SupplierRateItem supplierRateItem = new SupplierRateItem
                    {
                        RateId = supplierRate.Entity.SupplierRateId,
                        ZoneId = zoneId,
                        Codes = zoneSupplierCode.Code,
                        Rate = supplierRate.DisplayedRate,
                        RateBED = supplierRate.Entity.BED,
                        RateEED = supplierRate.Entity.EED,
                        ZoneName = supplierRate.SupplierZoneName,
                        CurrencySymbol = supplierRate.DisplayedCurrency,
                        ChangeDescription = Utilities.GetEnumDescription(supplierRate.Entity.RateChange),
                        PriceListFileId = supplierRate.Entity.PriceListFileId,
                        ServicesSymbol = zoneServicesName,
                        RatesByRateType = ratesByRateType
                    };
                    if (zoneSupplierCode.CodeGroupId.HasValue)
                    {
                        var codeGroup = new CodeGroupManager().GetCodeGroup(zoneSupplierCode.CodeGroupId.Value);
                        if (codeGroup != null)
                        {
                            supplierRateItem.CodeGroupsId = codeGroup.Code;
                            supplierRateItem.CountryNames = countryManager.GetCountryName(codeGroup.CountryId);
                        }
                    }

                    rateItems.Add(supplierRateItem);
                }
                return rateItems;
            }
            private SupplierRateItem GetRateItemByZone(SupplierRateDetail supplierRate, List<SupplierCode> supplierCodes, string zoneServicesName, Dictionary<int, SupplierOtherRate> ratesByRateType)
            {
                var rateItem = new SupplierRateItem
                {
                    ZoneId = supplierRate.Entity.ZoneId,
                    Rate = supplierRate.DisplayedRate,
                    RateBED = supplierRate.Entity.BED,
                    RateEED = supplierRate.Entity.EED,
                    ZoneName = supplierRate.SupplierZoneName,
                    PriceListFileId = supplierRate.Entity.PriceListFileId,
                    CurrencySymbol = supplierRate.DisplayedCurrency,
                    ChangeDescription = Utilities.GetEnumDescription(supplierRate.Entity.RateChange),
                    ServicesSymbol = zoneServicesName,
                    RatesByRateType = ratesByRateType
                };
                List<string> codeValues = new List<string>();
                HashSet<int> codeGroupsId = new HashSet<int>();
                HashSet<string> countryNames = new HashSet<string>();
                foreach (var supplierCode in supplierCodes)
                {
                    if (supplierCode.CodeGroupId.HasValue)
                    {
                        codeValues.Add(supplierCode.Code);
                        var codeGroup = new CodeGroupManager().GetCodeGroup(supplierCode.CodeGroupId.Value);
                        codeGroupsId.Add(supplierCode.CodeGroupId.Value);
                        string countryName = new CountryManager().GetCountryName(codeGroup.CountryId);
                        countryNames.Add(countryName);
                    }
                }
                if (codeValues.Count > 0)
                    rateItem.Codes = string.Join(",", codeValues);

                if (codeGroupsId.Count > 0)
                    rateItem.CodeGroupsId = string.Join(",", codeGroupsId);

                if (countryNames.Count > 0)
                    rateItem.CountryNames = string.Join(",", countryNames);

                return rateItem;
            }
            private Dictionary<long, List<SupplierCode>> StructureSupplierCodeByZoneId(IEnumerable<SupplierCode> supplierCodes)
            {
                var supplierCodesByZoneId = new Dictionary<long, List<SupplierCode>>();
                foreach (var supplierCode in supplierCodes)
                {
                    List<SupplierCode> codes = supplierCodesByZoneId.GetOrCreateItem(supplierCode.ZoneId);
                    codes.Add(supplierCode);
                }
                return supplierCodesByZoneId;
            }
            private OtherRateByZoneId StructureOtherRateByZoneId(IEnumerable<SupplierOtherRate> supplierOtherRates)
            {
                OtherRateByZoneId supplierOtherRateByZoneId = new OtherRateByZoneId();
                foreach (var supplierOtherRate in supplierOtherRates)
                {
                    if (!supplierOtherRate.RateTypeId.HasValue)
                        continue;

                    OtherRateByRateTypeId otherRateByRateTypeId = supplierOtherRateByZoneId.GetOrCreateItem(supplierOtherRate.ZoneId);

                    var rateTypeId = supplierOtherRate.RateTypeId.Value;
                    if (!otherRateByRateTypeId.ContainsKey(rateTypeId))
                    {
                        otherRateByRateTypeId.Add(rateTypeId, supplierOtherRate);
                    }
                }
                return supplierOtherRateByZoneId;
            }
        }

        private class SupplierRateItem
        {
            public long ZoneId { get; set; }
            public long RateId { get; set; }
            public decimal Rate { get; set; }
            public string Codes { get; set; }
            public string ZoneName { get; set; }
            public DateTime RateBED { get; set; }
            public DateTime? RateEED { get; set; }
            public string CodeGroupsId { get; set; }
            public string CountryNames { get; set; }
            public string RateInherited { get; set; }
            public long? PriceListFileId { get; set; }
            public string ServicesSymbol { get; set; }
            public string CurrencySymbol { get; set; }
            public string ChangeDescription { get; set; }
            public Dictionary<int, SupplierOtherRate> RatesByRateType { get; set; }
        }
        public class OtherRateByZoneId : Dictionary<long, OtherRateByRateTypeId>
        {
        }
        public class OtherRateByRateTypeId : Dictionary<int, SupplierOtherRate>
        {
        }

        private class SupplierRateLoggableEntity : VRLoggableEntityBase
        {
            public static SupplierRateLoggableEntity Instance = new SupplierRateLoggableEntity();

            private SupplierRateLoggableEntity()
            {

            }



            public override string EntityUniqueName
            {
                get { return "WhS_BusinessEntity_SupplierRate"; }
            }

            public override string ModuleName
            {
                get { return "Business Entity"; }
            }

            public override string EntityDisplayName
            {
                get { return "Supplier Rate"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "WhS_BusinessEntity_SupplierRate_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                SupplierRate supplierRate = context.Object.CastWithValidate<SupplierRate>("context.Object");
                return supplierRate.SupplierRateId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                return null;
            }
        }
        #endregion
    }
}

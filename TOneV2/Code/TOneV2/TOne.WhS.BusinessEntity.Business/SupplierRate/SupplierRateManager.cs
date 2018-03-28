using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;
using Vanrise.Common.Business;
using Vanrise.Common;
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
            throw new ArgumentNullException(String.Format("CostRate: {0}", rateId));
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
                    ExportExcelHandler = new SupplierRateExcelExportHandler()
                };
            }
        }

        private class SupplierRateExcelExportHandler : ExcelExportHandler<SupplierRateDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<SupplierRateDetail> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "Supplier Rates",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "ID" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Zone" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Normal Rate" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Rate Change" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Currency" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "BED", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.Date });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "EED", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.Date });

                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
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
                context.MainSheet = sheet;
            }
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

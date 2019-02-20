using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.SMSBusinessEntity.Data;
using TOne.WhS.SMSBusinessEntity.Data.RDB;
using TOne.WhS.SMSBusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.MobileNetwork.Business;
using Vanrise.MobileNetwork.Entities;

namespace TOne.WhS.SMSBusinessEntity.Business
{
    public class SupplierSMSRateManager
    {
        ISupplierSMSRateDataManager _supplierSMSRateDataManager = SMSBEDataFactory.GetDataManager<ISupplierSMSRateDataManager>();

        #region Public Methods
        public Vanrise.Entities.IDataRetrievalResult<SupplierSMSRateDetail> GetFilteredSupplierSMSRate(DataRetrievalInput<SupplierSMSRateQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new SupplierSMSRateRequestHandler());
        }

        public Vanrise.Entities.IDataRetrievalResult<SMSCostDetail> GetFilteredSMSCostDetails(DataRetrievalInput<SMSCostQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new SMSCostRequestHandler());
        }

        public Dictionary<int, SupplierSMSRateItem> GetEffectiveMobileNetworkRates(int supplierID, DateTime dateTime)
        {
            var supplierSMSRatesByMobileNetworkIDs = GetSupplierSMSRatesByMobileNetworkID(supplierID, dateTime);

            if (supplierSMSRatesByMobileNetworkIDs == null)
                return null;

            Dictionary<int, SupplierSMSRateItem> supplierSMSRates = new Dictionary<int, SupplierSMSRateItem>();

            foreach (var mobileNetworkKvp in supplierSMSRatesByMobileNetworkIDs)
            {
                int mobileNetworkId = mobileNetworkKvp.Key;
                List<SupplierSMSRate> supplierSMSRatesByMobileNetworkID = mobileNetworkKvp.Value;

                supplierSMSRates.Add(mobileNetworkId, BuildSupplierSMSRateItem(supplierSMSRatesByMobileNetworkID, dateTime));
            }
            return supplierSMSRates;
        }

        //used in data transformation
        public SupplierSMSRate GetMobileNetworkSupplierRate(int supplierID, int mobileNetworkID, DateTime attemptDateTime)
        {
            var supplierSMSRatesByMobileNetworkIdsBySupplierIds = GetCachedSupplierRates(attemptDateTime);

            var supplierSMSRatesForSupplierId = supplierSMSRatesByMobileNetworkIdsBySupplierIds.GetRecord(supplierID);
            if (supplierSMSRatesForSupplierId == null)
                return null;

            var supplierSMSRatesForMobileNetworkId = supplierSMSRatesForSupplierId.GetRecord(mobileNetworkID);
            if (supplierSMSRatesForMobileNetworkId == null || supplierSMSRatesForMobileNetworkId.Count == 0)
                return null;

            return Helper.GetBusinessEntityInfo<SupplierSMSRate>(supplierSMSRatesForMobileNetworkId, attemptDateTime);
        }

        #endregion

        #region Private Methods

        private Dictionary<int, Dictionary<int, List<SupplierSMSRate>>> GetCachedSupplierRates(DateTime effectiveDate)
        {
            DateTimeRange dateTimeRange = Helper.GetDateTimeRangeWithOffset(effectiveDate);

            var cacheName = new GetCachedSupplierSMSRatesCacheName() { EffectiveOn = dateTimeRange.From };

            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(cacheName, () =>
            {
                var priceLists = new SupplierSMSPriceListManager().GetCachedSupplierSMSPriceLists();

                var supplierSMSRates = _supplierSMSRateDataManager.GetSupplierSMSRatesEffectiveOn(dateTimeRange.From, dateTimeRange.To);

                Dictionary<int, Dictionary<int, List<SupplierSMSRate>>> supplierSMSRatesByMobileNetworkIdBySupplierId = new Dictionary<int, Dictionary<int, List<SupplierSMSRate>>>();

                foreach (var supplierSMSRate in supplierSMSRates)
                {
                    var priceList = priceLists.GetRecord(supplierSMSRate.PriceListID);
                    priceList.ThrowIfNull("priceList", supplierSMSRate.PriceListID);

                    Dictionary<int, List<SupplierSMSRate>> supplierSMSRatesByMobileNetworkId = supplierSMSRatesByMobileNetworkIdBySupplierId.GetOrCreateItem(priceList.SupplierID);

                    List<SupplierSMSRate> mobileNetworkSMSRates = supplierSMSRatesByMobileNetworkId.GetOrCreateItem(supplierSMSRate.MobileNetworkID);
                    mobileNetworkSMSRates.Add(supplierSMSRate);
                }

                return supplierSMSRatesByMobileNetworkIdBySupplierId;
            });
        }

        private Dictionary<int, List<SupplierSMSRate>> GetSupplierSMSRatesByMobileNetworkID(int supplierID, DateTime effectiveDate)
        {
            List<SupplierSMSRate> supplierSMSRates = _supplierSMSRateDataManager.GetSupplierSMSRatesEffectiveAfter(supplierID, effectiveDate);

            if (supplierSMSRates == null)
                return null;

            Dictionary<int, List<SupplierSMSRate>> supplierSMSRatesByMobileNetworkID = new Dictionary<int, List<SupplierSMSRate>>();

            foreach (var supplierSMSRate in supplierSMSRates)
            {
                var mobileNetworkList = supplierSMSRatesByMobileNetworkID.GetOrCreateItem(supplierSMSRate.MobileNetworkID);
                mobileNetworkList.Add(supplierSMSRate);
            }

            return supplierSMSRatesByMobileNetworkID;
        }

        private SupplierSMSRateItem BuildSupplierSMSRateItem(List<SupplierSMSRate> supplierSMSRates, DateTime dateTime)
        {
            if (supplierSMSRates == null || supplierSMSRates.Count == 0)
                return null;

            SupplierSMSRate supplierSMSRate = supplierSMSRates.First();

            if (!supplierSMSRate.IsEffective(dateTime))
                return new SupplierSMSRateItem() { FutureRate = supplierSMSRate };

            if (supplierSMSRates.Count > 1)
                return new SupplierSMSRateItem() { CurrentRate = supplierSMSRate, FutureRate = supplierSMSRates[1] };

            return new SupplierSMSRateItem() { CurrentRate = supplierSMSRate };
        }

        #endregion

        #region Private/Internal Classes

        private struct GetCachedSupplierSMSRatesCacheName
        {
            public DateTime EffectiveOn { get; set; }
        }

        private class SupplierSMSRateRequestHandler : BigDataRequestHandler<SupplierSMSRateQuery, SupplierSMSRateItem, SupplierSMSRateDetail>
        {
            SupplierSMSRateManager _manager = new SupplierSMSRateManager();
            MobileNetworkManager _mobileNetworkManager = new MobileNetworkManager();

            public override SupplierSMSRateDetail EntityDetailMapper(SupplierSMSRateItem entity)
            {
                return SupplierSMSRateDetailMapper(entity);
            }

            public override IEnumerable<SupplierSMSRateItem> RetrieveAllData(DataRetrievalInput<SupplierSMSRateQuery> input)
            {
                if (input != null && input.Query != null)
                {
                    List<SupplierSMSRateItem> _supplierSMSRates = _manager.GetEffectiveMobileNetworkRates(input.Query.SupplierID, input.Query.EffectiveDate).Values.ToList();
                    return _supplierSMSRates.Where(item => FilterSupplierSMSRates(item, input.Query));
                }

                return null;
            }

            private SupplierSMSRateDetail SupplierSMSRateDetailMapper(SupplierSMSRateItem supplierSMSRate)
            {
                if (supplierSMSRate == null)
                    return null;

                var currentRate = supplierSMSRate.CurrentRate;
                var futureRate = supplierSMSRate.FutureRate;

                int mobileNetworkID = 0;
                if (currentRate != null)
                    mobileNetworkID = currentRate.MobileNetworkID;
                else if (futureRate != null)
                    mobileNetworkID = futureRate.MobileNetworkID;

                MobileNetwork mobileNetwork = _mobileNetworkManager.GetMobileNetworkById(mobileNetworkID);
                mobileNetwork.ThrowIfNull("mobileNetwork", mobileNetworkID);

                return new SupplierSMSRateDetail()
                {
                    ID = currentRate.ID,
                    MobileCountryName = new MobileCountryManager().GetMobileCountryName(mobileNetwork.MobileCountryId),
                    MobileNetworkName = mobileNetwork.NetworkName,
                    Rate = currentRate.Rate,
                    MobileNetworkID = mobileNetworkID,
                    BED = currentRate.BED,
                    EED = currentRate.EED,
                    FutureRate = futureRate != null ? new SMSFutureRate() { Rate = futureRate.Rate, BED = futureRate.BED, EED = futureRate.EED } : null
                };
            }

            private bool FilterSupplierSMSRates(SupplierSMSRateItem supplierSMSRate, SupplierSMSRateQuery queryInput)
            {
                if (supplierSMSRate.CurrentRate == null)
                    return false;

                int mobileNetworkID = supplierSMSRate.CurrentRate.MobileNetworkID;

                if (queryInput.MobileNetworkIds != null && queryInput.MobileNetworkIds.Count != 0 && !queryInput.MobileNetworkIds.Contains(mobileNetworkID))
                    return false;

                if (queryInput.MobileCountryIds != null && queryInput.MobileCountryIds.Count != 0)
                {
                    MobileNetwork mobileNetwork = _mobileNetworkManager.GetMobileNetworkById(mobileNetworkID);
                    mobileNetwork.ThrowIfNull("mobileNetwork", mobileNetworkID);
                    int mobileCountryId = mobileNetwork.MobileCountryId;

                    if (!queryInput.MobileCountryIds.Contains(mobileCountryId))
                        return false;
                }

                return true;
            }

            protected override ResultProcessingHandler<SupplierSMSRateDetail> GetResultProcessingHandler(DataRetrievalInput<SupplierSMSRateQuery> input, BigResult<SupplierSMSRateDetail> bigResult)
            {
                var resultProcessingHandler = new ResultProcessingHandler<SupplierSMSRateDetail>() { ExportExcelHandler = new SupplierSMSRateExcelExportHandler() };
                return resultProcessingHandler;
            }
        }

        private class SupplierSMSRateExcelExportHandler : ExcelExportHandler<SupplierSMSRateDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<SupplierSMSRateDetail> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "Supplier SMS Rates",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() },
                    AutoFitColumns = true
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "ID", CellType = ExcelCellType.Number, NumberType = NumberType.BigInt });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Country" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Mobile Network", Width = 45 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Rate", Width = 25 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "BED", Width = 45, CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.Date });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "EED", Width = 45, CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.Date });

                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null && context.BigResult.Data.Count() > 0)
                {

                    foreach (var record in context.BigResult.Data)
                    {
                        var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                        row.Cells.Add(new ExportExcelCell { Value = record.ID });
                        row.Cells.Add(new ExportExcelCell { Value = record.MobileCountryName });
                        row.Cells.Add(new ExportExcelCell { Value = record.MobileNetworkName });
                        row.Cells.Add(new ExportExcelCell { Value = record.Rate.ToString() });
                        row.Cells.Add(new ExportExcelCell { Value = record.BED });
                        row.Cells.Add(new ExportExcelCell { Value = record.EED });

                        sheet.Rows.Add(row);
                    }
                }

                context.MainSheet = sheet;
            }
        }

        internal class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ISupplierSMSRateDataManager _supplierSMSRateDataManager = SMSBEDataFactory.GetDataManager<ISupplierSMSRateDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _supplierSMSRateDataManager.AreSupplierSMSRatesUpdated(ref _updateHandle);
            }
        }

        private class SMSCostRequestHandler : BigDataRequestHandler<SMSCostQuery, SMSCost, SMSCostDetail>
        {
            SupplierSMSRateDataManager _manager = new SupplierSMSRateDataManager();
            MobileNetworkManager _mobileNetworkManager = new MobileNetworkManager();
            MobileCountryManager _mobileCountryManager = new MobileCountryManager();
            CarrierAccountManager _carrierAccountManager = new CarrierAccountManager();

            public override SMSCostDetail EntityDetailMapper(SMSCost entity)
            {
                int mobileNetworkID = entity.MobileNetworkID;

                MobileNetwork mobileNetwork = _mobileNetworkManager.GetMobileNetworkById(mobileNetworkID);
                mobileNetwork.ThrowIfNull("mobileNetwork", mobileNetworkID);

                List<SMSCostOptionDetail> smsCostOptions = new List<SMSCostOptionDetail>();
                foreach (var costOption in entity.CostOptions)
                {
                    string supplierName = _carrierAccountManager.GetCarrierAccountName(costOption.SupplierId);
                    smsCostOptions.Add(new SMSCostOptionDetail() { SupplierName = supplierName, SupplierRate = costOption.SupplierRate });
                }

                return new SMSCostDetail()
                {
                    MobileCountryName = _mobileCountryManager.GetMobileCountryName(mobileNetwork.MobileCountryId),
                    MobileNetworkName = mobileNetwork.NetworkName,
                    CostOptions = smsCostOptions
                };
            }

            public override IEnumerable<SMSCost> RetrieveAllData(DataRetrievalInput<SMSCostQuery> input)
            {
                if (input != null && input.Query != null)
                {
                    var supplierSMSRates = _manager.GetSupplierSMSRatesEffectiveOn(input.Query.EffectiveDate);
                    if (supplierSMSRates == null || supplierSMSRates.Count == 0)
                        return null;

                    var filteredSupplierSMSRates = supplierSMSRates.Where(item => FilterSMSRates(item, input.Query)).ToList();

                    if (filteredSupplierSMSRates == null)
                        return null;

                    Dictionary<int, SMSCost> smsCostByMobileNetworkID = new Dictionary<int, SMSCost>();

                    Dictionary<long, SupplierSMSPriceList> priceLists = new SupplierSMSPriceListManager().GetCachedSupplierSMSPriceLists();
                    CurrencyExchangeRateManager currencyExchangeRateManager = new CurrencyExchangeRateManager();

                    GeneralSettingsManager generalSettingsManager = new GeneralSettingsManager();
                    int longPrecision = generalSettingsManager.GetLongPrecisionValue();

                    foreach (var supplierSMSRate in filteredSupplierSMSRates)
                    {
                        SupplierSMSPriceList priceList = priceLists.GetRecord(supplierSMSRate.PriceListID);
                        priceList.ThrowIfNull("priceList", supplierSMSRate.PriceListID);

                        decimal convertedRate = Math.Round(currencyExchangeRateManager.ConvertValueToSystemCurrency(supplierSMSRate.Rate, priceList.CurrencyID, input.Query.EffectiveDate), longPrecision);

                        SMSCost smsCost = smsCostByMobileNetworkID.GetOrCreateItem(supplierSMSRate.MobileNetworkID, () => { return new SMSCost() { MobileNetworkID = supplierSMSRate.MobileNetworkID, CostOptions = new List<SMSCostOption>() }; });

                        smsCost.CostOptions.Add(new SMSCostOption() { SupplierId = priceList.SupplierID, SupplierRate = convertedRate });
                    }

                    var smsCosts = smsCostByMobileNetworkID.Values.Take(input.Query.LimitResult);
                    foreach (var cost in smsCosts)
                    {
                        cost.CostOptions = cost.CostOptions.OrderBy(item => item.SupplierRate).Take(input.Query.NumberOfOptions).ToList();
                    }

                    return smsCosts;
                }

                return null;
            }

            private bool FilterSMSRates(SupplierSMSRate supplierSMSRate, SMSCostQuery queryInput)
            {
                int mobileNetworkID = supplierSMSRate.MobileNetworkID;

                if (queryInput.MobileNetworkIds != null && queryInput.MobileNetworkIds.Count != 0 && !queryInput.MobileNetworkIds.Contains(mobileNetworkID))
                    return false;

                if (queryInput.MobileCountryIds != null && queryInput.MobileCountryIds.Count != 0)
                {
                    MobileNetwork mobileNetwork = _mobileNetworkManager.GetMobileNetworkById(mobileNetworkID);
                    mobileNetwork.ThrowIfNull("mobileNetwork", mobileNetworkID);
                    int mobileCountryId = mobileNetwork.MobileCountryId;

                    if (!queryInput.MobileCountryIds.Contains(mobileCountryId))
                        return false;
                }

                return true;
            }

            protected override ResultProcessingHandler<SMSCostDetail> GetResultProcessingHandler(DataRetrievalInput<SMSCostQuery> input, BigResult<SMSCostDetail> bigResult)
            {
                var resultProcessingHandler = new ResultProcessingHandler<SMSCostDetail>() { ExportExcelHandler = new SMSCostExcelExportHandler() };
                return resultProcessingHandler;
            }
        }

        private class SMSCostExcelExportHandler : ExcelExportHandler<SMSCostDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<SMSCostDetail> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "SMS Cost Analysis",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() },
                    AutoFitColumns = true
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Mobile Country" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Mobile Network" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Option 1", Width = 50 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Rate 1", Width = 25 });

                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null && context.BigResult.Data.Count() > 0)
                {
                    int maxNumberOfOptions = context.BigResult.Data.Max(itm => itm.CostOptions != null ? itm.CostOptions.Count() : 0);

                    for (var optionNb = 2; optionNb <= maxNumberOfOptions; optionNb++)
                    {
                        sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = $"Option {optionNb}", Width = 50 });
                        sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = $"Rate {optionNb}", Width = 25 });
                    }

                    foreach (var record in context.BigResult.Data)
                    {
                        var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                        row.Cells.Add(new ExportExcelCell { Value = record.MobileCountryName });
                        row.Cells.Add(new ExportExcelCell { Value = record.MobileNetworkName });

                        if (record.CostOptions != null)
                        {
                            foreach (var costOption in record.CostOptions)
                            {
                                row.Cells.Add(new ExportExcelCell { Value = costOption.SupplierName });
                                row.Cells.Add(new ExportExcelCell { Value = costOption.SupplierRate });
                            }

                            int remainingOptions = maxNumberOfOptions - record.CostOptions.Count();
                            if (remainingOptions > 0)
                            {
                                for (int i = 1; i <= remainingOptions; i++)
                                {
                                    row.Cells.Add(new ExportExcelCell { Value = "" });
                                    row.Cells.Add(new ExportExcelCell { Value = "" });
                                }
                            }
                            sheet.Rows.Add(row);
                            continue;
                        }

                        if (maxNumberOfOptions > 0)
                        {
                            for (var optionNb = 1; optionNb <= maxNumberOfOptions; optionNb++)
                            {
                                row.Cells.Add(new ExportExcelCell { Value = "" });
                                row.Cells.Add(new ExportExcelCell { Value = "" });
                            }
                        }
                        else
                        {
                            row.Cells.Add(new ExportExcelCell { Value = "" });
                            row.Cells.Add(new ExportExcelCell { Value = "" });
                        }

                        sheet.Rows.Add(row);
                    }
                }
                context.MainSheet = sheet;
            }
        }

        #endregion
    }
}
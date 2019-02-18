using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.SMSBusinessEntity.Data;
using TOne.WhS.SMSBusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.MobileNetwork.Business;
using Vanrise.MobileNetwork.Entities;

namespace TOne.WhS.SMSBusinessEntity.Business
{
    public class CustomerSMSRateManager
    {
        ICustomerSMSRateDataManager _customerSMSRateDataManager = SMSBEDataFactory.GetDataManager<ICustomerSMSRateDataManager>();

        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<CustomerSMSRateDetail> GetFilteredCustomerSMSRate(DataRetrievalInput<CustomerSMSRateQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new CustomerSMSRateRequestHandler());
        }

        public Dictionary<int, CustomerSMSRateItem> GetEffectiveMobileNetworkRates(int customerID, DateTime dateTime)
        {
            var customerSMSRatesByMobileNetworkIDs = GetCustomerSMSRatesByMobileNetworkID(customerID, dateTime);

            if (customerSMSRatesByMobileNetworkIDs == null)
                return null;

            Dictionary<int, CustomerSMSRateItem> customerSMSRates = new Dictionary<int, CustomerSMSRateItem>();

            foreach (var mobileNetworkKvp in customerSMSRatesByMobileNetworkIDs)
            {
                int mobileNetworkId = mobileNetworkKvp.Key;
                List<CustomerSMSRate> customerSMSRatesByMobileNetworkID = mobileNetworkKvp.Value;

                customerSMSRates.Add(mobileNetworkId, BuildCustomerSMSRateItem(customerSMSRatesByMobileNetworkID, dateTime));
            }
            return customerSMSRates;
        }

        //used in data transformation
        public CustomerSMSRate GetMobileNetworkCustomerRate(int customerID, int mobileNetworkID, DateTime attemptDateTime)
        {
            var customerSMSRatesByMobileNetworkIdsByCustomerIds = GetCachedCustomerRates(attemptDateTime);
            var customerSMSRatesByMobileNetworkIds = customerSMSRatesByMobileNetworkIdsByCustomerIds.GetRecord(customerID);

            if (customerSMSRatesByMobileNetworkIds == null)
                return null;

            return customerSMSRatesByMobileNetworkIds.GetRecord(mobileNetworkID);
        }

        #endregion

        #region Private Methods 

        private Dictionary<int, Dictionary<int, CustomerSMSRate>> GetCachedCustomerRates(DateTime effectiveDate)
        {
            var modifiedEffectiveDate = effectiveDate.Date;
            var cacheName = new GetCachedCustomerSMSRatesCacheName { EffectiveOn = modifiedEffectiveDate };

            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(cacheName, () =>
            {

                var priceLists = new CustomerSMSPriceListManager().GetCachedCustomerSMSPriceLists();

                var customerSMSRates = _customerSMSRateDataManager.GetCustomerSMSRatesEffectiveOn(modifiedEffectiveDate);

                Dictionary<int, Dictionary<int, CustomerSMSRate>> customerSMSRateByMobileNetworkIdByCustomerId = new Dictionary<int, Dictionary<int, CustomerSMSRate>>();
                foreach (var customerSMSRate in customerSMSRates)
                {
                    var priceList = priceLists.GetRecord(customerSMSRate.PriceListID);
                    priceList.ThrowIfNull("priceList", customerSMSRate.PriceListID);

                    Dictionary<int, CustomerSMSRate> mobileNetworkRatesByMobileNetworkID = customerSMSRateByMobileNetworkIdByCustomerId.GetOrCreateItem(priceList.CustomerID);
                    mobileNetworkRatesByMobileNetworkID.Add(customerSMSRate.MobileNetworkID, customerSMSRate);
                }

                return customerSMSRateByMobileNetworkIdByCustomerId;
            });
        }

        private Dictionary<int, List<CustomerSMSRate>> GetCustomerSMSRatesByMobileNetworkID(int customerID, DateTime effectiveDate)
        {
            List<CustomerSMSRate> customerSMSRates = _customerSMSRateDataManager.GetCustomerSMSRatesEffectiveAfter(customerID, effectiveDate);

            if (customerSMSRates == null)
                return null;

            Dictionary<int, List<CustomerSMSRate>> customerSMSRatesByMobileNetworkID = new Dictionary<int, List<CustomerSMSRate>>();

            foreach (var customerSMSRate in customerSMSRates)
            {
                var mobileNetworkList = customerSMSRatesByMobileNetworkID.GetOrCreateItem(customerSMSRate.MobileNetworkID);
                mobileNetworkList.Add(customerSMSRate);
            }

            return customerSMSRatesByMobileNetworkID;
        }

        private CustomerSMSRateItem BuildCustomerSMSRateItem(List<CustomerSMSRate> customerSMSRates, DateTime dateTime)
        {
            if (customerSMSRates == null || customerSMSRates.Count == 0)
                return null;

            CustomerSMSRate customerSMSRate = customerSMSRates.First();

            if (!customerSMSRate.IsEffective(dateTime))
                return new CustomerSMSRateItem() { FutureRate = customerSMSRate };

            if (customerSMSRates.Count > 1)
                return new CustomerSMSRateItem() { CurrentRate = customerSMSRate, FutureRate = customerSMSRates[1] };

            return new CustomerSMSRateItem() { CurrentRate = customerSMSRate };
        }

        #endregion

        #region Private/Internal Classes

        private struct GetCachedCustomerSMSRatesCacheName
        {
            public DateTime EffectiveOn { get; set; }
        }

        private class CustomerSMSRateRequestHandler : BigDataRequestHandler<CustomerSMSRateQuery, CustomerSMSRateItem, CustomerSMSRateDetail>
        {
            CustomerSMSRateManager _manager = new CustomerSMSRateManager();
            MobileNetworkManager _mobileNetworkManager = new MobileNetworkManager();

            public override CustomerSMSRateDetail EntityDetailMapper(CustomerSMSRateItem entity)
            {
                return CustomerSMSRateDetailMapper(entity);
            }

            public override IEnumerable<CustomerSMSRateItem> RetrieveAllData(DataRetrievalInput<CustomerSMSRateQuery> input)
            {
                if (input != null && input.Query != null)
                {
                    List<CustomerSMSRateItem> _customerSMSRates = _manager.GetEffectiveMobileNetworkRates(input.Query.CustomerID, input.Query.EffectiveDate).Values.ToList();
                    return _customerSMSRates.Where(item => FilterCustomerSMSRates(item, input.Query));
                }

                return null;
            }

            private CustomerSMSRateDetail CustomerSMSRateDetailMapper(CustomerSMSRateItem customerSMSRate)
            {
                if (customerSMSRate == null)
                    return null;

                var currentRate = customerSMSRate.CurrentRate;
                var futureRate = customerSMSRate.FutureRate;

                int mobileNetworkID = 0;
                if (currentRate != null)
                {

                    mobileNetworkID = currentRate.MobileNetworkID;
                }
                else if (futureRate != null)
                    mobileNetworkID = futureRate.MobileNetworkID;

                MobileNetwork mobileNetwork = _mobileNetworkManager.GetMobileNetworkById(mobileNetworkID);
                mobileNetwork.ThrowIfNull("mobileNetwork", mobileNetworkID);

                return new CustomerSMSRateDetail()
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

            private bool FilterCustomerSMSRates(CustomerSMSRateItem customerSMSRate, CustomerSMSRateQuery queryInput)
            {
                if (customerSMSRate.CurrentRate == null)
                    return false;

                int mobileNetworkID = customerSMSRate.CurrentRate.MobileNetworkID;

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

            protected override ResultProcessingHandler<CustomerSMSRateDetail> GetResultProcessingHandler(DataRetrievalInput<CustomerSMSRateQuery> input, BigResult<CustomerSMSRateDetail> bigResult)
            {
                var resultProcessingHandler = new ResultProcessingHandler<CustomerSMSRateDetail>() { ExportExcelHandler = new CustomerSMSRateExcelExportHandler() };
                return resultProcessingHandler;
            }
        }

        private class CustomerSMSRateExcelExportHandler : ExcelExportHandler<CustomerSMSRateDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<CustomerSMSRateDetail> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "Customer SMS Rates",
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
            ICustomerSMSRateDataManager _customerSMSRateDataManager = SMSBEDataFactory.GetDataManager<ICustomerSMSRateDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _customerSMSRateDataManager.AreCustomerSMSRatesUpdated(ref _updateHandle);
            }
        }

        #endregion
    }
}
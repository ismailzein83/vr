﻿using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.SMSBusinessEntity.Data;
using TOne.WhS.SMSBusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Security.Business;

namespace TOne.WhS.SMSBusinessEntity.Business
{
    public class CustomerSMSPriceListManager
    {
        ICustomerSMSPriceListDataManager _customerSMSPriceListDataManager = SMSBEDataFactory.GetDataManager<ICustomerSMSPriceListDataManager>();

        #region Public Methods
        public Vanrise.Entities.IDataRetrievalResult<CustomerSMSPriceListDetail> GetFilteredCustomerSMSPriceLists(DataRetrievalInput<CustomerSMSPriceListQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new CustomerSMSPriceListRequestHandler());
        }

        public CustomerSMSPriceList CreateCustomerSMSPriceList(int customerID, int currencyID, DateTime effectiveDate, long processInstanceID, int userID)
        {
            return new CustomerSMSPriceList()
            {
                ID = ReserveIdRange(1),
                CustomerID = customerID,
                CurrencyID = currencyID,
                EffectiveOn = effectiveDate,
                UserID = userID,
                ProcessInstanceID = processInstanceID
            };
        }

        public CustomerSMSPriceList GetCustomerSMSPriceListByID(long priceListID)
        {
            var customerSMSPriceLists = GetCachedCustomerSMSPriceLists();
            return customerSMSPriceLists != null ? customerSMSPriceLists.GetRecord(priceListID) : null;
        }

        public Dictionary<long, CustomerSMSPriceList> GetCachedCustomerSMSPriceLists()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedCustomerSMSPriceLists", () =>
            {
                var priceLists = _customerSMSPriceListDataManager.GetCustomerSMSPriceLists();
                return priceLists != null ? priceLists.ToDictionary(item => item.ID, item => item) : null;
            });
        }

        #endregion

        #region Private Methods

        private long ReserveIdRange(int numberOfIds)
        {
            long startingId;
            IDManager.Instance.ReserveIDRange(this.GetType(), numberOfIds, out startingId);
            return startingId;
        }

        #endregion


        #region Internal/Private Classes
        internal class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ICustomerSMSPriceListDataManager _customerSMSPriceListDataManager = SMSBEDataFactory.GetDataManager<ICustomerSMSPriceListDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _customerSMSPriceListDataManager.AreCustomerSMSPriceListUpdated(ref _updateHandle);
            }
        }

        private class CustomerSMSPriceListRequestHandler : BigDataRequestHandler<CustomerSMSPriceListQuery, CustomerSMSPriceList, CustomerSMSPriceListDetail>
        {
            ICustomerSMSPriceListDataManager _customerSMSPriceListDataManager = SMSBEDataFactory.GetDataManager<ICustomerSMSPriceListDataManager>();
            public override CustomerSMSPriceListDetail EntityDetailMapper(CustomerSMSPriceList entity)
            {
                return new CustomerSMSPriceListDetail()
                {
                    ID = entity.ID,
                    CurrencyName = new CurrencyManager().GetCurrencySymbol(entity.CurrencyID),
                    CustomerName = new CarrierAccountManager().GetCarrierAccountName(entity.CustomerID),
                    EffectiveOn = entity.EffectiveOn,
                    UserName = new UserManager().GetUserName(entity.UserID)
                };
            }

            public override IEnumerable<CustomerSMSPriceList> RetrieveAllData(DataRetrievalInput<CustomerSMSPriceListQuery> input)
            {
                input.ThrowIfNull("input");
                var customerSMSPriceLists = _customerSMSPriceListDataManager.GetCustomerSMSPriceLists();

                if (input.Query == null)
                    return customerSMSPriceLists;

                return customerSMSPriceLists != null ? customerSMSPriceLists.Where(item => FilterCustomerSMSPriceList(item, input.Query)) : null;
            }
            protected override ResultProcessingHandler<CustomerSMSPriceListDetail> GetResultProcessingHandler(DataRetrievalInput<CustomerSMSPriceListQuery> input, BigResult<CustomerSMSPriceListDetail> bigResult)
            {
                var resultProcessingHandler = new ResultProcessingHandler<CustomerSMSPriceListDetail>() { ExportExcelHandler = new CustomerSMSPriceListExcelExportHandler() };
                return resultProcessingHandler;
            }

            private bool FilterCustomerSMSPriceList(CustomerSMSPriceList customerSMSPriceList, CustomerSMSPriceListQuery queryInput)
            {
                if (queryInput.CustomerIds != null && queryInput.CustomerIds.Count != 0 && !queryInput.CustomerIds.Contains(customerSMSPriceList.CustomerID))
                    return false;

                if (queryInput.EffectiveDate.HasValue && queryInput.EffectiveDate.Value > customerSMSPriceList.EffectiveOn)
                    return false;

                return true;
            }
        }

        private class CustomerSMSPriceListExcelExportHandler : ExcelExportHandler<CustomerSMSPriceListDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<CustomerSMSPriceListDetail> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "Customer SMS Price Lists",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() },
                    AutoFitColumns = true
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "ID", CellType = ExcelCellType.Number, NumberType = NumberType.BigInt });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Customer" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Currency" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Effective On", Width = 45, CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.Date });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Created By" });

                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null && context.BigResult.Data.Count() > 0)
                {

                    foreach (var record in context.BigResult.Data)
                    {
                        var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                        row.Cells.Add(new ExportExcelCell { Value = record.ID });
                        row.Cells.Add(new ExportExcelCell { Value = record.CustomerName });
                        row.Cells.Add(new ExportExcelCell { Value = record.CurrencyName });
                        row.Cells.Add(new ExportExcelCell { Value = record.EffectiveOn });
                        row.Cells.Add(new ExportExcelCell { Value = record.UserName });

                        sheet.Rows.Add(row);
                    }
                }

                context.MainSheet = sheet;
            }
        }
        #endregion
    }
}
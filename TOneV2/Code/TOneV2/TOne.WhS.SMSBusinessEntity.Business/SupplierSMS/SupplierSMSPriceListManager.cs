using System;
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
    public class SupplierSMSPriceListManager
    {
        ISupplierSMSPriceListDataManager _supplierSMSPriceListDataManager = SMSBEDataFactory.GetDataManager<ISupplierSMSPriceListDataManager>();

        #region Public Methods
        public Vanrise.Entities.IDataRetrievalResult<SupplierSMSPriceListDetail> GetFilteredSupplierSMSPriceLists(DataRetrievalInput<SupplierSMSPriceListQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new SupplierSMSPriceListRequestHandler());
        }

        public SupplierSMSPriceList CreateSupplierSMSPriceList(int supplierID, int currencyID, DateTime effectiveDate, long processInstanceID, int userID)
        {
            return new SupplierSMSPriceList()
            {
                ID = ReserveIdRange(1),
                SupplierID = supplierID,
                CurrencyID = currencyID,
                EffectiveOn = effectiveDate,
                UserID = userID,
                ProcessInstanceID = processInstanceID
            };
        }

        public SupplierSMSPriceList GetSupplierSMSPriceListByID(long priceListID)
        {
            var supplierSMSPriceLists = GetCachedSupplierSMSPriceLists();
            return supplierSMSPriceLists != null ? supplierSMSPriceLists.GetRecord(priceListID) : null;
        }

        public Dictionary<long, SupplierSMSPriceList> GetCachedSupplierSMSPriceLists()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedSupplierSMSPriceLists", () =>
            {
                var priceLists = _supplierSMSPriceListDataManager.GetSupplierSMSPriceLists();

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

        #region Private/Internal Classes

        internal class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ISupplierSMSPriceListDataManager _supplierSMSPriceListDataManager = SMSBEDataFactory.GetDataManager<ISupplierSMSPriceListDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _supplierSMSPriceListDataManager.AreSupplierSMSPriceListUpdated(ref _updateHandle);
            }
        }

        private class SupplierSMSPriceListRequestHandler : BigDataRequestHandler<SupplierSMSPriceListQuery, SupplierSMSPriceList, SupplierSMSPriceListDetail>
        {
            ISupplierSMSPriceListDataManager _supplierSMSPriceListDataManager = SMSBEDataFactory.GetDataManager<ISupplierSMSPriceListDataManager>();
            public override SupplierSMSPriceListDetail EntityDetailMapper(SupplierSMSPriceList entity)
            {
                return new SupplierSMSPriceListDetail()
                {
                    ID = entity.ID,
                    CurrencyName = new CurrencyManager().GetCurrencySymbol(entity.CurrencyID),
                    SupplierName = new CarrierAccountManager().GetCarrierAccountName(entity.SupplierID),
                    EffectiveOn = entity.EffectiveOn,
                    UserName = new UserManager().GetUserName(entity.UserID)
                };
            }

            public override IEnumerable<SupplierSMSPriceList> RetrieveAllData(DataRetrievalInput<SupplierSMSPriceListQuery> input)
            {
                input.ThrowIfNull("input");
                var supplierSMSPriceLists = _supplierSMSPriceListDataManager.GetSupplierSMSPriceLists();

                if (input.Query == null)
                    return supplierSMSPriceLists;

                return supplierSMSPriceLists != null ? supplierSMSPriceLists.Where(item => FilterSupplierSMSPriceList(item, input.Query)) : null;
            }
            protected override ResultProcessingHandler<SupplierSMSPriceListDetail> GetResultProcessingHandler(DataRetrievalInput<SupplierSMSPriceListQuery> input, BigResult<SupplierSMSPriceListDetail> bigResult)
            {
                var resultProcessingHandler = new ResultProcessingHandler<SupplierSMSPriceListDetail>() { ExportExcelHandler = new SupplierSMSPriceListExcelExportHandler() };
                return resultProcessingHandler;
            }

            private bool FilterSupplierSMSPriceList(SupplierSMSPriceList supplierSMSPriceList, SupplierSMSPriceListQuery queryInput)
            {
                if (queryInput.SupplierIds != null && queryInput.SupplierIds.Count != 0 && !queryInput.SupplierIds.Contains(supplierSMSPriceList.SupplierID))
                    return false;

                if (queryInput.EffectiveDate.HasValue && queryInput.EffectiveDate.Value > supplierSMSPriceList.EffectiveOn)
                    return false;

                return true;
            }
        }

        private class SupplierSMSPriceListExcelExportHandler : ExcelExportHandler<SupplierSMSPriceListDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<SupplierSMSPriceListDetail> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "Supplier SMS Price Lists",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() },
                    AutoFitColumns = true
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "ID", CellType = ExcelCellType.Number, NumberType = NumberType.BigInt });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Supplier" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Currency" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Effective On", Width = 45, CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.DateTime });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Created By" });



                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null && context.BigResult.Data.Count() > 0)
                {

                    foreach (var record in context.BigResult.Data)
                    {
                        var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                        row.Cells.Add(new ExportExcelCell { Value = record.ID });
                        row.Cells.Add(new ExportExcelCell { Value = record.SupplierName });
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
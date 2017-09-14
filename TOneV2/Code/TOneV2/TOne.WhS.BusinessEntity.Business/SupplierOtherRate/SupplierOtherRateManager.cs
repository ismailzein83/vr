using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Common.Business;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SupplierOtherRateManager
    {

        #region Public Methods
        public Vanrise.Entities.IDataRetrievalResult<SupplierOtherRateDetail> GetFilteredSupplierOtherRates(Vanrise.Entities.DataRetrievalInput<SupplierOtherRateQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new SupplierOtherRateRequestHandler());
        }

        #endregion

       
      

        #region Private Classes

        private class SupplierOtherRateRequestHandler : BigDataRequestHandler<SupplierOtherRateQuery, SupplierOtherRate, SupplierOtherRateDetail>
        {
            private SupplierPriceListManager _supplierPriceListManager;
            private RateTypeManager _rateTypeManager;
            private CurrencyExchangeRateManager _currencyExchangeRateManager;
            private CurrencyManager _currencyManager;


            public SupplierOtherRateRequestHandler()
            {
                _supplierPriceListManager = new SupplierPriceListManager();
                _rateTypeManager = new RateTypeManager();
                _currencyExchangeRateManager = new CurrencyExchangeRateManager();
                _currencyManager = new CurrencyManager();
            }
            public override SupplierOtherRateDetail EntityDetailMapper(SupplierOtherRate entity)
            {
                throw new NotSupportedException();
            }

            public override IEnumerable<SupplierOtherRate> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<SupplierOtherRateQuery> input)
            {
                ISupplierOtherRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierOtherRateDataManager>();
                return dataManager.GetFilteredSupplierOtherRates(input.Query);
            }

            protected override BigResult<SupplierOtherRateDetail> AllRecordsToBigResult(DataRetrievalInput<SupplierOtherRateQuery> input, IEnumerable<SupplierOtherRate> allRecords)
            {
                int? systemCurrencyId = (input.Query.IsSystemCurrency) ? (int?)new Vanrise.Common.Business.ConfigManager().GetSystemCurrencyId() : null;
                return allRecords.ToBigResult(input, null, (entity) => SupplierOtherRateDetailMapper(entity, systemCurrencyId));
            }
            protected override ResultProcessingHandler<SupplierOtherRateDetail> GetResultProcessingHandler(DataRetrievalInput<SupplierOtherRateQuery> input, BigResult<SupplierOtherRateDetail> bigResult)
            {
                return new ResultProcessingHandler<SupplierOtherRateDetail>
                {
                    ExportExcelHandler = new SupplierOtherRateExcelExportHandler()
                };
            }

            private SupplierOtherRateDetail SupplierOtherRateDetailMapper(SupplierOtherRate supplierOtherRate, int? systemCurrencyId)
            {
                int currencyId;

                if (supplierOtherRate.CurrencyId.HasValue)
                    currencyId = supplierOtherRate.CurrencyId.Value;
                else
                {
                    SupplierPriceList priceList = _supplierPriceListManager.GetPriceList(supplierOtherRate.PriceListId);
                    currencyId = priceList.CurrencyId;
                }
                int displayedCurrencyId = systemCurrencyId.HasValue ? systemCurrencyId.Value : currencyId;
                string rateTypeDescription = _rateTypeManager.GetRateTypeName(supplierOtherRate.RateTypeId.Value);

                return new SupplierOtherRateDetail()
                {
                    Entity = supplierOtherRate,
                    DisplayedCurrency = _currencyManager.GetCurrencySymbol(displayedCurrencyId),
                    DisplayedRate = systemCurrencyId.HasValue ? _currencyExchangeRateManager.ConvertValueToCurrency(supplierOtherRate.Rate,currencyId,systemCurrencyId.Value,supplierOtherRate.BED):supplierOtherRate.Rate,
                    RateTypeDescription = rateTypeDescription
                };
            }
        }

        private class SupplierOtherRateExcelExportHandler : ExcelExportHandler<SupplierOtherRateDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<SupplierOtherRateDetail> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "Supplier Other Rates",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "ID" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Rate" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Rate Type" });
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
                            row.Cells.Add(new ExportExcelCell { Value = record.DisplayedRate });
                            row.Cells.Add(new ExportExcelCell { Value = record.RateTypeDescription });
                            row.Cells.Add(new ExportExcelCell { Value = record.DisplayedCurrency });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.BED });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.EED });
                        }
                    }
                }
                context.MainSheet = sheet;
            }
        }

        #endregion
    }
}

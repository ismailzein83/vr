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

       
        #region Mappers
        private SupplierOtherRateDetail SupplierOtherRateDetailMapper(SupplierOtherRate supplierOtherRate)
        {
            CurrencyManager currencyManager = new CurrencyManager();
            int currencyId;

            if (supplierOtherRate.CurrencyId.HasValue)
                currencyId = supplierOtherRate.CurrencyId.Value;
            else
            {
                SupplierPriceListManager manager = new SupplierPriceListManager();
                SupplierPriceList priceList = manager.GetPriceList(supplierOtherRate.PriceListId);
                currencyId = priceList.CurrencyId;
            }

            RateTypeManager rateTypeManager = new RateTypeManager();
            string rateTypeDescription = rateTypeManager.GetRateTypeName(supplierOtherRate.RateTypeId.Value);

            return new SupplierOtherRateDetail()
            {
                Entity = supplierOtherRate,
                CurrencyName = currencyManager.GetCurrencySymbol(currencyId),
                RateTypeDescription = rateTypeDescription
            };
        }
        #endregion

        #region Private Classes

        private class SupplierOtherRateRequestHandler : BigDataRequestHandler<SupplierOtherRateQuery, SupplierOtherRate, SupplierOtherRateDetail>
        {
            public override SupplierOtherRateDetail EntityDetailMapper(SupplierOtherRate entity)
            {
                SupplierOtherRateManager manager = new SupplierOtherRateManager();
                return manager.SupplierOtherRateDetailMapper(entity);
            }

            public override IEnumerable<SupplierOtherRate> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<SupplierOtherRateQuery> input)
            {
                ISupplierOtherRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierOtherRateDataManager>();
                return dataManager.GetFilteredSupplierOtherRates(input.Query);
            }
            protected override ResultProcessingHandler<SupplierOtherRateDetail> GetResultProcessingHandler(DataRetrievalInput<SupplierOtherRateQuery> input, BigResult<SupplierOtherRateDetail> bigResult)
            {
                return new ResultProcessingHandler<SupplierOtherRateDetail>
                {
                    ExportExcelHandler = new SupplierOtherRateExcelExportHandler()
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
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.Rate });
                            row.Cells.Add(new ExportExcelCell { Value = record.RateTypeDescription });
                            row.Cells.Add(new ExportExcelCell { Value = record.CurrencyName });
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

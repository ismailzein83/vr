using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Analytics.Data;
using TOne.WhS.Analytics.Entities;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Common.Business;

namespace TOne.WhS.Analytics.Business
{
    public class RepeatedNumberManager
    {
        private readonly CarrierAccountManager _carrierAccountManager;
        private readonly SaleZoneManager _saleZoneManager;

        public RepeatedNumberManager()
        {
            _carrierAccountManager = new CarrierAccountManager();
            _saleZoneManager = new SaleZoneManager();
        }
        public Vanrise.Entities.IDataRetrievalResult<RepeatedNumberDetail> GetAllFilteredRepeatedNumbers(Vanrise.Entities.DataRetrievalInput<RepeatedNumberQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new RepeatedNumberRequestHandler());
        }

        public RepeatedNumberDetail RepeatedNumberDetailMapper(RepeatedNumber repeatedNumber)
        {
            string customerName = repeatedNumber.CustomerId.HasValue ? _carrierAccountManager.GetCarrierAccountName(repeatedNumber.CustomerId.Value) : null;
            string supplierName = repeatedNumber.SupplierId.HasValue ? _carrierAccountManager.GetCarrierAccountName(repeatedNumber.SupplierId.Value) : null;
            string saleZoneName = repeatedNumber.SaleZoneId.HasValue ? _saleZoneManager.GetSaleZoneName(repeatedNumber.SaleZoneId.Value) : null;

            RepeatedNumberDetail repeatedNumberDetail = new RepeatedNumberDetail
            {
                Entity = repeatedNumber,
                CustomerName = customerName,
                SupplierName = supplierName,
                SaleZoneName = saleZoneName
            };
            return repeatedNumberDetail;
        }

        #region Private Classes

        private class RepeatedNumberRequestHandler : BigDataRequestHandler<RepeatedNumberQuery, RepeatedNumber, RepeatedNumberDetail>
        {
            public override RepeatedNumberDetail EntityDetailMapper(RepeatedNumber entity)
            {
                RepeatedNumberManager manager = new RepeatedNumberManager();
                return manager.RepeatedNumberDetailMapper(entity);
            }

            public override IEnumerable<RepeatedNumber> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<RepeatedNumberQuery> input)
            {
                IRepeatedNumberDataManager dataManager = AnalyticsDataManagerFactory.GetDataManager<IRepeatedNumberDataManager>();
                return dataManager.GetAllFilteredRepeatedNumbers(input);
            }

            protected override ResultProcessingHandler<RepeatedNumberDetail> GetResultProcessingHandler(DataRetrievalInput<RepeatedNumberQuery> input, BigResult<RepeatedNumberDetail> bigResult)
            {
                return new ResultProcessingHandler<RepeatedNumberDetail>
                {
                    ExportExcelHandler = new RepeatedNumberExcelExportHandler(input.Query)
                };
            }
        }

        private class RepeatedNumberExcelExportHandler : ExcelExportHandler<RepeatedNumberDetail>
        {
            RepeatedNumberQuery _query;
            public RepeatedNumberExcelExportHandler(RepeatedNumberQuery query)
            {
                if (query == null)
                    throw new ArgumentNullException("query");
                _query = query;
            }
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<RepeatedNumberDetail> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "Repeated Numbers",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
                };
                if(_query.Filter!=null && _query.Filter.ColumnsToShow!=null && _query.Filter.ColumnsToShow.Count > 0)
                {
                    if (_query.Filter.ColumnsToShow.Contains("CustomerName"))
                        sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Customer", Width = 50 });
                    if (_query.Filter.ColumnsToShow.Contains("SupplierName"))
                        sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Supplier", Width = 50 });
                    if (_query.Filter.ColumnsToShow.Contains("SaleZoneName"))
                        sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Zone", Width = 50 });
                    if (_query.Filter.ColumnsToShow.Contains("Attempt"))
                        sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Attempts" });
                    if (_query.Filter.ColumnsToShow.Contains("DurationInMinutes"))
                        sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Duration (min)" });
                    if (_query.Filter.ColumnsToShow.Contains("PhoneNumber"))
                        sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Phone Number" });
                }
                else
                {
                    sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Customer", Width = 50 });
                    sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Supplier", Width = 50 });
                    sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Zone", Width = 50 });
                    sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Attempts" });
                    sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Duration (min)" });
                    sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Phone Number" });
                }

                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    foreach (var record in context.BigResult.Data)
                    {
                        if (record.Entity != null)
                        {
                            var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                            sheet.Rows.Add(row);
                            if (_query.Filter != null && _query.Filter.ColumnsToShow != null && _query.Filter.ColumnsToShow.Count > 0)
                            {
                                if(_query.Filter.ColumnsToShow.Contains("CustomerName"))
                                    row.Cells.Add(new ExportExcelCell { Value = record.CustomerName });
                                if (_query.Filter.ColumnsToShow.Contains("SupplierName"))
                                    row.Cells.Add(new ExportExcelCell { Value = record.SupplierName });
                                if (_query.Filter.ColumnsToShow.Contains("SaleZoneName"))
                                    row.Cells.Add(new ExportExcelCell { Value = record.SaleZoneName });
                                if (_query.Filter.ColumnsToShow.Contains("Attempt"))
                                    row.Cells.Add(new ExportExcelCell { Value = record.Entity.Attempt });
                                if (_query.Filter.ColumnsToShow.Contains("DurationInMinutes"))
                                    row.Cells.Add(new ExportExcelCell { Value = record.Entity.DurationInMinutes });
                                if (_query.Filter.ColumnsToShow.Contains("PhoneNumber"))
                                    row.Cells.Add(new ExportExcelCell { Value = record.Entity.PhoneNumber });

                            }
                            else
                            {
                                row.Cells.Add(new ExportExcelCell { Value = record.CustomerName });
                                row.Cells.Add(new ExportExcelCell { Value = record.SupplierName });
                                row.Cells.Add(new ExportExcelCell { Value = record.SaleZoneName });
                                row.Cells.Add(new ExportExcelCell { Value = record.Entity.Attempt });
                                row.Cells.Add(new ExportExcelCell { Value = record.Entity.DurationInMinutes });
                                row.Cells.Add(new ExportExcelCell { Value = record.Entity.PhoneNumber });
                            }
                        }
                    }
                }
                context.MainSheet = sheet;
            }
        }

        #endregion
    }
}

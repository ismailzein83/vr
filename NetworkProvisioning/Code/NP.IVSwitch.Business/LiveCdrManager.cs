using NP.IVSwitch.Data;
using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
namespace NP.IVSwitch.Business
{
    public class LiveCdrManager
    {
       
        public Vanrise.Entities.IDataRetrievalResult<LiveCdrItemDetail> GetFilteredLiveCdrs(Vanrise.Entities.DataRetrievalInput<LiveCdrQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new LiveCdrRequestHandler());
        }

        private class LiveCdrRequestHandler : BigDataRequestHandler<LiveCdrQuery, LiveCdrItem, LiveCdrItemDetail>
        {
            EndPointManager endPointManager = new EndPointManager();
            RouteManager routeManager = new RouteManager();
            public override LiveCdrItemDetail EntityDetailMapper(LiveCdrItem entity)
            {
                return new LiveCdrItemDetail()
                {
                    customerName = endPointManager.GetEndPointAccountName(entity.customerId),
                  sourceIP =entity.sourceIP,
                 attemptDate =entity.attemptDate,
                 cli =entity.cli,
                 destinationCode=entity.destinationCode,
                 destinationName =entity.destinationName,
                    supplierName = routeManager.GetRouteCarrierAccountName(entity.routeId),
                 routeIP=   entity.routeIP,
                 supplierCode=entity.supplierCode,
                 supplierZone=entity.supplierZone,   
                 alertDate=entity.alertDate,
                 connectDate=   entity.connectDate,
                 duration=entity.duration
                };
            }

            public override IEnumerable<LiveCdrItem> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<LiveCdrQuery> input)
            {
                ICDRDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<ICDRDataManager>();
                Helper.SetSwitchConfig(dataManager);
                return dataManager.GetFilteredLiveCdrs(input.Query.EndPointIds, input.Query.RouteIds, input.Query.SourceIP, input.Query.RouteIP, input.Query.CallsMode, input.Query.TimeUnit, input.Query.Time);

            }

            protected override ResultProcessingHandler<LiveCdrItemDetail> GetResultProcessingHandler(DataRetrievalInput<LiveCdrQuery> input, BigResult<LiveCdrItemDetail> bigResult)
            {
                return new ResultProcessingHandler<LiveCdrItemDetail>
                {
                    ExportExcelHandler = new LiveCdrItemDetailExportExcelHandler()
                };
            }
        }
        private class LiveCdrItemDetailExportExcelHandler : ExcelExportHandler<LiveCdrItemDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<LiveCdrItemDetail> context)
            {
                var sheet = new ExportExcelSheet()
                {
                    SheetName = "Live CDRs",
                    Header = new ExportExcelHeader() { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Customer" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Source IP"});
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Attempt", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.Date });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "CLI" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Destination Code" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Destination Name" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Supplier" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Route IP" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Supplier Code" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Supplier Zone" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Alert", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.Date });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Connection", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.Date });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Duration" });

                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    foreach (var record in context.BigResult.Data)
                    {
                        if (record != null)
                        {
                            var row = new ExportExcelRow() { Cells = new List<ExportExcelCell>() };
                            row.Cells.Add(new ExportExcelCell() { Value = record.customerName });
                            row.Cells.Add(new ExportExcelCell() { Value = record.sourceIP });
                            row.Cells.Add(new ExportExcelCell() { Value = record.attemptDate });
                            row.Cells.Add(new ExportExcelCell() { Value = record.cli });
                            row.Cells.Add(new ExportExcelCell() { Value = record.destinationCode });
                            row.Cells.Add(new ExportExcelCell() { Value = record.destinationName });
                            row.Cells.Add(new ExportExcelCell() { Value = record.supplierName });
                            row.Cells.Add(new ExportExcelCell() { Value = record.routeIP });
                            row.Cells.Add(new ExportExcelCell() { Value = record.supplierCode });
                            row.Cells.Add(new ExportExcelCell() { Value = record.supplierZone });
                            row.Cells.Add(new ExportExcelCell() { Value = record.alertDate });
                            row.Cells.Add(new ExportExcelCell() { Value = record.connectDate });
                            row.Cells.Add(new ExportExcelCell() { Value = record.duration });
                            sheet.Rows.Add(row);
                        }
                    }
                }

                context.MainSheet = sheet;
            }
        }
    }
}

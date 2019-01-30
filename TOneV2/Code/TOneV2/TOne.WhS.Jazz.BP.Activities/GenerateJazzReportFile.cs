using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Excel;
using Vanrise.Common.Business;
using Vanrise.Entities;
using TOne.WhS.Jazz.Entities;

namespace TOne.WhS.Jazz.BP.Activities
{
    public sealed class GenerateJazzReportFile : CodeActivity
    {
        public InArgument<List<JazzReport>> JazzReports { get; set; }
        public OutArgument<long> FileId { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            var jazzReports = JazzReports.Get(context);
            var excelFile = new VRExcelFile();
            if (jazzReports != null && jazzReports.Count > 0)
            {
                foreach(var report in jazzReports)
                {
                    var excelSheet = excelFile.CreateSheet();
                    var excelTable = excelSheet.CreateTable(0,0);
                    var headerRow=  excelTable.CreateHeaderRow();
                    if (report.Direction.Equals(ReportDefinitionDirectionEnum.In))
                    {
                        CreateCell("Customer", headerRow);
                        CreateCell("SaleDuration", headerRow);
                        CreateCell("SaleNet", headerRow);
                    }
                    else
                    {
                        CreateCell("Supplier", headerRow);
                        CreateCell("CostDuration", headerRow);
                        CreateCell("CostNet", headerRow);
                    }
                    CreateCell("Market", headerRow);
                    CreateCell("Region", headerRow);
                    CreateCell("Market Value", headerRow);
                    CreateCell("Region Value", headerRow);
                    if(report.ReportData!=null && report.ReportData.Count > 0)
                    {
                        foreach(var reportData in report.ReportData)
                        {
                            if(reportData.Markets!=null && reportData.Markets.Count > 0)
                            {
                                foreach(var market in reportData.Markets)
                                {
                                    if(market.Regions!=null & market.Regions.Count > 0)
                                    {
                                        foreach(var region in market.Regions)
                                        {
                                            var row = excelTable.CreateDataRow();
                                            CreateCell(string.Join(reportData.CarrierAccountId.ToString(), reportData.CarrierAccountName), row);
                                            CreateCell(reportData.Amount.ToString(), row);
                                            CreateCell(reportData.Duration.ToString(), row);
                                            CreateCell(string.Join(market.MarketName," ", market.Percentage), row);
                                            CreateCell(string.Join(region.RegionName," ", region.Percentage), row);
                                            CreateCell(market.MarketValue.ToString(), row);
                                            CreateCell(region.RegionValue.ToString(), row);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            var file = excelFile.GenerateExcelFile();
            var fileManager = new VRFileManager();
            var fileId = fileManager.AddFile(new VRFile {
                Name="Jazz Reports",
                Content = file });
            FileId.Set(context, fileId);

        }
        public void CreateCell(string cellValue, VRExcelTableRow row)
        {
            var cell = row.CreateCell();
            cell.SetValue(cellValue);
        }
    }
}

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
using System.IO;
using Aspose.Cells;

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
                    excelSheet.SheetName = (report.ReportName.Length > 31) ? string.Format("{0}...", report.ReportName.Substring(0, 27)) : report.ReportName;
                    var excelTable = excelSheet.CreateTable(0,0);

                    var title = excelTable.CreateHeaderRow();
                    CreateCell(report.ReportName, title);

                    var headerRow = excelTable.CreateHeaderRow();
                    if (report.Direction.Equals(ReportDefinitionDirectionEnum.In))
                    {
                        CreateCell("Customer", headerRow);
                    }
                    else
                    {
                        CreateCell("Supplier", headerRow);
                    }
                    CreateCell("Duration", headerRow);
                    if (!report.SplitRateValue.HasValue)
                    {
                        CreateCell("Amount", headerRow);
                    }
                    else
                    {
                        CreateCell(string.Format("Amount Rate-{0}", Decimal.Round(report.SplitRateValue.Value,4)), headerRow);
                        CreateCell(string.Format("Amount {0}", Decimal.Round(report.SplitRateValue.Value,4)), headerRow);
                    }

                    if (report.TaxOption.HasValue)
                        CreateCell("STAX", headerRow);

                    CreateCell("Market", headerRow);
                    CreateCell("Region", headerRow);

                    if (!report.SplitRateValue.HasValue)
                    {
                        CreateCell("Region Value", headerRow);
                    }

                    else
                    {
                        CreateCell(string.Format("Region Value Rate-{0}", Decimal.Round(report.SplitRateValue.Value,4)), headerRow);
                        CreateCell(string.Format("Region Value {0}", Decimal.Round(report.SplitRateValue.Value,4)), headerRow);
                    }
              
                    if (report.ReportData!=null && report.ReportData.Count > 0)
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
                                            CreateCell(string.Format("{0}{1}",reportData.CarrierAccountId.ToString(), reportData.CarrierAccountName), row);
                                            CreateCell(reportData.Duration.ToString(), row);
                                            CreateCell(reportData.Amount1.ToString(), row);

                                            if (report.SplitRateValue.HasValue)
                                                CreateCell(reportData.Amount2.ToString(), row);

                                            if (report.TaxOption.HasValue)
                                                CreateCell(reportData.Tax.ToString(), row);

                                            CreateCell(string.Format("{0} {1}%",market.MarketName, market.Percentage), row);
                                            CreateCell(string.Format("{0} {1}%", region.RegionName, region.Percentage), row);
                                            CreateCell(region.RegionValue1.ToString(), row);

                                            if (report.SplitRateValue.HasValue)
                                                CreateCell(region.RegionValue2.ToString(), row);

                                           
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
            var fileId = fileManager.AddFile(new VRFile
            {
                Name = "JazzReports.xlsx",
                Content = file
            });
            
            FileId.Set(context, fileId);

        }
        private void CreateCell(string cellValue, VRExcelTableRow row)
        {
            var cell = row.CreateCell();
            cell.SetValue(cellValue);
        }
    }
}

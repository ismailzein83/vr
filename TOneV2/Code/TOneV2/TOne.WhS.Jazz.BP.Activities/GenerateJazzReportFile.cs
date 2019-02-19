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
                    var excelTable = excelSheet.CreateTable(1,0);

                    excelTable.EnableRowVerticalMerge();
                    excelTable.EnableMergeHeaders();

                    VRExcelTableRowCellStyle cellStyle1 = new VRExcelTableRowCellStyle
                    {
                        VerticalAlignment=VRExcelContainerVerticalAlignment.Center,
                        HorizontalAlignment=VRExcelContainerHorizontalAlignment.Center
                    };
                    VRExcelTableRowCellStyle cellStyle2 = new VRExcelTableRowCellStyle
                    {
                        VerticalAlignment = VRExcelContainerVerticalAlignment.Center
                    };
                    VRExcelCell titleCell = new VRExcelCell
                    {
                        Value = report.ReportName,
                        RowIndex = 0,
                        ColumnIndex = 0,
                        Style = new VRExcelCellStyle
                        {
                            HorizontalAlignment = VRExcelContainerHorizontalAlignment.Center
                        }
                    };
                    titleCell.MergeCells(6 + (report.TaxOption.HasValue ? 1 : 0) + (report.SplitRateValue.HasValue ? 2 : 0), 1);
                    excelSheet.AddCell(titleCell);

                    var headerRow = excelTable.CreateHeaderRow();
                    headerRow.CreateStyle();
                    if (report.Direction.Equals(ReportDefinitionDirectionEnum.In))
                    {
                        CreateCell("Customer", headerRow, cellStyle1);
                    }
                    else
                    {
                        CreateCell("Supplier", headerRow, cellStyle1);
                    }

                    CreateCell("Duration", headerRow, cellStyle1);
                    if (!report.SplitRateValue.HasValue)
                    {
                        CreateCell("Amount", headerRow, cellStyle1);
                    }
                    else
                    {

                        CreateCell(string.Format("Amount Rate-{0}", Decimal.Round(report.SplitRateValue.Value,4)), headerRow, cellStyle1);
                        CreateCell(string.Format("Amount {0}", Decimal.Round(report.SplitRateValue.Value,4)), headerRow, cellStyle1);
                    }

                    if (report.TaxOption.HasValue)
                    {
                        CreateCell("STAX", headerRow, cellStyle1);
                    }

                    CreateCell("Market", headerRow, cellStyle1);
                    CreateCell("Region", headerRow, cellStyle1);

                    if (!report.SplitRateValue.HasValue)
                    {
                        CreateCell("Region Value", headerRow, cellStyle1);
                    }

                    else
                    {
                        CreateCell(string.Format("Region Value Rate-{0}", Decimal.Round(report.SplitRateValue.Value,4)), headerRow, cellStyle1);
                        CreateCell(string.Format("Region Value {0}", Decimal.Round(report.SplitRateValue.Value,4)), headerRow, cellStyle1);
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
                                            CreateCell(string.Format("{0}", reportData.CarrierAccountName), row, cellStyle1);
                                            CreateCell(reportData.Duration.ToString(), row, cellStyle1);
                                            CreateCell(reportData.Amount1.ToString(), row, cellStyle1);

                                            if (report.SplitRateValue.HasValue)
                                                CreateCell(reportData.Amount2.ToString(), row, cellStyle1);

                                            if (report.TaxOption.HasValue)
                                                CreateCell(reportData.Tax.ToString(), row, cellStyle1);

                                            CreateCell(string.Format("{0} {1}%",market.MarketName, market.Percentage), row, cellStyle1);
                                            CreateCell(string.Format("{0} {1}%", region.RegionName, region.Percentage), row, cellStyle1);
                                            CreateCell(region.RegionValue1.ToString(), row, cellStyle2);

                                            if (report.SplitRateValue.HasValue)
                                                CreateCell(region.RegionValue2.ToString(), row, cellStyle2);

                                           
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                excelFile.CreateSheet();
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
        private void CreateCell(string cellValue, VRExcelTableRow row, VRExcelTableRowCellStyle cellStyle)
        {
            var cell = row.CreateCell();
            cell.SetValue(cellValue);
            var style = cell.CreateStyle();
            style.VerticalAlignment = cellStyle.VerticalAlignment;
            style.HorizontalAlignment = cellStyle.HorizontalAlignment;
        }
    }
}

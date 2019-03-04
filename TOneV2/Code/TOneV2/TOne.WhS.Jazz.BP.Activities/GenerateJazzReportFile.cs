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
                    titleCell.MergeCells(1, 6 + (report.TaxOption.HasValue ? 1 : 0));
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
                    
                        CreateCell("Amount", headerRow, cellStyle1);
                    

                    if (report.TaxOption.HasValue)
                    {
                        CreateCell("STAX", headerRow, cellStyle1);
                    }

                    CreateCell("Market", headerRow, cellStyle1);
                    CreateCell("Region", headerRow, cellStyle1);

                   
                        CreateCell("Region Value", headerRow, cellStyle1);
                   
              
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
                                            CreateCell(reportData.Duration, row, cellStyle1);
                                            CreateCell(reportData.Amount, row, cellStyle1);
                                           

                                            if (report.TaxOption.HasValue)
                                                CreateCell(reportData.Tax, row, cellStyle1);

                                            CreateCell(string.Format("{0} {1}%",market.MarketName, market.Percentage), row, cellStyle1);
                                            CreateCell(string.Format("{0} {1}%", region.RegionName, region.Percentage), row, cellStyle1);
                                            CreateCell(region.RegionValue, row, cellStyle2);
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
                Name = "ERPIntegrationReport.xlsx",
                Content = file
            });
            
            FileId.Set(context, fileId);

        }
        private void CreateCell(object cellValue, VRExcelTableRow row, VRExcelTableRowCellStyle cellStyle)
        {
            var cell = row.CreateCell();
            cell.SetValue(cellValue);
            var style = cell.CreateStyle();
            style.VerticalAlignment = cellStyle.VerticalAlignment;
            style.HorizontalAlignment = cellStyle.HorizontalAlignment;
        }
    }
}

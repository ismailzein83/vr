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
                jazzReports= jazzReports.OrderBy(x => x.ReportName).ToList();
                foreach(var report in jazzReports)
                {
                    var excelSheet = excelFile.CreateSheet();
                    excelSheet.SheetName = (report.ReportName.Length > 31) ? string.Format("{0}...", report.ReportName.Substring(0, 27)) : report.ReportName;
                    var excelTable = excelSheet.CreateTable(1,0);

                    excelTable.EnableRowVerticalMerge();

                    VRExcelTableRowCellStyle numberCellStyle = new VRExcelTableRowCellStyle
                    {
                        VerticalAlignment=VRExcelContainerVerticalAlignment.Center,
                        HorizontalAlignment=VRExcelContainerHorizontalAlignment.Right
                    };
                    VRExcelTableRowCellStyle textCellStyle = new VRExcelTableRowCellStyle
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
                    if (report.Direction==ReportDefinitionDirection.In)
                    {
                        CreateCell("Customer", headerRow, textCellStyle);
                    }
                    else
                    {
                        CreateCell("Supplier", headerRow, textCellStyle);
                    }

                    CreateCell("Duration", headerRow, textCellStyle);
                    
                        CreateCell("Amount", headerRow, textCellStyle);
                    

                    if (report.TaxOption.HasValue)
                    {
                        CreateCell("STAX", headerRow, textCellStyle);
                    }

                    CreateCell("Market", headerRow, textCellStyle);
                    CreateCell("Region", headerRow, textCellStyle);

                   
                        CreateCell("Region Value", headerRow, textCellStyle);
                   
              
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
                                            CreateCell(string.Format("{0}", reportData.CarrierAccountName), row, textCellStyle);
                                            CreateCell(reportData.Duration, row, numberCellStyle);
                                            CreateCell(reportData.Amount, row, numberCellStyle);
                                           

                                            if (report.TaxOption.HasValue)
                                                CreateCell(reportData.Tax, row, numberCellStyle);

                                            CreateCell(string.Format("{0} {1}%",market.MarketName, market.Percentage), row, textCellStyle);
                                            CreateCell(string.Format("{0} {1}%", region.RegionName, region.Percentage), row, textCellStyle);
                                            CreateCell(region.RegionValue, row, numberCellStyle);
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

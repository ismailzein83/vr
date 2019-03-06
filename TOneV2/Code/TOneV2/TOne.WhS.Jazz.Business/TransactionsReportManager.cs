using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using TOne.WhS.Jazz.Entities;
using System.Web.Http;
using Vanrise.BusinessProcess.Business;
using Vanrise.Web.Base;
using Vanrise.Common.Excel;
using TOne.WhS.Jazz.Data;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.Jazz.Business
{
    public class TransactionsReportManager
    {
        public byte[] DownloadTransactionsReports(long processInstanceId)
        {
            IDraftReportDataManager draftReportDataManager = JazzDataManagerFactory.GetDataManager<IDraftReportDataManager>();

            var excelFile = new VRExcelFile();

            var transactionsReports = draftReportDataManager.GetTransactionsReports(processInstanceId);
            if (transactionsReports != null && transactionsReports.Count > 0)
            {
                JazzReportDefinitionManager jazzReportDefinitionManager = new JazzReportDefinitionManager();
                var orderedReports = transactionsReports.OrderBy(x => jazzReportDefinitionManager.GetJazzReportDefinitionName(x.ReportDefinitionId));
                transactionsReports = orderedReports.OrderBy(x => x.SheetName).ToList();

                var reportsIds = transactionsReports.MapRecords(x=>x.ReportId).ToList();
                var transactionsReportsDictionary = transactionsReports.ToDictionary(x => x.ReportId, x => x);
                IDraftReportTransactionDataManager draftReportTransactionDataManager = JazzDataManagerFactory.GetDataManager<IDraftReportTransactionDataManager>();
                var transactionsReportsData = draftReportTransactionDataManager.GetTransactionsReportsData(reportsIds);

                if (transactionsReportsData != null && transactionsReportsData.Count > 0)
                {

                    foreach (var transactionsReport in transactionsReports)
                    {
                        List<ERPDraftReportTranaction> reportDatas = null;
                        if (transactionsReportsData.TryGetValue(transactionsReport.ReportId, out reportDatas))
                        {
                            var excelSheet = excelFile.CreateSheet();

                            excelSheet.SheetName = (transactionsReport.SheetName.Length > 31) ? string.Format("{0}...", transactionsReport.SheetName.Substring(0, 28)) : transactionsReport.SheetName;

                            if (reportDatas != null && reportDatas.Count > 0)
                            {
                                var excelTable = excelSheet.CreateTable(1, 0);

                                VRExcelTableRowCellStyle numberCellStyle = new VRExcelTableRowCellStyle
                                {
                                    VerticalAlignment = VRExcelContainerVerticalAlignment.Center,
                                    HorizontalAlignment = VRExcelContainerHorizontalAlignment.Right
                                };
                                VRExcelTableRowCellStyle textCellStyle = new VRExcelTableRowCellStyle
                                {
                                    VerticalAlignment = VRExcelContainerVerticalAlignment.Center
                                };
                                VRExcelCell titleCell = new VRExcelCell
                                {
                                    Value = transactionsReport.SheetName,
                                    RowIndex = 0,
                                    ColumnIndex = 0,
                                    Style = new VRExcelCellStyle
                                    {
                                        HorizontalAlignment = VRExcelContainerHorizontalAlignment.Center
                                    }
                                };
                                titleCell.MergeCells(1, 4);
                                excelSheet.AddCell(titleCell);

                                var headerRow = excelTable.CreateHeaderRow();
                                CreateCell("Transaction Code", headerRow, textCellStyle);
                                CreateCell("Transaction Description", headerRow, textCellStyle);
                                CreateCell("Credit", headerRow, textCellStyle);
                                CreateCell("Debit", headerRow, textCellStyle);
                                foreach (var reportData in reportDatas)
                                {
                                    var row = excelTable.CreateDataRow();
                                    CreateCell(reportData.TransactionCode, row, textCellStyle);
                                    CreateCell(reportData.TransationDescription, row, textCellStyle);
                                    CreateCell(reportData.Credit, row, numberCellStyle);
                                    CreateCell(reportData.Debit, row, numberCellStyle);
                                }
                            }
                        }
                    }
                }
                else
                {
                    excelFile.CreateSheet();
                }

            }
            else
            {
                excelFile.CreateSheet();
            }

            return excelFile.GenerateExcelFile();
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

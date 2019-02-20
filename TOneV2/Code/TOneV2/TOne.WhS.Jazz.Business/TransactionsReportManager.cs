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
                var reportsIds = transactionsReports.MapRecords(x=>x.ReportId).ToList();
                var transactionsReportsDictionary = transactionsReports.ToDictionary(x => x.ReportId, x => x);
                IDraftReportTransactionDataManager draftReportTransactionDataManager = JazzDataManagerFactory.GetDataManager<IDraftReportTransactionDataManager>();
                var transactionsReportsData = draftReportTransactionDataManager.GetTransactionsReportsData(reportsIds);

                if (transactionsReportsData != null && transactionsReportsData.Count > 0)
                {
                    foreach (var transactionsReportData in transactionsReportsData)
                    {
                        ERPDraftReport report = null;
                        if (transactionsReportsDictionary.TryGetValue(transactionsReportData.Key, out report))
                        {
                            var excelSheet = excelFile.CreateSheet();

                            excelSheet.SheetName = (report.SheetName.Length > 31) ? string.Format("{0}...", report.SheetName.Substring(0, 28)) : report.SheetName;

                            if (transactionsReportData.Value != null && transactionsReportData.Value.Count > 0)
                            {
                                var excelTable = excelSheet.CreateTable(1, 0);

                                VRExcelTableRowCellStyle cellStyle1 = new VRExcelTableRowCellStyle
                                {
                                    VerticalAlignment = VRExcelContainerVerticalAlignment.Center,
                                    HorizontalAlignment = VRExcelContainerHorizontalAlignment.Center
                                };
                                VRExcelTableRowCellStyle cellStyle2 = new VRExcelTableRowCellStyle
                                {
                                    VerticalAlignment = VRExcelContainerVerticalAlignment.Center
                                };
                                VRExcelCell titleCell = new VRExcelCell
                                {
                                    Value = report.SheetName,
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
                                CreateCell("Transaction Code", headerRow, cellStyle2);
                                CreateCell("Transaction Description", headerRow, cellStyle2);
                                CreateCell("Credit", headerRow, cellStyle2);
                                CreateCell("Debit", headerRow, cellStyle2);
                                foreach (var transactionReportData in transactionsReportData.Value)
                                {
                                    var row = excelTable.CreateDataRow();
                                    CreateCell(transactionReportData.TransactionCode, row, cellStyle2);
                                    CreateCell(transactionReportData.TransationDescription, row, cellStyle2);
                                    CreateCell(transactionReportData.Credit.ToString(), row, cellStyle2);
                                    CreateCell(transactionReportData.Debit.ToString(), row, cellStyle2);
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

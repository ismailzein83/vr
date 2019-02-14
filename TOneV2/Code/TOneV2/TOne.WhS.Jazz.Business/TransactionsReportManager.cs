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
                                var excelTable = excelSheet.CreateTable(0, 0);
                                var title = excelTable.CreateHeaderRow();
                                CreateCell(report.SheetName, title);
                                var headerRow = excelTable.CreateHeaderRow();
                                CreateCell("Transaction Code", headerRow);
                                CreateCell("Transaction Description", headerRow);
                                CreateCell("Credit", headerRow);
                                CreateCell("Debit", headerRow);
                                foreach (var transactionReportData in transactionsReportData.Value)
                                {
                                    var row = excelTable.CreateDataRow();
                                    CreateCell(transactionReportData.TransactionCode, row);
                                    CreateCell(transactionReportData.TransationDescription, row);
                                    CreateCell(transactionReportData.Credit.ToString(), row);
                                    CreateCell(transactionReportData.Debit.ToString(), row);
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
        private void CreateCell(string cellValue, VRExcelTableRow row)
        {
            var cell = row.CreateCell();
            cell.SetValue(cellValue);
        }
    }
}

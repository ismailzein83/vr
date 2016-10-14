using CDRComparison.Data;
using CDRComparison.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Entities;

namespace CDRComparison.Business
{
    public class DisputeCDRManager
    {
        #region Public Methods

        public IDataRetrievalResult<DisputeCDR> GetFilteredDisputeCDRs(DataRetrievalInput<DisputeCDRQuery> input)
        {
            IDisputeCDRDataManager dataManager = CDRComparisonDataManagerFactory.GetDataManager<IDisputeCDRDataManager>();
            dataManager.TableNameKey = input.Query.TableKey;

            var resultProcessingHandler = new ResultProcessingHandler<DisputeCDR>()
            {
                ExportExcelHandler = new DisputeCDRExportExcelHandler()
            };

            BigResult<DisputeCDR> bigResult = dataManager.GetFilteredDisputeCDRs(input);

            if (bigResult.Data != null)
            {
                foreach (DisputeCDR cdr in bigResult.Data)
                    cdr.DurationDifferenceInSec = Math.Abs(cdr.PartnerDurationInSec - cdr.SystemDurationInSec);
            }

            return DataRetrievalManager.Instance.ProcessResult(input, bigResult, resultProcessingHandler);
        }

        public int GetDisputeCDRsCount(string tableKey)
        {
            IDisputeCDRDataManager dataManager = CDRComparisonDataManagerFactory.GetDataManager<IDisputeCDRDataManager>();
            dataManager.TableNameKey = tableKey;
            return dataManager.GetDisputeCDRsCount();
        }

        public decimal GetDurationOfDisputeCDRs(string tableKey, bool isPartner)
        {
            var dataManager = CDRComparisonDataManagerFactory.GetDataManager<IDisputeCDRDataManager>();
            dataManager.TableNameKey = tableKey;
            return dataManager.GetDurationOfDisputeCDRs(isPartner);
        }

        #endregion

        #region Private Classes

        private class DisputeCDRExportExcelHandler : ExcelExportHandler<DisputeCDR>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<DisputeCDR> context)
            {
                if (context.BigResult == null || context.BigResult.Data == null)
                    return;

                var sheet = new ExportExcelSheet();
                sheet.SheetName = "Match CDRs";

                sheet.Header = new ExportExcelHeader() { Cells = new List<ExportExcelHeaderCell>() };
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "System CDPN" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "System CGPN" });
				sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "System Time", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.LongDateTime });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "System Duration (SEC)" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Partner CDPN" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Partner CGPN" });
				sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Partner Time", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.LongDateTime });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Partner Duration (SEC)" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Duration Difference (SEC)" });

                sheet.Rows = new List<ExportExcelRow>();
                foreach (var record in context.BigResult.Data)
                {
                    var row = new ExportExcelRow() { Cells = new List<ExportExcelCell>() };
                    row.Cells.Add(new ExportExcelCell() { Value = record.SystemCDPN });
                    row.Cells.Add(new ExportExcelCell() { Value = record.SystemCGPN });
                    row.Cells.Add(new ExportExcelCell() { Value = record.SystemTime });
                    row.Cells.Add(new ExportExcelCell() { Value = record.SystemDurationInSec });
                    row.Cells.Add(new ExportExcelCell() { Value = record.PartnerCDPN });
                    row.Cells.Add(new ExportExcelCell() { Value = record.PartnerCGPN });
                    row.Cells.Add(new ExportExcelCell() { Value = record.PartnerTime });
                    row.Cells.Add(new ExportExcelCell() { Value = record.PartnerDurationInSec });
                    row.Cells.Add(new ExportExcelCell() { Value = record.DurationDifferenceInSec });
                    sheet.Rows.Add(row);
                }

                context.MainSheet = sheet;
            }
        }

        #endregion
    }
}

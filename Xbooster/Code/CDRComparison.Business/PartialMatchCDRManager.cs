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
    public class PartialMatchCDRManager
    {
        #region Public Methods

        public IDataRetrievalResult<PartialMatchCDR> GetFilteredPartialMatchCDRs(DataRetrievalInput<PartialMatchCDRQuery> input)
        {
            IPartialMatchCDRDataManager dataManager = CDRComparisonDataManagerFactory.GetDataManager<IPartialMatchCDRDataManager>();
            dataManager.TableNameKey = input.Query.TableKey;

            var partialMatchCDRBigResult = new PartialMatchCDRBigResult();
            BigResult<PartialMatchCDR> bigResult = dataManager.GetFilteredPartialMatchCDRs(input);

            partialMatchCDRBigResult.ResultKey = bigResult.ResultKey;
            partialMatchCDRBigResult.Data = bigResult.Data;
            partialMatchCDRBigResult.TotalCount = bigResult.TotalCount;

            partialMatchCDRBigResult.Summary = new PartialMatchCDR();
            partialMatchCDRBigResult.Summary.SystemDurationInSec = 0;
            partialMatchCDRBigResult.Summary.PartnerDurationInSec = 0;
            partialMatchCDRBigResult.Summary.DurationDifferenceInSec = 0;

            foreach (PartialMatchCDR cdr in bigResult.Data)
            {
                decimal durationDifference = cdr.PartnerDurationInSec - cdr.SystemDurationInSec;
                cdr.DurationDifferenceInSec = Math.Abs(durationDifference);
                cdr.DurationDifferencePercentageOfPartner = (durationDifference * 100) / cdr.SystemDurationInSec;

                partialMatchCDRBigResult.Summary.SystemDurationInSec += cdr.SystemDurationInSec;
                partialMatchCDRBigResult.Summary.PartnerDurationInSec += cdr.PartnerDurationInSec;
                partialMatchCDRBigResult.Summary.DurationDifferenceInSec += cdr.DurationDifferenceInSec;
            }

            var resultProcessingHandler = new ResultProcessingHandler<PartialMatchCDR>()
            {
                ExportExcelHandler = new PartialMatchCDRExportExcelHandler()
            };

            return DataRetrievalManager.Instance.ProcessResult(input, partialMatchCDRBigResult, resultProcessingHandler);
        }

        public int GetPartialMatchCDRsCount(string tableKey)
        {
            IPartialMatchCDRDataManager dataManager = CDRComparisonDataManagerFactory.GetDataManager<IPartialMatchCDRDataManager>();
            dataManager.TableNameKey = tableKey;
            return dataManager.GetPartialMatchCDRsCount();
        }

        public decimal GetDurationOfPartialMatchCDRs(string tableKey, bool isPartner)
        {
            var dataManager = CDRComparisonDataManagerFactory.GetDataManager<IPartialMatchCDRDataManager>();
            dataManager.TableNameKey = tableKey;
            return dataManager.GetDurationOfPartialMatchCDRs(isPartner);
        }

        public decimal GetTotalDurationDifferenceOfPartialMatchCDRs(string tableKey)
        {
            var dataManager = CDRComparisonDataManagerFactory.GetDataManager<IPartialMatchCDRDataManager>();
            dataManager.TableNameKey = tableKey;
            return dataManager.GetTotalDurationDifferenceOfPartialMatchCDRs(tableKey);
        }

        #endregion

        #region Private Classes

        private class PartialMatchCDRExportExcelHandler : ExcelExportHandler<PartialMatchCDR>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<PartialMatchCDR> context)
            {
                if (context.BigResult == null || context.BigResult.Data == null)
                    return;

                var sheet = new ExportExcelSheet();
                sheet.SheetName = "Partial Match CDRs";

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
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Duration Difference % Of Partner" });

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
                    row.Cells.Add(new ExportExcelCell() { Value = record.PartnerTime});
                    row.Cells.Add(new ExportExcelCell() { Value = record.PartnerDurationInSec });
                    row.Cells.Add(new ExportExcelCell() { Value = record.DurationDifferenceInSec});
                    row.Cells.Add(new ExportExcelCell() { Value = record.DurationDifferencePercentageOfPartner });
                    sheet.Rows.Add(row);
                }

                context.MainSheet = sheet;
            }
        }
        
        #endregion
    }
}

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
    public class MissingCDRManager
    {
        #region Public Methods

        public IDataRetrievalResult<MissingCDR> GetFilteredMissingCDRs(DataRetrievalInput<MissingCDRQuery> input)
        {
            IMissingCDRDataManager dataManager = CDRComparisonDataManagerFactory.GetDataManager<IMissingCDRDataManager>();
            dataManager.TableNameKey = input.Query.TableKey;

            var missingCDRBigResult = new MissingCDRBigResult();
            BigResult<MissingCDR> bigResult = dataManager.GetFilteredMissingCDRs(input);

            missingCDRBigResult.ResultKey = bigResult.ResultKey;
            missingCDRBigResult.Data = bigResult.Data;
            missingCDRBigResult.TotalCount = bigResult.TotalCount;

            missingCDRBigResult.Summary = new MissingCDR();
            missingCDRBigResult.Summary.DurationInSec = bigResult.Data.Sum(x => x.DurationInSec);

            var resultProcessingHandler = new ResultProcessingHandler<MissingCDR>()
            {
                ExportExcelHandler = new MissingCDRExcelExportHandler()
            };

            return DataRetrievalManager.Instance.ProcessResult(input, missingCDRBigResult, resultProcessingHandler);
        }

        public int GetMissingCDRsCount(string tableKey, bool isPartnerCDRs)
        {
            IMissingCDRDataManager dataManager = CDRComparisonDataManagerFactory.GetDataManager<IMissingCDRDataManager>();
            dataManager.TableNameKey = tableKey;
            return dataManager.GetMissingCDRsCount(isPartnerCDRs);
        }

        public decimal GetDurationOfMissingCDRs(string tableKey, bool? isPartner)
        {
            var dataManager = CDRComparisonDataManagerFactory.GetDataManager<IMissingCDRDataManager>();
            dataManager.TableNameKey = tableKey;
            return dataManager.GetDurationOfMissingCDRs(isPartner);
        }

        #endregion

        #region Private Classes

        private class MissingCDRExcelExportHandler : ExcelExportHandler<MissingCDR>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<MissingCDR> context)
            {
                if (context.BigResult == null || context.BigResult.Data == null)
                    return;

                var sheet = new ExportExcelSheet();
                sheet.SheetName = "Missing CDRs";

                sheet.Header = new ExportExcelHeader() { Cells = new List<ExportExcelHeaderCell>() };
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "CDPN" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "CGPN" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Time", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.LongDateTime });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Duration (SEC)" });

                sheet.Rows = new List<ExportExcelRow>();
                foreach (var record in context.BigResult.Data)
                {
                    var row = new ExportExcelRow() { Cells = new List<ExportExcelCell>() };
                    row.Cells.Add(new ExportExcelCell() { Value = record.CDPN });
                    row.Cells.Add(new ExportExcelCell() { Value = record.CGPN });
                    row.Cells.Add(new ExportExcelCell() { Value = record.Time });
                    row.Cells.Add(new ExportExcelCell() { Value = record.DurationInSec });
                    sheet.Rows.Add(row);
                }

                context.MainSheet = sheet;
            }
        }
        
        #endregion
    }
}

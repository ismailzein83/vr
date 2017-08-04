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
    public class InvalidCDRManager
    {
        public Vanrise.Entities.IDataRetrievalResult<InvalidCDR> GetFilteredInvalidCDRs(Vanrise.Entities.DataRetrievalInput<InvalidCDRQuery> input)
        {
            return Vanrise.Common.Business.BigDataManager.Instance.RetrieveData<InvalidCDRQuery, InvalidCDR, InvalidCDR>(input, new InvalidCDRRequestHandler());
        }
        public int GetInvalidCDRsCount(string tableKey, bool isPartnerCDRs)
        {
            var dataManager = CDRComparisonDataManagerFactory.GetDataManager<IInvalidCDRDataManager>();
            dataManager.TableNameKey = tableKey;
            return dataManager.GetInvalidCDRsCount(isPartnerCDRs);
        }
        public decimal GetInvalidCDRsDuration(string tableKey, bool isPartnerCDRs)
        {
            var dataManager = CDRComparisonDataManagerFactory.GetDataManager<IInvalidCDRDataManager>();
            dataManager.TableNameKey = tableKey;
            return dataManager.GetInvalidCDRsDuration(isPartnerCDRs);
        }

        #region Private Classes

        private class InvalidCDRExcelExportHandler : ExcelExportHandler<InvalidCDR>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<InvalidCDR> context)
            {
                if (context.BigResult == null || context.BigResult.Data == null)
                    return;

                var sheet = new ExportExcelSheet();
                sheet.SheetName = "Invalid CDRs";

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
        private class InvalidCDRRequestHandler : Vanrise.Common.Business.BigDataRequestHandler<InvalidCDRQuery, InvalidCDR, InvalidCDR>
        {
            public override IEnumerable<InvalidCDR> RetrieveAllData(DataRetrievalInput<InvalidCDRQuery> input)
            {
                IInvalidCDRDataManager dataManager = CDRComparisonDataManagerFactory.GetDataManager<IInvalidCDRDataManager>();
                dataManager.TableNameKey = input.Query.TableKey;
                return dataManager.GetAllInvalidCDRs(input.Query.IsPartnerCDRs);
            }
            protected override BigResult<InvalidCDR> AllRecordsToBigResult(DataRetrievalInput<InvalidCDRQuery> input, IEnumerable<InvalidCDR> allRecords)
            {
                BigResult<InvalidCDR> bigResult = allRecords.ToBigResult(input, null);

                var invalidCDRBigResult = new InvalidCDRBigResult()
                {
                    ResultKey = bigResult.ResultKey,
                    Data = bigResult.Data,
                    TotalCount = bigResult.TotalCount
                };

                invalidCDRBigResult.Summary = new InvalidCDR();
                invalidCDRBigResult.Summary.DurationInSec = bigResult.Data.Sum(x => x.DurationInSec);

                return invalidCDRBigResult;
            }
            protected override ResultProcessingHandler<InvalidCDR> GetResultProcessingHandler(DataRetrievalInput<InvalidCDRQuery> input, BigResult<InvalidCDR> bigResult)
            {
                return new ResultProcessingHandler<InvalidCDR>()
                {
                    ExportExcelHandler = new InvalidCDRExcelExportHandler()
                };
            }
            public override InvalidCDR EntityDetailMapper(InvalidCDR entity)
            {
                return entity;
            }
        }

        #endregion
    }
}

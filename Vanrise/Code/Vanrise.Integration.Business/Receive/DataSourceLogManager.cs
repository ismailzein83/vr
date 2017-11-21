using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Integration.Data;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Business
{
    public class DataSourceLogManager
    {
        public Vanrise.Entities.IDataRetrievalResult<DataSourceLog> GetFilteredDataSourceLogs(Vanrise.Entities.DataRetrievalInput<DataSourceLogQuery> input)
        {
            IDataSourceLogDataManager dataManager = IntegrationDataManagerFactory.GetDataManager<IDataSourceLogDataManager>();

            ResultProcessingHandler<DataSourceLog> handler = new ResultProcessingHandler<DataSourceLog>()
            {
                ExportExcelHandler = new DataSourceLogExcelExportHandler()
            };

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetFilteredDataSourceLogs(input), handler);
        }

        public void WriteEntry(Vanrise.Entities.LogEntryType entryType, Guid dataSourceId, long? importedBatchId, string message, string logEntryTime)
        {
            IDataSourceLogDataManager manager = IntegrationDataManagerFactory.GetDataManager<IDataSourceLogDataManager>();
            manager.InsertEntry(entryType, message, dataSourceId, importedBatchId, logEntryTime);
        }

        #region Private Classes
        private class DataSourceLogExcelExportHandler : ExcelExportHandler<DataSourceLog>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<DataSourceLog> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "Data Source Log",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "ID" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Time", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.LongDateTime });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Severity" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Message", Width = 100 });


                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    foreach (var record in context.BigResult.Data)
                    {
                        var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                        sheet.Rows.Add(row);
                        row.Cells.Add(new ExportExcelCell { Value = record.ID });
                        row.Cells.Add(new ExportExcelCell { Value = record.LogEntryTime });
                        row.Cells.Add(new ExportExcelCell { Value = Vanrise.Common.Utilities.GetEnumDescription(record.Severity) });
                        row.Cells.Add(new ExportExcelCell { Value = record.Message });
                    }
                }
                context.MainSheet = sheet;
            }
        }
        #endregion
    }
}

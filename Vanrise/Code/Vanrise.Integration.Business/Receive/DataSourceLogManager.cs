using System;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Integration.Data;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Business
{
    public class DataSourceLogManager
    {
        public Vanrise.Entities.IDataRetrievalResult<DataSourceLogDetail> GetFilteredDataSourceLogs(Vanrise.Entities.DataRetrievalInput<DataSourceLogQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new DataSourceLogRequestHandler());
        }


        public void WriteEntry(Vanrise.Entities.LogEntryType entryType, Guid dataSourceId, long? importedBatchId, string message, string logEntryTime)
        {
            IDataSourceLogDataManager manager = IntegrationDataManagerFactory.GetDataManager<IDataSourceLogDataManager>();
            manager.InsertEntry(entryType, message, dataSourceId, importedBatchId, logEntryTime);
        }

        #region Private Classes

        private class DataSourceLogRequestHandler : BigDataRequestHandler<DataSourceLogQuery, DataSourceLog, DataSourceLogDetail>
        {
            public override DataSourceLogDetail EntityDetailMapper(DataSourceLog entity)
            {
                return new DataSourceLogDetail
                {
                    ID = entity.ID,
                    DataSourceId = entity.DataSourceId,
                    SeverityDescription = Utilities.GetEnumDescription<LogEntryType>(entity.Severity),
                    Message = entity.Message,
                    LogEntryTime = entity.LogEntryTime
                };
            }



            public override IEnumerable<DataSourceLog> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<DataSourceLogQuery> input)
            {
                var maxTop = new Vanrise.Common.Business.ConfigManager().GetMaxSearchRecordCount();
                if (input.Query.Top > maxTop)
                    throw new VRBusinessException(string.Format("Top record count cannot be greater than {0}", maxTop));
                IDataSourceLogDataManager dataManager = IntegrationDataManagerFactory.GetDataManager<IDataSourceLogDataManager>();
                return dataManager.GetFilteredDataSourceLogs(input.Query);
            }

            protected override ResultProcessingHandler<DataSourceLogDetail> GetResultProcessingHandler(DataRetrievalInput<DataSourceLogQuery> input, BigResult<DataSourceLogDetail> bigResult)
            {
                return new ResultProcessingHandler<DataSourceLogDetail>
                {
                    ExportExcelHandler = new DataSourceLogExcelExportHandler()
                };
            }
        }

        private class DataSourceLogExcelExportHandler : ExcelExportHandler<DataSourceLogDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<DataSourceLogDetail> context)
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
                        row.Cells.Add(new ExportExcelCell { Value = record.SeverityDescription });
                        row.Cells.Add(new ExportExcelCell { Value = record.Message });
                    }
                }
                context.MainSheet = sheet;
            }
        }

        #endregion
    }
}
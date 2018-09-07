using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Integration.Data;
using Vanrise.Integration.Entities;
using Vanrise.Queueing;
using Vanrise.Queueing.Entities;

namespace Vanrise.Integration.Business
{
    public class DataSourceImportedBatchManager
    {
        public long WriteEntry(Entities.ImportedBatchEntry entry, Guid dataSourceId, string logEntryTime)
        {
            IDataSourceImportedBatchDataManager manager = IntegrationDataManagerFactory.GetDataManager<IDataSourceImportedBatchDataManager>();
            return manager.InsertEntry(dataSourceId, entry.BatchDescription, entry.BatchSize, entry.RecordsCount, entry.Result, entry.MapperMessage, entry.QueueItemsIds, logEntryTime);
        }

        public Vanrise.Entities.IDataRetrievalResult<DataSourceImportedBatch> GetFilteredDataSourceImportedBatches(Vanrise.Entities.DataRetrievalInput<DataSourceImportedBatchQuery> input)
        {
            var maxTop = new ConfigManager().GetMaxSearchRecordCount();
            if (input.Query.Top > maxTop)
                throw new VRBusinessException(string.Format("Top record count cannot be greater than {0}", maxTop));

            IDataSourceImportedBatchDataManager dataManager = IntegrationDataManagerFactory.GetDataManager<IDataSourceImportedBatchDataManager>();
            Vanrise.Entities.BigResult<DataSourceImportedBatch> bigResult = dataManager.GetFilteredDataSourceImportedBatches(input);

            //Technically no need to use Hashset because ids in seperate rows will not repeat. 
            //Just in case it did, the use of hashset is to avoid this problem.
            HashSet<long> queueItemIds = new HashSet<long>();
            foreach (DataSourceImportedBatch batch in bigResult.Data)
            {
                if (string.IsNullOrEmpty(batch.QueueItemIds))
                    continue;

                string[] qIds = batch.QueueItemIds.Split(',');
                queueItemIds.UnionWith(qIds.Select(itm => long.Parse(itm)));
            }

            QueueingManager queueingManager = new QueueingManager();
            Dictionary<long, ItemExecutionFlowInfo> dicItemExecutionStatus = queueingManager.GetItemsExecutionFlowStatus(queueItemIds.ToList());

            foreach (DataSourceImportedBatch batch in bigResult.Data)
            {
                if (string.IsNullOrEmpty(batch.QueueItemIds))
                {
                    batch.ExecutionStatus = ItemExecutionFlowStatus.NoBatches;
                    continue;
                }

                string[] batchQueueItemIds = batch.QueueItemIds.Split(',');
                List<ItemExecutionFlowInfo> list = batchQueueItemIds.Select(itm => dicItemExecutionStatus.GetRecord(long.Parse(itm))).ToList();
                batch.ExecutionStatus = list.Count > 1 ? queueingManager.GetExecutionFlowStatus(list) : list.First().Status;
            }

            ResultProcessingHandler<DataSourceImportedBatch> handler = new ResultProcessingHandler<DataSourceImportedBatch>()
            {
                ExportExcelHandler = new DataSourceImportedBatchExcelExportHandler()
            };

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, bigResult, handler);
        }


        #region Private Classes
        private class DataSourceImportedBatchExcelExportHandler : ExcelExportHandler<DataSourceImportedBatch>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<DataSourceImportedBatch> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "Data Source Imported Batch",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "ID" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Time", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.LongDateTime });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Batch Description" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Batch Size" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Records Count" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Mapping Result" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Mapper Message" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Execution Status" });

                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    foreach (var record in context.BigResult.Data)
                    {
                        var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                        sheet.Rows.Add(row);
                        row.Cells.Add(new ExportExcelCell { Value = record.ID });
                        row.Cells.Add(new ExportExcelCell { Value = record.LogEntryTime });
                        row.Cells.Add(new ExportExcelCell { Value = record.BatchDescription });
                        row.Cells.Add(new ExportExcelCell { Value = record.BatchSize });
                        row.Cells.Add(new ExportExcelCell { Value = record.RecordsCount });
                        row.Cells.Add(new ExportExcelCell { Value = Vanrise.Common.Utilities.GetEnumDescription(record.MappingResult) });
                        row.Cells.Add(new ExportExcelCell { Value = record.MapperMessage });
                        row.Cells.Add(new ExportExcelCell { Value = Vanrise.Common.Utilities.GetEnumDescription(record.ExecutionStatus) });
                    }
                }
                context.MainSheet = sheet;
            }
        }
        #endregion
    }
}
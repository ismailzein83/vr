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
            return manager.InsertEntry(dataSourceId, entry.BatchDescription, entry.BatchSize, entry.BatchState, entry.IsDuplicateSameSize, entry.RecordsCount, entry.Result, entry.MapperMessage, entry.QueueItemsIds, logEntryTime, entry.BatchStart, entry.BatchEnd);
        }

        public Vanrise.Entities.IDataRetrievalResult<DataSourceImportedBatchDetail> GetFilteredDataSourceImportedBatches(Vanrise.Entities.DataRetrievalInput<DataSourceImportedBatchQuery> input)
        {

            return BigDataManager.Instance.RetrieveData(input, new DataSourceImportedBatchRequestHandler());

        }

        public List<DataSourceImportedBatch> GetDataSourceImportedBatches(Guid DataSourceId, DateTime from)
        {
            IDataSourceImportedBatchDataManager dataManager = IntegrationDataManagerFactory.GetDataManager<IDataSourceImportedBatchDataManager>();
            return dataManager.GetDataSourceImportedBatches(DataSourceId, from);
        }

        public List<DataSourceSummary> GetDataSourcesSummary(DateTime fromTime, List<Guid> dataSourcesIds)
        {
            IDataSourceImportedBatchDataManager dataManager = IntegrationDataManagerFactory.GetDataManager<IDataSourceImportedBatchDataManager>();
            return dataManager.GetDataSourcesSummary(fromTime, dataSourcesIds);
        }

        #region Private Classes

        private class DataSourceImportedBatchRequestHandler : BigDataRequestHandler<DataSourceImportedBatchQuery, DataSourceImportedBatch, DataSourceImportedBatchDetail>
        {
            public override DataSourceImportedBatchDetail EntityDetailMapper(DataSourceImportedBatch entity)
            {
                return new DataSourceImportedBatchDetail
                {
                    ID = entity.ID,
                    BatchDescription = entity.BatchDescription,
                    BatchSize = entity.BatchSize,
                    BatchStateDescription = Utilities.GetEnumDescription<BatchState>(entity.BatchState),
                    RecordsCount = entity.RecordsCount,
                    MappingResult = entity.MappingResult,
                    MappingResultDescription = Utilities.GetEnumDescription<MappingResult>(entity.MappingResult),
                    MapperMessage = entity.MapperMessage,
                    QueueItemIds = entity.QueueItemIds,
                    LogEntryTime = entity.LogEntryTime,
                    BatchStart = entity.BatchStart,
                    BatchEnd = entity.BatchEnd,
                    ExecutionStatus = entity.ExecutionStatus
                };
            }

           

            public override IEnumerable<DataSourceImportedBatch> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<DataSourceImportedBatchQuery> input)
            {
                var maxTop = new Vanrise.Common.Business.ConfigManager().GetMaxSearchRecordCount();
                if (input.Query.Top > maxTop)
                    throw new VRBusinessException(string.Format("Top record count cannot be greater than {0}", maxTop));
                IDataSourceImportedBatchDataManager dataManager = IntegrationDataManagerFactory.GetDataManager<IDataSourceImportedBatchDataManager>();
                return dataManager.GetFilteredDataSourceImportedBatches(input.Query);
            }

            protected override Vanrise.Entities.BigResult<DataSourceImportedBatchDetail> AllRecordsToBigResult(Vanrise.Entities.DataRetrievalInput<DataSourceImportedBatchQuery> input, IEnumerable<DataSourceImportedBatch> allRecords)
            {
                var bigResult = base.AllRecordsToBigResult(input, allRecords);

                if (bigResult != null && bigResult.Data != null)
                {
                    HashSet<long> queueItemIds = new HashSet<long>();
                    foreach (DataSourceImportedBatchDetail batch in bigResult.Data)
                    {
                        if (string.IsNullOrEmpty(batch.QueueItemIds))
                            continue;

                        string[] qIds = batch.QueueItemIds.Split(',');
                        queueItemIds.UnionWith(qIds.Select(itm => long.Parse(itm)));
                    }

                    QueueingManager queueingManager = new QueueingManager();
                    Dictionary<long, ItemExecutionFlowInfo> dicItemExecutionStatus = queueingManager.GetItemsExecutionFlowStatus(queueItemIds.ToList());

                    foreach (DataSourceImportedBatchDetail batch in bigResult.Data)
                    {
                        if (batch.MappingResult == MappingResult.Invalid)
                        {
                            batch.ExecutionStatus = ItemExecutionFlowStatus.Failed;
                            continue;
                        }

                        if (string.IsNullOrEmpty(batch.QueueItemIds))
                        {
                            batch.ExecutionStatus = ItemExecutionFlowStatus.NoBatches;
                            continue;
                        }

                        string[] batchQueueItemIds = batch.QueueItemIds.Split(',');
                        List<ItemExecutionFlowInfo> list = batchQueueItemIds.Select(itm => dicItemExecutionStatus.GetRecord(long.Parse(itm))).ToList();
                        batch.ExecutionStatus = list.Count > 1 ? queueingManager.GetExecutionFlowStatus(list) : list.First().Status;
                    }
                }
                return bigResult;
            }

            protected override ResultProcessingHandler<DataSourceImportedBatchDetail> GetResultProcessingHandler(DataRetrievalInput<DataSourceImportedBatchQuery> input, BigResult<DataSourceImportedBatchDetail> bigResult)
            {
                return  new ResultProcessingHandler<DataSourceImportedBatchDetail>
                {
                    ExportExcelHandler = new DataSourceImportedBatchExcelExportHandler()
                };
            }
        }


        private class DataSourceImportedBatchExcelExportHandler : ExcelExportHandler<DataSourceImportedBatchDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<DataSourceImportedBatchDetail> context)
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
                        row.Cells.Add(new ExportExcelCell { Value = record.MappingResultDescription });
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
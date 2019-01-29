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

        #region Public Methods
        public long WriteEntry(Entities.ImportedBatchEntry entry, Guid dataSourceId, string logEntryTime, ItemExecutionFlowStatus executionStatus = ItemExecutionFlowStatus.New)
        {
            IDataSourceImportedBatchDataManager manager = IntegrationDataManagerFactory.GetDataManager<IDataSourceImportedBatchDataManager>();
            return manager.InsertEntry(dataSourceId, entry.BatchDescription, entry.BatchSize, entry.BatchState, entry.IsDuplicateSameSize, entry.RecordsCount, entry.Result, entry.MapperMessage, entry.QueueItemsIds, logEntryTime, entry.BatchStart, entry.BatchEnd, executionStatus);
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

        public void UpdateExecutionStatus(Dictionary<long, ItemExecutionFlowStatus> executionStatusToUpdateById)
        {
            IDataSourceImportedBatchDataManager manager = IntegrationDataManagerFactory.GetDataManager<IDataSourceImportedBatchDataManager>();
            manager.UpdateExecutionStatus(executionStatusToUpdateById);
        }

        public void ApplyEvaluatedExecutionStatus()
        {
            string processStateUniqueName = "DataSourceImportedBatch_EvaluateExecutionStatus";
            int limitResult = 1000;

            ProcessStateManager processStateManager = new ProcessStateManager();
            DataSourceImportedBatchProcessStateSettings processState = processStateManager.GetProcessStateSetting<DataSourceImportedBatchProcessStateSettings>(processStateUniqueName);

            long? afterId = null;
            if (processState != null)
                afterId = processState.LastFinalizedId;

            IDataSourceImportedBatchDataManager importedBatchDataManager = IntegrationDataManagerFactory.GetDataManager<IDataSourceImportedBatchDataManager>();
            List<ItemExecutionFlowStatus> executionFlowsOpenStatus = new List<ItemExecutionFlowStatus>() { ItemExecutionFlowStatus.New, ItemExecutionFlowStatus.Processing };

            long? lastFinalizedIdToStore = null;
            bool loadMoreData = true;

            bool allPreviousBatchesAreFinalized = true;

            while (loadMoreData)
            {
                List<DataSourceImportedBatch> dataSourceImportedBatches = importedBatchDataManager.GetDataSourceImportedBatchesAfterID(limitResult, afterId, executionFlowsOpenStatus);

                if (dataSourceImportedBatches == null || dataSourceImportedBatches.Count == 0)
                    break;

                long lastFinalizedIdPerBatch;
                Dictionary<long, ItemExecutionFlowStatus> executionStatusToUpdateById = EvaluateImportedBatchesExecutionStatus(dataSourceImportedBatches, out lastFinalizedIdPerBatch);

                if (executionStatusToUpdateById.Count > 0)
                    UpdateExecutionStatus(executionStatusToUpdateById);

                long lastId = dataSourceImportedBatches.Last().ID;

                bool currentBatchIsFinalized = lastFinalizedIdPerBatch == lastId;

                if (allPreviousBatchesAreFinalized)
                    lastFinalizedIdToStore = lastFinalizedIdPerBatch;

                allPreviousBatchesAreFinalized = allPreviousBatchesAreFinalized && currentBatchIsFinalized;

                loadMoreData = (dataSourceImportedBatches.Count == limitResult);
                afterId = lastId;
            }

            if (lastFinalizedIdToStore.HasValue)
                processStateManager.InsertOrUpdate(processStateUniqueName, new DataSourceImportedBatchProcessStateSettings() { LastFinalizedId = lastFinalizedIdToStore.Value });
        }

        #endregion

        #region Private Methods
        private HashSet<long> GetQueueItemIds(List<DataSourceImportedBatch> dataSourceImportedBatches)
        {
            //Technically no need to use Hashset because ids in seperate rows will not repeat. 
            //Just in case it did, the use of hashset is to avoid this problem.
            HashSet<long> queueItemIds = new HashSet<long>();
            foreach (DataSourceImportedBatch batch in dataSourceImportedBatches)
            {
                if (string.IsNullOrEmpty(batch.QueueItemIds))
                    continue;

                string[] qIds = batch.QueueItemIds.Split(',');
                queueItemIds.UnionWith(qIds.Select(itm => long.Parse(itm)));
            }
            return queueItemIds;
        }

        private Dictionary<long, ItemExecutionFlowStatus> EvaluateImportedBatchesExecutionStatus(List<DataSourceImportedBatch> dataSourceImportedBatches, out long lastFinalizedExecutionStatusItemId)
        {
            HashSet<long> queueItemIds = GetQueueItemIds(dataSourceImportedBatches);

            QueueingManager queueingManager = new QueueingManager();
            Dictionary<long, ItemExecutionFlowInfo> dicItemExecutionStatus = queueingManager.GetItemsExecutionFlowStatus(queueItemIds.ToList());

            lastFinalizedExecutionStatusItemId = dataSourceImportedBatches.First().ID - 1;
            bool checkExecutionStatus = true;

            Dictionary<long, ItemExecutionFlowStatus> dictUpdatedDataSourceImportedBatches = new Dictionary<long, ItemExecutionFlowStatus>();

            foreach (DataSourceImportedBatch batch in dataSourceImportedBatches)
            {
                ItemExecutionFlowStatus previousStatus = batch.ExecutionStatus;

                if (batch.MappingResult == MappingResult.Invalid)
                {
                    if (checkExecutionStatus)
                        lastFinalizedExecutionStatusItemId = batch.ID;

                    batch.ExecutionStatus = ItemExecutionFlowStatus.Failed;

                    dictUpdatedDataSourceImportedBatches.Add(batch.ID, batch.ExecutionStatus);

                    continue;
                }

                if (string.IsNullOrEmpty(batch.QueueItemIds))
                {
                    if (checkExecutionStatus)
                        lastFinalizedExecutionStatusItemId = batch.ID;

                    batch.ExecutionStatus = ItemExecutionFlowStatus.NoBatches;

                    dictUpdatedDataSourceImportedBatches.Add(batch.ID, batch.ExecutionStatus);

                    continue;
                }

                string[] batchQueueItemIds = batch.QueueItemIds.Split(',');
                if (dicItemExecutionStatus == null || dicItemExecutionStatus.Count == 0)
                {
                    if (checkExecutionStatus)
                        lastFinalizedExecutionStatusItemId = batch.ID;

                    batch.ExecutionStatus = ItemExecutionFlowStatus.NotIdentified;

                    dictUpdatedDataSourceImportedBatches.Add(batch.ID, batch.ExecutionStatus);

                    continue;
                }

                bool shouldEvaluateStatus = true;

                List<ItemExecutionFlowInfo> list = new List<ItemExecutionFlowInfo>();
                foreach (var queueItemId in batchQueueItemIds)
                {
                    ItemExecutionFlowInfo item = dicItemExecutionStatus.GetRecord(long.Parse(queueItemId));
                    if (item == null)
                    {
                        batch.ExecutionStatus = ItemExecutionFlowStatus.NotIdentified;

                        // no need to try to evaluate status in the following code block because one of the queue items is deleted -> ExecutionStatus will be NotIdentified
                        shouldEvaluateStatus = false;

                        dictUpdatedDataSourceImportedBatches.Add(batch.ID, batch.ExecutionStatus);
                        break;
                    }
                    list.Add(item);
                }

                if (shouldEvaluateStatus) 
                {
                    batch.ExecutionStatus = list.Count > 1 ? queueingManager.GetExecutionFlowStatus(list) : list.First().Status;

                    if (batch.ExecutionStatus != previousStatus)
                        dictUpdatedDataSourceImportedBatches.Add(batch.ID, batch.ExecutionStatus);
                }

                if (checkExecutionStatus) // this value will be true as long as we don't have any not closed item
                {
                    if (IsClosedExecution(batch.ExecutionStatus))
                        lastFinalizedExecutionStatusItemId = batch.ID;
                    else
                        checkExecutionStatus = false;
                }
            }

            return dictUpdatedDataSourceImportedBatches;
        }

        private bool IsClosedExecution(ItemExecutionFlowStatus executionStatus)
        {
            switch (executionStatus)
            {
                case ItemExecutionFlowStatus.New:
                case ItemExecutionFlowStatus.Processing: return false;
                default: return true;
            }
        }

        #endregion

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

            protected override ResultProcessingHandler<DataSourceImportedBatchDetail> GetResultProcessingHandler(DataRetrievalInput<DataSourceImportedBatchQuery> input, BigResult<DataSourceImportedBatchDetail> bigResult)
            {
                return new ResultProcessingHandler<DataSourceImportedBatchDetail>
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
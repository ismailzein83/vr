using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.Common.Business.SummaryTransformation;
using Vanrise.Entities.SummaryTransformation;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Data;
using Vanrise.GenericData.Data.SQL;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.QueueActivators
{
    public class GenerateSummaryQueueActivator : Vanrise.Queueing.Entities.QueueActivator, Vanrise.Reprocess.Entities.IReprocessStageActivator
    {
        public int SummaryTransformationDefinitionId { get; set; }

        public string NextStageName { get; set; }

        public override void ProcessItem(Queueing.Entities.IQueueActivatorExecutionContext context)
        {
            DataRecordBatch dataRecordBatch = context.ItemToProcess as DataRecordBatch;
            var queueItemType = context.CurrentStage.QueueItemType as DataRecordBatchQueueItemType;
            if (queueItemType == null)
                throw new Exception("current stage QueueItemType is not of type DataRecordBatchQueueItemType");
            var recordTypeId = queueItemType.DataRecordTypeId;


            var batchRecords = dataRecordBatch.GetBatchRecords(recordTypeId);

            var transformationManager = new GenericSummaryTransformationManager() { SummaryTransformationDefinitionId = this.SummaryTransformationDefinitionId };
            var summaryRecordTypeId = transformationManager.SummaryTransformationDefinition.SummaryItemRecordTypeId;
            var recortTypeManager = new DataRecordTypeManager();
            var summaryRecordRuntimeType = recortTypeManager.GetDataRecordRuntimeType(summaryRecordTypeId);
            if (summaryRecordRuntimeType == null)
                throw new NullReferenceException(String.Format("summaryRecordRuntimeType {0}", recordTypeId));

            var summaryBatches = transformationManager.ConvertRawItemsToBatches(batchRecords, () => new GenericSummaryRecordBatch());
            if (summaryBatches != null)
            {
                foreach (var b in summaryBatches)
                {
                    b.SummaryTransformationDefinitionId = this.SummaryTransformationDefinitionId;
                    b.DescriptionTemplate = queueItemType.BatchDescription;
                    context.OutputItems.Add(this.NextStageName, b);
                }
            }
        }

        void Reprocess.Entities.IReprocessStageActivator.ExecuteStage(Reprocess.Entities.IReprocessStageActivatorExecutionContext context)
        {
            var transformationManager = new GenericSummaryTransformationManager() { SummaryTransformationDefinitionId = this.SummaryTransformationDefinitionId };
            var allSummaryBatches = new Dictionary<DateTime, Dictionary<string, Vanrise.Common.Business.SummaryTransformation.SummaryItemInProcess<GenericSummaryItem>>>();
            context.DoWhilePreviousRunning(() =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = context.InputQueue.TryDequeue((reprocessBatch) =>
                    {
                        Reprocess.Entities.GenericDataRecordBatch genericDataRecordBatch = reprocessBatch as Reprocess.Entities.GenericDataRecordBatch;
                        if (genericDataRecordBatch == null)
                            throw new Exception(String.Format("reprocessBatch should be of type 'Reprocess.Entities.GenericDataRecordBatch'. and not of type '{0}'", reprocessBatch.GetType()));

                        var summaryRecordBatches = transformationManager.ConvertRawItemsToBatches(genericDataRecordBatch.Records, () => new GenericSummaryRecordBatch());
                        if (summaryRecordBatches != null)
                        {
                            foreach (var summaryBatch in summaryRecordBatches)
                            {
                                Dictionary<string, Vanrise.Common.Business.SummaryTransformation.SummaryItemInProcess<GenericSummaryItem>> matchBatch;
                                if (!allSummaryBatches.TryGetValue(summaryBatch.BatchStart, out matchBatch))
                                {
                                    matchBatch = new Dictionary<string, Common.Business.SummaryTransformation.SummaryItemInProcess<GenericSummaryItem>>();
                                    allSummaryBatches.Add(summaryBatch.BatchStart, matchBatch);
                                }
                                transformationManager.UpdateExistingFromNew(matchBatch, summaryBatch);
                            }
                        }
                    });
                } while (!context.ShouldStop() && hasItem);

            });

            IStagingSummaryRecordDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IStagingSummaryRecordDataManager>();

            var dbApplyStream = dataManager.InitialiazeStreamForDBApply();

            //Store Summary Batches for finalization step
            foreach (var summaryBatchEntry in allSummaryBatches)
            {
                var genericSummaryBatch = new GenericSummaryRecordBatch
                {
                    BatchStart = summaryBatchEntry.Key,
                    Items = summaryBatchEntry.Value.Values.Select(summaryItemInProcess => summaryItemInProcess.SummaryItem).ToList(),
                    SummaryTransformationDefinitionId = this.SummaryTransformationDefinitionId
                };
                byte[] serializedBatch = genericSummaryBatch.Serialize();

                StagingSummaryRecord obj = new StagingSummaryRecord()
                {
                    ProcessInstanceId = context.ProcessInstanceId,
                    Data = serializedBatch,
                    BatchStart = summaryBatchEntry.Key,
                    StageName = context.CurrentStageName,
                };


                dataManager.WriteRecordToStream(obj, dbApplyStream);
            }
            var streamReadyToApply = dataManager.FinishDBApplyStream(dbApplyStream);
            dataManager.ApplyStreamToDB(streamReadyToApply);
        }

        void Reprocess.Entities.IReprocessStageActivator.FinalizeStage(Reprocess.Entities.IReprocessStageActivatorFinalizingContext context)
        {
            DataRecordStorageManager _dataRecordStorageManager = new DataRecordStorageManager();
            var transformationManager = new GenericSummaryTransformationManager() { SummaryTransformationDefinitionId = this.SummaryTransformationDefinitionId };

            var recordStorageDataManager = _dataRecordStorageManager.GetStorageDataManager(transformationManager.SummaryTransformationDefinition.DataRecordStorageId);
            if (recordStorageDataManager == null)
                throw new NullReferenceException(String.Format("recordStorageDataManager. ID '{0}'", transformationManager.SummaryTransformationDefinition.DataRecordStorageId));

            Queueing.MemoryQueue<Object> queueLoadedBatches = new Queueing.MemoryQueue<object>();
            AsyncActivityStatus loadBatchStatus = new AsyncActivityStatus();
            StartLoadingBatches(context, queueLoadedBatches, loadBatchStatus);

            Queueing.MemoryQueue<Object> queuePreparedBatches = new Queueing.MemoryQueue<object>();
            AsyncActivityStatus prepareBatchStatus = new AsyncActivityStatus();
            StartPreparingBatches(context, queueLoadedBatches, loadBatchStatus, queuePreparedBatches, prepareBatchStatus);

            StartInsertingBatches(transformationManager, recordStorageDataManager, queuePreparedBatches, prepareBatchStatus);
        }

        private static void StartLoadingBatches(Reprocess.Entities.IReprocessStageActivatorFinalizingContext context, Queueing.MemoryQueue<Object> queueLoadedBatches, AsyncActivityStatus loadBatchStatus)
        {
            Task loadDataTask = new Task(() =>
            {
                IStagingSummaryRecordDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IStagingSummaryRecordDataManager>();
                try
                {
                    dataManager.GetStagingSummaryRecords(context.ProcessInstanceId, context.CurrentStageName, (stagingSummaryRecord) =>
                    {
                        GenericSummaryRecordBatch genericSummaryRecordBatch = new GenericSummaryRecordBatch();
                        genericSummaryRecordBatch = genericSummaryRecordBatch.Deserialize<GenericSummaryRecordBatch>(stagingSummaryRecord.Data);
                        queueLoadedBatches.Enqueue(genericSummaryRecordBatch);
                    });
                }
                finally
                {
                    dataManager.DeleteStagingSummaryRecords(context.ProcessInstanceId, context.CurrentStageName);
                    loadBatchStatus.IsComplete = true;
                }
            });
            loadDataTask.Start();
        }

        private static void StartPreparingBatches(Reprocess.Entities.IReprocessStageActivatorFinalizingContext context, Queueing.MemoryQueue<Object> queueLoadedBatches, AsyncActivityStatus loadBatchStatus, Queueing.MemoryQueue<Object> queuePreparedBatches, AsyncActivityStatus prepareBatchStatus)
        {
            List<GenericSummaryRecordBatch> batches = null;
            DateTime batchStart = default(DateTime);

            Task prepareDataTask = new Task(() =>
            {
                try
                {
                    bool hasItem = false;
                    do
                    {
                        hasItem = queueLoadedBatches.TryDequeue((item) =>
                        {
                            GenericSummaryRecordBatch genericSummaryRecordBatch = item as GenericSummaryRecordBatch;
                            if (genericSummaryRecordBatch == null)
                                throw new Exception(String.Format("item should be of type 'GenericSummaryRecordBatch' and not of type '{0}'", item.GetType()));

                            if (genericSummaryRecordBatch.BatchStart != batchStart)
                            {
                                if (batches != null)
                                {
                                    queuePreparedBatches.Enqueue(batches);
                                }
                                batches = new List<GenericSummaryRecordBatch>();
                            }
                            batches.Add(genericSummaryRecordBatch);
                            batchStart = genericSummaryRecordBatch.BatchStart;
                        });
                    } while (!loadBatchStatus.IsComplete || hasItem);
                }
                finally
                {
                    if (batches != null && batches.Count > 0)
                        queuePreparedBatches.Enqueue(batches);

                    prepareBatchStatus.IsComplete = true;
                }
            });
            prepareDataTask.Start();
        }

        private static void StartInsertingBatches(GenericSummaryTransformationManager transformationManager, IDataRecordDataManager recordStorageDataManager, Queueing.MemoryQueue<Object> queuePreparedBatches, AsyncActivityStatus prepareBatchStatus)
        {
            IStagingSummaryRecordDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IStagingSummaryRecordDataManager>();

            bool hasItem = false;
            do
            {
                hasItem = queuePreparedBatches.TryDequeue((item) =>
                {
                    List<GenericSummaryRecordBatch> genericSummaryRecordBatchList = item as List<GenericSummaryRecordBatch>;
                    if (genericSummaryRecordBatchList == null)
                        throw new Exception(String.Format("item should be of type 'List<GenericSummaryRecordBatch>' and not of type '{0}'", item.GetType()));

                    DateTime batchStart = genericSummaryRecordBatchList[0].BatchStart;
                    recordStorageDataManager.DeleteRecords(batchStart);

                    Dictionary<string, SummaryItemInProcess<GenericSummaryItem>> _existingSummaryBatches = new Dictionary<string, SummaryItemInProcess<GenericSummaryItem>>();

                    foreach (GenericSummaryRecordBatch genericSummaryRecordBatch in genericSummaryRecordBatchList)
                        transformationManager.UpdateExistingFromNew(_existingSummaryBatches, genericSummaryRecordBatch);

                    transformationManager.SaveSummaryBatchToDB(_existingSummaryBatches.Values);
                });
            } while (!prepareBatchStatus.IsComplete || hasItem);
        }

        List<string> Reprocess.Entities.IReprocessStageActivator.GetOutputStages(List<string> stageNames)
        {
            return null;
        }

        Queueing.BaseQueue<Reprocess.Entities.IReprocessBatch> Reprocess.Entities.IReprocessStageActivator.GetQueue()
        {
            return new Queueing.MemoryQueue<Reprocess.Entities.IReprocessBatch>();
        }


        public List<Reprocess.Entities.StageRecordInfo> GetStageRecordInfo(Reprocess.Entities.IReprocessStageActivatorPreparingContext context)
        {
            IStagingSummaryRecordDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IStagingSummaryRecordDataManager>();
            return dataManager.GetStageRecordInfo(context.ProcessInstanceId, context.CurrentStageName);
        }
    }
}

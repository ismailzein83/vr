using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.Common.Business.SummaryTransformation;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Data;
using Vanrise.GenericData.Entities;
using Vanrise.Reprocess.Entities;

namespace Vanrise.GenericData.QueueActivators
{
    public class GenerateSummaryQueueActivator : Vanrise.Queueing.Entities.QueueActivator, Vanrise.Reprocess.Entities.IReprocessStageActivator
    {
        public Guid SummaryTransformationDefinitionId { get; set; }

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
            var allSummaryBatches = new ConcurrentDictionary<DateTime, ConcurrentDictionary<string, Vanrise.Common.Business.SummaryTransformation.SummaryItemInProcess<GenericSummaryItem>>>();
            ConcurrentDictionary<DateTime, List<StagingSummaryRecord>> stagingSummaryRecordDict = new ConcurrentDictionary<DateTime, List<StagingSummaryRecord>>();

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
                                ConcurrentDictionary<string, Vanrise.Common.Business.SummaryTransformation.SummaryItemInProcess<GenericSummaryItem>> matchBatch;
                                if (!allSummaryBatches.TryGetValue(summaryBatch.BatchStart, out matchBatch))
                                {
                                    matchBatch = new ConcurrentDictionary<string, Common.Business.SummaryTransformation.SummaryItemInProcess<GenericSummaryItem>>();
                                    allSummaryBatches.TryAdd(summaryBatch.BatchStart, matchBatch);
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
                var summaryItems = summaryBatchEntry.Value.Values;
                var genericSummaryBatch = new GenericSummaryRecordBatch
                {
                    BatchStart = summaryBatchEntry.Key,
                    Items = summaryItems.Select(summaryItemInProcess => summaryItemInProcess.SummaryItem).ToList(),
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


                if (context.To >= summaryItems.First().SummaryItem.BatchEnd && context.From <= summaryBatchEntry.Key)
                {
                    List<StagingSummaryRecord> stagingSummaryRecords;
                    if (!stagingSummaryRecordDict.TryGetValue(summaryBatchEntry.Key, out stagingSummaryRecords))
                    {
                        stagingSummaryRecords = new List<StagingSummaryRecord>();
                        stagingSummaryRecordDict.TryAdd(summaryBatchEntry.Key, stagingSummaryRecords);
                    }
                    stagingSummaryRecords.Add(obj);
                }
                else
                    dataManager.WriteRecordToStream(obj, dbApplyStream);
            }

            if (stagingSummaryRecordDict.Count > 0)
            {
                StartEnqueuingBatches(context, stagingSummaryRecordDict, transformationManager);
            }

            var streamReadyToApply = dataManager.FinishDBApplyStream(dbApplyStream);
            dataManager.ApplyStreamToDB(streamReadyToApply);
        }


        void Reprocess.Entities.IReprocessStageActivator.FinalizeStage(Reprocess.Entities.IReprocessStageActivatorFinalizingContext context)
        {
            StageBatchRecord stageBatchRecord = context.BatchRecord as StageBatchRecord;
            if (stageBatchRecord == null)
                throw new Exception(String.Format("context.BatchRecord should be of type 'StageRecordInfo' and not of type '{0}'", context.BatchRecord.GetType()));

            DataRecordStorageManager _dataRecordStorageManager = new DataRecordStorageManager();
            var transformationManager = new GenericSummaryTransformationManager() { SummaryTransformationDefinitionId = this.SummaryTransformationDefinitionId };

            var recordStorageDataManager = _dataRecordStorageManager.GetStorageDataManager(transformationManager.SummaryTransformationDefinition.DataRecordStorageId);
            if (recordStorageDataManager == null)
                throw new NullReferenceException(String.Format("recordStorageDataManager. ID '{0}'", transformationManager.SummaryTransformationDefinition.DataRecordStorageId));

            Queueing.MemoryQueue<GenericSummaryRecordBatch> queueLoadedBatches = new Queueing.MemoryQueue<GenericSummaryRecordBatch>();
            AsyncActivityStatus loadBatchStatus = new AsyncActivityStatus();
            StartLoadingBatches(context, queueLoadedBatches, loadBatchStatus, stageBatchRecord);

            PrepareAndInsertBatches(context.WriteTrackingMessage, context.DoWhilePreviousRunning, transformationManager, recordStorageDataManager, queueLoadedBatches, loadBatchStatus);
        }


        private static void StartEnqueuingBatches(Reprocess.Entities.IReprocessStageActivatorExecutionContext context, ConcurrentDictionary<DateTime, List<StagingSummaryRecord>> stagingSummaryRecordDict, GenericSummaryTransformationManager transformationManager)
        {
            Task loadDataTask = new Task(() =>
            {
                context.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Start Loading Batches");
                DataRecordStorageManager dataRecordStorageManager = new DataRecordStorageManager();
                var recordStorageDataManager = dataRecordStorageManager.GetStorageDataManager(transformationManager.SummaryTransformationDefinition.DataRecordStorageId);
                if (recordStorageDataManager == null)
                    throw new NullReferenceException(String.Format("recordStorageDataManager. ID '{0}'", transformationManager.SummaryTransformationDefinition.DataRecordStorageId));

                AsyncActivityStatus loadBatchStatus = new AsyncActivityStatus();
                foreach (KeyValuePair<DateTime, List<StagingSummaryRecord>> item in stagingSummaryRecordDict)
                {
                    Queueing.MemoryQueue<GenericSummaryRecordBatch> queueLoadedBatches = new Queueing.MemoryQueue<GenericSummaryRecordBatch>();
                    foreach (StagingSummaryRecord stagingSummaryRecord in item.Value)
                    {
                        GenericSummaryRecordBatch genericSummaryRecordBatch = new GenericSummaryRecordBatch();
                        genericSummaryRecordBatch = genericSummaryRecordBatch.Deserialize<GenericSummaryRecordBatch>(stagingSummaryRecord.Data);
                        queueLoadedBatches.Enqueue(genericSummaryRecordBatch);
                    }
                    PrepareAndInsertBatches(context.WriteTrackingMessage, context.DoWhilePreviousRunning, transformationManager, recordStorageDataManager, queueLoadedBatches, loadBatchStatus);
                }
                loadBatchStatus.IsComplete = true;
                context.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Finish Loading Batches");
            });
            loadDataTask.Start();
        }

        private static void PrepareAndInsertBatches(Action<LogEntryType, string> writeTrackingMessage, Action<AsyncActivityStatus, Action> doWhilePreviousRunning, GenericSummaryTransformationManager transformationManager, IDataRecordDataManager recordStorageDataManager, Queueing.MemoryQueue<GenericSummaryRecordBatch> queueLoadedBatches, AsyncActivityStatus loadBatchStatus)
        {
            Queueing.MemoryQueue<List<GenericSummaryRecordBatch>> queuePreparedBatches = new Queueing.MemoryQueue<List<GenericSummaryRecordBatch>>();
            AsyncActivityStatus prepareBatchStatus = new AsyncActivityStatus();
            StartPreparingBatches(writeTrackingMessage, doWhilePreviousRunning, queueLoadedBatches, loadBatchStatus, queuePreparedBatches, prepareBatchStatus);

            StartInsertingBatches(writeTrackingMessage, doWhilePreviousRunning, transformationManager, recordStorageDataManager, queuePreparedBatches, prepareBatchStatus);
        }

        private static void StartLoadingBatches(Reprocess.Entities.IReprocessStageActivatorFinalizingContext context, Queueing.MemoryQueue<GenericSummaryRecordBatch> queueLoadedBatches, AsyncActivityStatus loadBatchStatus, StageBatchRecord stageBatchRecord)
        {
            Task loadDataTask = new Task(() =>
            {
                IStagingSummaryRecordDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IStagingSummaryRecordDataManager>();
                try
                {
                    context.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Start Loading Batches");
                    dataManager.GetStagingSummaryRecords(context.ProcessInstanceId, context.CurrentStageName, stageBatchRecord.BatchStart, (stagingSummaryRecord) =>
                    {
                        GenericSummaryRecordBatch genericSummaryRecordBatch = new GenericSummaryRecordBatch();
                        genericSummaryRecordBatch = genericSummaryRecordBatch.Deserialize<GenericSummaryRecordBatch>(stagingSummaryRecord.Data);
                        queueLoadedBatches.Enqueue(genericSummaryRecordBatch);
                    });
                }
                finally
                {
                    dataManager.DeleteStagingSummaryRecords(context.ProcessInstanceId, context.CurrentStageName, stageBatchRecord.BatchStart);
                    loadBatchStatus.IsComplete = true;
                    context.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Finish Loading Batches");
                }
            });
            loadDataTask.Start();
        }

        private static void StartPreparingBatches(Action<LogEntryType, string> writeTrackingMessage, Action<AsyncActivityStatus, Action> doWhilePreviousRunning, Queueing.MemoryQueue<GenericSummaryRecordBatch> queueLoadedBatches, AsyncActivityStatus loadBatchStatus, Queueing.MemoryQueue<List<GenericSummaryRecordBatch>> queuePreparedBatches, AsyncActivityStatus prepareBatchStatus)
        {
            List<GenericSummaryRecordBatch> batches = null;
            DateTime batchStart = default(DateTime);

            Task prepareDataTask = new Task(() =>
            {
                try
                {
                    writeTrackingMessage(LogEntryType.Information, "Start Preparing Batches");
                    doWhilePreviousRunning(loadBatchStatus, () =>
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
                    });
                }
                finally
                {
                    if (batches != null && batches.Count > 0)
                        queuePreparedBatches.Enqueue(batches);

                    prepareBatchStatus.IsComplete = true;
                    writeTrackingMessage(LogEntryType.Information, "Finish Preparing Batches");
                }
            });
            prepareDataTask.Start();
        }

        private static void StartInsertingBatches(Action<LogEntryType, string> writeTrackingMessage, Action<AsyncActivityStatus, Action> doWhilePreviousRunning, GenericSummaryTransformationManager transformationManager, IDataRecordDataManager recordStorageDataManager, Queueing.MemoryQueue<List<GenericSummaryRecordBatch>> queuePreparedBatches, AsyncActivityStatus prepareBatchStatus)
        {
            writeTrackingMessage(LogEntryType.Information, "Start Inserting Batches");
            IStagingSummaryRecordDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IStagingSummaryRecordDataManager>();
            bool hasItem = false;
            doWhilePreviousRunning(prepareBatchStatus, () =>
            {
                do
                {
                    hasItem = queuePreparedBatches.TryDequeue((item) =>
                    {
                        List<GenericSummaryRecordBatch> genericSummaryRecordBatchList = item as List<GenericSummaryRecordBatch>;
                        if (genericSummaryRecordBatchList == null)
                            throw new Exception(String.Format("item should be of type 'List<GenericSummaryRecordBatch>' and not of type '{0}'", item.GetType()));

                        DateTime batchStart = genericSummaryRecordBatchList[0].BatchStart;
                        recordStorageDataManager.DeleteRecords(batchStart);

                        ConcurrentDictionary<string, SummaryItemInProcess<GenericSummaryItem>> _existingSummaryBatches = new ConcurrentDictionary<string, SummaryItemInProcess<GenericSummaryItem>>();

                        foreach (GenericSummaryRecordBatch genericSummaryRecordBatch in genericSummaryRecordBatchList)
                            transformationManager.UpdateExistingFromNew(_existingSummaryBatches, genericSummaryRecordBatch);

                        transformationManager.SaveSummaryBatchToDB(_existingSummaryBatches.Values);
                    });
                } while (!prepareBatchStatus.IsComplete || hasItem);
            });
            writeTrackingMessage(LogEntryType.Information, "Finish Inserting Batches");
        }

        List<string> Reprocess.Entities.IReprocessStageActivator.GetOutputStages(List<string> stageNames)
        {
            return null;
        }

        Queueing.BaseQueue<Reprocess.Entities.IReprocessBatch> Reprocess.Entities.IReprocessStageActivator.GetQueue()
        {
            return new Queueing.MemoryQueue<Reprocess.Entities.IReprocessBatch>();
        }


        public List<Reprocess.Entities.BatchRecord> GetStageBatchRecords(Reprocess.Entities.IReprocessStageActivatorPreparingContext context)
        {
            IStagingSummaryRecordDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IStagingSummaryRecordDataManager>();
            List<StagingSummaryInfo> stagingSummaryInfoList = dataManager.GetStagingSummaryInfo(context.ProcessInstanceId, context.CurrentStageName);

            if (stagingSummaryInfoList == null || stagingSummaryInfoList.Count == 0)
                return null;

            List<BatchRecord> stageBatchRecords = new List<BatchRecord>();
            foreach (StagingSummaryInfo stagingSummaryInfo in stagingSummaryInfoList)
            {
                StageBatchRecord stageBatchRecord = new StageBatchRecord()
                {
                    BatchStart = stagingSummaryInfo.BatchStart
                };
                stageBatchRecords.Add(stageBatchRecord);
            }

            return stageBatchRecords;
        }
    }

    public class StageBatchRecord : BatchRecord
    {
        public DateTime BatchStart { get; set; }
        public override string GetBatchTitle()
        {
            return string.Format("Batch Start: {0}", BatchStart);
        }
    }
}







//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Vanrise.BusinessProcess;
//using Vanrise.Common.Business;
//using Vanrise.Common.Business.SummaryTransformation;
//using Vanrise.Entities.SummaryTransformation;
//using Vanrise.GenericData.Business;
//using Vanrise.GenericData.Data;
//using Vanrise.GenericData.Data.SQL;
//using Vanrise.GenericData.Entities;

//namespace Vanrise.GenericData.QueueActivators
//{
//    public class GenerateSummaryQueueActivator : Vanrise.Queueing.Entities.QueueActivator, Vanrise.Reprocess.Entities.IReprocessStageActivator
//    {
//        public int SummaryTransformationDefinitionId { get; set; }

//        public string NextStageName { get; set; }

//        public override void ProcessItem(Queueing.Entities.IQueueActivatorExecutionContext context)
//        {
//            DataRecordBatch dataRecordBatch = context.ItemToProcess as DataRecordBatch;
//            var queueItemType = context.CurrentStage.QueueItemType as DataRecordBatchQueueItemType;
//            if (queueItemType == null)
//                throw new Exception("current stage QueueItemType is not of type DataRecordBatchQueueItemType");
//            var recordTypeId = queueItemType.DataRecordTypeId;


//            var batchRecords = dataRecordBatch.GetBatchRecords(recordTypeId);

//            var transformationManager = new GenericSummaryTransformationManager() { SummaryTransformationDefinitionId = this.SummaryTransformationDefinitionId };
//            var summaryRecordTypeId = transformationManager.SummaryTransformationDefinition.SummaryItemRecordTypeId;
//            var recortTypeManager = new DataRecordTypeManager();
//            var summaryRecordRuntimeType = recortTypeManager.GetDataRecordRuntimeType(summaryRecordTypeId);
//            if (summaryRecordRuntimeType == null)
//                throw new NullReferenceException(String.Format("summaryRecordRuntimeType {0}", recordTypeId));

//            var summaryBatches = transformationManager.ConvertRawItemsToBatches(batchRecords, () => new GenericSummaryRecordBatch());
//            if (summaryBatches != null)
//            {
//                foreach (var b in summaryBatches)
//                {
//                    b.SummaryTransformationDefinitionId = this.SummaryTransformationDefinitionId;
//                    b.DescriptionTemplate = queueItemType.BatchDescription;
//                    context.OutputItems.Add(this.NextStageName, b);
//                }
//            }
//        }

//        void Reprocess.Entities.IReprocessStageActivator.ExecuteStage(Reprocess.Entities.IReprocessStageActivatorExecutionContext context)
//        {
//            var transformationManager = new GenericSummaryTransformationManager() { SummaryTransformationDefinitionId = this.SummaryTransformationDefinitionId };
//            var allSummaryBatches = new Dictionary<DateTime, Dictionary<string, Vanrise.Common.Business.SummaryTransformation.SummaryItemInProcess<GenericSummaryItem>>>();
//            context.DoWhilePreviousRunning(() =>
//            {
//                bool hasItem = false;
//                do
//                {
//                    hasItem = context.InputQueue.TryDequeue((reprocessBatch) =>
//                    {
//                        Reprocess.Entities.GenericDataRecordBatch genericDataRecordBatch = reprocessBatch as Reprocess.Entities.GenericDataRecordBatch;
//                        if (genericDataRecordBatch == null)
//                            throw new Exception(String.Format("reprocessBatch should be of type 'Reprocess.Entities.GenericDataRecordBatch'. and not of type '{0}'", reprocessBatch.GetType()));

//                        var summaryRecordBatches = transformationManager.ConvertRawItemsToBatches(genericDataRecordBatch.Records, () => new GenericSummaryRecordBatch());
//                        if (summaryRecordBatches != null)
//                        {
//                            foreach (var summaryBatch in summaryRecordBatches)
//                            {
//                                Dictionary<string, Vanrise.Common.Business.SummaryTransformation.SummaryItemInProcess<GenericSummaryItem>> matchBatch;
//                                if (!allSummaryBatches.TryGetValue(summaryBatch.BatchStart, out matchBatch))
//                                {
//                                    matchBatch = new Dictionary<string, Common.Business.SummaryTransformation.SummaryItemInProcess<GenericSummaryItem>>();
//                                    allSummaryBatches.Add(summaryBatch.BatchStart, matchBatch);
//                                }
//                                transformationManager.UpdateExistingFromNew(matchBatch, summaryBatch);
//                            }
//                        }
//                    });
//                } while (!context.ShouldStop() && hasItem);

//            });
//            foreach (var summaryBatchEntry in allSummaryBatches)
//            {
//                string dataAnalysisName = BuildDataAnalysisName(context.ProcessInstanceId, context.CurrentStageName, summaryBatchEntry.Key);
//                DistributedDataGrouper dataGrouper = new DistributedDataGrouper(dataAnalysisName, new SummaryRecordDataGroupingHandler { SummaryTransformationDefinitionId = this.SummaryTransformationDefinitionId });
//                dataGrouper.DistributeGroupingItems(summaryBatchEntry.Value.Select(itm => new SummaryRecordDataGroupingItem { SummaryItem = itm.Value.SummaryItem } as IDataGroupingItem).ToList());                
//            }
//        }

//        static string BuildDataAnalysisName(long processInstanceId, string currentStageName, DateTime batchStart)
//        {
//            return string.Format("{0}{1}", BuildDataAnalysisNamePrefix(processInstanceId, currentStageName), batchStart);
//        }

//        static string BuildDataAnalysisNamePrefix(long processInstanceId, string currentStageName)
//        {
//            return string.Format("Reprocess_GenerateSummary_{0}_{1}_", processInstanceId, currentStageName);
//        }
//        void Reprocess.Entities.IReprocessStageActivator.FinalizeStage(Reprocess.Entities.IReprocessStageActivatorFinalizingContext context)
//        {
//            DataRecordStorageManager _dataRecordStorageManager = new DataRecordStorageManager();
//            var transformationManager = new GenericSummaryTransformationManager() { SummaryTransformationDefinitionId = this.SummaryTransformationDefinitionId };

//            var recordStorageDataManager = _dataRecordStorageManager.GetStorageDataManager(transformationManager.SummaryTransformationDefinition.DataRecordStorageId);
//            if (recordStorageDataManager == null)
//                throw new NullReferenceException(String.Format("recordStorageDataManager. ID '{0}'", transformationManager.SummaryTransformationDefinition.DataRecordStorageId));
//            recordStorageDataManager.DeleteRecords(context.BatchStart);
//            string dataAnalysisName = BuildDataAnalysisName(context.ProcessInstanceId, context.CurrentStageName, context.BatchStart);
//            DistributedDataGrouper dataGrouper = new DistributedDataGrouper(dataAnalysisName, new SummaryRecordDataGroupingHandler { SummaryTransformationDefinitionId = this.SummaryTransformationDefinitionId });
//            dataGrouper.StartGettingFinalResults(null);


//            //Queueing.MemoryQueue<List<GenericSummaryItem>> queueLoadedBatches = new Queueing.MemoryQueue<List<GenericSummaryItem>>();
//            //AsyncActivityStatus loadBatchStatus = new AsyncActivityStatus();
//            //StartLoadingBatches(context, queueLoadedBatches, loadBatchStatus);
//            //StartInsertingBatches(context, transformationManager, recordStorageDataManager, queueLoadedBatches, loadBatchStatus);
//        }

//        private void StartLoadingBatches(Reprocess.Entities.IReprocessStageActivatorFinalizingContext context, Queueing.MemoryQueue<List<GenericSummaryItem>> queueLoadedBatches, AsyncActivityStatus loadBatchStatus)
//        {
//            Task loadDataTask = new Task(() =>
//            {
//                IStagingSummaryRecordDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IStagingSummaryRecordDataManager>();
//                try
//                {
//                    context.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Start Loading Batches");
//                    string dataAnalysisName = BuildDataAnalysisName(context.ProcessInstanceId, context.CurrentStageName, context.BatchStart);
//                    DistributedDataGrouper dataGrouper = new DistributedDataGrouper(dataAnalysisName, new SummaryRecordDataGroupingHandler { SummaryTransformationDefinitionId = this.SummaryTransformationDefinitionId });
//                    dataGrouper.StartGettingFinalResults((dataGroupingItems) =>
//                        {
//                            queueLoadedBatches.Enqueue(dataGroupingItems.Select(itm => (itm as SummaryRecordDataGroupingItem).SummaryItem).ToList());
//                        });                    
//                }
//                finally
//                {
//                    loadBatchStatus.IsComplete = true;
//                    context.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Finish Loading Batches");
//                }
//            });
//            loadDataTask.Start();
//        }

//        private static void StartInsertingBatches(Reprocess.Entities.IReprocessStageActivatorFinalizingContext context, GenericSummaryTransformationManager transformationManager, IDataRecordDataManager recordStorageDataManager, Queueing.MemoryQueue<List<GenericSummaryItem>> queuePreparedBatches, AsyncActivityStatus prepareBatchStatus)
//        {
//            context.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Start Inserting Batches");

//            bool hasItem = false;
//            recordStorageDataManager.DeleteRecords(context.BatchStart);
//            context.DoWhilePreviousRunning(prepareBatchStatus, () =>
//            {
//                do
//                {
//                    hasItem = queuePreparedBatches.TryDequeue((summaryItems) =>
//                    {
//                        transformationManager.SaveSummaryBatchToDB(summaryItems.Select(itm => new SummaryItemInProcess<GenericSummaryItem>{ SummaryItem = itm, ShouldUpdate = true}));
//                    });
//                } while (!prepareBatchStatus.IsComplete || hasItem);
//            });
//            context.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Finish Inserting Batches");
//        }

//        List<string> Reprocess.Entities.IReprocessStageActivator.GetOutputStages(List<string> stageNames)
//        {
//            return null;
//        }

//        Queueing.BaseQueue<Reprocess.Entities.IReprocessBatch> Reprocess.Entities.IReprocessStageActivator.GetQueue()
//        {
//            return new Queueing.MemoryQueue<Reprocess.Entities.IReprocessBatch>();
//        }


//        public List<Reprocess.Entities.StageRecordInfo> GetStageRecordInfo(Reprocess.Entities.IReprocessStageActivatorPreparingContext context)
//        {
//            string dataAnalysisNamePrefix = BuildDataAnalysisNamePrefix(context.ProcessInstanceId, context.CurrentStageName);
//            List<string> dataAnalysisNames = new DataGroupingAnalysisInfoManager().GetDataAnalysisNames(dataAnalysisNamePrefix);
//            if (dataAnalysisNames != null && dataAnalysisNames.Count > 0)
//            {
//                int prefixLength = dataAnalysisNamePrefix.Length;
//                List<Reprocess.Entities.StageRecordInfo> stageInfos = new List<Reprocess.Entities.StageRecordInfo>();
//                foreach(var dataAnalysisName in dataAnalysisNames)
//                {
//                    DateTime batchStart = DateTime.Parse(dataAnalysisName.Remove(0, prefixLength));
//                    stageInfos.Add(new Reprocess.Entities.StageRecordInfo
//                        {
//                            BatchStart = batchStart
//                        });
//                }
//                return stageInfos;
//            }
//            else
//                return null;
//        }
//    }

//    public class SummaryRecordDataGroupingItem : IDataGroupingItem
//    {
//        public GenericSummaryItem SummaryItem { get; set; }
//    }

//    public class SummaryRecordDataGroupingHandler : DataGroupingHandler
//    {
//        public int SummaryTransformationDefinitionId { get; set; }


//        public override string GetItemGroupingKey(IDataGroupingHandlerGetItemGroupingKeyContext context)
//        {
//            GenericSummaryItem summaryItem = (context.Item as SummaryRecordDataGroupingItem).SummaryItem;
//            return GetSummaryTransformationManager().GetSummaryItemKey(summaryItem);
//        }

//        public override void UpdateExistingItemFromNew(IDataGroupingHandlerUpdateExistingFromNewContext context)
//        {
//            GenericSummaryItem existingItem = (context.Existing as SummaryRecordDataGroupingItem).SummaryItem;
//            GenericSummaryItem newItem = (context.New as SummaryRecordDataGroupingItem).SummaryItem;
//            GetSummaryTransformationManager().UpdateSummaryItemFromSummaryItem(existingItem, newItem);
//        }

//        public override void FinalizeGrouping(IDataGroupingHandlerFinalizeGroupingContext context)
//        {
//            var transformationManager = GetSummaryTransformationManager();
//            var summaryItems = context.GroupedItems.Select(itm => new SummaryItemInProcess<GenericSummaryItem> { SummaryItem = (itm as SummaryRecordDataGroupingItem).SummaryItem, ShouldUpdate = true }).ToList();
//            transformationManager.SaveSummaryBatchToDB(summaryItems);
//        }

//        public override string SerializeItems(List<IDataGroupingItem> items)
//        {
//            var summaryTransformationManager = GetSummaryTransformationManager();
//            List<dynamic> dataRecords = items.Select(itm => summaryTransformationManager.GetDataRecordFromSummaryItem((itm as SummaryRecordDataGroupingItem).SummaryItem)).ToList();
//            return Common.Serializer.Serialize(dataRecords, true);
//        }

//        DataRecordTypeManager _dataRecordTypeManager = new DataRecordTypeManager();

//        public override List<IDataGroupingItem> DeserializeItems(string serializedItems)
//        {
//            var summaryTransformationManager = GetSummaryTransformationManager();
//            var summaryRecordTypeId = summaryTransformationManager.SummaryTransformationDefinition.SummaryItemRecordTypeId;

//            Type recordTypeRuntimeType = _dataRecordTypeManager.GetDataRecordRuntimeType(summaryRecordTypeId);
//            Type genericListType = typeof(List<>);
//            Type recordListType = genericListType.MakeGenericType(recordTypeRuntimeType);
//            var deserializedRecords = Common.Serializer.Deserialize(serializedItems, recordListType) as dynamic;

//            List<IDataGroupingItem> deserializedItems = new List<IDataGroupingItem>();

//            foreach(dynamic record in deserializedRecords)
//            {
//                GenericSummaryItem summaryItem = summaryTransformationManager.GetSummaryItemFromDataRecord(record);
//                deserializedItems.Add(new SummaryRecordDataGroupingItem
//                    {
//                        SummaryItem = summaryItem
//                    });
//            }
//            return deserializedItems;
//        }

//        public override string SerializeFinalResultItems(List<dynamic> finalResultItems)
//        {
//            var summaryTransformationManager = GetSummaryTransformationManager();
//            List<dynamic> dataRecords = finalResultItems.Select(itm => summaryTransformationManager.GetDataRecordFromSummaryItem((itm as SummaryRecordDataGroupingItem).SummaryItem)).ToList();
//            return Common.Serializer.Serialize(dataRecords, true);
//        }

//        public override List<dynamic> DeserializeFinalResultItems(string serializedItems)
//        {
//            var summaryTransformationManager = GetSummaryTransformationManager();
//            var summaryRecordTypeId = summaryTransformationManager.SummaryTransformationDefinition.SummaryItemRecordTypeId;

//            Type recordTypeRuntimeType = _dataRecordTypeManager.GetDataRecordRuntimeType(summaryRecordTypeId);
//            Type genericListType = typeof(List<>);
//            Type recordListType = genericListType.MakeGenericType(recordTypeRuntimeType);
//            var deserializedRecords = Common.Serializer.Deserialize(serializedItems, recordListType) as dynamic;

//            List<dynamic> deserializedItems = new List<dynamic>();

//            foreach (dynamic record in deserializedRecords)
//            {
//                GenericSummaryItem summaryItem = summaryTransformationManager.GetSummaryItemFromDataRecord(record);
//                deserializedItems.Add(new SummaryRecordDataGroupingItem
//                {
//                    SummaryItem = summaryItem
//                });
//            }
//            return deserializedItems;
//        }

//        GenericSummaryTransformationManager _summaryTransformationManager;

//        GenericSummaryTransformationManager GetSummaryTransformationManager()
//        {
//            if (_summaryTransformationManager == null)
//                _summaryTransformationManager = new GenericSummaryTransformationManager { SummaryTransformationDefinitionId = this.SummaryTransformationDefinitionId };
//            return _summaryTransformationManager;
//        }
//    }

//}
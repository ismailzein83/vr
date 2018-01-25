using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.Common;
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


        #region QueueActivator

        public override void OnDisposed()
        {
        }

        public override void ProcessItem(Queueing.Entities.PersistentQueueItem item, Queueing.Entities.ItemsToEnqueue outputItems)
        {
        }

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

        #endregion

        #region IReprocessStageActivator

        public object InitializeStage(IReprocessStageActivatorInitializingContext context)
        {
            SummaryTransformationDefinition summaryTransformationDefinition = new SummaryTransformationDefinitionManager().GetSummaryTransformationDefinition(this.SummaryTransformationDefinitionId);
            summaryTransformationDefinition.ThrowIfNull("summaryTransformationDefinition", this.SummaryTransformationDefinitionId);

            return new DataRecordStorageManager().CreateTempStorage(summaryTransformationDefinition.DataRecordStorageId, context.ProcessId);
        }

        public void ExecuteStage(Reprocess.Entities.IReprocessStageActivatorExecutionContext context)
        {
            TempStorageInformation tempStorageInformation = context.InitializationStageOutput != null ? context.InitializationStageOutput.CastWithValidate<TempStorageInformation>("tempStorageInformation") : null;

            var transformationManager = new GenericSummaryTransformationManager() { SummaryTransformationDefinitionId = this.SummaryTransformationDefinitionId, TempStorageInformation = tempStorageInformation };
            var allSummaryBatches = new VRDictionary<DateTime, VRDictionary<string, Vanrise.Common.Business.SummaryTransformation.SummaryItemInProcess<GenericSummaryItem>>>();
            Dictionary<DateTime, GenericSummaryRecordBatch> genericSummaryRecordBatchesDict = new Dictionary<DateTime, GenericSummaryRecordBatch>();

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
                                VRDictionary<string, Vanrise.Common.Business.SummaryTransformation.SummaryItemInProcess<GenericSummaryItem>> matchBatch;
                                if (!allSummaryBatches.TryGetValue(summaryBatch.BatchStart, out matchBatch))
                                {
                                    matchBatch = new VRDictionary<string, Common.Business.SummaryTransformation.SummaryItemInProcess<GenericSummaryItem>>();
                                    allSummaryBatches.Add(summaryBatch.BatchStart, matchBatch);
                                }
                                transformationManager.UpdateExistingFromNew(matchBatch, summaryBatch);
                            }
                        }
                    });
                } while (!context.ShouldStop() && hasItem);
            });

            IStagingSummaryRecordDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IStagingSummaryRecordDataManager>();
            List<Task> runningTasks = new List<Task>();

            if (allSummaryBatches != null && allSummaryBatches.Count > 0)
            {
                object dbApplyStream = dataManager.InitialiazeStreamForDBApply();

                //Store Summary Batches for finalization step
                foreach (var summaryBatchEntry in allSummaryBatches)
                {
                    var summaryItems = summaryBatchEntry.Value.Values;

                    var currentBatchStart = summaryBatchEntry.Key;
                    var currentBatchEnd = summaryItems.First().SummaryItem.BatchEnd;

                    var genericSummaryBatch = new GenericSummaryRecordBatch { Items = summaryItems.Select(summaryItemInProcess => summaryItemInProcess.SummaryItem).ToList(), SummaryTransformationDefinitionId = this.SummaryTransformationDefinitionId };
                    genericSummaryBatch.SetBatchStart(currentBatchStart);
                    genericSummaryBatch.SetBatchEnd(currentBatchEnd);

                    StagingSummaryRecord stagingSummaryRecord = new StagingSummaryRecord() { ProcessInstanceId = context.ProcessInstanceId, BatchStart = currentBatchStart, BatchEnd = currentBatchEnd, StageName = context.CurrentStageName };

                    if (context.To >= currentBatchEnd && context.From <= currentBatchStart)
                    {
                        genericSummaryRecordBatchesDict.Add(currentBatchStart, genericSummaryBatch);
                        stagingSummaryRecord.AlreadyFinalised = true;
                    }
                    else
                    {
                        stagingSummaryRecord.Data = genericSummaryBatch.Serialize();
                        stagingSummaryRecord.AlreadyFinalised = false;
                    }

                    dataManager.WriteRecordToStream(stagingSummaryRecord, dbApplyStream);
                }

                Task applyDataTask = new Task(() =>
                {
                    var streamReadyToApply = dataManager.FinishDBApplyStream(dbApplyStream);
                    dataManager.ApplyStreamToDB(streamReadyToApply);
                });
                applyDataTask.Start();
                runningTasks.Add(applyDataTask);
            }

            if (genericSummaryRecordBatchesDict.Count > 0)
                StartEnqueuingBatches(context, genericSummaryRecordBatchesDict, transformationManager, tempStorageInformation);

            if (runningTasks.Count > 0)
                Task.WaitAll(runningTasks.ToArray());
        }

        public void FinalizeStage(Reprocess.Entities.IReprocessStageActivatorFinalizingContext context)
        {
            TempStorageInformation tempStorageInformation = context.InitializationStageOutput != null ? context.InitializationStageOutput.CastWithValidate<TempStorageInformation>("tempStorageInformation") : null;

            StageBatchRecord stageBatchRecord = context.BatchRecord as StageBatchRecord;
            if (stageBatchRecord == null)
                throw new Exception(String.Format("context.BatchRecord should be of type 'StageRecordInfo' and not of type '{0}'", context.BatchRecord.GetType()));

            DataRecordStorageManager _dataRecordStorageManager = new DataRecordStorageManager();
            var transformationManager = new GenericSummaryTransformationManager() { SummaryTransformationDefinitionId = this.SummaryTransformationDefinitionId, TempStorageInformation = tempStorageInformation };

            var recordStorageDataManager = _dataRecordStorageManager.GetStorageDataManager(transformationManager.SummaryTransformationDefinition.DataRecordStorageId, tempStorageInformation);
            if (recordStorageDataManager == null)
                throw new NullReferenceException(String.Format("recordStorageDataManager. ID '{0}'", transformationManager.SummaryTransformationDefinition.DataRecordStorageId));

            var recordFilterGroup = context.GetRecordFilterGroup(transformationManager.SummaryTransformationDefinition.SummaryItemRecordTypeId);
            if (tempStorageInformation == null && stageBatchRecord.IsEmptyBatch)
            {
                recordStorageDataManager.DeleteRecords(stageBatchRecord.BatchStart, stageBatchRecord.BatchEnd, recordFilterGroup);
            }
            else
            {
                Queueing.MemoryQueue<GenericSummaryRecordBatch> queueLoadedBatches = new Queueing.MemoryQueue<GenericSummaryRecordBatch>();
                AsyncActivityStatus loadBatchStatus = new AsyncActivityStatus();
                Task loadDataTask = new Task(() =>
                {
                    StartLoadingBatches(context, queueLoadedBatches, loadBatchStatus, stageBatchRecord);
                });
                loadDataTask.Start();

                PrepareAndInsertBatches(context.WriteTrackingMessage, context.DoWhilePreviousRunning, transformationManager, recordStorageDataManager, queueLoadedBatches, loadBatchStatus, context.CurrentStageName,
                    stageBatchRecord.BatchStart, tempStorageInformation, recordFilterGroup);
                loadDataTask.Wait();
            }
        }

        public int? GetStorageRowCount(IReprocessStageActivatorGetStorageRowCountContext context)
        {
            TempStorageInformation tempStorageInformation = null;

            if (context.InitializationStageOutput != null)
                tempStorageInformation = context.InitializationStageOutput.CastWithValidate<TempStorageInformation>("context.InitializationStageOutput");

            SummaryTransformationDefinition summaryTransformationDefinition = new SummaryTransformationDefinitionManager().GetSummaryTransformationDefinition(this.SummaryTransformationDefinitionId);
            summaryTransformationDefinition.ThrowIfNull("summaryTransformationDefinition", this.SummaryTransformationDefinitionId);

            DataRecordStorageManager dataRecordStorageManager = new DataRecordStorageManager();
            return dataRecordStorageManager.GetStorageRowCount(summaryTransformationDefinition.DataRecordStorageId, tempStorageInformation);
        }

        public void CommitChanges(IReprocessStageActivatorCommitChangesContext context)
        {
            TempStorageInformation tempStorageInformation = context.InitializationStageOutput.CastWithValidate<TempStorageInformation>("context.InitializationStageOutput");

            SummaryTransformationDefinition summaryTransformationDefinition = new SummaryTransformationDefinitionManager().GetSummaryTransformationDefinition(this.SummaryTransformationDefinitionId);
            summaryTransformationDefinition.ThrowIfNull("summaryTransformationDefinition", this.SummaryTransformationDefinitionId);

            var recordFilterGroup = context.GetRecordFilterGroup(summaryTransformationDefinition.SummaryItemRecordTypeId);
            new DataRecordStorageManager().FillDataRecordStorageFromTempStorage(summaryTransformationDefinition.DataRecordStorageId, tempStorageInformation, context.From, context.To, recordFilterGroup);
        }

        public void DropStorage(Reprocess.Entities.IReprocessStageActivatorDropStorageContext context)
        {
            TempStorageInformation tempStorageInformation = context.InitializationStageOutput.CastWithValidate<TempStorageInformation>("context.InitializationStageOutput");

            SummaryTransformationDefinition summaryTransformationDefinition = new SummaryTransformationDefinitionManager().GetSummaryTransformationDefinition(this.SummaryTransformationDefinitionId);
            summaryTransformationDefinition.ThrowIfNull("summaryTransformationDefinition", this.SummaryTransformationDefinitionId);

            new DataRecordStorageManager().DropStorage(summaryTransformationDefinition.DataRecordStorageId, tempStorageInformation);
        }

        public List<string> GetOutputStages(List<string> stageNames)
        {
            return null;
        }

        public Queueing.BaseQueue<Reprocess.Entities.IReprocessBatch> GetQueue()
        {
            return new Queueing.MemoryQueue<Reprocess.Entities.IReprocessBatch>();
        }

        public List<Reprocess.Entities.BatchRecord> GetStageBatchRecords(Reprocess.Entities.IReprocessStageActivatorPreparingContext context)
        {
            IStagingSummaryRecordDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IStagingSummaryRecordDataManager>();
            List<StagingSummaryInfo> stagingSummaryInfoList = dataManager.GetStagingSummaryInfo(context.ProcessInstanceId, context.CurrentStageName);

            List<BatchRecord> stageBatchRecords = new List<BatchRecord>();

            if (stagingSummaryInfoList == null || stagingSummaryInfoList.Count == 0)
            {
                StageBatchRecord batchRecord = new StageBatchRecord()
                {
                    BatchStart = context.StartDate,
                    BatchEnd = context.EndDate,
                    IsEmptyBatch = true
                };
                stageBatchRecords.Add(batchRecord);
            }
            else
            {
                stagingSummaryInfoList = stagingSummaryInfoList.GroupBy(itm => new { itm.BatchStart, itm.BatchEnd, itm.AlreadyFinalised }).Select(itm => itm.First()).ToList();

                DateTime current = context.StartDate;
                foreach (var stagingSummaryInfo in stagingSummaryInfoList.OrderBy(itm => itm.BatchStart))
                {
                    if (current < stagingSummaryInfo.BatchStart)
                    {
                        stageBatchRecords.Add(new StageBatchRecord { BatchStart = current, BatchEnd = stagingSummaryInfo.BatchStart, IsEmptyBatch = true });
                    }
                    current = stagingSummaryInfo.BatchEnd;
                    if (stagingSummaryInfo.AlreadyFinalised)
                    {
                        dataManager.DeleteStagingSummaryRecords(context.ProcessInstanceId, context.CurrentStageName, stagingSummaryInfo.BatchStart, stagingSummaryInfo.BatchEnd);
                    }
                    else
                    {
                        stageBatchRecords.Add(new StageBatchRecord { BatchStart = stagingSummaryInfo.BatchStart, BatchEnd = stagingSummaryInfo.BatchEnd, IsEmptyBatch = false });
                    }
                }
                if (current < context.EndDate)
                {
                    stageBatchRecords.Add(new StageBatchRecord { BatchStart = current, BatchEnd = context.EndDate, IsEmptyBatch = true });
                }
            }
            return stageBatchRecords;
        }

        #endregion

        #region Private Methods

        private static void StartEnqueuingBatches(Reprocess.Entities.IReprocessStageActivatorExecutionContext context, Dictionary<DateTime, GenericSummaryRecordBatch> genericSummaryRecordBatchesDict,
            GenericSummaryTransformationManager transformationManager, TempStorageInformation tempStorageInformation)
        {
            DataRecordStorageManager dataRecordStorageManager = new DataRecordStorageManager();
            var recordStorageDataManager = dataRecordStorageManager.GetStorageDataManager(transformationManager.SummaryTransformationDefinition.DataRecordStorageId, tempStorageInformation);
            if (recordStorageDataManager == null)
                throw new NullReferenceException(String.Format("recordStorageDataManager. ID '{0}'", transformationManager.SummaryTransformationDefinition.DataRecordStorageId));

            var recordFilterGroup = context.GetRecordFilterGroup(transformationManager.SummaryTransformationDefinition.SummaryItemRecordTypeId);
            foreach (var genericSummaryRecordBatch in genericSummaryRecordBatchesDict)
            {
                InsertBatches(context.WriteTrackingMessage, transformationManager, recordStorageDataManager, new List<GenericSummaryRecordBatch>() { genericSummaryRecordBatch.Value },
                    context.CurrentStageName, genericSummaryRecordBatch.Key, tempStorageInformation, recordFilterGroup);
            }
        }

        private static void StartLoadingBatches(Reprocess.Entities.IReprocessStageActivatorFinalizingContext context, Queueing.MemoryQueue<GenericSummaryRecordBatch> queueLoadedBatches,
            AsyncActivityStatus loadBatchStatus, StageBatchRecord stageBatchRecord)
        {
            IStagingSummaryRecordDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IStagingSummaryRecordDataManager>();
            try
            {
                context.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, string.Format("Start Loading Batches for Stage {0}", context.CurrentStageName));
                dataManager.GetStagingSummaryRecords(context.ProcessInstanceId, context.CurrentStageName, stageBatchRecord.BatchStart, stageBatchRecord.BatchEnd, (stagingSummaryRecord) =>
                {
                    GenericSummaryRecordBatch genericSummaryRecordBatch = new GenericSummaryRecordBatch();
                    genericSummaryRecordBatch = genericSummaryRecordBatch.Deserialize<GenericSummaryRecordBatch>(stagingSummaryRecord.Data);
                    queueLoadedBatches.Enqueue(genericSummaryRecordBatch);
                });
            }
            finally
            {
                dataManager.DeleteStagingSummaryRecords(context.ProcessInstanceId, context.CurrentStageName, stageBatchRecord.BatchStart, stageBatchRecord.BatchEnd);
                loadBatchStatus.IsComplete = true;
                context.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, string.Format("Finish Loading Batches for Stage {0}", context.CurrentStageName));
            }
        }

        private static void PrepareAndInsertBatches(Action<LogEntryType, string> writeTrackingMessage, Action<AsyncActivityStatus, Action> doWhilePreviousRunning,
            GenericSummaryTransformationManager transformationManager, IDataRecordDataManager recordStorageDataManager, Queueing.MemoryQueue<GenericSummaryRecordBatch> queueLoadedBatches,
            AsyncActivityStatus loadBatchStatus, string currentStageName, DateTime batchStart, TempStorageInformation tempStorageInformation, RecordFilterGroup recordFilterGroup)
        {
            List<GenericSummaryRecordBatch> preparedBatches = PrepareBatches(writeTrackingMessage, doWhilePreviousRunning, queueLoadedBatches, loadBatchStatus, currentStageName);
            InsertBatches(writeTrackingMessage, transformationManager, recordStorageDataManager, preparedBatches, currentStageName, batchStart, tempStorageInformation, recordFilterGroup);
        }

        private static List<GenericSummaryRecordBatch> PrepareBatches(Action<LogEntryType, string> writeTrackingMessage, Action<AsyncActivityStatus, Action> doWhilePreviousRunning,
            Queueing.MemoryQueue<GenericSummaryRecordBatch> queueLoadedBatches, AsyncActivityStatus loadBatchStatus, string currentStageName)
        {
            List<GenericSummaryRecordBatch> preparedBatches = new List<GenericSummaryRecordBatch>();
            try
            {
                writeTrackingMessage(Vanrise.Entities.LogEntryType.Information, string.Format("Start Preparing Batches for Stage {0}", currentStageName));
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

                            preparedBatches.Add(genericSummaryRecordBatch);
                        });
                    } while (!loadBatchStatus.IsComplete || hasItem);
                });
            }
            finally
            {
                writeTrackingMessage(Vanrise.Entities.LogEntryType.Information, string.Format("Finish Preparing Batches for Stage {0}", currentStageName));
            }
            return preparedBatches;
        }

        private static void InsertBatches(Action<LogEntryType, string> writeTrackingMessage, GenericSummaryTransformationManager transformationManager, IDataRecordDataManager recordStorageDataManager,
            List<GenericSummaryRecordBatch> queuePreparedBatches, string currentStageName, DateTime batchStart, TempStorageInformation tempStorageInformation, RecordFilterGroup recordFilterGroup)
        {
            VRDictionary<string, SummaryItemInProcess<GenericSummaryItem>> _existingSummaryBatches = new VRDictionary<string, SummaryItemInProcess<GenericSummaryItem>>();

            if (tempStorageInformation == null)
                recordStorageDataManager.DeleteRecords(batchStart, recordFilterGroup);

            foreach (GenericSummaryRecordBatch genericSummaryRecordBatch in queuePreparedBatches)
                transformationManager.UpdateExistingFromNew(_existingSummaryBatches, genericSummaryRecordBatch);

            if (_existingSummaryBatches.Values.Count > 0)
            {
                transformationManager.SaveSummaryBatchToDB(_existingSummaryBatches.Values);
            }
        }

        #endregion
    }

    public class StageBatchRecord : BatchRecord
    {
        public DateTime BatchStart { get; set; }
        public DateTime BatchEnd { get; set; }
        public bool IsEmptyBatch { get; set; }
        public override string GetBatchTitle()
        {
            return string.Format("Batch Start: {0}, Batch End : {1}", BatchStart.ToString("yyyy-MM-dd HH:mm:ss"), BatchEnd.ToString("yyyy-MM-dd HH:mm:ss"));
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

//        void ExecuteStage(Reprocess.Entities.IReprocessStageActivatorExecutionContext context)
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
//        void FinalizeStage(Reprocess.Entities.IReprocessStageActivatorFinalizingContext context)
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

//        List<string> GetOutputStages(List<string> stageNames)
//        {
//            return null;
//        }

//        Queueing.BaseQueue<Reprocess.Entities.IReprocessBatch> GetQueue()
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
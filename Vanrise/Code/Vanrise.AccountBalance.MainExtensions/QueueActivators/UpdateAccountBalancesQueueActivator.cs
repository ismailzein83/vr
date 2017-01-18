using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Business;
using Vanrise.AccountBalance.Entities;
using Vanrise.GenericData.QueueActivators;
using Vanrise.GenericData.Transformation;
using Vanrise.Common;
using Vanrise.GenericData.Data;
using Vanrise.GenericData.Entities;
using Vanrise.Reprocess.Entities;

namespace Vanrise.AccountBalance.MainExtensions.QueueActivators
{
    public class UpdateAccountBalancesQueueActivator : Vanrise.Queueing.Entities.QueueActivator, Vanrise.Reprocess.Entities.IReprocessStageActivator
    {
        public Guid AccountTypeId { get; set; }
        public string AccountId { get; set; }
        public string EffectiveOn { get; set; }
        public string Amount { get; set; }
        public string CurrencyId { get; set; }
        public Guid TransactionTypeId { get; set; }

        public override void ProcessItem(Queueing.Entities.IQueueActivatorExecutionContext context)
        {
            DataRecordBatch dataRecordBatch = context.ItemToProcess as DataRecordBatch;
            var queueItemType = context.CurrentStage.QueueItemType as DataRecordBatchQueueItemType;
            if (queueItemType == null)
                throw new Exception("current stage QueueItemType is not of type DataRecordBatchQueueItemType");
            var recordTypeId = queueItemType.DataRecordTypeId;
            var batchRecords = dataRecordBatch.GetBatchRecords(recordTypeId);

            UpdateUsageBalancePayload balanceUsageDetail = new UpdateUsageBalancePayload();
            balanceUsageDetail.UpdateUsageBalanceItems = new List<UpdateUsageBalanceItem>();
            balanceUsageDetail.TransactionTypeId = this.TransactionTypeId;
            foreach (var record in batchRecords)
            {
                decimal? amount = Vanrise.Common.Utilities.GetPropValueReader(this.Amount).GetPropertyValue(record);

                if (amount.HasValue && amount.Value > 0)
                {
                    UpdateUsageBalanceItem usageBalanceUpdate = new UpdateUsageBalanceItem();
                    usageBalanceUpdate.Value = amount.Value;
                    usageBalanceUpdate.AccountId = Vanrise.Common.Utilities.GetPropValueReader(this.AccountId).GetPropertyValue(record);
                    usageBalanceUpdate.EffectiveOn = Vanrise.Common.Utilities.GetPropValueReader(this.EffectiveOn).GetPropertyValue(record);
                    usageBalanceUpdate.CurrencyId = Vanrise.Common.Utilities.GetPropValueReader(this.CurrencyId).GetPropertyValue(record);

                    balanceUsageDetail.UpdateUsageBalanceItems.Add(usageBalanceUpdate);
                }
            }

            if (balanceUsageDetail.UpdateUsageBalanceItems.Count > 0)
            {
                UsageBalanceManager usageBalanceManager = new UsageBalanceManager();
                usageBalanceManager.UpdateUsageBalance(this.AccountTypeId, balanceUsageDetail);
            }
        }

        public override void ProcessItem(Queueing.Entities.PersistentQueueItem item, Queueing.Entities.ItemsToEnqueue outputItems)
        {
        }

        public override void OnDisposed()
        {
        }


        public void ExecuteStage(Reprocess.Entities.IReprocessStageActivatorExecutionContext context)
        {
            Dictionary<DateTime, Dictionary<long, List<UpdateUsageBalanceItem>>> accountUsageBalanceItemsByBatchStart = new Dictionary<DateTime, Dictionary<long, List<UpdateUsageBalanceItem>>>();
            Dictionary<long, List<UpdateUsageBalanceItem>> accountUpdateUsageBalanceItems;

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

                        foreach (var record in genericDataRecordBatch.Records)
                        {
                            decimal? amount = Vanrise.Common.Utilities.GetPropValueReader(this.Amount).GetPropertyValue(record);
                            DateTime effectiveOn = Vanrise.Common.Utilities.GetPropValueReader(this.EffectiveOn).GetPropertyValue(record);

                            if (amount.HasValue && amount.Value > 0)
                            {
                                UpdateUsageBalanceItem usageBalanceUpdate = new UpdateUsageBalanceItem();
                                usageBalanceUpdate.Value = amount.Value;
                                usageBalanceUpdate.AccountId = Vanrise.Common.Utilities.GetPropValueReader(this.AccountId).GetPropertyValue(record);
                                usageBalanceUpdate.EffectiveOn = effectiveOn.Date;
                                usageBalanceUpdate.CurrencyId = Vanrise.Common.Utilities.GetPropValueReader(this.CurrencyId).GetPropertyValue(record);

                                accountUpdateUsageBalanceItems = accountUsageBalanceItemsByBatchStart.GetOrCreateItem(usageBalanceUpdate.EffectiveOn);
                                List<UpdateUsageBalanceItem> updateUsageBalanceItems = accountUpdateUsageBalanceItems.GetOrCreateItem(usageBalanceUpdate.AccountId);
                                updateUsageBalanceItems.Add(usageBalanceUpdate);
                            }
                        }
                    });
                } while (!context.ShouldStop() && hasItem);
            });

            IStagingSummaryRecordDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IStagingSummaryRecordDataManager>();
            object dbApplyStream = null;

            //Store UsageBalance Batches for finalization step
            foreach (var accountUsageBalanceBatchEntry in accountUsageBalanceItemsByBatchStart)
            {
                var accountUsageBalanceItems = accountUsageBalanceBatchEntry.Value;

                byte[] serializedBatch = ProtoBufSerializer.Serialize(accountUsageBalanceItems);

                StagingSummaryRecord obj = new StagingSummaryRecord()
                {
                    ProcessInstanceId = context.ProcessInstanceId,
                    Data = serializedBatch,
                    BatchStart = accountUsageBalanceBatchEntry.Key,
                    StageName = context.CurrentStageName
                };

                if (dbApplyStream == null)
                    dbApplyStream = dataManager.InitialiazeStreamForDBApply();
                dataManager.WriteRecordToStream(obj, dbApplyStream);
            }

            if (dbApplyStream != null)
            {
                var streamReadyToApply = dataManager.FinishDBApplyStream(dbApplyStream);
                dataManager.ApplyStreamToDB(streamReadyToApply);
            }
        }

        public void FinalizeStage(Reprocess.Entities.IReprocessStageActivatorFinalizingContext context)
        {

        }

        public List<string> GetOutputStages(List<string> stageNames)
        {
            return null;
        }

        public Vanrise.Queueing.BaseQueue<Reprocess.Entities.IReprocessBatch> GetQueue()
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
}

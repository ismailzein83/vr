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
using Vanrise.Common.Business;
using Vanrise.BusinessProcess;
using Vanrise.Entities;

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
                    usageBalanceUpdate.AccountId = Vanrise.Common.Utilities.GetPropValueReader(this.AccountId).GetPropertyValue(record).ToString();
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
            Dictionary<DateTime, Dictionary<String, CorrectUsageBalanceItem>> accountCorrectUsageBalanceItemsByBatchStart = new Dictionary<DateTime, Dictionary<String, CorrectUsageBalanceItem>>();
            Dictionary<String, CorrectUsageBalanceItem> correctUsageBalanceItems;
            CorrectUsageBalanceItem correctUsageBalanceItem;

            var currencyExchangeRateManager = new Vanrise.Common.Business.CurrencyExchangeRateManager();
            var configManager = new Vanrise.Common.Business.ConfigManager();
            int systemCurrencyId = configManager.GetSystemCurrencyId();

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

                            if (amount.HasValue && amount.Value > 0)
                            {
                                String accountId = Vanrise.Common.Utilities.GetPropValueReader(this.AccountId).GetPropertyValue(record).ToString();
                                DateTime effectiveOn = Vanrise.Common.Utilities.GetPropValueReader(this.EffectiveOn).GetPropertyValue(record);
                                int currencyId = Vanrise.Common.Utilities.GetPropValueReader(this.CurrencyId).GetPropertyValue(record);
                                decimal convertedAmount = currencyExchangeRateManager.ConvertValueToCurrency(amount.Value, currencyId, systemCurrencyId, effectiveOn);

                                correctUsageBalanceItems = accountCorrectUsageBalanceItemsByBatchStart.GetOrCreateItem(effectiveOn.Date);
                                if (!correctUsageBalanceItems.TryGetValue(accountId, out correctUsageBalanceItem))
                                {
                                    correctUsageBalanceItem = new CorrectUsageBalanceItem()
                                    {
                                        Value = convertedAmount,
                                        AccountId = accountId,
                                        CurrencyId = systemCurrencyId
                                    };
                                    correctUsageBalanceItems.Add(accountId, correctUsageBalanceItem);
                                }
                                else
                                {
                                    correctUsageBalanceItem.Value += convertedAmount;
                                }
                            }
                        }
                    });
                } while (!context.ShouldStop() && hasItem);
            });

            IStagingSummaryRecordDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IStagingSummaryRecordDataManager>();
            object dbApplyStream = null;

            //Store UsageBalance Batches for finalization step
            foreach (var accountUsageBalanceBatchEntry in accountCorrectUsageBalanceItemsByBatchStart)
            {
                byte[] serializedBatch = ProtoBufSerializer.Serialize(accountUsageBalanceBatchEntry.Value);

                StagingSummaryRecord obj = new StagingSummaryRecord()
                {
                    ProcessInstanceId = context.ProcessInstanceId,
                    StageName = context.CurrentStageName,
                    BatchStart = accountUsageBalanceBatchEntry.Key,
                    BatchEnd = accountUsageBalanceBatchEntry.Key.AddDays(1),
                    Data = serializedBatch
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
            StageBatchRecord stageBatchRecord = context.BatchRecord as StageBatchRecord;
            if (stageBatchRecord == null)
                throw new Exception(String.Format("context.BatchRecord should be of type 'StageBatchRecord' and not of type '{0}'", context.BatchRecord.GetType()));

            if (stageBatchRecord.IsEmptyBatch)
            {
                UsageBalanceManager usageBalanceManager = new UsageBalanceManager();
                Guid correctionProcessId = usageBalanceManager.InitializeUpdateUsageBalance();

                CorrectUsageBalancePayload correctUsageBalancePayload = new CorrectUsageBalancePayload()
                {
                    CorrectUsageBalanceItems = null,
                    PeriodDate = stageBatchRecord.BatchStart,
                    CorrectionProcessId = correctionProcessId,
                    IsLastBatch = true,
                    TransactionTypeId = this.TransactionTypeId
                };
                usageBalanceManager.CorrectUsageBalance(this.AccountTypeId, correctUsageBalancePayload);
            }
            else
            {
                Queueing.MemoryQueue<Dictionary<long, CorrectUsageBalanceItem>> queueLoadedBatches = new Queueing.MemoryQueue<Dictionary<long, CorrectUsageBalanceItem>>();
                AsyncActivityStatus loadBatchStatus = new AsyncActivityStatus();
                Task loadDataTask = new Task(() =>
                {
                    StartLoadingBatches(context, queueLoadedBatches, loadBatchStatus, stageBatchRecord);
                });
                loadDataTask.Start();

                Dictionary<long, CorrectUsageBalanceItem> preparedCorrectUsageBalanceItems = new Dictionary<long, CorrectUsageBalanceItem>();
                PrepareUsageBalanceItems(context.WriteTrackingMessage, context.DoWhilePreviousRunning, queueLoadedBatches, loadBatchStatus, context.CurrentStageName, preparedCorrectUsageBalanceItems);
                InsertUsageBalanceItems(context.WriteTrackingMessage, context.CurrentStageName, stageBatchRecord.BatchStart, this.AccountTypeId, this.TransactionTypeId, preparedCorrectUsageBalanceItems);
            }
        }

        private static void StartLoadingBatches(Reprocess.Entities.IReprocessStageActivatorFinalizingContext context, Queueing.MemoryQueue<Dictionary<long, CorrectUsageBalanceItem>> queueLoadedBatches,
            AsyncActivityStatus loadBatchStatus, StageBatchRecord stageBatchRecord)
        {
            IStagingSummaryRecordDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IStagingSummaryRecordDataManager>();
            try
            {
                context.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, string.Format("Start Loading Batches for Stage {0}", context.CurrentStageName));
                dataManager.GetStagingSummaryRecords(context.ProcessInstanceId, context.CurrentStageName, stageBatchRecord.BatchStart, (stagingSummaryRecord) =>
                {
                    Dictionary<long, CorrectUsageBalanceItem> accountUsageBalance = ProtoBufSerializer.Deserialize<Dictionary<long, CorrectUsageBalanceItem>>(stagingSummaryRecord.Data);
                    queueLoadedBatches.Enqueue(accountUsageBalance);
                });
            }
            finally
            {
                dataManager.DeleteStagingSummaryRecords(context.ProcessInstanceId, context.CurrentStageName, stageBatchRecord.BatchStart);
                loadBatchStatus.IsComplete = true;
                context.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, string.Format("Finish Loading Batches for Stage {0}", context.CurrentStageName));
            }
        }

        private static void PrepareUsageBalanceItems(Action<LogEntryType, string> writeTrackingMessage, Action<AsyncActivityStatus, Action> doWhilePreviousRunning,
            Queueing.MemoryQueue<Dictionary<long, CorrectUsageBalanceItem>> queueLoadedBatches, AsyncActivityStatus loadBatchStatus,
            string currentStageName, Dictionary<long, CorrectUsageBalanceItem> preparedCorrectUsageBalanceItems)
        {
            CorrectUsageBalanceItem correctUsageBalanceItem;

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
                            Dictionary<long, CorrectUsageBalanceItem> accountUsageBalanceItems = item as Dictionary<long, CorrectUsageBalanceItem>;
                            if (accountUsageBalanceItems == null)
                                throw new Exception(String.Format("item should be of type 'Dictionary<long, CorrectUsageBalanceItem>' and not of type '{0}'", item.GetType()));

                            foreach (var accountUsageBalanceItem in accountUsageBalanceItems)
                            {
                                if (!preparedCorrectUsageBalanceItems.TryGetValue(accountUsageBalanceItem.Key, out correctUsageBalanceItem))
                                {
                                    preparedCorrectUsageBalanceItems.Add(accountUsageBalanceItem.Key, new CorrectUsageBalanceItem()
                                    {
                                        Value = accountUsageBalanceItem.Value.Value,
                                        AccountId = accountUsageBalanceItem.Value.AccountId,
                                        CurrencyId = accountUsageBalanceItem.Value.CurrencyId
                                    });
                                }
                                else
                                {
                                    correctUsageBalanceItem.Value += accountUsageBalanceItem.Value.Value;
                                }
                            }
                        });
                    } while (!loadBatchStatus.IsComplete || hasItem);
                });
            }
            finally
            {
                writeTrackingMessage(Vanrise.Entities.LogEntryType.Information, string.Format("Finish Preparing Batches for Stage {0}", currentStageName));
            }
        }

        private static void InsertUsageBalanceItems(Action<LogEntryType, string> writeTrackingMessage, string currentStageName, DateTime batchStart, Guid accountTypeId,
            Guid transactionTypeId, Dictionary<long, CorrectUsageBalanceItem> preparedCorrectUsageBalanceItems)
        {
            writeTrackingMessage(Vanrise.Entities.LogEntryType.Information, string.Format("Start Inserting Batches for Stage {0}", currentStageName));

            UsageBalanceManager usageBalanceManager = new UsageBalanceManager();
            Guid correctionProcessId = usageBalanceManager.InitializeUpdateUsageBalance();

            int counter = 1;
            decimal maxCount = 10000;
            int numberOfBatches = (int)(Math.Ceiling(preparedCorrectUsageBalanceItems.Values.Count / maxCount));

            CorrectUsageBalancePayload correctUsageBalancePayload = new CorrectUsageBalancePayload()
            {
                CorrectUsageBalanceItems = new List<CorrectUsageBalanceItem>(),
                PeriodDate = batchStart,
                CorrectionProcessId = correctionProcessId,
                IsLastBatch = counter == numberOfBatches,
                TransactionTypeId = transactionTypeId
            };

            foreach (var itm in preparedCorrectUsageBalanceItems.Values)
            {
                correctUsageBalancePayload.CorrectUsageBalanceItems.Add(itm);
                if (correctUsageBalancePayload.CorrectUsageBalanceItems.Count == maxCount)
                {
                    usageBalanceManager.CorrectUsageBalance(accountTypeId, correctUsageBalancePayload);
                    counter++;
                    correctUsageBalancePayload = new CorrectUsageBalancePayload()
                    {
                        CorrectUsageBalanceItems = new List<CorrectUsageBalanceItem>(),
                        PeriodDate = batchStart,
                        CorrectionProcessId = correctionProcessId,
                        IsLastBatch = counter == numberOfBatches,
                        TransactionTypeId = transactionTypeId
                    };
                }
            }

            if (correctUsageBalancePayload.CorrectUsageBalanceItems.Count > 0)
                usageBalanceManager.CorrectUsageBalance(accountTypeId, correctUsageBalancePayload);

            writeTrackingMessage(Vanrise.Entities.LogEntryType.Information, string.Format("Finish Inserting Batches for Stage {0}", currentStageName));
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
                Dictionary<DateTime, StagingSummaryInfo> availableStagingSummaryInfos = stagingSummaryInfoList.ToDictionary(itm => itm.BatchStart, itm => itm);

                var firstStagingSummaryInfoItem = stagingSummaryInfoList[0];
                DateTime firstBatchStart = firstStagingSummaryInfoItem.BatchStart;
                DateTime firstBatchEnd = firstStagingSummaryInfoItem.BatchEnd;

                var batchDurationInMinutes = (firstBatchEnd - firstBatchStart).TotalMinutes;

                DateTime endDate = context.StartDate;
                DateTime startDate;

                while (endDate != context.EndDate)
                {
                    startDate = endDate;
                    endDate = startDate.AddMinutes(batchDurationInMinutes);

                    if (endDate > context.EndDate)
                        endDate = context.EndDate;

                    StageBatchRecord stageBatchRecord = new StageBatchRecord()
                    {
                        BatchStart = startDate,
                        BatchEnd = endDate
                    };

                    if (availableStagingSummaryInfos.ContainsKey(startDate))
                    {
                        stageBatchRecord.IsEmptyBatch = false;
                    }
                    else
                    {
                        stageBatchRecord.IsEmptyBatch = true;
                    }
                    stageBatchRecords.Add(stageBatchRecord);
                }
            }
            return stageBatchRecords;
        }

        public static UpdateUsageBalanceItem usageBalanceItem { get; set; }
    }
}

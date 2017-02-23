using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Data;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.QueueActivators;
using Vanrise.Reprocess.Entities;

namespace Vanrise.AccountBalance.Business
{
    public abstract class BaseUpdateAccountBalancesQueueActivator : Vanrise.Queueing.Entities.QueueActivator, Vanrise.Reprocess.Entities.IReprocessStageActivator
    {
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

            Dictionary<AccountBalanceType, UpdateUsageBalancePayload> updateUsageBalanceItemsByType = new Dictionary<AccountBalanceType, UpdateUsageBalancePayload>();

            Action<BalanceUpdatePayload> submitBalanceUpdate = (balanceUpdatePayload) =>
                {
                    AccountBalanceType accountBalanceType = new AccountBalanceType()
                    {
                        AccountTypeId = balanceUpdatePayload.AccountTypeId,
                        TransactionTypeId = balanceUpdatePayload.TransactionTypeId
                    };

                    UpdateUsageBalancePayload updateUsageBalancePayload;
                    if (!updateUsageBalanceItemsByType.TryGetValue(accountBalanceType, out updateUsageBalancePayload))
                    {
                        updateUsageBalancePayload = new UpdateUsageBalancePayload();
                        updateUsageBalancePayload.TransactionTypeId = balanceUpdatePayload.TransactionTypeId;
                        updateUsageBalancePayload.UpdateUsageBalanceItems = new List<UpdateUsageBalanceItem>();
                        updateUsageBalancePayload.UpdateUsageBalanceItems.Add(new UpdateUsageBalanceItem()
                        {
                            AccountId = balanceUpdatePayload.AccountId,
                            EffectiveOn = balanceUpdatePayload.EffectiveOn,
                            Value = balanceUpdatePayload.Amount,
                            CurrencyId = balanceUpdatePayload.CurrencyId
                        });

                        updateUsageBalanceItemsByType.Add(accountBalanceType, updateUsageBalancePayload);
                    }
                    else
                    {
                        updateUsageBalancePayload.UpdateUsageBalanceItems.Add(new UpdateUsageBalanceItem()
                        {
                            AccountId = balanceUpdatePayload.AccountId,
                            EffectiveOn = balanceUpdatePayload.EffectiveOn,
                            Value = balanceUpdatePayload.Amount,
                            CurrencyId = balanceUpdatePayload.CurrencyId
                        });
                    }
                };

            foreach (var record in batchRecords)
            {

                var convertToAccountBalanceContext = new ConvertToBalanceUpdateContext(submitBalanceUpdate) { Record = record };
                ConvertToBalanceUpdate(convertToAccountBalanceContext);
            }

            foreach (var updateUsageBalanceItem in updateUsageBalanceItemsByType)
            {
                UsageBalanceManager usageBalanceManager = new UsageBalanceManager();
                usageBalanceManager.UpdateUsageBalance(updateUsageBalanceItem.Key.AccountTypeId, updateUsageBalanceItem.Value);
            }
        }

        protected abstract void ConvertToBalanceUpdate(IConvertToBalanceUpdateContext context);

        public void ExecuteStage(Reprocess.Entities.IReprocessStageActivatorExecutionContext context)
        {
            CurrencyExchangeRateManager currencyExchangeRateManager = new Vanrise.Common.Business.CurrencyExchangeRateManager();
            int systemCurrencyId = new ConfigManager().GetSystemCurrencyId();

            var correctUsageBalanceItemsByTypeByBatchStart = new CorrectUsageBalanceItemByTypeByBatchStart();
            CorrectUsageBalanceItemByType correctUsageBalanceItemsByType;
            CorrectUsageBalanceItemByAccount correctUsageBalanceItems;
            CorrectUsageBalanceItem correctUsageBalanceItem;

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

                        Action<BalanceUpdatePayload> submitBalanceUpdate = (balanceUpdatePayload) =>
                            {
                                AccountBalanceType accountBalanceType = new AccountBalanceType() { AccountTypeId = balanceUpdatePayload.AccountTypeId, TransactionTypeId = balanceUpdatePayload.TransactionTypeId };

                                decimal convertedAmount = currencyExchangeRateManager.ConvertValueToCurrency(balanceUpdatePayload.Amount, balanceUpdatePayload.CurrencyId, systemCurrencyId, balanceUpdatePayload.EffectiveOn); ;

                                correctUsageBalanceItemsByType = correctUsageBalanceItemsByTypeByBatchStart.GetOrCreateItem(balanceUpdatePayload.EffectiveOn.Date);
                                if (!correctUsageBalanceItemsByType.TryGetValue(accountBalanceType, out correctUsageBalanceItems))
                                {
                                    correctUsageBalanceItem = new CorrectUsageBalanceItem() { Value = convertedAmount, AccountId = balanceUpdatePayload.AccountId, CurrencyId = systemCurrencyId };
                                    correctUsageBalanceItems = new CorrectUsageBalanceItemByAccount();
                                    correctUsageBalanceItems.Add(balanceUpdatePayload.AccountId, correctUsageBalanceItem);
                                    correctUsageBalanceItemsByType.Add(accountBalanceType, correctUsageBalanceItems);
                                }
                                else
                                {
                                    if (!correctUsageBalanceItems.TryGetValue(balanceUpdatePayload.AccountId, out correctUsageBalanceItem))
                                    {
                                        correctUsageBalanceItem = new CorrectUsageBalanceItem() { Value = convertedAmount, AccountId = balanceUpdatePayload.AccountId, CurrencyId = systemCurrencyId };
                                        correctUsageBalanceItems.Add(balanceUpdatePayload.AccountId, correctUsageBalanceItem);
                                    }
                                    else
                                        correctUsageBalanceItem.Value += convertedAmount;
                                }
                            };

                        foreach (var record in genericDataRecordBatch.Records)
                        {
                            var convertToBalanceUpdateContext = new ConvertToBalanceUpdateContext(submitBalanceUpdate) { Record = record };
                            ConvertToBalanceUpdate(convertToBalanceUpdateContext);
                        }
                    });
                } while (!context.ShouldStop() && hasItem);
            });

            IStagingSummaryRecordDataManager stagingSummaryRecordDataManager = GenericDataDataManagerFactory.GetDataManager<IStagingSummaryRecordDataManager>();
            List<Task> runningTasks = new List<Task>();

            CorrectUsageBalanceItemByTypeByBatchStart finalizedCorrectUsageBalanceItemByTypeByBatchStart = new CorrectUsageBalanceItemByTypeByBatchStart();

            if (correctUsageBalanceItemsByTypeByBatchStart != null && correctUsageBalanceItemsByTypeByBatchStart.Count > 0)
            {
                object dbApplyStream = stagingSummaryRecordDataManager.InitialiazeStreamForDBApply();

                //Store UsageBalance Batches for finalization step
                foreach (var correctUsageBalanceBatchEntry in correctUsageBalanceItemsByTypeByBatchStart)
                {
                    var currentBatchStart = correctUsageBalanceBatchEntry.Key;
                    var currentBatchEnd = correctUsageBalanceBatchEntry.Key.AddDays(1);

                    StagingSummaryRecord stagingSummaryRecord = new StagingSummaryRecord() { ProcessInstanceId = context.ProcessInstanceId, StageName = context.CurrentStageName, BatchStart = currentBatchStart, BatchEnd = currentBatchEnd };

                    if (context.To >= currentBatchEnd && context.From <= currentBatchStart)
                    {
                        finalizedCorrectUsageBalanceItemByTypeByBatchStart.Add(currentBatchStart, correctUsageBalanceBatchEntry.Value);
                        stagingSummaryRecord.AlreadyFinalised = true;
                    }
                    else
                    {
                        byte[] serializedBatch = ProtoBufSerializer.Serialize(correctUsageBalanceBatchEntry.Value);
                        stagingSummaryRecord.Data = serializedBatch;
                        stagingSummaryRecord.AlreadyFinalised = false;
                    }
                    stagingSummaryRecordDataManager.WriteRecordToStream(stagingSummaryRecord, dbApplyStream);
                }

                Task applyDataTask = new Task(() =>
                {
                    var streamReadyToApply = stagingSummaryRecordDataManager.FinishDBApplyStream(dbApplyStream);
                    stagingSummaryRecordDataManager.ApplyStreamToDB(streamReadyToApply);
                });

                applyDataTask.Start();
                runningTasks.Add(applyDataTask);
            }

            if (finalizedCorrectUsageBalanceItemByTypeByBatchStart.Count > 0)
            {
                foreach (var correctUsageBalanceItemByType in finalizedCorrectUsageBalanceItemByTypeByBatchStart)
                    InsertUsageBalanceItems(context.WriteTrackingMessage, context.CurrentStageName, correctUsageBalanceItemByType.Key, correctUsageBalanceItemByType.Value);
            }

            if (runningTasks.Count > 0)
                Task.WaitAll(runningTasks.ToArray());
        }

        public void FinalizeStage(Reprocess.Entities.IReprocessStageActivatorFinalizingContext context)
        {
            StageBatchRecord stageBatchRecord = context.BatchRecord as StageBatchRecord;
            if (stageBatchRecord == null)
                throw new Exception(String.Format("context.BatchRecord should be of type 'StageBatchRecord' and not of type '{0}'", context.BatchRecord.GetType()));

            if (stageBatchRecord.IsEmptyBatch)
            {
                UsageBalanceManager usageBalanceManager = new UsageBalanceManager();

                FinalizeEmptyBatchesContext finalizeEmptyBatchesContext = new FinalizeEmptyBatchesContext((accountBalanceType) =>
                {
                    DateTime currentBatchStart = stageBatchRecord.BatchStart;
                    do
                    {
                        Guid correctionProcessId = usageBalanceManager.InitializeUpdateUsageBalance();

                        CorrectUsageBalancePayload correctUsageBalancePayload = new CorrectUsageBalancePayload()
                        {
                            CorrectUsageBalanceItems = null,
                            PeriodDate = currentBatchStart,
                            CorrectionProcessId = correctionProcessId,
                            IsLastBatch = true,
                            TransactionTypeId = accountBalanceType.TransactionTypeId
                        };
                        usageBalanceManager.CorrectUsageBalance(accountBalanceType.AccountTypeId, correctUsageBalancePayload);
                        currentBatchStart = currentBatchStart.AddDays(1);
                    } while (currentBatchStart < stageBatchRecord.BatchEnd);
                });

                FinalizeEmptyBatches(finalizeEmptyBatchesContext);
            }
            else
            {
                Queueing.MemoryQueue<CorrectUsageBalanceItemByType> queueLoadedBatches = new Queueing.MemoryQueue<CorrectUsageBalanceItemByType>();
                AsyncActivityStatus loadBatchStatus = new AsyncActivityStatus();
                Task loadDataTask = new Task(() =>
                {
                    StartLoadingBatches(context, queueLoadedBatches, loadBatchStatus, stageBatchRecord);
                });
                loadDataTask.Start();

                CorrectUsageBalanceItemByType preparedCorrectUsageBalanceItems = new CorrectUsageBalanceItemByType();
                PrepareUsageBalanceItems(context.WriteTrackingMessage, context.DoWhilePreviousRunning, queueLoadedBatches, loadBatchStatus, context.CurrentStageName, preparedCorrectUsageBalanceItems);
                InsertUsageBalanceItems(context.WriteTrackingMessage, context.CurrentStageName, stageBatchRecord.BatchStart, preparedCorrectUsageBalanceItems);
            }
        }

        protected abstract void FinalizeEmptyBatches(IFinalizeEmptyBatchesContext context);

        private static void StartLoadingBatches(Reprocess.Entities.IReprocessStageActivatorFinalizingContext context, Queueing.MemoryQueue<CorrectUsageBalanceItemByType> queueLoadedBatches,
            AsyncActivityStatus loadBatchStatus, StageBatchRecord stageBatchRecord)
        {
            IStagingSummaryRecordDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IStagingSummaryRecordDataManager>();
            try
            {
                context.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, string.Format("Start Loading Batches for Stage {0}", context.CurrentStageName));
                dataManager.GetStagingSummaryRecords(context.ProcessInstanceId, context.CurrentStageName, stageBatchRecord.BatchStart, (stagingSummaryRecord) =>
                {
                    var correctUsageBalanceItemsByType = ProtoBufSerializer.Deserialize<CorrectUsageBalanceItemByType>(stagingSummaryRecord.Data);
                    queueLoadedBatches.Enqueue(correctUsageBalanceItemsByType);
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
            Queueing.MemoryQueue<CorrectUsageBalanceItemByType> queueLoadedBatches, AsyncActivityStatus loadBatchStatus, string currentStageName, CorrectUsageBalanceItemByType preparedCorrectUsageBalanceItems)
        {
            CorrectUsageBalanceItemByAccount correctUsageBalanceItemsByAccountId;
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
                            var correctUsageBalanceItemsByType = item as CorrectUsageBalanceItemByType;
                            if (correctUsageBalanceItemsByType == null)
                                throw new Exception(String.Format("item should be of type 'CorrectUsageBalanceItemByType' and not of type '{0}'", item.GetType()));

                            foreach (var usageBalanceItemByType in correctUsageBalanceItemsByType)
                            {
                                var accountBalanceType = usageBalanceItemByType.Key;
                                var correctUsageBalanceItemByAccount = usageBalanceItemByType.Value;

                                foreach (var accountUsageBalanceItem in correctUsageBalanceItemByAccount)
                                {
                                    var accountId = accountUsageBalanceItem.Key;
                                    var usageBalanceItem = accountUsageBalanceItem.Value;

                                    if (!preparedCorrectUsageBalanceItems.TryGetValue(accountBalanceType, out correctUsageBalanceItemsByAccountId))
                                    {
                                        correctUsageBalanceItemsByAccountId = new CorrectUsageBalanceItemByAccount();
                                        correctUsageBalanceItemsByAccountId.Add(accountId, new CorrectUsageBalanceItem()
                                        {
                                            Value = usageBalanceItem.Value,
                                            AccountId = usageBalanceItem.AccountId,
                                            CurrencyId = usageBalanceItem.CurrencyId
                                        });
                                        preparedCorrectUsageBalanceItems.Add(accountBalanceType, correctUsageBalanceItemsByAccountId);
                                    }
                                    else
                                    {
                                        if (!correctUsageBalanceItemsByAccountId.TryGetValue(accountId, out correctUsageBalanceItem))
                                        {
                                            correctUsageBalanceItemsByAccountId.Add(accountId, new CorrectUsageBalanceItem()
                                            {
                                                Value = usageBalanceItem.Value,
                                                AccountId = usageBalanceItem.AccountId,
                                                CurrencyId = usageBalanceItem.CurrencyId
                                            });
                                        }
                                        else
                                            correctUsageBalanceItem.Value += usageBalanceItem.Value;
                                    }
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

        private static void InsertUsageBalanceItems(Action<LogEntryType, string> writeTrackingMessage, string currentStageName, DateTime batchStart, CorrectUsageBalanceItemByType preparedCorrectUsageBalanceItems)
        {
            writeTrackingMessage(Vanrise.Entities.LogEntryType.Information, string.Format("Start Inserting Batches for Stage {0}", currentStageName));

            CorrectUsageBalancePayload correctUsageBalancePayload;
            UsageBalanceManager usageBalanceManager = new UsageBalanceManager();

            decimal maxCount = 10000;

            foreach (var usageBalanceItemByType in preparedCorrectUsageBalanceItems)
            {
                int counter = 1;

                AccountBalanceType accountBalanceType = usageBalanceItemByType.Key;
                var correctUsageBalanceItemByAccount = usageBalanceItemByType.Value;

                int numberOfBatches = (int)(Math.Ceiling(correctUsageBalanceItemByAccount.Count / maxCount));
                Guid correctionProcessId = usageBalanceManager.InitializeUpdateUsageBalance();

                correctUsageBalancePayload = new CorrectUsageBalancePayload()
                {
                    CorrectUsageBalanceItems = new List<CorrectUsageBalanceItem>(),
                    PeriodDate = batchStart,
                    CorrectionProcessId = correctionProcessId,
                    IsLastBatch = counter == numberOfBatches,
                    TransactionTypeId = accountBalanceType.TransactionTypeId
                };

                foreach (var itm in correctUsageBalanceItemByAccount.Values)
                {
                    correctUsageBalancePayload.CorrectUsageBalanceItems.Add(itm);
                    if (correctUsageBalancePayload.CorrectUsageBalanceItems.Count == maxCount)
                    {
                        usageBalanceManager.CorrectUsageBalance(accountBalanceType.AccountTypeId, correctUsageBalancePayload);
                        counter++;
                        correctUsageBalancePayload = new CorrectUsageBalancePayload()
                        {
                            CorrectUsageBalanceItems = new List<CorrectUsageBalanceItem>(),
                            PeriodDate = batchStart,
                            CorrectionProcessId = correctionProcessId,
                            IsLastBatch = counter == numberOfBatches,
                            TransactionTypeId = accountBalanceType.TransactionTypeId
                        };
                    }
                }

                if (correctUsageBalancePayload.CorrectUsageBalanceItems.Count > 0)
                    usageBalanceManager.CorrectUsageBalance(accountBalanceType.AccountTypeId, correctUsageBalancePayload);
            }

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
                        dataManager.DeleteStagingSummaryRecords(context.ProcessInstanceId, context.CurrentStageName, stagingSummaryInfo.BatchStart);
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

        #region Private Classes


        private class CorrectUsageBalanceItemByAccount : Dictionary<String, CorrectUsageBalanceItem>
        {
            static CorrectUsageBalanceItemByAccount()
            {
                Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(CorrectUsageBalanceItemByAccount));
            }
        }
        private class CorrectUsageBalanceItemByType : Dictionary<AccountBalanceType, CorrectUsageBalanceItemByAccount>
        {
            static CorrectUsageBalanceItemByType()
            {
                new CorrectUsageBalanceItemByAccount();
            }
        }
        private class CorrectUsageBalanceItemByTypeByBatchStart : Dictionary<DateTime, CorrectUsageBalanceItemByType>
        {

        }

        #endregion
    }
}
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
        #region Properties

        AccountTypeManager _accountTypeManager = new AccountTypeManager();
        UsageBalanceManager _usageBalanceManager = new UsageBalanceManager();

        #endregion

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

            UsageBalanceManager usageBalanceManager = new UsageBalanceManager();
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
                var convertToAccountBalanceContext = new ConvertToBalanceUpdateContext(submitBalanceUpdate) { Record = record, DataRecordTypeId = recordTypeId };
                ConvertToBalanceUpdate(convertToAccountBalanceContext);
            }

            foreach (var updateUsageBalanceItem in updateUsageBalanceItemsByType)
            {
                usageBalanceManager.UpdateUsageBalance(updateUsageBalanceItem.Key.AccountTypeId, updateUsageBalanceItem.Value);
            }
        }

        #endregion

        #region IReprocessStageActivator

        public object InitializeStage(IReprocessStageActivatorInitializingContext context)
        {
            return null;
        }

        public void ExecuteStage(Reprocess.Entities.IReprocessStageActivatorExecutionContext context)
        {
            var queueItemType = context.QueueExecutionFlowStage.QueueItemType as DataRecordBatchQueueItemType;
            if (queueItemType == null)
                throw new Exception("current stage QueueItemType is not of type DataRecordBatchQueueItemType");
            var recordTypeId = queueItemType.DataRecordTypeId;

            CurrencyExchangeRateManager currencyExchangeRateManager = new CurrencyExchangeRateManager();
            int systemCurrencyId = new ConfigManager().GetSystemCurrencyId();

            var correctUsageBalanceItemsByTypeByPeriodRange = new CorrectUsageBalanceItemByTypeByPeriodRange();
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
                            PeriodRange periodRange = GetPeriodRange(balanceUpdatePayload.AccountTypeId, balanceUpdatePayload.EffectiveOn);

                            AccountBalanceType accountBalanceType = new AccountBalanceType() { AccountTypeId = balanceUpdatePayload.AccountTypeId, TransactionTypeId = balanceUpdatePayload.TransactionTypeId };

                            decimal convertedAmount = currencyExchangeRateManager.ConvertValueToCurrency(balanceUpdatePayload.Amount, balanceUpdatePayload.CurrencyId, systemCurrencyId, balanceUpdatePayload.EffectiveOn);

                            correctUsageBalanceItemsByType = correctUsageBalanceItemsByTypeByPeriodRange.GetOrCreateItem(periodRange);
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
                                {
                                    correctUsageBalanceItem.Value += convertedAmount;
                                }
                            }
                        };

                        foreach (var record in genericDataRecordBatch.Records)
                        {
                            var convertToBalanceUpdateContext = new ConvertToBalanceUpdateContext(submitBalanceUpdate) { Record = record, DataRecordTypeId = recordTypeId };
                            ConvertToBalanceUpdate(convertToBalanceUpdateContext);
                        }
                    });
                } while (!context.ShouldStop() && hasItem);
            });

            IStagingSummaryRecordDataManager stagingSummaryRecordDataManager = GenericDataDataManagerFactory.GetDataManager<IStagingSummaryRecordDataManager>();
            List<Task> runningTasks = new List<Task>();

            CorrectUsageBalanceItemByTypeByPeriodRange finalizedCorrectUsageBalanceItemByTypeByPeriodRange = new CorrectUsageBalanceItemByTypeByPeriodRange();

            if (correctUsageBalanceItemsByTypeByPeriodRange != null && correctUsageBalanceItemsByTypeByPeriodRange.Count > 0)
            {
                object dbApplyStream = stagingSummaryRecordDataManager.InitialiazeStreamForDBApply();

                //Store UsageBalance Batches for finalization step
                foreach (var correctUsageBalanceBatchEntry in correctUsageBalanceItemsByTypeByPeriodRange)
                {
                    var periodRange = correctUsageBalanceBatchEntry.Key;
                    var currentBatchStart = periodRange.PeriodStart;
                    var currentBatchEnd = periodRange.PeriodEnd;

                    CorrectUsageBalanceItemByType correctUsageBalanceItemByType = correctUsageBalanceBatchEntry.Value;

                    StagingSummaryRecord stagingSummaryRecord = new StagingSummaryRecord()
                    {
                        ProcessInstanceId = context.ProcessInstanceId,
                        StageName = context.CurrentStageName,
                        BatchStart = currentBatchStart,
                        BatchEnd = currentBatchEnd,
                        Payload = Vanrise.Common.Serializer.Serialize(correctUsageBalanceItemByType.Keys.ToList())
                    };

                    if (context.To >= currentBatchEnd && context.From <= currentBatchStart)
                    {
                        finalizedCorrectUsageBalanceItemByTypeByPeriodRange.Add(periodRange, correctUsageBalanceBatchEntry.Value);
                        stagingSummaryRecord.AlreadyFinalised = true;
                    }
                    else
                    {
                        byte[] serializedBatch = ProtoBufSerializer.Serialize(correctUsageBalanceBatchEntry.Value);
                        stagingSummaryRecord.AlreadyFinalised = false;
                        stagingSummaryRecord.Data = serializedBatch;
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

            //Finalized CorrectUsageBalanceItems
            if (finalizedCorrectUsageBalanceItemByTypeByPeriodRange.Count > 0)
            {
                foreach (var correctUsageBalanceItemByType in finalizedCorrectUsageBalanceItemByTypeByPeriodRange)
                    InsertUsageBalanceItems(context.WriteTrackingMessage, context.CurrentStageName, correctUsageBalanceItemByType.Key.PeriodStart, correctUsageBalanceItemByType.Value);
            }

            if (runningTasks.Count > 0)
                Task.WaitAll(runningTasks.ToArray());
        }

        public void FinalizeStage(Reprocess.Entities.IReprocessStageActivatorFinalizingContext context)
        {
            BalanceStageBatchRecord balanceStageBatchRecord = context.BatchRecord as BalanceStageBatchRecord;
            if (balanceStageBatchRecord == null)
                throw new Exception(String.Format("context.BatchRecord should be of type 'StageBatchRecord' and not of type '{0}'", context.BatchRecord.GetType()));

            if (balanceStageBatchRecord.IsEmptyBatch)
            {
                foreach (var accountBalanceType in balanceStageBatchRecord.AccountBalanceTypes)
                    GenerateEmptyBatch(accountBalanceType, balanceStageBatchRecord.BatchStart, balanceStageBatchRecord.BatchEnd);
            }
            else
            {
                Queueing.MemoryQueue<CorrectUsageBalanceItemByType> queueLoadedBatches = new Queueing.MemoryQueue<CorrectUsageBalanceItemByType>();
                AsyncActivityStatus loadBatchStatus = new AsyncActivityStatus();
                Task loadDataTask = new Task(() =>
                {
                    StartLoadingBatches(context, queueLoadedBatches, loadBatchStatus, balanceStageBatchRecord);
                });
                loadDataTask.Start();

                CorrectUsageBalanceItemByType preparedCorrectUsageBalanceItems = new CorrectUsageBalanceItemByType();
                PrepareUsageBalanceItems(context.WriteTrackingMessage, context.DoWhilePreviousRunning, queueLoadedBatches, loadBatchStatus, context.CurrentStageName, preparedCorrectUsageBalanceItems);
                loadDataTask.Wait();
                InsertUsageBalanceItems(context.WriteTrackingMessage, context.CurrentStageName, balanceStageBatchRecord.BatchStart, preparedCorrectUsageBalanceItems);
            }
        }

        public int? GetStorageRowCount(IReprocessStageActivatorGetStorageRowCountContext context)
        {
            return null;
        }

        public void CommitChanges(IReprocessStageActivatorCommitChangesContext context)
        {
        }

        public void DropStorage(Reprocess.Entities.IReprocessStageActivatorDropStorageContext context)
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
            Dictionary<BalanceStageBatchRecordKey, BalanceStageBatchRecord> results = new Dictionary<BalanceStageBatchRecordKey, BalanceStageBatchRecord>();

            IStagingSummaryRecordDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IStagingSummaryRecordDataManager>();
            List<StagingSummaryInfo> stagingSummaryInfo = dataManager.GetStagingSummaryInfo(context.ProcessInstanceId, context.CurrentStageName);

            List<AccountBalanceType> accountBalanceTypes = this.GetAccountBalanceTypeCombinations(new GetAccountBalanceTypeCombinationsContext());
            Dictionary<AccountBalanceType, List<BatchRecordData>> batchRecordDataByType = accountBalanceTypes.ToDictionary(itm => itm, itm => new List<BatchRecordData>());

            if (stagingSummaryInfo != null && stagingSummaryInfo.Count > 0)
            {
                List<AccountBalanceSummaryInfo> accountBalanceSummaryInfoList = this.BuildAccountBalanceSummaryInfoList(stagingSummaryInfo);

                foreach (var accountBalanceSummaryInfo in accountBalanceSummaryInfoList.OrderBy(itm => itm.BatchStart))
                {
                    foreach (var accountBalanceType in accountBalanceSummaryInfo.AccountBalanceTypes)
                    {
                        List<BatchRecordData> batchRecordData = batchRecordDataByType.GetOrCreateItem(accountBalanceType);
                        batchRecordData.Add(new BatchRecordData()
                        {
                            BatchStart = accountBalanceSummaryInfo.BatchStart,
                            BatchEnd = accountBalanceSummaryInfo.BatchEnd,
                            AlreadyFinalised = accountBalanceSummaryInfo.AlreadyFinalised
                        });
                    }
                }
            }

            foreach (var kvp in batchRecordDataByType)
            {
                var currentAccountBalanceType = kvp.Key;
                var currentBatchRecordData = kvp.Value;

                if (currentBatchRecordData == null || currentBatchRecordData.Count == 0)
                {
                    AddToStageBatchRecordDict(results, context.StartDate, context.EndDate, true, currentAccountBalanceType);
                }
                else
                {
                    DateTime current = context.StartDate;
                    foreach (var batchRecordDataItem in currentBatchRecordData.OrderBy(itm => itm.BatchStart))
                    {
                        if (current < batchRecordDataItem.BatchStart)
                            AddToStageBatchRecordDict(results, current, batchRecordDataItem.BatchStart, true, currentAccountBalanceType);

                        current = batchRecordDataItem.BatchEnd;
                        if (batchRecordDataItem.AlreadyFinalised)
                        {
                            dataManager.DeleteStagingSummaryRecords(context.ProcessInstanceId, context.CurrentStageName, batchRecordDataItem.BatchStart, batchRecordDataItem.BatchEnd);
                        }
                        else
                        {
                            AddToStageBatchRecordDict(results, batchRecordDataItem.BatchStart, batchRecordDataItem.BatchEnd, false, currentAccountBalanceType);
                        }
                    }

                    if (current < context.EndDate)
                        AddToStageBatchRecordDict(results, current, context.EndDate, true, currentAccountBalanceType);
                }
            }

            return results.Values.Cast<BatchRecord>().ToList();
        }

        #endregion

        #region Private/Protected Methods

        private List<AccountBalanceSummaryInfo> BuildAccountBalanceSummaryInfoList(List<StagingSummaryInfo> stagingSummaryInfo)
        {
            return stagingSummaryInfo.GroupBy(itm => new { itm.BatchStart, itm.BatchEnd, itm.AlreadyFinalised })
                                     .Select(itm => new AccountBalanceSummaryInfo()
                                     {
                                         BatchStart = itm.Key.BatchStart,
                                         BatchEnd = itm.Key.BatchEnd,
                                         AlreadyFinalised = itm.Key.AlreadyFinalised,
                                         AccountBalanceTypes = itm.SelectMany(x => Vanrise.Common.Serializer.Deserialize<List<AccountBalanceType>>(x.Payload)).ToHashSet()
                                     }).ToList();
        }

        private void AddToStageBatchRecordDict(Dictionary<BalanceStageBatchRecordKey, BalanceStageBatchRecord> results, DateTime batchStart, DateTime BatchEnd,
            bool isEmptyBatch, AccountBalanceType accountBalanceType)
        {
            BalanceStageBatchRecordKey balanceStageBatchRecordKey = new BalanceStageBatchRecordKey() { BatchStart = batchStart, BatchEnd = BatchEnd, IsEmptyBatch = isEmptyBatch };

            BalanceStageBatchRecord balanceStageBatchRecord;
            if (!results.TryGetValue(balanceStageBatchRecordKey, out balanceStageBatchRecord))
            {
                balanceStageBatchRecord = this.BuildUpdateAccountBalanceStageBatchRecord(batchStart, BatchEnd, isEmptyBatch, accountBalanceType);
                results.Add(balanceStageBatchRecordKey, balanceStageBatchRecord);
            }
            else
            {
                balanceStageBatchRecord.AccountBalanceTypes.Add(accountBalanceType);
            }
        }

        private BalanceStageBatchRecord BuildUpdateAccountBalanceStageBatchRecord(DateTime batchStart, DateTime BatchEnd, bool isEmptyBatch, AccountBalanceType accountBalanceType)
        {
            return new BalanceStageBatchRecord
            {
                BatchStart = batchStart,
                BatchEnd = BatchEnd,
                IsEmptyBatch = isEmptyBatch,
                AccountBalanceTypes = new HashSet<AccountBalanceType>() { accountBalanceType }
            };
        }

        private PeriodRange GetPeriodRange(Guid accountTypeId, DateTime usageDateTime)
        {
            AccountUsagePeriodSettings accountUsagePeriodSettings = _accountTypeManager.GetAccountUsagePeriodSettings(accountTypeId);
            accountUsagePeriodSettings.ThrowIfNull("accountType.Settings.accountUsagePeriodSettings", accountTypeId);

            AccountUsagePeriodEvaluationContext context = new AccountUsagePeriodEvaluationContext() { UsageTime = usageDateTime };
            accountUsagePeriodSettings.EvaluatePeriod(context);

            return new PeriodRange() { PeriodStart = context.PeriodStart, PeriodEnd = context.PeriodEnd };
        }

        private void GenerateEmptyBatch(AccountBalanceType accountBalanceType, DateTime batchStart, DateTime batchEnd)
        {
            do
            {
                Guid correctionProcessId = _usageBalanceManager.InitializeUpdateUsageBalance();

                CorrectUsageBalancePayload correctUsageBalancePayload = new CorrectUsageBalancePayload()
                {
                    CorrectUsageBalanceItems = null,
                    PeriodDate = batchStart,
                    CorrectionProcessId = correctionProcessId,
                    IsLastBatch = true,
                    TransactionTypeId = accountBalanceType.TransactionTypeId
                };
                _usageBalanceManager.CorrectUsageBalance(accountBalanceType.AccountTypeId, correctUsageBalancePayload);

                PeriodRange periodRange = GetPeriodRange(accountBalanceType.AccountTypeId, batchStart);
                batchStart = periodRange.PeriodEnd;
            } while (batchStart < batchEnd);
        }

        private void StartLoadingBatches(Reprocess.Entities.IReprocessStageActivatorFinalizingContext context, Queueing.MemoryQueue<CorrectUsageBalanceItemByType> queueLoadedBatches,
            AsyncActivityStatus loadBatchStatus, BalanceStageBatchRecord balanceStageBatchRecord)
        {
            IStagingSummaryRecordDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IStagingSummaryRecordDataManager>();

            try
            {
                context.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, string.Format("Start Loading Batches for Stage {0}", context.CurrentStageName));
                dataManager.GetStagingSummaryRecords(context.ProcessInstanceId, context.CurrentStageName, balanceStageBatchRecord.BatchStart, balanceStageBatchRecord.BatchEnd, (stagingSummaryRecord) =>
                {
                    var correctUsageBalanceItemsByType = ProtoBufSerializer.Deserialize<CorrectUsageBalanceItemByType>(stagingSummaryRecord.Data);
                    queueLoadedBatches.Enqueue(correctUsageBalanceItemsByType);
                });
            }
            finally
            {
                dataManager.DeleteStagingSummaryRecords(context.ProcessInstanceId, context.CurrentStageName, balanceStageBatchRecord.BatchStart, balanceStageBatchRecord.BatchEnd);
                loadBatchStatus.IsComplete = true;
                context.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, string.Format("Finish Loading Batches for Stage {0}", context.CurrentStageName));
            }
        }

        private void PrepareUsageBalanceItems(Action<LogEntryType, string> writeTrackingMessage, Action<AsyncActivityStatus, Action> doWhilePreviousRunning,
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
                                        {
                                            correctUsageBalanceItem.Value += usageBalanceItem.Value;
                                        }
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

        private void InsertUsageBalanceItems(Action<LogEntryType, string> writeTrackingMessage, string currentStageName, DateTime batchStart, CorrectUsageBalanceItemByType preparedCorrectUsageBalanceItems)
        {
            writeTrackingMessage(Vanrise.Entities.LogEntryType.Information, string.Format("Start Inserting Batches for Stage {0}", currentStageName));
            CorrectUsageBalancePayload correctUsageBalancePayload;

            decimal maxCount = 10000;

            foreach (var usageBalanceItemByType in preparedCorrectUsageBalanceItems)
            {
                AccountBalanceType accountBalanceType = usageBalanceItemByType.Key;
                CorrectUsageBalanceItemByAccount correctUsageBalanceItemByAccount = usageBalanceItemByType.Value;

                Guid correctionProcessId = _usageBalanceManager.InitializeUpdateUsageBalance();
                int numberOfBatches = (int)(Math.Ceiling(correctUsageBalanceItemByAccount.Count / maxCount));

                int counter = 1;

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
                        _usageBalanceManager.CorrectUsageBalance(accountBalanceType.AccountTypeId, correctUsageBalancePayload);
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
                    _usageBalanceManager.CorrectUsageBalance(accountBalanceType.AccountTypeId, correctUsageBalancePayload);
            }

            writeTrackingMessage(Vanrise.Entities.LogEntryType.Information, string.Format("Finish Inserting Batches for Stage {0}", currentStageName));
        }

        protected List<AccountBalanceType> GetAccountBalanceTypeCombinations(UpdateAccountBalanceSettings updateAccountBalanceSettings)
        {
            updateAccountBalanceSettings.ThrowIfNull("updateAccountBalanceSettings");
            updateAccountBalanceSettings.UpdateAccountBalanceTypeCombinations.ThrowIfNull("updateAccountBalanceSettings.UpdateAccountBalanceTypeCombinations");

            List<AccountBalanceType> accountBalanceTypeCombinations = new List<AccountBalanceType>();

            foreach (var combination in updateAccountBalanceSettings.UpdateAccountBalanceTypeCombinations)
            {
                foreach (var transactionTypeId in combination.TransactionTypeIds)
                {
                    accountBalanceTypeCombinations.Add(new AccountBalanceType() { AccountTypeId = combination.BalanceAccountTypeId, TransactionTypeId = transactionTypeId });
                }
            }
            return accountBalanceTypeCombinations;
        }

        #endregion

        #region Private Classes

        private struct PeriodRange
        {
            public DateTime PeriodStart { get; set; }

            public DateTime PeriodEnd { get; set; }
        }
        private class AccountBalanceSummaryInfo
        {
            public DateTime BatchStart { get; set; }
            public DateTime BatchEnd { get; set; }
            public bool AlreadyFinalised { get; set; }
            public HashSet<AccountBalanceType> AccountBalanceTypes { get; set; }
        }
        private struct BatchRecordData
        {
            public DateTime BatchStart { get; set; }
            public DateTime BatchEnd { get; set; }
            public bool AlreadyFinalised { get; set; }
        }
        private struct BalanceStageBatchRecordKey
        {
            public DateTime BatchStart { get; set; }
            public DateTime BatchEnd { get; set; }
            public bool IsEmptyBatch { get; set; }
        }
        private class BalanceStageBatchRecord : BatchRecord
        {
            public DateTime BatchStart { get; set; }
            public DateTime BatchEnd { get; set; }
            public bool IsEmptyBatch { get; set; }
            public HashSet<AccountBalanceType> AccountBalanceTypes { get; set; }

            public override string GetBatchTitle()
            {
                return string.Format("Batch Start: {0}, Batch End : {1}", BatchStart.ToString("yyyy-MM-dd HH:mm:ss"), BatchEnd.ToString("yyyy-MM-dd HH:mm:ss"));
            }
        }

        private class CorrectUsageBalanceItemByAccount : Dictionary<String, CorrectUsageBalanceItem>
        {
            static CorrectUsageBalanceItemByAccount()
            {
                var dummyCorrectUsageBalanceItem = new CorrectUsageBalanceItem();
                Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(CorrectUsageBalanceItemByAccount));
            }
        }
        private class CorrectUsageBalanceItemByType : Dictionary<AccountBalanceType, CorrectUsageBalanceItemByAccount>
        {
            static CorrectUsageBalanceItemByType()
            {
                var dummyCorrectUsageBalanceItemByAccount = new CorrectUsageBalanceItemByAccount();
                Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(CorrectUsageBalanceItemByType));
            }
        }
        private class CorrectUsageBalanceItemByTypeByPeriodRange : Dictionary<PeriodRange, CorrectUsageBalanceItemByType>
        {
            static CorrectUsageBalanceItemByTypeByPeriodRange()
            {
                var dummyCorrectUsageBalanceItemByType = new CorrectUsageBalanceItemByType();
                Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(CorrectUsageBalanceItemByTypeByPeriodRange));
            }
        }

        #endregion

        #region Abstract Methods

        protected abstract void ConvertToBalanceUpdate(IConvertToBalanceUpdateContext context);

        protected abstract List<AccountBalanceType> GetAccountBalanceTypeCombinations(IGetAccountBalanceTypeCombinationsContext context);

        #endregion
    }
}
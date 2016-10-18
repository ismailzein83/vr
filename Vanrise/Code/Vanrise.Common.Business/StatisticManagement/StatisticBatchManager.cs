using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Common.Data;
//using Vanrise.Common.Data.StatisticManagement;
using Vanrise.Entities;
//using Vanrise.Entities.StatisticManagement;
using Vanrise.Runtime;

namespace Vanrise.Common.Business.StatisticManagement
{
    //public abstract class StatisticBatchManager<T, Q, R>
    //    where T : IRawItem
    //    where Q : IStatisticItem
    //    where R : IStatisticBatch<Q>
    //{
    //    #region ctor/Local Variables

    //    IStatisticBatchDataManager _dataManager = CommonDataManagerFactory.GetDataManager<IStatisticBatchDataManager>();
    //    RunningProcessManager _runningProcessManager = new RunningProcessManager();
    //    static int _maxLockRetryCount;
    //    static TimeSpan _lockRetryInterval;
    //    static StatisticBatchManager()
    //    {
    //        if (!int.TryParse(ConfigurationManager.AppSettings["StatisticManagement_MaxLockRetryCount"], out _maxLockRetryCount))
    //            _maxLockRetryCount = 30;
    //        if (!TimeSpan.TryParse(ConfigurationManager.AppSettings["StatisticManagement_LockRetryInterval"], out _lockRetryInterval))
    //            _lockRetryInterval = new TimeSpan(0, 0, 1);
    //    }

    //    #endregion

    //    #region Public Methods

    //    public IEnumerable<R> ConvertRawItemsToBatches(IEnumerable<T> items)
    //    {
    //        Dictionary<DateTime, R> batches = new Dictionary<DateTime, R>();
    //        foreach (var item in items)
    //        {
    //            DateTime batchStart;
    //            DateTime batchEnd;
    //            GetRawItemBatchTimeRange(item, out batchStart, out batchEnd);

    //            R batch;
    //            if (!batches.TryGetValue(batchStart, out batch))
    //            {
    //                batch = Activator.CreateInstance<R>();
    //                batch.BatchStart = batchStart;
    //                batch.BatchEnd = batchEnd;
    //                batch.ItemsByKey = new Dictionary<string, Q>();
    //                batches.Add(batchStart, batch);
    //            }

    //            string itemKey = GetStatisticItemKey(item);
    //            Q statisticItem;
    //            if (!batch.ItemsByKey.TryGetValue(itemKey, out statisticItem))
    //            {
    //                statisticItem = CreateStatisticItemFromRawItem(item);
    //                batch.ItemsByKey.Add(itemKey, statisticItem);
    //            }
    //            UpdateStatisticItemFromRawItem(statisticItem, item);
    //        }
    //        return batches.Values;
    //    }

    //    public void UpdateStatisticBatch(R batch)
    //    {
    //        StatisticBatchLockOutput lockOutput = LockBatch(batch.BatchStart);
    //        Dictionary<string, long> existingItemsIdsByKey = lockOutput.BatchInfo != null ? lockOutput.BatchInfo.ItemsIdsByKey : null;
    //        switch (lockOutput.Result)
    //        {
    //            case StatisticBatchLockResult.NewBatch:
    //                Dictionary<string, long> itemsIdsByKey;
    //                GenerateStatisticItemsIds(batch.ItemsByKey.Values, out itemsIdsByKey);
    //                UpdateBatchInfo(batch.BatchStart, itemsIdsByKey);
    //                InsertStatisticItemsToDB(batch.ItemsByKey.Values);
    //                break;
    //            case StatisticBatchLockResult.ExistingBatch:

    //                break;
    //            case StatisticBatchLockResult.BatchInfoCorrupted:
    //                existingItemsIdsByKey = GetStatisticItemsIdsByKeyFromDB(batch);
    //                break;
    //        }
    //        if (lockOutput.Result == StatisticBatchLockResult.ExistingBatch || lockOutput.Result == StatisticBatchLockResult.BatchInfoCorrupted)
    //        {
    //            List<Q> itemsToAdd = new List<Q>();
    //            List<Q> itemsToUpdate = new List<Q>();
    //            foreach (var itemEntry in batch.ItemsByKey)
    //            {
    //                long itemId;
    //                if (existingItemsIdsByKey.TryGetValue(itemEntry.Key, out itemId))
    //                {
    //                    itemEntry.Value.StatisticItemId = itemId;
    //                    itemsToUpdate.Add(itemEntry.Value);
    //                }
    //                else
    //                    itemsToAdd.Add(itemEntry.Value);
    //            }
    //            Dictionary<string, long> itemsToAddIdsByKey;
    //            GenerateStatisticItemsIds(itemsToAdd, out itemsToAddIdsByKey);
    //            foreach (var itemIdAndKey in itemsToAddIdsByKey)
    //            {
    //                existingItemsIdsByKey.Add(itemIdAndKey.Key, itemIdAndKey.Value);
    //            }
    //            UpdateStatisticItemsInDB(itemsToUpdate);
    //            UpdateBatchInfo(batch.BatchStart, existingItemsIdsByKey);
    //            InsertStatisticItemsToDB(itemsToAdd);
    //        }
    //        UnlockBatch(batch.BatchStart);
    //    }

    //    #endregion

    //    #region abstract Methods

    //    protected abstract void InsertStatisticItemsToDB(IEnumerable<Q> items);

    //    protected abstract void UpdateStatisticItemsInDB(IEnumerable<Q> items);

    //    protected abstract Dictionary<string, long> GetStatisticItemsIdsByKeyFromDB(R batch);
        
    //    protected abstract void UpdateStatisticItemFromRawItem(Q statisticItem, T item);

    //    protected abstract Q CreateStatisticItemFromRawItem(T rawItem);

    //    protected abstract string GetStatisticItemKey(T rawItem);

    //    protected abstract string GetStatisticItemKey(Q statisticItem);

    //    protected abstract void GetRawItemBatchTimeRange(T rawItem, out DateTime batchStart, out DateTime batchEnd);

    //    #endregion

    //    #region Private Methods

    //    private void UnlockBatch(DateTime batchStart)
    //    {
    //        _dataManager.UnlockBatch(GetTypeId(), batchStart);
    //    }

    //    private void UpdateBatchInfo(DateTime batchStart, Dictionary<string, long> statisticItemsIdsByKey)
    //    {
    //        StatisticBatchInfo batchInfo = new StatisticBatchInfo
    //        {
    //             ItemsIdsByKey = statisticItemsIdsByKey
    //        };
    //        _dataManager.UpdateBatchInfo(GetTypeId(), batchStart, batchInfo);
    //    }

    //    private void GenerateStatisticItemsIds(IEnumerable<Q> statisticItems, out Dictionary<string, long> statisticItemsIdsByKey)
    //    {
    //        statisticItemsIdsByKey = new Dictionary<string, long>();
    //        long startingId;
    //        IDManager.Instance.ReserveIDRange(this.GetType(), statisticItems.Count(), out startingId);
    //        long currentId = startingId;
    //        foreach(var statisticItem in statisticItems)
    //        {
    //            statisticItem.StatisticItemId = currentId;
    //            statisticItemsIdsByKey.Add(GetStatisticItemKey(statisticItem), currentId);
    //            currentId++;
    //        }
    //    }

    //    private StatisticBatchLockOutput LockBatch(DateTime batchStart)
    //    {
    //        StatisticBatchInfo batchInfo;
    //        bool isInfoCorrupted;
    //        int typeId = GetTypeId();
    //        int currentRuntimeProcessId = RunningProcessManager.CurrentProcess.ProcessId;
    //        IEnumerable<int> runningRuntimeProcessesIds = _runningProcessManager.GetCachedRunningProcesses(new TimeSpan(0, 0, 15)).Select(itm => itm.ProcessId);
    //        bool isLocked;
    //        int retryCount = 0;
    //        do
    //        {
    //            isLocked = _dataManager.TryLock(typeId, batchStart, currentRuntimeProcessId, runningRuntimeProcessesIds, out batchInfo, out isInfoCorrupted);
    //            if(isLocked)
    //            {
    //                StatisticBatchLockOutput output = new StatisticBatchLockOutput
    //                {
    //                    BatchInfo = batchInfo
    //                };
    //                if (batchInfo == null)
    //                    output.Result = StatisticBatchLockResult.NewBatch;
    //                else if (isInfoCorrupted)
    //                    output.Result = StatisticBatchLockResult.BatchInfoCorrupted;
    //                else
    //                    output.Result = StatisticBatchLockResult.ExistingBatch;
    //                return output;
    //            }
    //            else
    //            {
    //                if(retryCount < _maxLockRetryCount)
    //                {
    //                    retryCount++;
    //                    Thread.Sleep(_lockRetryInterval);
    //                }
    //                else
    //                    throw new Exception(String.Format("Max retry time exceeded when trying to lock a Statistic Batch. Batch Type '{0}', Batch Start '{1}', Max Retry Count '{2}', Retry Interval '{3}'", this.GetType(), batchStart, _maxLockRetryCount, _lockRetryInterval));
    //            }
    //        }
    //        while (!isLocked);
    //        return null;
    //    }

    //    private int GetTypeId()
    //    {
    //        return TypeManager.Instance.GetTypeId(this.GetType());
    //    }

    //    #endregion
    //}
}

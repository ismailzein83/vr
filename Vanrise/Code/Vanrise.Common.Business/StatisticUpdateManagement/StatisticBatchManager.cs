using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public abstract class StatisticBatchManager<T, Q>
        where T : StatisticItem
        where Q : StatisticBatch<T>
    {
        IStatisticBatchDataManager _dataManager = CommonDataManagerFactory.GetDataManager<IStatisticBatchDataManager>();

        public void UpdateStatisticBatch(Q batch)
        {
            StatisticBatchLockOutput lockOutput = LockBatch(batch.BatchKey);
            Dictionary<string, long> existingItemsIdsByKey = lockOutput.BatchInfo.ItemsIdsByKey;
            switch (lockOutput.Result)
            {
                case StatisticBatchLockResult.NewBatch:
                    Dictionary<string, long> itemsIdsByKey;
                    GenerateItemsIds(batch.Items, out itemsIdsByKey);
                    UpdateBatchInfo(lockOutput.BatchId, itemsIdsByKey);
                    InsertItems(batch.Items);
                    break;
                case StatisticBatchLockResult.ExistingBatch:

                    break;
                case StatisticBatchLockResult.BatchInfoCorrupted:
                    existingItemsIdsByKey = GetItemsIdsByKeyFromStore(batch.BatchKey);
                    break;
            }
            if (lockOutput.Result == StatisticBatchLockResult.ExistingBatch || lockOutput.Result == StatisticBatchLockResult.BatchInfoCorrupted)
            {
                List<T> itemsToAdd = new List<T>();
                List<T> itemsToUpdate = new List<T>();
                foreach (var item in batch.Items)
                {
                    long itemId;
                    if (existingItemsIdsByKey.TryGetValue(item.ItemKey, out itemId))
                    {
                        item.StatisticItemId = itemId;
                        itemsToUpdate.Add(item);
                    }
                    else
                        itemsToAdd.Add(item);
                }
                Dictionary<string, long> itemsToAddIdsByKey;
                GenerateItemsIds(itemsToAdd, out itemsToAddIdsByKey);
                foreach (var itemIdAndKey in itemsToAddIdsByKey)
                {
                    existingItemsIdsByKey.Add(itemIdAndKey.Key, itemIdAndKey.Value);
                }
                UpdateItems(itemsToUpdate);
                UpdateBatchInfo(lockOutput.BatchId, existingItemsIdsByKey);
                InsertItems(itemsToAdd);
            }
            UnlockBatch(lockOutput.BatchId);
        }

        protected abstract void InsertItems(List<T> items);

        protected abstract void UpdateItems(List<T> items);

        protected abstract Dictionary<string, long> GetItemsIdsByKeyFromStore(DateTime batchKey);

        private void UnlockBatch(long batchId)
        {
            throw new NotImplementedException();
        }

        private void UpdateBatchInfo(long batchId, Dictionary<string, long> itemsIdsByKey)
        {
            throw new NotImplementedException();
        }

        private void GenerateItemsIds(List<T> items, out Dictionary<string, long> itemsIdsByKey)
        {
            itemsIdsByKey = new Dictionary<string, long>();
            long startingId;
            IDManager.Instance.ReserveIDRange(this.GetType(), items.Count, out startingId);
            long currentId = startingId;
            foreach(var item in items)
            {
                item.StatisticItemId = currentId;
                itemsIdsByKey.Add(item.ItemKey, currentId);
                currentId++;
            }
        }

        private StatisticBatchLockOutput LockBatch(DateTime batchKey)
        {
            StatisticBatchInfo batchInfo;
            bool isInfoCorrupted; throw new NotImplementedException();
            //while(!_dataManager.TryLock())
        }
    }
}

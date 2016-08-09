using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Entities;
using Vanrise.Entities.SummaryTransformation;
using Vanrise.Runtime;

namespace Vanrise.Common.Business.SummaryTransformation
{

    public abstract class SummaryTransformationManager<T, Q, R> : IVanriseType
        where Q : class, ISummaryItem
        where R : class, ISummaryBatch<Q>
    {
        #region ctor/Local Variables

        ConcurrentDictionary<DateTime, Dictionary<string, SummaryItemInProcess<Q>>> _existingSummaryBatches = new ConcurrentDictionary<DateTime, Dictionary<string, SummaryItemInProcess<Q>>>();
        ISummaryTransformationDataManager _dataManager = CommonDataManagerFactory.GetDataManager<ISummaryTransformationDataManager>();

        #endregion

        #region Public Methods

        public IEnumerable<R> ConvertRawItemsToBatches(IEnumerable<T> items, Func<R> createSummaryBatchObj)
        {
            Dictionary<DateTime, SummaryBatchInProcess<Q>> batches = new Dictionary<DateTime, SummaryBatchInProcess<Q>>();
            
            foreach (var item in items)
            {
                DateTime batchStart;
                GetRawItemBatchTimeRange(item, out batchStart);

                SummaryBatchInProcess<Q> batch;
                if (!batches.TryGetValue(batchStart, out batch))
                {
                    batch = new SummaryBatchInProcess<Q>();
                    batch.BatchStart = batchStart;
                    batch.ItemsBySummaryKey = new Dictionary<string, Q>();
                    batches.Add(batchStart, batch);
                }

                Q summaryItem = CreateSummaryItemFromRawItem(item);
                summaryItem.BatchStart = batch.BatchStart;
                string itemKey = GetSummaryItemKey(summaryItem);
                Q existingSummaryItem;
                if (batch.ItemsBySummaryKey.TryGetValue(itemKey, out existingSummaryItem))
                    UpdateSummaryItemFromSummaryItem(existingSummaryItem, summaryItem);
                else
                    batch.ItemsBySummaryKey.Add(itemKey, summaryItem);
            }
            return batches.Values.Select(
                itm =>
                {
                    var summaryBatch = createSummaryBatchObj();
                    summaryBatch.BatchStart = itm.BatchStart;
                    summaryBatch.Items = itm.ItemsBySummaryKey.Values;
                    return summaryBatch;
                });
        }

        public void UpdateNewBatches(DateTime batchStart, IEnumerable<R> newBatches)
        {
            Dictionary<string, SummaryItemInProcess<Q>> existingSummaryBatch;
            if (!_existingSummaryBatches.TryGetValue(batchStart, out existingSummaryBatch))
                throw new Exception(String.Format("Summary Transformation should be locked before calling the UpdateNewBatches. Batch Start: '{0}'", batchStart));

            try
            {
                foreach (var newBatch in newBatches)
                {
                    if (newBatch.BatchStart != batchStart)
                        throw new Exception(String.Format("newBatch.BatchStart '{0}' is not same as batchStart '{1}'.", newBatch.BatchStart, batchStart));
                    UpdateExistingFromNew(existingSummaryBatch, newBatch);
                }

                SaveSummaryBatchToDB(existingSummaryBatch.Values);

            }
            catch
            {
                Unlock(batchStart);
                throw;
            }
        }


        public bool TryLock(DateTime batchStart)
        {
            int currentRuntimeProcessId = RunningProcessManager.CurrentProcess.ProcessId;
            IEnumerable<int> runningRuntimeProcessesIds = (new RunningProcessManager()).GetCachedRunningProcesses().Select(itm => itm.ProcessId);
            if (_dataManager.TryLock(GetTypeId(), batchStart, currentRuntimeProcessId, runningRuntimeProcessesIds))
            {
                var items = GetItemsFromDB(batchStart);
                Dictionary<string, SummaryItemInProcess<Q>> itemsByKey = new Dictionary<string, SummaryItemInProcess<Q>>();
                if(items != null)
                {
                    foreach(var itm in items)
                    {
                        string itemKey = GetSummaryItemKey(itm);
                        if (!itemsByKey.ContainsKey(itemKey))
                            itemsByKey.Add(itemKey, new SummaryItemInProcess<Q> { SummaryItem = itm });
                    }
                }
                _existingSummaryBatches.TryAdd(batchStart, itemsByKey);
                return true;
            }
            else
                return false;
        }

        public void Unlock(DateTime batchStart)
        {
            _dataManager.UnLock(GetTypeId(), batchStart);
            Dictionary<string, SummaryItemInProcess<Q>> dummy;
            _existingSummaryBatches.TryRemove(batchStart, out dummy);
        }
        
        #endregion

        #region abstract Methods

        protected abstract void GetRawItemBatchTimeRange(T rawItem, out DateTime batchStart);

        protected abstract string GetSummaryItemKey(Q summaryItem);

        protected abstract Q CreateSummaryItemFromRawItem(T item);

        protected abstract void UpdateSummaryItemFromSummaryItem(Q existingItem, Q newItem);

        protected abstract void UpdateItemsInDB(List<Q> itemsToUpdate);

        protected abstract void InsertItemsToDB(List<Q> itemsToAdd);

        protected abstract IEnumerable<Q> GetItemsFromDB(DateTime batchStart);

        #endregion

        #region Private Methods

        private int GetTypeId()
        {
            return TypeManager.Instance.GetTypeId(this);
        }

        private void GenerateSummaryItemsIds(IEnumerable<Q> summaryItems)
        {
            long startingId;
            IDManager.Instance.ReserveIDRange(this, summaryItems.Count(), out startingId);
            long currentId = startingId;
            foreach (var summaryItem in summaryItems)
            {
                summaryItem.SummaryItemId = currentId;
                currentId++;
            }
        }

        public void UpdateExistingFromNew(Dictionary<string, SummaryItemInProcess<Q>> existingSummaryItemsByKey, R newSummaryBatch)
        {
            if (newSummaryBatch == null)
                throw new NullReferenceException("newSummaryBatch");
            if (newSummaryBatch.Items == null)
                throw new NullReferenceException("newSummaryBatch.Items");
            
            foreach (var newSummaryItem in newSummaryBatch.Items)
            {
                string summaryItemKey = GetSummaryItemKey(newSummaryItem);
                SummaryItemInProcess<Q> matchSummaryItem;
                if (!existingSummaryItemsByKey.TryGetValue(summaryItemKey, out matchSummaryItem))
                {
                    matchSummaryItem = new SummaryItemInProcess<Q> { SummaryItem = newSummaryItem};
                    existingSummaryItemsByKey.Add(summaryItemKey, matchSummaryItem);
                }
                else
                {
                    UpdateSummaryItemFromSummaryItem(matchSummaryItem.SummaryItem, newSummaryItem);
                }
                matchSummaryItem.ShouldUpdate = true;
            }
        }

        public void SaveSummaryBatchToDB(IEnumerable<SummaryItemInProcess<Q>> summaryBatch)
        {           
            List<Q> itemsToAdd = new List<Q>();
            List<Q> itemsToUpdate = new List<Q>();
            foreach (var itm in summaryBatch)
            {
                if (itm.ShouldUpdate)
                {
                    if (itm.SummaryItem.SummaryItemId > 0)
                        itemsToUpdate.Add(itm.SummaryItem);
                    else
                        itemsToAdd.Add(itm.SummaryItem);
                    itm.ShouldUpdate = false;
                }
            }
            if (itemsToAdd.Count > 0)
                GenerateSummaryItemsIds(itemsToAdd);
            if (itemsToAdd.Count > 0)
                InsertItemsToDB(itemsToAdd);
            if (itemsToUpdate.Count > 0)
                UpdateItemsInDB(itemsToUpdate);
        }

        #endregion

        #region Private Classes

        private class SummaryBatchInProcess<T> where T : ISummaryItem
        {
            public DateTime BatchStart { get; set; }

            public Dictionary<string, T> ItemsBySummaryKey { get; set; }
        }

        #endregion

        public virtual string UniqueTypeName
        {
            get { return this.GetType().AssemblyQualifiedName; }
        }
    }
    
    public class SummaryItemInProcess<T> where T : ISummaryItem
    {
        public T SummaryItem { get; set; }

        public bool ShouldUpdate { get; set; }
    }

}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common.Business.SummaryTransformation
{
    public abstract class SummaryTransformationManager<T, Q>
        where T : IRawItem
        where Q : ISummaryItem
    {
        #region ctor/Local Variables

        #endregion

        #region Public Methods

        public IEnumerable<SummaryBatch<Q>> ConvertRawItemsToBatches(IEnumerable<T> items)
        {
            Dictionary<DateTime, SummaryBatchInProcess<Q>> batches = new Dictionary<DateTime, SummaryBatchInProcess<Q>>();
            
            foreach (var item in items)
            {
                DateTime batchStart;
                DateTime batchEnd;
                GetRawItemBatchTimeRange(item, out batchStart, out batchEnd);

                SummaryBatchInProcess<Q> batch;
                if (!batches.TryGetValue(batchStart, out batch))
                {
                    batch = new SummaryBatchInProcess<Q>();
                    batch.BatchStart = batchStart;
                    batch.BatchEnd = batchEnd;
                    batch.ItemsBySummaryKey = new Dictionary<string, Q>();
                    batches.Add(batchStart, batch);
                }

                string itemKey = GetSummaryItemKey(item);
                Q summaryItem;
                if (!batch.ItemsBySummaryKey.TryGetValue(itemKey, out summaryItem))
                {
                    summaryItem = Activator.CreateInstance<Q>();
                    SetSummaryItemGroupingFields(summaryItem, item);
                    batch.ItemsBySummaryKey.Add(itemKey, summaryItem);
                }
                UpdateSummaryItemFromRawItem(summaryItem, item);
            }
            return batches.Values.Select(itm => new SummaryBatch<Q>
            {
                BatchStart = itm.BatchStart,
                BatchEnd = itm.BatchEnd,
                Items = itm.ItemsBySummaryKey.Values
            });
        }

        public void UpdateNewBatches(DateTime batchStart, Func<IEnumerable<SummaryBatch<Q>>> getNewBatches)
        {
            try
            {
                if (!TryLockBatch(batchStart))
                    return;

                SummaryBatch<Q> existingSummaryBatch = GetBatchFromDB(batchStart);
                var newBatches = getNewBatches();
                foreach (var newBatch in newBatches)
                {
                    if (newBatch.BatchStart != batchStart)
                        throw new Exception(String.Format("newBatch.BatchStart '{0}' is not same as batchStart '{1}'.", newBatch.BatchStart, batchStart));
                    if (existingSummaryBatch == null)
                        existingSummaryBatch = newBatch;
                    else
                        UpdateExistingFromNew(existingSummaryBatch, newBatch);
                }

                SaveSummaryBatchToDB(existingSummaryBatch);
            }
            finally
            {
                UnlockBatch(batchStart);
            }
        }
        
        #endregion

        #region abstract Methods

        protected abstract void GetRawItemBatchTimeRange(T rawItem, out DateTime batchStart, out DateTime batchEnd);

        protected abstract void SetSummaryItemGroupingFields(Q summaryItem, T item);

        protected abstract string GetSummaryItemKey(T rawItem);

        protected abstract string GetSummaryItemKey(Q summaryItem);

        protected abstract void UpdateSummaryItemFromRawItem(Q summaryItem, T item);

        protected abstract void UpdateSummaryItemFromSummaryItem(Q existingItem, Q newItem);

        protected abstract void UpdateItemsInDB(List<Q> itemsToUpdate);

        protected abstract void InsertItemsToDB(List<Q> itemsToAdd);

        protected abstract SummaryBatch<Q> GetBatchFromDB(DateTime batchStart);

        #endregion

        #region Private Methods

        private int GetTypeId()
        {
            return TypeManager.Instance.GetTypeId(this.GetType());
        }

        private void GenerateSummaryItemsIds(IEnumerable<Q> summaryItems)
        {
            long startingId;
            IDManager.Instance.ReserveIDRange(this.GetType(), summaryItems.Count(), out startingId);
            long currentId = startingId;
            foreach (var summaryItem in summaryItems)
            {
                summaryItem.SummaryItemId = currentId;
                currentId++;
            }
        }

        private bool TryLockBatch(DateTime batchStart)
        {
            //if locked successfully

            throw new NotImplementedException();
        }

        private void UpdateExistingFromNew(SummaryBatch<Q> existingSummaryBatch, SummaryBatch<Q> newSummaryBatch)
        {
            if (existingSummaryBatch == null)
                throw new NullReferenceException("existingSummaryBatch");
            if (existingSummaryBatch.Items == null)
                throw new NullReferenceException("existingSummaryBatch.Items");
            if (newSummaryBatch == null)
                throw new NullReferenceException("newSummaryBatch");
            if (newSummaryBatch.Items == null)
                throw new NullReferenceException("newSummaryBatch.Items");


            Dictionary<string, Q> existingSummaryItemsByKey = existingSummaryBatch.Items.ToDictionary((itm) => GetSummaryItemKey(itm), (itm) => itm);

            foreach (var newSummaryItem in newSummaryBatch.Items)
            {
                string summaryItemKey = GetSummaryItemKey(newSummaryItem);
                Q matchSummaryItem;
                if (!existingSummaryItemsByKey.TryGetValue(summaryItemKey, out matchSummaryItem))
                    existingSummaryItemsByKey.Add(summaryItemKey, newSummaryItem);
                else
                    UpdateSummaryItemFromSummaryItem(matchSummaryItem, newSummaryItem);
            }
        }

        private void SaveSummaryBatchToDB(SummaryBatch<Q> summaryBatch)
        {
            if (summaryBatch == null)
                throw new NullReferenceException("summaryBatch");
            if (summaryBatch.Items == null)
                throw new NullReferenceException("summaryBatch.Items");
            List<Q> itemsToAdd = new List<Q>();
            List<Q> itemsToUpdate = new List<Q>();
            foreach (var summaryItem in summaryBatch.Items)
            {
                if (summaryItem.SummaryItemId > 0)
                    itemsToUpdate.Add(summaryItem);
                else
                    itemsToAdd.Add(summaryItem);
            }
            if (itemsToAdd.Count > 0)
                GenerateSummaryItemsIds(itemsToAdd);
            if (itemsToAdd.Count > 0)
                InsertItemsToDB(itemsToAdd);
            if (itemsToUpdate.Count > 0)
                UpdateItemsInDB(itemsToUpdate);
        }

        private void UnlockBatch(DateTime batchStart)
        {
            throw new NotImplementedException();
            //unlock from DB

        }

        #endregion

        #region Private Classes

        private class SummaryBatchInProcess<T> where T : ISummaryItem
        {
            public DateTime BatchStart { get; set; }

            public DateTime BatchEnd { get; set; }

            public Dictionary<string, T> ItemsBySummaryKey { get; set; }
        }

        #endregion
    }

    public interface IRawItem
    {
    }

    public interface ISummaryItem
    {
        long SummaryItemId { get; set; }

    }

    public class SummaryBatch<T> where T : ISummaryItem
    {
        public DateTime BatchStart { get; set; }

        public DateTime BatchEnd { get; set; }

        public IEnumerable<T> Items { get; set; }
    }

    
    
}

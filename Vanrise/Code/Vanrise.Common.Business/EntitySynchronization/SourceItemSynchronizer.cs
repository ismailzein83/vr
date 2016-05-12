using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities.EntitySynchronization;

namespace Vanrise.Common.Business.EntitySynchronization
{
    public abstract class SourceItemSynchronizer<TSourceItem, TItem, TSourceItemReader>
        where TSourceItem : ISourceItem
        where TItem : IItem
        where TSourceItemReader : ISourceItemReader<TSourceItem>
    {
        protected TSourceItemReader SourceItemReader
        {
            get
            {
                return _sourceItemReader;
            }
        }
        TSourceItemReader _sourceItemReader;

        public SourceItemSynchronizer(TSourceItemReader sourceItemReader)
        {
            _sourceItemReader = sourceItemReader;
        }
        public virtual void Synchronize()
        {
            Object itemUpdateHandle = GetRecentUpdateHandle();
            var sourceItems = _sourceItemReader.GetChangedItems(ref itemUpdateHandle);
            if (sourceItems != null)
            {
                Dictionary<string, long> existingItemIdsBySourceId;
                var sourceItemIds = sourceItems.Select(itm => itm.SourceId);
                existingItemIdsBySourceId = GetExistingItemIds(sourceItemIds);
                List<TItem> itemsToAdd = new List<TItem>();
                List<TItem> itemsToUpdate = new List<TItem>();
                foreach (var sourceItem in sourceItems)
                {
                    var item = BuildItemFromSource(sourceItem);
                    long itemId;
                    if (existingItemIdsBySourceId != null && sourceItem.SourceId != null && existingItemIdsBySourceId.TryGetValue(sourceItem.SourceId, out itemId))
                    {
                        item.ItemId = itemId;
                        itemsToUpdate.Add(item);
                    }
                    else
                    {
                        if (_sourceItemReader.UseSourceItemId)
                        {
                            if (!long.TryParse(sourceItem.SourceId, out itemId))
                                throw new Exception(String.Format("SourceItemId '{0}' is not a valid long", sourceItem.SourceId));
                            item.ItemId = itemId;
                        }
                        itemsToAdd.Add(item);
                    }
                }
                if (itemsToAdd.Count > 0 && !_sourceItemReader.UseSourceItemId)
                {
                    long startingId;
                    ReserveIdRange(itemsToAdd.Count, out startingId);

                    foreach (var item in itemsToAdd)
                    {
                        item.ItemId = startingId++;
                    }
                }
                HashSet<long> itemsToUpdateIds = itemsToUpdate.Select(itm => itm.ItemId).ToHashSet();
                List<long> itemIdsToDelete = existingItemIdsBySourceId.Values.Where(existingId => !itemsToUpdateIds.Contains(existingId)).ToList();
                UpdateItems(itemsToUpdate);
                AddItems(itemsToAdd);
                SetItemsDeleted(itemIdsToDelete);
                UpdateItemUpdateHandle(itemUpdateHandle);
            }
        }

        private void UpdateItemUpdateHandle(object itemUpdateHandle)
        {

        }

        protected abstract void AddItems(List<TItem> itemsToAdd);

        protected abstract void UpdateItems(List<TItem> itemsToUpdate);

        protected abstract void SetItemsDeleted(List<long> itemIds);

        protected abstract TItem BuildItemFromSource(TSourceItem sourceItem);

        protected abstract Dictionary<string, long> GetExistingItemIds(IEnumerable<string> sourceItemIds);

        protected abstract void ReserveIdRange(int nbOfIds, out long startingId);

        private object GetRecentUpdateHandle()
        {
            return null;
        }
    }
}

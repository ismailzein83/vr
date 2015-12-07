using QM.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.BusinessEntity.Business
{
    public interface ISourceItemReader<T> where T : ISourceItem
    {
        bool UseSourceItemId { get; }

        IEnumerable<T> GetChangedItems(ref object updatedHandle);
    }

    public abstract class SourceItemSynchronizer<TSourceItem, TItem, TSourceItemReader>
        where TSourceItem : ISourceItem
        where TItem : IItem
        where TSourceItemReader : ISourceItemReader<TSourceItem>
    {
        public void Synchronize()
        {
            ISourceItemReader<TSourceItem> changedSourceItemReader = null;
            Object itemUpdateHandle = GetRecentUpdateHandle();
            var sourceItems = changedSourceItemReader.GetChangedItems(ref itemUpdateHandle);
            if (sourceItems != null)
            {
                Dictionary<string, long> itemIdsBySourceId;
                if (changedSourceItemReader.UseSourceItemId)
                {
                    var itemIds = sourceItems.Select(itm => long.Parse(itm.SourceId));
                    itemIdsBySourceId = GetExistingItemIds(itemIds);
                }
                else
                {
                    var sourceZoneIds = sourceItems.Select(itm => itm.SourceId);
                    itemIdsBySourceId = GetExistingItemIds(sourceZoneIds);
                }
                List<TItem> itemsToAdd = new List<TItem>();
                List<TItem> itemsToUpdate = new List<TItem>();
                foreach (var sourceItem in sourceItems)
                {
                    var item = BuildItemFromSource(sourceItem);
                    long itemId;
                    if (itemIdsBySourceId != null && itemIdsBySourceId.TryGetValue(sourceItem.SourceId, out itemId))
                    {
                        item.ItemId = itemId;
                        itemsToUpdate.Add(item);
                    }
                    else
                    {
                        if (changedSourceItemReader.UseSourceItemId)
                        {
                            if (!long.TryParse(sourceItem.SourceId, out itemId))
                                throw new Exception(String.Format("SourceZoneId '{0}' is not a valid long", sourceItem.SourceId));
                            item.ItemId = itemId;
                        }
                        itemsToAdd.Add(item);
                    }
                }
                if (itemsToAdd.Count > 0 && !changedSourceItemReader.UseSourceItemId)
                {
                    long startingId;
                    ReserveIdRange(itemsToAdd.Count, out startingId);

                    foreach (var item in itemsToAdd)
                    {
                        item.ItemId = startingId++;
                    }
                }
                UpdateItems(itemsToUpdate);
                AddItems(itemsToAdd);
                UpdateItemUpdateHandle(itemUpdateHandle);
            }
        }

        private void UpdateItemUpdateHandle(object itemUpdateHandle)
        {
            throw new NotImplementedException();
        }

        protected abstract void AddItems(List<TItem> itemsToAdd);

        protected abstract void UpdateItems(List<TItem> itemsToUpdate);

        protected abstract TItem BuildItemFromSource(TSourceItem sourceZone);

        protected abstract Dictionary<string, long> GetExistingItemIds(IEnumerable<string> sourceZoneIds);

        protected abstract Dictionary<string, long> GetExistingItemIds(IEnumerable<long> itemIds);

        protected abstract void ReserveIdRange(int nbOfIds, out long startingId);

        private object GetRecentUpdateHandle()
        {
            throw new NotImplementedException();
        }
    }
}

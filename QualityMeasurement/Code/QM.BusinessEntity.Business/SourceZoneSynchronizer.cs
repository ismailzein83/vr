using QM.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.BusinessEntity.Business
{  

    public class SourceZone : ISourceItem
    {
        public string SourceId { get; set; }

        public string CountryName { get; set; }

        public string SourceCountryId { get; set; }

        public string Name { get; set; }

        public DateTime BeginEffectiveDate { get; set; }

        public DateTime? EndEffectiveDate { get; set; }
    }

    public interface ISourceItemReader<T> where T : ISourceItem
    {
        bool UseSourceItemId { get; }

        IEnumerable<T> GetChangedItems(ref object updatedHandle);
    }

    public interface IChangedSourceZoneReader
    {
        bool UseSourceZoneId { get; }

        IEnumerable<SourceZone> GetChangedZones(ref object updatedHandle);
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
                    Vanrise.Common.Business.IDManager.Instance.ReserveIDRange(this.GetType(), itemsToAdd.Count, out startingId);
                    foreach (var item in itemsToAdd)
                    {
                        item.ItemId = startingId++;
                    }
                }
                UpdateItems(itemsToUpdate);
                AddItems(itemsToAdd);
                UpdateZoneUpdateHandle(itemUpdateHandle);
            }
        }

        private void UpdateZoneUpdateHandle(object itemUpdateHandle)
        {
            throw new NotImplementedException();
        }

        protected abstract void AddItems(List<TItem> itemsToAdd);

        protected abstract void UpdateItems(List<TItem> itemsToUpdate);

        protected abstract TItem BuildItemFromSource(TSourceItem sourceZone);

        protected abstract Dictionary<string, long> GetExistingItemIds(IEnumerable<string> sourceZoneIds);

        protected abstract Dictionary<string, long> GetExistingItemIds(IEnumerable<long> itemIds);

        private object GetRecentUpdateHandle()
        {
            throw new NotImplementedException();
        }
    }

    public class SourceZoneSynchronizer : SourceItemSynchronizer<SourceZone, Zone, ISourceItemReader<SourceZone>>
    {
        Vanrise.Common.Business.CountryManager countryManager = new Vanrise.Common.Business.CountryManager();
        ZoneManager zoneManager = new ZoneManager();

        protected override void AddItems(List<Zone> itemsToAdd)
        {
            throw new NotImplementedException();
        }

        protected override void UpdateItems(List<Zone> itemsToUpdate)
        {
            throw new NotImplementedException();
        }

        protected override Zone BuildItemFromSource(SourceZone sourceZone)
        {
            Zone zone = new Zone
            {
                Name = sourceZone.Name,
                BeginEffectiveDate = sourceZone.BeginEffectiveDate,
                EndEffectiveDate = sourceZone.EndEffectiveDate
            };
            var country = countryManager.GetCountry(sourceZone.CountryName);
            if (country == null)
            {
                var addResult = countryManager.AddCountry(new Vanrise.Entities.Country
                {
                    Name = sourceZone.CountryName
                });
                if (addResult.Result != Vanrise.Entities.InsertOperationResult.Succeeded)
                    throw new Exception(String.Format("Cannot add Country '{0}'", sourceZone.CountryName));
                country = addResult.InsertedObject.Entity;
            }
            zone.CountryId = country.CountryId;
            return zone;
        }

        protected override Dictionary<string, long> GetExistingItemIds(IEnumerable<string> sourceZoneIds)
        {
            throw new NotImplementedException();
        }

        protected override Dictionary<string, long> GetExistingItemIds(IEnumerable<long> itemIds)
        {
            throw new NotImplementedException();
        }
    }
}

using System.Collections.Generic;
using TOne.WhS.DBSync.Entities;

namespace TOne.WhS.DBSync.Business.EntityMigrator
{
    public abstract class SourceItemMigrator<TSourceItem, TItem, TMigrationSourceItemReader>
        where TSourceItem : Vanrise.Entities.EntitySynchronization.ISourceItem
        where TItem : Vanrise.Entities.EntitySynchronization.IItem
        where TMigrationSourceItemReader : Vanrise.Entities.EntityMigrator.IMigrationSourceItemReader<TSourceItem>
    {
        protected TMigrationSourceItemReader MigrationSourceItemReader
        {
            get
            {
                return _migrationSourceItemReader;
            }
        }
        TMigrationSourceItemReader _migrationSourceItemReader;

        public SourceItemMigrator(TMigrationSourceItemReader sourceItemReader)
        {
            _migrationSourceItemReader = sourceItemReader;
        }
        public virtual void Migrate(List<DBTable> context)
        {
            var sourceItems = _migrationSourceItemReader.GetSourceItems();
            if (sourceItems != null)
            {
                List<TItem> itemsToAdd = new List<TItem>();
                foreach (var sourceItem in sourceItems)
                {
                    var item = BuildItemFromSource(sourceItem);
                    if (item != null)
                        itemsToAdd.Add(item);
                }
                AddItems(itemsToAdd, context);
            }
        }

        protected abstract void AddItems(List<TItem> itemsToAdd, List<DBTable> context);

        protected abstract TItem BuildItemFromSource(TSourceItem sourceItem);
    }
}

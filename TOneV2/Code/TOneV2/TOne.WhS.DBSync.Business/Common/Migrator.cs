using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Entities;
using Vanrise.Entities.EntitySynchronization;

namespace TOne.WhS.DBSync.Business
{
    public abstract class Migrator<T, Q> where T : ISourceItem
    {
        public string TableName { get; set; }
        public MigrationContext Context { get; set; }

        protected Migrator(MigrationContext context)
        {
            Context = context;
        }

        public virtual void Migrate()
        {
            Context.WriteInformation("Migrating table '" + TableName + "' started");
            var sourceItems = GetSourceItems();
            if (sourceItems != null)
            {
                List<Q> itemsToAdd = new List<Q>();
                foreach (var sourceItem in sourceItems)
                {
                    var item = BuildItemFromSource(sourceItem);
                    if (item != null)
                        itemsToAdd.Add(item);
                }
                AddItems(itemsToAdd);
            }
            Context.WriteInformation("Migrating table '" + TableName + "' ended");
        }


        public abstract void AddItems(List<Q> itemsToAdd);

        public abstract IEnumerable<T> GetSourceItems();

        public abstract Q BuildItemFromSource(T sourceItem);
    }
}

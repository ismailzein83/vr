using System;
using System.Collections.Generic;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Entities;
using Vanrise.Entities.EntitySynchronization;

namespace TOne.WhS.DBSync.Business
{
    public abstract class Migrator<T, Q> : IMigrator where T : ISourceItem
    {
        public string TableName { get; set; }
        public MigrationContext Context { get; set; }
        public int TotalRows { get; set; }
        protected Migrator(MigrationContext context)
        {
            Context = context;
        }

        public virtual void Migrate(MigrationInfoContext context)
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
                if (context.GeneratedIdsInfoContext != null)
                    context.GeneratedIdsInfoContext.LastTakenId = TotalRows;
            }
            Context.WriteInformation(string.Format("Migrating table '" + TableName + "' ended: {0} rows ", TotalRows));
        }

        public abstract void FillTableInfo(bool useTempTables);

        public abstract void AddItems(List<Q> itemsToAdd);

        public abstract IEnumerable<T> GetSourceItems();

        public abstract Q BuildItemFromSource(T sourceItem);
    }
}

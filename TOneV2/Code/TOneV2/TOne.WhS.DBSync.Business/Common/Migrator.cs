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
        public int TotalRowsSuccess { get; set; }
        public int TotalRowsFailed { get; set; }
        protected Migrator(MigrationContext context)
        {
            Context = context;
        }

        public virtual void Migrate(MigrationInfoContext context)
        {
            Context.WriteInformation("Migrating table '" + TableName + "' started");
            if (IsLoadItemsApproach)
            {
                List<Q> itemsToAdd = new List<Q>();
                Action<T> onSourceItemLoaded = (sourceItem) =>
                {
                    var item = BuildItemFromSource(sourceItem);
                    if (item != null)
                        itemsToAdd.Add(item);
                    if (itemsToAdd.Count >= 1000)
                    {
                        AddItems(itemsToAdd);
                        itemsToAdd = new List<Q>();
                    }
                };

                LoadSourceItems(onSourceItemLoaded);
                if (itemsToAdd.Count > 0)
                {
                    AddItems(itemsToAdd);
                }
            }
            else
            {
                var sourceItems = GetSourceItems();
                if (sourceItems != null)
                {
                    List<Q> itemsToAdd = null;
                    if(IsBuildAllItemsOnce)
                    {
                        itemsToAdd = BuildAllItemsFromSource(sourceItems);
                    }
                    else
                    {
                        itemsToAdd = new List<Q>();
                        foreach (var sourceItem in sourceItems)
                        {
                            var item = BuildItemFromSource(sourceItem);
                            if (item != null)
                                itemsToAdd.Add(item);
                        }
                    }

                    if (itemsToAdd != null)
                        AddItems(itemsToAdd);
                }
            }

            if (context.GeneratedIdsInfoContext != null)
                context.GeneratedIdsInfoContext.LastTakenId = TotalRowsSuccess;
            if(TotalRowsFailed > 0)
                Context.WriteWarning(string.Format("Migrating table '" + TableName + "' : {0} rows failed", TotalRowsFailed));
            Context.WriteInformation(string.Format("Migrating table '" + TableName + "' ended: {0} rows ", TotalRowsSuccess));
        }

        public abstract void FillTableInfo(bool useTempTables);

        public abstract void AddItems(List<Q> itemsToAdd);

        public virtual void LoadSourceItems(Action<T> onItemLoaded)
        {

        }

        public virtual bool IsLoadItemsApproach
        {
            get
            {
                return false;
            }
        }

        public abstract IEnumerable<T> GetSourceItems();

        public abstract Q BuildItemFromSource(T sourceItem);

        public virtual bool IsBuildAllItemsOnce
        {
            get
            {
                return false;
            }
        }


        public virtual List<Q> BuildAllItemsFromSource(IEnumerable<T> sourceItems)
        {
            return null;
        }

    }
}

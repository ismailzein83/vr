using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
        public virtual bool IsLoadItemsApproach { get { return false; } }
        public virtual bool IsBuildAllItemsOnce { get { return false; } }

        protected Migrator(MigrationContext context)
        {
            Context = context;
        }

        public virtual void Migrate(MigrationInfoContext context)
        {
            Context.WriteInformation("Migrating table '" + TableName + "' started");
            if (IsLoadItemsApproach)
            {
                bool isLoadItemsRunning = true;
                int totalRowsInserted = 0;

                ConcurrentQueue<List<Q>> qItemsToAdd = new ConcurrentQueue<List<Q>>();
                Exception exceptionWhileAdding = null;
                Task taskAddItems = new Task(() =>
                {
                    try
                    {
                        do
                        {
                            List<Q> newItems;
                            while (qItemsToAdd.TryDequeue(out newItems))
                            {
                                AddItems(newItems);
                                totalRowsInserted += newItems.Count;
                                Context.WriteInformation(string.Format("{0} rows inserted to {1}", totalRowsInserted, this.TableName));
                            }
                            Thread.Sleep(250);
                        }
                        while (isLoadItemsRunning || qItemsToAdd.Count > 0);
                    }
                    catch (Exception ex)
                    {
                        exceptionWhileAdding = ex;
                        throw;
                    }
                    finally
                    {
                        taskAddItems = null;
                    }
                });
                taskAddItems.Start();
                List<Q> itemsToAdd = new List<Q>();
                Action<T> onSourceItemLoaded = (sourceItem) =>
                {
                    var item = BuildItemFromSource(sourceItem);
                    if (item != null)
                        itemsToAdd.Add(item);
                    if (itemsToAdd.Count >= 500000)
                    {
                        qItemsToAdd.Enqueue(itemsToAdd);
                        itemsToAdd = new List<Q>();
                    }
                };

                LoadSourceItems(onSourceItemLoaded);
                if (itemsToAdd.Count > 0)
                {
                    qItemsToAdd.Enqueue(itemsToAdd);
                }
                isLoadItemsRunning = false;
                while (taskAddItems != null)
                {
                    Thread.Sleep(250);
                }
                if (exceptionWhileAdding != null)
                    throw exceptionWhileAdding;
            }
            else
            {
                var sourceItems = GetSourceItems();
                if (sourceItems != null)
                {
                    List<Q> itemsToAdd = null;
                    if (IsBuildAllItemsOnce)
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
            if (TotalRowsFailed > 0)
                Context.WriteWarning(string.Format("Migrating table '" + TableName + "' : {0} rows failed", TotalRowsFailed));
            Context.WriteInformation(string.Format("Migrating table '" + TableName + "' ended: {0} rows ", TotalRowsSuccess));
        }

        public abstract void FillTableInfo(bool useTempTables);

        public abstract void AddItems(List<Q> itemsToAdd);

        public virtual void LoadSourceItems(Action<T> onItemLoaded)
        {

        }

        public abstract IEnumerable<T> GetSourceItems();

        public abstract Q BuildItemFromSource(T sourceItem);

        public virtual List<Q> BuildAllItemsFromSource(IEnumerable<T> sourceItems)
        {
            return null;
        }

        public virtual void FinalizeRelatedEntities()
        {

        }
    }
}
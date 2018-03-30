using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Common.Data;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class VRBulkActionDraftManager
    {
     
        #region Public Methods

        //public void CreateIfNotExistVRBulkActionDraft<T>(BulkActionState bulkActionState, IEnumerable<T> items, Func<IEnumerable<T>, IEnumerable<BulkActionItem>> mapper)
        //{
        //    if (bulkActionState.IsAllSelected)
        //    {
        //        if(!bulkActionState.ReflectedToDB)
        //        {
        //            var mappedData = mapper(items);
        //            CreateVRBulkActionDrafts(bulkActionState.BulkActionDraftIdentifier, mappedData);
        //        }
        //    }
        //    else
        //    {
        //        ClearVRBulkActionDraft(bulkActionState.BulkActionDraftIdentifier);
        //    }

        //}
        public void CreateWithClearVRBulkActionDraft<T>(BulkActionState bulkActionState, IEnumerable<T> items, Func<IEnumerable<T>, IEnumerable<BulkActionItem>> mapper)
        {
            ClearVRBulkActionDraft(bulkActionState.BulkActionDraftIdentifier);

            if (bulkActionState.IsAllSelected)
            {
                var mappedData = mapper(items);
                CreateVRBulkActionDrafts(bulkActionState.BulkActionDraftIdentifier, mappedData);
            }
        }

        public IEnumerable<VRBulkActionDraft> GetVRBulkActionDrafts(BulkActionFinalState bulkActionFinalState)
        {
            if (bulkActionFinalState.IsAllSelected)
            {
                IVRBulkActionDraftDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRBulkActionDraftDataManager>();
                var vrBulkActionDrafts = dataManager.GetVRBulkActionDrafts(bulkActionFinalState.BulkActionDraftIdentifier);
                HashSet<BulkActionItem> bulkActionItems = null;
                if (bulkActionFinalState.TargetItems != null && bulkActionFinalState.TargetItems.Count() > 0)
                {
                    bulkActionItems = bulkActionFinalState.TargetItems.ToHashSet();
                }

                    Func<VRBulkActionDraft, bool> filterExpression = (vrBulkActionDraft) =>
                {

                    if (bulkActionItems != null && bulkActionItems.Count() > 0)
                    {
                        if (bulkActionItems.Any(x => x.ItemId == vrBulkActionDraft.ItemId))
                        {
                            return false;
                        }
                    }
                    return true;
                };
                return vrBulkActionDrafts.FindAllRecords(filterExpression);


            }
            else
            {
                if (bulkActionFinalState.TargetItems != null && bulkActionFinalState.TargetItems.Count() > 0)
                {
                    List<VRBulkActionDraft> vrBulkActionDraft = new List<VRBulkActionDraft>();

                    foreach (var targetItem in bulkActionFinalState.TargetItems)
                    {
                        vrBulkActionDraft.Add(new VRBulkActionDraft
                        {
                            ItemId = targetItem.ItemId,
                        });
                    }
                    return vrBulkActionDraft;
                }
            }
            return null;
        }
        public IEnumerable<T> GetOrCreateCachedWithSelectionHandling<T, Q>(ref string resultKey, BulkActionState bulkActionState, Func<IEnumerable<T>> getItems, Func<IEnumerable<T>, IEnumerable<BulkActionItem>> mapper) where Q : BaseCacheManager
        {
            return GetOrCreateCachedWithSelectionHandling(ref resultKey, bulkActionState, getItems, Vanrise.Caching.CacheManagerFactory.GetCacheManager<Q>().GetOrCreateObject, mapper);
        }
        public IEnumerable<T> GetOrCreateCachedWithSelectionHandling<T, Q, R>(ref string resultKey, BulkActionState bulkActionState, Func<IEnumerable<T>> getItems, Func<IEnumerable<T>, IEnumerable<BulkActionItem>> mapper, R cacheManagerParameter) where Q : BaseCacheManager<R>
        {
            return GetOrCreateCachedWithSelectionHandling(ref resultKey, bulkActionState, getItems,
                (cacheName, cacheExpirationChecker, createItems) => Vanrise.Caching.CacheManagerFactory.GetCacheManager<Q>().GetOrCreateObject(cacheName, cacheManagerParameter, cacheExpirationChecker, createItems), mapper);
        }

        #endregion

        #region Private Methods

        private void ClearVRBulkActionDraft(Guid bulkActionDraftIdentifier)
        {
            DateTime removeBeforeDate = DateTime.Now.AddDays(-3);
            IVRBulkActionDraftDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRBulkActionDraftDataManager>();
            dataManager.ClearVRBulkActionDraft(bulkActionDraftIdentifier, removeBeforeDate);
        }
        private void CreateVRBulkActionDrafts(Guid bulkActionDraftIdentifier, IEnumerable<BulkActionItem> items)
        {
            if (items != null && items.Count() > 0)
            {
                var bulkActionDrafts = items.MapRecords(x => new VRBulkActionDraft
                {
                    ItemId = x.ItemId,
                });
                IVRBulkActionDraftDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRBulkActionDraftDataManager>();
                dataManager.CreateVRBulkActionDrafts(bulkActionDrafts, bulkActionDraftIdentifier);
            }
        }

        private IEnumerable<T> GetOrCreateCachedWithSelectionHandling<T>(ref string resultKey, BulkActionState bulkActionState, Func<IEnumerable<T>> getItems, Func<string, CacheExpirationChecker, Func<IEnumerable<T>>, IEnumerable<T>> getOrCreateCachedItems, Func<IEnumerable<T>, IEnumerable<BulkActionItem>> mapper)
        {
            if (String.IsNullOrEmpty(resultKey))
                resultKey = Guid.NewGuid().ToString();

            var items = getOrCreateCachedItems(resultKey,
                new Vanrise.Caching.SlidingWindowCacheExpirationChecker(TimeSpan.FromMinutes(15)),
                () =>
                {
                    var items_local = getItems();

                    if (bulkActionState != null)
                    {
                        CreateWithClearVRBulkActionDraft(bulkActionState, items_local, mapper);
                        bulkActionState.ReflectedToDB = true;
                    }
                        
                    return items_local;
                });

            //if (bulkActionState != null && !bulkActionState.ReflectedToDB)
            //    CreateIfNotExistVRBulkActionDraft(bulkActionState, items, mapper);

            return items;
        }
        
        #endregion

    }

    
}

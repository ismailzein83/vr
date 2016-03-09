using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Security.Data;
using Vanrise.Security.Entities;
using Vanrise.Common;
namespace Vanrise.Security.Business
{
    public class ViewManager
    {
     
        #region ctor

        ModuleManager _moduleManager;
        IViewDataManager _dataManager;
        public ViewManager()
        {
            _dataManager = SecurityDataManagerFactory.GetDataManager<IViewDataManager>();
            _moduleManager = new ModuleManager();
        }
        #endregion
       
        #region Public Members
        public Vanrise.Entities.IDataRetrievalResult<ViewDetail> GetFilteredDynamicViews(Vanrise.Entities.DataRetrievalInput<string> input)
        {
            var dynamicViews = GetDynamicViews();

            Func<View, bool> filterExpression = (prod) =>
                 (input.Query == null || prod.Name.ToLower().Contains(input.Query.ToLower()));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dynamicViews.ToBigResult(input, filterExpression, ViewDetailMapper));
        }
        public Vanrise.Entities.InsertOperationOutput<ViewDetail> AddView(View view)
        {
            InsertOperationOutput<ViewDetail> insertOperationOutput = new InsertOperationOutput<ViewDetail>();

            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int viewId = -1;
            IViewDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IViewDataManager>();
            bool insertActionSucc = dataManager.AddView(view, out viewId);

            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                view.ViewId = viewId;
                insertOperationOutput.InsertedObject = ViewDetailMapper(view);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }
        public Vanrise.Entities.UpdateOperationOutput<ViewDetail> UpdateView(View view)
        {
            UpdateOperationOutput<ViewDetail> updateOperationOutput = new UpdateOperationOutput<ViewDetail>();

            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            IViewDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IViewDataManager>();
            bool updateActionSucc = dataManager.UpdateView(view);

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = ViewDetailMapper(view);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }
        public View GetView(int viewId)
        {
            var allViews = GetCachedViews();
            return allViews.GetRecord(viewId);
        }

        public List<View> GetViews()
        {
            return GetCachedViews().Values.ToList();
        } 

        public Vanrise.Entities.DeleteOperationOutput<object> DeleteView(int viewId)
        {
            DeleteOperationOutput<object> deleteOperationOutput = new DeleteOperationOutput<object>();
            deleteOperationOutput.Result = DeleteOperationResult.Failed;
            IViewDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IViewDataManager>();
            bool deleteActionSucc = dataManager.DeleteView(viewId);

            if (deleteActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                deleteOperationOutput.Result = DeleteOperationResult.Succeeded;
            }

            return deleteOperationOutput;
        }
        public Vanrise.Entities.UpdateOperationOutput<List<MenuItem>> UpdateViewsRank(List<MenuItem> updatedMenuItem)
        {
            UpdateOperationOutput<List<MenuItem>> updateOperationOutput = new UpdateOperationOutput<List<MenuItem>>();

            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            bool updateActionSucc = false;

            updateActionSucc = updateMenuChilds(updatedMenuItem);

            MenuManager menuManager = new MenuManager();
            if (updateActionSucc)
            {

                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                List<MenuItem> updatedView = menuManager.GetMenuItems(SecurityContext.Current.GetLoggedInUserId());
                updateOperationOutput.UpdatedObject = updatedView;
            }
            return updateOperationOutput;
        }
        public bool updateMenuChilds(List<MenuItem> updatedChildsMenuItem)
        {
            for (int i = 0; i < updatedChildsMenuItem.Count; i++)
            {
                if (updatedChildsMenuItem[i].Childs == null || updatedChildsMenuItem[i].Childs.Count == 0)
                    _dataManager.UpdateViewRank(updatedChildsMenuItem[i].Id, (i + 1 * 10));
                else
                {
                    _moduleManager.UpdateModuleRank(updatedChildsMenuItem[i].Id, (i + 1 * 10));
                    updateMenuChilds(updatedChildsMenuItem[i].Childs);
                }

            }
            return true;
        }
        public bool UpdateViewRank(int viewId,int rank)
        {
            return _dataManager.UpdateViewRank(viewId, rank);
        }
        public IEnumerable<View> GetDynamicViews()
        {
            var allViews = GetCachedViews().Values;
            return allViews.FindAllRecords(x => x.Type == ViewType.Dynamic);
        }

        public IDataRetrievalResult<ViewDetail> GetFilteredViews(DataRetrievalInput<ViewQuery> input)
        {
            var allItems = GetCachedViews();

            Func<View, bool> filterExpression = (itemObject) =>
                 (input.Query.ModuleId == null || itemObject.ModuleId == input.Query.ModuleId);

            return DataRetrievalManager.Instance.ProcessResult(input, allItems.ToBigResult(input, filterExpression, ViewDetailMapper));
        }

        #endregion
        
        #region Private Members

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IViewDataManager _dataManager = SecurityDataManagerFactory.GetDataManager<IViewDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreViewsUpdated(ref _updateHandle);
            }
        }
        private Dictionary<int, View> GetCachedViews()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetViews",
               () =>
               {
                   IViewDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IViewDataManager>();
                   IEnumerable<View> views = SetViewsProperties(dataManager.GetViews());

                   return views.ToDictionary(kvp => kvp.ViewId, kvp => kvp);
               });
        }
        private IEnumerable<View> SetViewsProperties(List<View> views)
        {
            for (int i = 0; i < views.Count; i++)
            {
                var view = views[i];
                if (view.Settings != null)
                    view.Url = view.Settings.GetURL(view);
                if (view.Type == ViewType.Dynamic)
                    view.Url = string.Format("{0}/{{\"viewId\":\"{1}\"}}", view.Url, view.ViewId);
            }
            return views;

        } 
        #endregion

        #region  Mappers
        private ViewDetail ViewDetailMapper(View view)
        {
            ViewDetail viewDetail = new ViewDetail();

            viewDetail.Entity = view;
            viewDetail.ModuleName = _moduleManager.GetModuleName(view.ModuleId);
            return viewDetail;
        }

        #endregion
      
    }
}                               

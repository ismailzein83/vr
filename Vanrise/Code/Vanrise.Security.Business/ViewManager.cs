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
    public class ViewManager : IViewManager
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
        public Vanrise.Entities.InsertOperationOutput<ViewDetail> AddView(ViewToAdd view)
        {
            InsertOperationOutput<ViewDetail> insertOperationOutput = new InsertOperationOutput<ViewDetail>();

            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            view.ViewId = Guid.NewGuid();
            IViewDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IViewDataManager>();
            if(view.ViewTypeName != null)
            {
                ViewTypeManager viewTypeManager = new ViewTypeManager();
                Guid viewTypeId = viewTypeManager.GetViewTypeIdByName(view.ViewTypeName);
                view.Type = viewTypeId;
            }
            bool insertActionSucc = dataManager.AddView(view);

            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
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
        public View GetView(Guid viewId)
        {
            var allViews = GetCachedViews();
            return allViews.GetRecord(viewId);
        }
        public IEnumerable<ViewInfo> GetViewsInfo()
        {
            return GetCachedViews().MapRecords(ViewInfoMapper);
        }
        public List<View> GetViews()
        {
            return GetCachedViews().Values.ToList();
        }

        public Vanrise.Entities.DeleteOperationOutput<object> DeleteView(Guid viewId)
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

            updateActionSucc = UpdateMenuChilds(updatedMenuItem);

            MenuManager menuManager = new MenuManager();
            if (updateActionSucc)
            {

                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                List<MenuItem> updatedView = menuManager.GetMenuItems(false, true);
                updateOperationOutput.UpdatedObject = updatedView;
            }
            return updateOperationOutput;
        }
        public bool UpdateMenuChilds(List<MenuItem> updatedChildsMenuItem)
        {
            var rank = 1;
            for (int i = 0; i < updatedChildsMenuItem.Count; i++)
            {
                var menuItem = updatedChildsMenuItem[i];
                if(menuItem.MenuType == MenuType.Module)
                {
                    rank++;
                    _moduleManager.UpdateModuleRank(menuItem.Id, null, rank);
                    PrepareViewsAndModulesObjects(menuItem.Childs, menuItem);

                }else if (menuItem.MenuType == MenuType.View)
                {
                    rank++;
                    UpdateViewRank(menuItem.Id, Guid.Empty, rank);
                }               
            }
            return true;
        }
        public void PrepareViewsAndModulesObjects(List<MenuItem> childs, MenuItem parent)
        {
            if (childs != null)
             {
                 var rank = 1;
                for (int i = 0; i < childs.Count; i++) {
                    var child = childs[i];
                    if (child.MenuType == MenuType.View)
                    {
                        rank++;
                        UpdateViewRank(child.Id, parent.Id, rank);
                    }
                    else
                    {
                        rank++;
                        _moduleManager.UpdateModuleRank(child.Id, parent.Id, rank);
                        PrepareViewsAndModulesObjects(child.Childs, child);
                    }
                }
            }
        }



        public bool UpdateViewRank(Guid viewId,Guid moduleId,int rank)
        {
            return _dataManager.UpdateViewRank(viewId, moduleId,rank);
        }
        public IEnumerable<View> GetDynamicViews()
        {
            var allViews = GetCachedViews().Values;
            return allViews.FindAllRecords(x => x.ViewContent != null);
        }

        public IDataRetrievalResult<ViewDetail> GetFilteredViews(DataRetrievalInput<ViewQuery> input)
        {
            var allItems = GetCachedViews();

            Func<View, bool> filterExpression = (itemObject) =>
                 ((input.Query.ModuleId == null || itemObject.ModuleId == input.Query.ModuleId)
                 && (input.Query.ViewTypes == null || input.Query.ViewTypes.Contains(itemObject.Type))
                  && (input.Query.Name == null || itemObject.Name.ToLower().Contains(input.Query.Name.ToLower()))
                 );

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
        private Dictionary<Guid, View> GetCachedViews()
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
                if (view.ViewContent !=null)
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
            if (view.ModuleId.HasValue)
                viewDetail.ModuleName = _moduleManager.GetModuleName(view.ModuleId.Value);
            return viewDetail;
        }
        private ViewInfo ViewInfoMapper(View view)
        {
            return new ViewInfo() { 
                ViewId = view.ViewId,
                Name = view.Name
            };

        }
        #endregion


    }
}                               

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Security.Data;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Business
{
    public class ViewManager
    {
        //public Vanrise.Entities.IDataRetrievalResult<View> GetDynamicPages()
        //{
        //    IViewDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IViewDataManager>();
        //    return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(null,dataManager.GetDynamicPages());
        //}

        public Vanrise.Entities.InsertOperationOutput<View> AddView(View view)
        {
            InsertOperationOutput<View> insertOperationOutput = new InsertOperationOutput<View>();

            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int viewId = -1;
            IViewDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IViewDataManager>();
            bool insertActionSucc = dataManager.AddView(view, out viewId);

             if (insertActionSucc)
             {
                 
                 insertOperationOutput.Result = InsertOperationResult.Succeeded;
                 view.ViewId = viewId;
                 insertOperationOutput.InsertedObject = dataManager.GetView(viewId); ;
             }
             else
             {
                 insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
             }

             return insertOperationOutput; 
        }

        public Vanrise.Entities.UpdateOperationOutput<View> UpdateView(View view)
        {
            UpdateOperationOutput<View> updateOperationOutput = new UpdateOperationOutput<View>();

            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            IViewDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IViewDataManager>();
            bool updateActionSucc = dataManager.UpdateView(view);

            if (updateActionSucc)
            {

                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                View updatedView = dataManager.GetView(view.ViewId);
                updateOperationOutput.UpdatedObject = updatedView;
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public View GetView(int viewId)
        {
            IViewDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IViewDataManager>();
            return dataManager.GetView(viewId);
        }
        public Vanrise.Entities.DeleteOperationOutput<object> DeleteView(int viewId)
        {
            DeleteOperationOutput<object> deleteOperationOutput = new DeleteOperationOutput<object>();
            deleteOperationOutput.Result = DeleteOperationResult.Failed;
            IViewDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IViewDataManager>();
            bool deleteActionSucc = dataManager.DeleteView(viewId);

            if (deleteActionSucc)
            {
                deleteOperationOutput.Result = DeleteOperationResult.Succeeded;
            }

            return deleteOperationOutput;
        }
        public Vanrise.Entities.IDataRetrievalResult<View> GetFilteredDynamicViews(Vanrise.Entities.DataRetrievalInput<string> filter)
        {
            IViewDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IViewDataManager>();
            Vanrise.Entities.BigResult<View> views = dataManager.GetFilteredDynamicViews(filter);

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(filter, dataManager.GetFilteredDynamicViews(filter));
        }
        public Vanrise.Entities.UpdateOperationOutput<List<MenuItem>> UpdateViewsRank(List<MenuItem> updatedMenuItem)
        {
            UpdateOperationOutput<List<MenuItem>> updateOperationOutput = new UpdateOperationOutput<List<MenuItem>>();

            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IViewDataManager viewDataManager = SecurityDataManagerFactory.GetDataManager<IViewDataManager>();
            IModuleDataManager moduleDataManager = SecurityDataManagerFactory.GetDataManager<IModuleDataManager>();
            bool  updateActionSucc=false;
            for (int i = 0; i < updatedMenuItem.Count; i++)
            {
                if (updatedMenuItem[i].Childs == null || updatedMenuItem[i].Childs.Count == 0)
                    updateActionSucc = viewDataManager.UpdateViewRank(updatedMenuItem[i].Id, (i + 1 * 10));
                else
                    updateActionSucc = moduleDataManager.UpdateModuleRank(updatedMenuItem[i].Id, (i + 1 * 10));
            }
            MenuManager menuManager = new MenuManager();
            if (updateActionSucc)
            {

                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                List<MenuItem> updatedView = menuManager.GetMenuItems(SecurityContext.Current.GetLoggedInUserId());
                updateOperationOutput.UpdatedObject = updatedView;
            }
            return updateOperationOutput;
        }

      
    }
}                               

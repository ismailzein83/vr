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
        public List<View> GetDynamicPages()
        {
            IViewDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IViewDataManager>();
            return dataManager.GetDynamicPages();
        }

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
        public List<View> GetFilteredDynamicViews(string filter)
        {
            IViewDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IViewDataManager>();
            return dataManager.GetFilteredDynamicViews(filter);
        }
  
    }
}                               

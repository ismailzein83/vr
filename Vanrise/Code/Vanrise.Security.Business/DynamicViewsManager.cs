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
    public class DynamicViewsManager
    {
        public List<View> GetDynamicPages()
        {
            IViewDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IViewDataManager>();
            return dataManager.GetDynamicPages();
        }
  
        public Vanrise.Entities.InsertOperationOutput<View> SaveView(View view)
        {
            InsertOperationOutput<View> insertOperationOutput = new InsertOperationOutput<View>();

            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int viewId = -1;
            IViewDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IViewDataManager>();
              bool insertActionSucc = dataManager.SaveView(view, out viewId);

             if (insertActionSucc)
             {
                 
                 insertOperationOutput.Result = InsertOperationResult.Succeeded;
                 view.ViewId = viewId;
                 insertOperationOutput.InsertedObject = view;
             }

             return insertOperationOutput; 
        }

        public View GetView(int viewId)
        {
            IViewDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IViewDataManager>();
            return dataManager.GetView(viewId);
        }
    }
}                               

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
    public class DynamicPagesManager
    {
        public List<DynamicPage> GetDynamicPages()
        {
            IDynamicPagesDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IDynamicPagesDataManager>();
            return dataManager.GetDynamicPages();
        }
        public List<WidgetDefinition> GetWidgets()
        {
            IDynamicPagesDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IDynamicPagesDataManager>();
            return dataManager.GetWidgets();
        }
        public Vanrise.Entities.InsertOperationOutput<View> SaveView(View view)
        {
            InsertOperationOutput<View> insertOperationOutput = new InsertOperationOutput<View>();

            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int viewId = -1;
              IDynamicPagesDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IDynamicPagesDataManager>();
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
            IDynamicPagesDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IDynamicPagesDataManager>();
            return dataManager.GetView(viewId);
        }
    }
}                               

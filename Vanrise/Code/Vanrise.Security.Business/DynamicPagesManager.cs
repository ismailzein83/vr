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
        public List<Widget> GetWidgets()
        {
            IDynamicPagesDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IDynamicPagesDataManager>();
            return dataManager.GetWidgets();
        }
        public Vanrise.Entities.InsertOperationOutput<PageSettings> SavePage(PageSettings PageSettings)
        {
            InsertOperationOutput<PageSettings> insertOperationOutput = new InsertOperationOutput<PageSettings>();

            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int pageId = -1;
              IDynamicPagesDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IDynamicPagesDataManager>();
              bool insertActionSucc = dataManager.SavePage(PageSettings, out pageId);

             if (insertActionSucc)
             {
                 
                 insertOperationOutput.Result = InsertOperationResult.Succeeded;
                 PageSettings.PageID = pageId;
                 insertOperationOutput.InsertedObject = PageSettings;
             }

             return insertOperationOutput; 
        }
     
        public List<VisualElement> GetPage(int PageId)
        {
            IDynamicPagesDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IDynamicPagesDataManager>();
            return dataManager.GetPage(PageId);
        }
    }
}                               

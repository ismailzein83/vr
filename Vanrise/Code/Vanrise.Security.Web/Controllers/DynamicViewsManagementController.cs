using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;
using Vanrise.Web.Base;
namespace Vanrise.Security.Web.Controllers
{
     [JSONWithTypeAttribute]
    public class DynamicViewsManagementController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpGet]
        public List<DynamicPage> GetDynamicPages()
        {
            DynamicViewsManager manager = new DynamicViewsManager();
            return manager.GetDynamicPages();
        }
      
        [HttpPost]
        public Vanrise.Entities.InsertOperationOutput<View> SaveView(View view)
        {
             DynamicViewsManager manager = new DynamicViewsManager();
             return manager.SaveView(view);
          }
        [HttpGet]
        public View GetView(int viewId)
        {
            DynamicViewsManager manager = new DynamicViewsManager();
            return manager.GetView(viewId);
        }

    }
}
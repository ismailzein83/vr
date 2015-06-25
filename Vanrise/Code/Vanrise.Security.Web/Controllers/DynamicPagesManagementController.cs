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
    public class DynamicPagesManagementController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpGet]
        public List<DynamicPage> GetDynamicPages()
        {
            DynamicPagesManager manager = new DynamicPagesManager();
            return manager.GetDynamicPages();
        }
        [HttpGet]
        public List<WidgetDefinition> GetWidgets()
        {
            DynamicPagesManager manager = new DynamicPagesManager();
            return manager.GetWidgets(); 
        }
        [HttpPost]
        public Vanrise.Entities.InsertOperationOutput<View> SaveView(View view)
        {
             DynamicPagesManager manager = new DynamicPagesManager();
             return manager.SaveView(view);
          }
        [HttpGet]
        public View GetView(int viewId)
        {
            DynamicPagesManager manager = new DynamicPagesManager();
            return manager.GetView(viewId);
        }

    }
}
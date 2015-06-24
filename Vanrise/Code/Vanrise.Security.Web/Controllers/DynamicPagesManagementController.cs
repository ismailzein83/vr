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
        public List<Widget> GetWidgets()
        {
            DynamicPagesManager manager = new DynamicPagesManager();
            return manager.GetWidgets(); 
        }
        [HttpPost]
        public Vanrise.Entities.InsertOperationOutput<PageSettings> SavePage(PageSettings PageSettings)
        {
             DynamicPagesManager manager = new DynamicPagesManager();
             return manager.SavePage(PageSettings);
          }
        [HttpGet]
        public List<VisualElement> GetPage(int PageId)
        {
            DynamicPagesManager manager = new DynamicPagesManager();
            return manager.GetPage(PageId);
        }

    }
}
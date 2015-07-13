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
    public class ViewController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpGet]
        public List<View> GetDynamicPages()
        {
            ViewManager manager = new ViewManager();
            return manager.GetDynamicPages();
        }
      
        [HttpPost]
        public Vanrise.Entities.InsertOperationOutput<View> SaveView(View view)
        {
             ViewManager manager = new ViewManager();
             return manager.SaveView(view);
          }
        [HttpPost]
        public Vanrise.Entities.UpdateOperationOutput<View> UpdateView(View view)
        {
            ViewManager manager = new ViewManager();
            return manager.UpdateView(view);
        }
        [HttpGet]
        public View GetView(int viewId)
        {
            ViewManager manager = new ViewManager();
            return manager.GetView(viewId);
        }
        [HttpGet]
        public Vanrise.Entities.DeleteOperationOutput<object> DeleteView(int viewId)
        {
            ViewManager manager = new ViewManager();
            return manager.DeleteView(viewId);
        }
           [HttpGet]
        public List<View> GetFilteredDynamicViews(string filter)
        {
            ViewManager manager = new ViewManager();
            return manager.GetFilteredDynamicViews(filter);
        }
    }
}
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
         public object GetDynamicPages()
        {
            ViewManager manager = new ViewManager();
            return GetWebResponse(null,manager.GetDynamicPages());
        }
      
        [HttpPost]
        public Vanrise.Entities.InsertOperationOutput<View> AddView(View view)
        {
             ViewManager manager = new ViewManager();
             return manager.AddView(view);
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
           [HttpPost]
        public object GetFilteredDynamicViews(Vanrise.Entities.DataRetrievalInput<string> filter)
        {
            ViewManager manager = new ViewManager();
            return GetWebResponse(filter, manager.GetFilteredDynamicViews(filter));
        }
    }
}
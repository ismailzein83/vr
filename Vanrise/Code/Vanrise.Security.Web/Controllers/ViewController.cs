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
    [RoutePrefix(Constants.ROUTE_PREFIX + "View")]
    public class ViewController : Vanrise.Web.Base.BaseAPIController
    {

        [HttpPost]
        [Route("AddView")]
        public Vanrise.Entities.InsertOperationOutput<ViewDetail> AddView(View view)
        {
            ViewManager manager = new ViewManager();
            return manager.AddView(view);
        }
        [HttpPost]
        [Route("UpdateView")]
        public Vanrise.Entities.UpdateOperationOutput<ViewDetail> UpdateView(View view)
        {
            ViewManager manager = new ViewManager();
            return manager.UpdateView(view);
        }
        [HttpGet]
        [Route("GetView")]
        public View GetView(int viewId)
        {
            ViewManager manager = new ViewManager();
            return manager.GetView(viewId);
        }
        [HttpGet]
        [Route("DeleteView")]
        public Vanrise.Entities.DeleteOperationOutput<object> DeleteView(int viewId)
        {
            ViewManager manager = new ViewManager();
            return manager.DeleteView(viewId);
        }
        [HttpPost]
        [Route("GetFilteredDynamicViews")]
        public object GetFilteredDynamicViews(Vanrise.Entities.DataRetrievalInput<string> filter)
        {
            ViewManager manager = new ViewManager();
            return GetWebResponse(filter, manager.GetFilteredDynamicViews(filter));
        }
        [HttpPost]
        [Route("UpdateViewsRank")]
        public Vanrise.Entities.UpdateOperationOutput<List<MenuItem>> UpdateViewsRank(List<MenuItem> updatedMenuItem)
        {
            ViewManager manager = new ViewManager();
            return manager.UpdateViewsRank(updatedMenuItem);
        }

    }
}
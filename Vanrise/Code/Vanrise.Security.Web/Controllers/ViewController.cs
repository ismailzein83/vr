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
        ViewManager _manager;
        public ViewController()
        {
            _manager = new ViewManager();
        }

        [HttpPost]
        [Route("AddView")]
        public Vanrise.Entities.InsertOperationOutput<ViewDetail> AddView(ViewToAdd view)
        {
            return _manager.AddView(view);
        }

        [HttpPost]
        [Route("UpdateView")]
        public Vanrise.Entities.UpdateOperationOutput<ViewDetail> UpdateView(View view)
        {
            return _manager.UpdateView(view);
        }

        [HttpPost]
        [Route("UpdateViewAudiences")]
        public Vanrise.Entities.UpdateOperationOutput<ViewDetail> UpdateViewAudiences(UpdateViewAudiencesInput viewAudienceInput)
        {
            return _manager.UpdateViewAudiences(viewAudienceInput.ViewId,viewAudienceInput.Audience);
        }

        [HttpGet]
        [Route("GetView")]
        public View GetView(Guid viewId)
        {
            return _manager.GetView(viewId);
        }

        [HttpGet]
        [Route("GetViewAudiencesInfo")]
        public ViewAudiencesInfo GetViewAudiencesInfo(Guid viewId)
        {
            return _manager.GetViewAudiencesInfo(viewId);
        }  

        [HttpGet]
        [Route("GetViewsInfo")]
        public IEnumerable<ViewInfo> GetViewsInfo()
        {
            return _manager.GetViewsInfo();
        }


        [HttpGet]
        [Route("DeleteView")]
        public Vanrise.Entities.DeleteOperationOutput<object> DeleteView(Guid viewId)
        {
            return _manager.DeleteView(viewId);
        }

        [HttpPost]
        [Route("GetFilteredDynamicViews")]
        public object GetFilteredDynamicViews(Vanrise.Entities.DataRetrievalInput<string> filter)
        {
            return GetWebResponse(filter, _manager.GetFilteredDynamicViews(filter), "Dynamic Views");
        }

        [HttpPost]
        [Route("UpdateViewsRank")]
        public Vanrise.Entities.UpdateOperationOutput<List<MenuItem>> UpdateViewsRank(List<MenuItem> updatedMenuItem)
        {
            return _manager.UpdateViewsRank(updatedMenuItem);
        }

        [HttpPost]
        [Route("GetFilteredViews")]
        public object GetFilteredViews(Vanrise.Entities.DataRetrievalInput<ViewQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredViews(input), "Views");
        }        
    }
   
}
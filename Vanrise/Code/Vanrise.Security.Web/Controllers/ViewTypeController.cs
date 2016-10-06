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
    [RoutePrefix(Constants.ROUTE_PREFIX + "ViewType")]
    public class ViewTypeController:BaseAPIController
    { 
        ViewTypeManager _manager;
        public ViewTypeController()
        {
            _manager = new ViewTypeManager();
        }
        [HttpGet]
        [Route("GetViewTypeIdByName")]
        public Guid GetViewTypeIdByName(string name)
        {
            return _manager.GetViewTypeIdByName(name);
        }
        [HttpGet]
        [Route("GetViewTypes")]
        public IEnumerable<ViewType> GetViewTypes()
        {
            return _manager.GetViewTypes();
        }
    }
}
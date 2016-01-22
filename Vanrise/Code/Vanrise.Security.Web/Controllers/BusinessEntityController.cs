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
    [RoutePrefix(Constants.ROUTE_PREFIX + "BusinessEntity")]
    // VR_Sec_BusinessEntityController is used because the BusinessEntity module already has a controller named BusinessEntityController which causes a name conflict
    public class VR_Sec_BusinessEntityController : Vanrise.Web.Base.BaseAPIController
    {
        BusinessEntityManager _manager;
        public VR_Sec_BusinessEntityController()
        {
            _manager = new BusinessEntityManager();
        }

        [HttpGet]
        [Route("GetEntityNodes")]
        public IEnumerable<BusinessEntityNode> GetEntityNodes()
        {
            return _manager.GetEntityNodes();
        }

        [HttpGet]
        [Route("ToggleBreakInheritance")]
        public Vanrise.Entities.UpdateOperationOutput<object> ToggleBreakInheritance(EntityType entityType, string entityId)
        {
            return _manager.ToggleBreakInheritance(entityType, entityId);
        }
    }
}
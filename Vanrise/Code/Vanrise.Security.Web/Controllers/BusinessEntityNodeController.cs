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
    [RoutePrefix(Constants.ROUTE_PREFIX + "BusinessEntityNode")]
    public class BusinessEntityNodeController : BaseAPIController
    {
        BusinessEntityNodeManager _manager = new BusinessEntityNodeManager();

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
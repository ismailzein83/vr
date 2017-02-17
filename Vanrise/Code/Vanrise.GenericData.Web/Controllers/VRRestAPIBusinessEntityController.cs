using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Web.Base;

namespace Vanrise.GenericData.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "VRRestAPIBusinessEntity")]
    public class VRRestAPIBusinessEntityController : BaseAPIController
    {
        VRRestAPIBusinessEntityManager _manager = new VRRestAPIBusinessEntityManager();

        [HttpGet]
        [Route("GetBusinessEntitiesInfo")]
        public IEnumerable<BusinessEntityInfo> GetBusinessEntitiesInfo(Guid businessEntityDefinitionId)
        {
            return _manager.GetBusinessEntitiesInfo(businessEntityDefinitionId);
        }
    }
}
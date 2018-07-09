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
    [RoutePrefix(Constants.ROUTE_PREFIX + "LKUPBusinessEntity")]
    [JSONWithTypeAttribute]
    public class LKUPBusinessEntityController : BaseAPIController
    {
        LKUPBusinessEntityManager _manager = new LKUPBusinessEntityManager();

        [HttpGet]
        [Route("GetLKUPBusinessEntityInfo")]
        public IEnumerable<LKUPBusinessEntityItemInfo> GetLKUPBusinessEntityInfo(Guid businessEntityDefinitionId, LKUPBusinessEntityInfoFilter filter)
        {
            return _manager.GetLKUPBusinessEntityInfo(businessEntityDefinitionId, filter);
        }
    }
}
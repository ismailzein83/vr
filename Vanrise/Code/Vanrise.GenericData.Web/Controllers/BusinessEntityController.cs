using System;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Web.Base;

namespace Vanrise.GenericData.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "BusinessEntity")]
    public class BusinessEntityController : BaseAPIController
    {
        BusinessEntityManager _manager = new BusinessEntityManager();

        [HttpGet]
        [Route("GetBusinessEntitiesInfo")]
        public IEnumerable<BusinessEntityInfo> GetBusinessEntitiesInfo(Guid businessEntityDefinitionId)
        {
            return _manager.GetBusinessEntitiesInfo(businessEntityDefinitionId);
        }
    }
}
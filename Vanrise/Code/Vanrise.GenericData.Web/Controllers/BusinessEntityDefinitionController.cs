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
    [RoutePrefix(Constants.ROUTE_PREFIX + "BusinessEntityDefinition")]
    public class BusinessEntityDefinitionController : BaseAPIController
    {
        BusinessEntityDefinitionManager _manager = new BusinessEntityDefinitionManager();

        [HttpGet]
        [Route("GetBusinessEntityDefinition")]
        public BusinessEntityDefinition GetBusinessEntityDefinition(int businessEntityDefinitionId)
        {
            return _manager.GetBusinessEntityDefinition(businessEntityDefinitionId);
        }

        [HttpGet]
        [Route("GetBusinessEntityDefinitionsInfo")]
        public IEnumerable<BusinessEntityDefinitionInfo> GetBusinessEntityDefinitionsInfo(string filter = null)
        {
            BusinessEntityDefinitionInfoFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<BusinessEntityDefinitionInfoFilter>(filter) : null;
            return _manager.GetBusinessEntityDefinitionsInfo(deserializedFilter);
        }
    }
}
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
    [RoutePrefix(Constants.ROUTE_PREFIX + "GenericBusinessEntityDefinition")]
    [JSONWithTypeAttribute]
    public class GenericBusinessEntityDefinitionController : BaseAPIController
    {
        GenericBusinessEntityDefinitionManager _manager = new GenericBusinessEntityDefinitionManager();
        [HttpGet]
        [Route("GetGenericBEGridDefinition")]
        public GenericBEDefinitionGridDefinition GetGenericBEGridDefinition(Guid businessEntityDefinitionId)
        {
            return _manager.GetGenericBEDefinitionGridDefinition(businessEntityDefinitionId);
        }
        [HttpGet]
        [Route("GetGenericBEGridColumnAttributes")]
        public List<GenericBEDefinitionGridColumnAttribute> GetGenericBEGridColumnAttributes(Guid businessEntityDefinitionId)
        {
            return _manager.GetGenericBEDefinitionGridColumnAttributes(businessEntityDefinitionId);
        }
        [HttpGet]
        [Route("GetGenericBEDefinitionSettings")]
        public GenericBEDefinitionSettings GetGenericBEDefinitionSettings(Guid businessEntityDefinitionId)
        {
            return _manager.GetGenericBEDefinitionSettings(businessEntityDefinitionId);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace Vanrise.Common.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "CompanyDefinitions")]
    [JSONWithTypeAttribute]
    public class VRCommon_CompanyDefinitionsController : BaseAPIController
    {
        [HttpGet]
        [Route("GetCompanyDefinitionConfigs")]
        public IEnumerable<CompanyDefinitionConfig> GetCompanyDefinitionConfigs()
        {
            CompanyDefinitionManager manager = new CompanyDefinitionManager();
            return manager.GetCompanyDefinitionConfigs();
        }
    }
}
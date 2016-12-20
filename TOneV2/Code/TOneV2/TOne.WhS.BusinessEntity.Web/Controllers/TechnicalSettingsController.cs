using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Web.Base;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "TechnicalSettings")]
    public class TechnicalSettingsController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpGet]
        [Route("GetDocumentItemDefinitionsInfo")]
        public IEnumerable<VRDocumentItemDefinition> GetDocumentItemDefinitionsInfo()
        {
            ConfigManager settingManager = new ConfigManager();
            return settingManager.GetDocumentItemDefinitionsInfo();
        }
    }
}

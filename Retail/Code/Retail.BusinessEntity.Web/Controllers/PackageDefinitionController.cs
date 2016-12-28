using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;

namespace Retail.BusinessEntity.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "PackageDefinition")]
    [JSONWithTypeAttribute]
    public class PackageDefinitionController : BaseAPIController
    {
        [HttpGet]
        [Route("GetPackageDefinitionExtendedSettingsConfigs")]
        public IEnumerable<PackageDefinitionConfig> GetPackageDefinitionExtendedSettingsConfigs()
        {
            PackageDefinitionManager manager = new PackageDefinitionManager();
            return manager.GetPackageDefinitionExtendedSettingsConfigs();
        }

        [HttpGet]
        [Route("GetPackageDefinitionsInfo")]
        public IEnumerable<PackageDefinitionInfo> GetPackageDefinitionsInfo(string serializedFilter = null)
        {
            PackageDefinitionManager manager = new PackageDefinitionManager();
            return manager.GetPackageDefinitionsInfo();
        }
    }
}
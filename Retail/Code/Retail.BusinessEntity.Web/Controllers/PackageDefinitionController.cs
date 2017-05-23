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
        PackageDefinitionManager manager = new PackageDefinitionManager();

        [HttpGet]
        [Route("GetPackageDefinitionExtendedSettingsConfigs")]
        public IEnumerable<PackageDefinitionConfig> GetPackageDefinitionExtendedSettingsConfigs()
        {
            return manager.GetPackageDefinitionExtendedSettingsConfigs();
        }

        [HttpGet]
        [Route("GetPackageDefinitionsInfo")]
        public IEnumerable<PackageDefinitionInfo> GetPackageDefinitionsInfo(string serializedFilter = null)
        {
            PackageDefinitionFilter deserializedFilter = (serializedFilter != null) ? Vanrise.Common.Serializer.Deserialize<PackageDefinitionFilter>(serializedFilter) : null;
            return manager.GetPackageDefinitionsInfo(deserializedFilter);
        }
        [HttpGet]
        [Route("GetRecurringChargeEvaluatorConfigs")]
        public IEnumerable<RecurringChargeEvaluatorConfig> GetRecurringChargeEvaluatorConfigs()
        {
            return manager.GetRecurringChargeEvaluatorConfigs();
        }
        [HttpGet]
        [Route("GetPackageDefinition")]
        public PackageDefinition GetPackageDefinition(Guid packageDefinitionId)
        {
            return manager.GetPackageDefinitionById(packageDefinitionId);
        }
    }
}
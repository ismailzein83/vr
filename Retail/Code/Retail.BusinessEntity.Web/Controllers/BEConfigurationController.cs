using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Web.Base;

namespace Retail.BusinessEntity.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "BEConfiguration")]
    [JSONWithTypeAttribute]
    public class BEConfigurationController : BaseAPIController
    {
        BEConfigurationManager manager = new BEConfigurationManager();

        [HttpGet]
        [Route("GetPackageUsageVolumeRecurringPeriodConfigs")]
        public IEnumerable<PackageUsageVolumeRecurringPeriodConfig> GetPackageUsageVolumeRecurringPeriodConfigs()
        {
            return manager.GetPackageUsageVolumeRecurringPeriodConfigs();
        }
    }
}
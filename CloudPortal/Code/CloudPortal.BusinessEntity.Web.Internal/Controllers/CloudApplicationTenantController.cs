using CloudPortal.BusinessEntity.Business;
using CloudPortal.BusinessEntity.Entities;
using System.Web.Http;

namespace CloudPortal.BusinessEntity.Web.Internal.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "CloudApplicationTenant")]
    public class CloudApplicationTenantController : Vanrise.Web.Base.BaseAPIController
    {

        [HttpPost]
        [Route("GetFilteredCloudApplicationTenants")]
        public object GetFilteredCloudApplicationTenants(Vanrise.Entities.DataRetrievalInput<CloudApplicationTenantQuery> input)
        {
            CloudApplicationTenantManager manager = new CloudApplicationTenantManager();
            return GetWebResponse(input, manager.GetFilteredCloudApplicationTenants(input));
        }

        [HttpPost]
        [Route("AssignCloudApplicationTenant")]
        public Vanrise.Entities.InsertOperationOutput<CloudApplicationTenant> AssignCloudApplicationTenant(CloudApplicationTenant cloudApplicationTenant)
        {
            CloudApplicationTenantManager manager = new CloudApplicationTenantManager();
            return manager.AssignCloudApplicationTenant(cloudApplicationTenant);
        }

        [HttpGet]
        [Route("GetCloudApplicationTenant")]
        public CloudApplicationTenant GetCloudApplicationTenant(int cloudApplicationTenantId)
        {
            CloudApplicationTenantManager manager = new CloudApplicationTenantManager();
            return manager.GetApplicationTenantById(cloudApplicationTenantId);
        }

        [HttpPost]
        [Route("UpdateCloudApplicationTenant")]
        public Vanrise.Entities.UpdateOperationOutput<CloudApplicationTenant> UpdateCloudApplicationTenant(CloudApplicationTenant cloudApplicationTenant)
        {
            CloudApplicationTenantManager manager = new CloudApplicationTenantManager();
            return manager.UpdateCloudApplicationTenant(cloudApplicationTenant);
        }
    }
}
using CloudPortal.BusinessEntity.Business;
using CloudPortal.BusinessEntity.Entities;
using System.Web.Http;

namespace CloudPortal.BusinessEntity.Web.Internal.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "CloudApplicationUser")]
    public class CloudApplicationUserController : Vanrise.Web.Base.BaseAPIController
    {

        [HttpPost]
        [Route("GetFilteredCloudApplicationUsers")]
        public object GetFilteredCloudApplicationUsers(Vanrise.Entities.DataRetrievalInput<CloudApplicationUserQuery> input)
        {
            CloudApplicationUserManager manager = new CloudApplicationUserManager();
            return GetWebResponse(input, manager.GetFilteredCloudApplicationUsers(input));
        }

        [HttpPost]
        [Route("AssignCloudApplicationUser")]
        public Vanrise.Entities.InsertOperationOutput<CloudApplicationUserToAssign> AssignCloudApplicationUser(CloudApplicationUserToAssign cloudApplicationUser)
        {
            CloudApplicationUserManager manager = new CloudApplicationUserManager();
            return manager.AssignCloudApplicationUsers(cloudApplicationUser, false);
        }

        [HttpPost]
        [Route("AssignCloudApplicationUserWithPermission")]
        public Vanrise.Entities.InsertOperationOutput<CloudApplicationUserToAssign> AssignCloudApplicationUserWithPermission(CloudApplicationUserToAssign cloudApplicationUser)
        {
            CloudApplicationUserManager manager = new CloudApplicationUserManager();
            return manager.AssignCloudApplicationUsers(cloudApplicationUser, true);
        }

    }
}
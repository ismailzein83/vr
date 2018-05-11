using CloudPortal.BusinessEntity.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Security.Entities;

namespace CloudPortal.BusinessEntity.Web.Internal.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "CloudService")]
    public class CloudServiceController : Vanrise.Web.Base.BaseAPIController, ICloudService
    {
        CloudServiceManager _manager = new CloudServiceManager();

        [HttpPost]
        [Route("AddUserToApplication")]
        [IsInternalAPI]
        public AddUserToApplicationOutput AddUserToApplication(AddUserToApplicationInput input)
        {
            var applicationIdentification = GetApplicationIdentification();
            var output = new AddUserToApplicationOutput();
             output.OperationOutput = _manager.AddUserToApplication(applicationIdentification, input.Email, UserStatus.Active, input.Description, input.TenantId);
            return output;
        }

        [HttpPost]
        [Route("CheckApplicationUsersUpdated")]
        [IsInternalAPI]
        public CheckApplicationUsersUpdatedOuput CheckApplicationUsersUpdated(CheckApplicationUsersUpdatedInput input)
        {
            DateTime? lastCheckTime = default(DateTime?);
            if (input.LastReceivedUpdateInfo != null)
                lastCheckTime = (DateTime?)input.LastReceivedUpdateInfo;
            CheckApplicationUsersUpdatedOuput output = new CheckApplicationUsersUpdatedOuput();
            output.Updated = _manager.CheckAppUsersUpdated(ref lastCheckTime);
            output.LastUpdateInfo = lastCheckTime;
            return output;
        }

        [HttpPost]
        [Route("GetApplicationUsers")]
        [IsInternalAPI]
        public GetApplicationUsersOutput GetApplicationUsers(GetApplicationUsersInput input)
        {
            var applicationIdentification = GetApplicationIdentification();
            return new GetApplicationUsersOutput
            {
                Users = _manager.GetApplicationUsers(applicationIdentification)
            };
        }

        [HttpPost]
        [Route("UpdateUserToApplication")]
        [IsInternalAPI]
        public UpdateUserToApplicationOutput UpdateUserToApplication(UpdateUserToApplicationInput input)
        {
            var applicationIdentification = GetApplicationIdentification();
            var output = new UpdateUserToApplicationOutput();
            output.OperationOutput = _manager.UpdateUserToApplication(applicationIdentification, input.UserId, UserStatus.Active, input.Description);
            return output;
        }

        CloudApplicationIdentification GetApplicationIdentification()
        {
            var headerItem = HttpContext.Current.Request.Headers[CloudAuthServer.CLOUDSERVICE_HTTPHEADERNAME];
            if (headerItem == null)
                throw new NullReferenceException("headerItem");
            CloudApplicationIdentification cloudApplicationIdentification = Vanrise.Common.Serializer.Deserialize<CloudApplicationIdentification>(headerItem);
            if (cloudApplicationIdentification == null)
                throw new NullReferenceException("cloudApplicationIdentification");
            return cloudApplicationIdentification;
        }

        [HttpPost]
        [Route("AddTenantToApplication")]
        [IsInternalAPI]
        public AddTenantToApplicationOutput AddTenantToApplication(AddTenantToApplicationInput input)
        {
            var applicationIdentification = GetApplicationIdentification();
            var output = new AddTenantToApplicationOutput();
            output.OperationOutput = _manager.AddTenantToApplication(applicationIdentification, input.Name, input.ParentTenantId, input.Settings);
            return output;
        }

        [HttpPost]
        [Route("CheckApplicationTenantsUpdated")]
        [IsInternalAPI]
        public CheckApplicationTenantsUpdatedOuput CheckApplicationTenantsUpdated(CheckApplicationTenantsUpdatedInput input)
        {
            DateTime? lastCheckTime = default(DateTime?);
            if (input.LastReceivedUpdateInfo != null)
                lastCheckTime = (DateTime?)input.LastReceivedUpdateInfo;
            CheckApplicationTenantsUpdatedOuput output = new CheckApplicationTenantsUpdatedOuput();
            output.Updated = _manager.CheckAppTenantsUpdated(ref lastCheckTime);
            output.LastUpdateInfo = lastCheckTime;
            return output;
        }

        [HttpPost]
        [Route("GetApplicationTenants")]
        [IsInternalAPI]
        public GetApplicationTenantsOutput GetApplicationTenants(GetApplicationTenantsInput input)
        {
            var applicationIdentification = GetApplicationIdentification();
            return new GetApplicationTenantsOutput
            {
                Tenants = _manager.GetApplicationTenants(applicationIdentification)
            };
        }

        [HttpPost]
        [Route("UpdateTenantToApplication")]
        [IsInternalAPI]
        public UpdateTenantToApplicationOutput UpdateTenantToApplication(UpdateTenantToApplicationInput input)
        {
            var applicationIdentification = GetApplicationIdentification();
            var output = new UpdateTenantToApplicationOutput();
            output.OperationOutput = _manager.UpdateTenantToApplication(applicationIdentification, input.TenantId);
            return output;
        }


        [HttpPost]
        [Route("GetCloudTenants")]
        [IsInternalAPI]
        public List<CloudTenantOutput> GetCloudTenants(CloudTenantInput input)
        {
            var applicationIdentification = GetApplicationIdentification();
            return _manager.GetCloudTenants(applicationIdentification);
        }

        [HttpPost]
        [Route("ResetUserPasswordApplication")]
        [IsInternalAPI]
        public ResetUserPasswordApplicationOutput ResetUserPasswordApplication(ResetUserPasswordApplicationInput input)
        {
            var output = new ResetUserPasswordApplicationOutput();
            output.OperationOutput = _manager.ResetUserPasswordApplication(input.UserId);
            return output;
        }


        [HttpPost]
        [Route("ForgotUserPasswordApplication")]
        [IsInternalAPI]
        public ForgotUserPasswordApplicationOutput ForgotUserPasswordApplication(ForgotUserPasswordApplicationInput input)
        {
            var output = new ForgotUserPasswordApplicationOutput();
            output.OperationOutput = _manager.ForgotUserPasswordApplication(input.Email);
            return output;
        }
    }
}
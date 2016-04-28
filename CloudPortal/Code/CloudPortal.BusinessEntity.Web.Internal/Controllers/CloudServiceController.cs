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
        [IsAnonymous]
        public AddUserToApplicationOutput AddUserToApplication(AddUserToApplicationInput input)
        {
            var applicationIdentification = GetApplicationIdentification();
            var output = new AddUserToApplicationOutput();
             output.OperationOutput = _manager.AddUserToApplication(applicationIdentification, input.Email, input.Status, input.Description);
            return output;
        }

        [HttpPost]
        [Route("CheckApplicationUsersUpdated")]
        [IsAnonymous]
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
        [IsAnonymous]
        public GetApplicationUsersOutput GetApplicationUsers(GetApplicationUsersInput input)
        {
            var applicationIdentification = GetApplicationIdentification();
            return new GetApplicationUsersOutput
            {
                Users = _manager.GetApplicationUsers(applicationIdentification)
            };
        }

        CloudApplicationIdentification GetApplicationIdentification()
        {
            var headerItem = HttpContext.Current.Request.Headers["Vanrise_CloudApplicationIdentification"];
            if (headerItem == null)
                throw new NullReferenceException("headerItem");
            CloudApplicationIdentification cloudApplicationIdentification = Vanrise.Common.Serializer.Deserialize<CloudApplicationIdentification>(headerItem);
            if (cloudApplicationIdentification == null)
                throw new NullReferenceException("cloudApplicationIdentification");
            return cloudApplicationIdentification;
        }

    }
}
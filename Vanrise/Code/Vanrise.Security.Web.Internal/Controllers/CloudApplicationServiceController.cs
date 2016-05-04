using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Web.Internal.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "CloudApplicationService")]
    public class CloudApplicationServiceController : Vanrise.Web.Base.BaseAPIController, ICloudApplicationService
    {
        CloudApplicationServiceManager _manager = new CloudApplicationServiceManager();

        [HttpPost]
        [Route("ConfigureAuthServer")]
        [IsInternalAPI]
        public ConfigureAuthServerOutput ConfigureAuthServer(ConfigureAuthServerInput input)
        {
            return _manager.ConfigureAuthServer(input);
        }

        [HttpPost]
        [Route("UpdateAuthServer")]
        [IsInternalAPI]
        public UpdateAuthServerOutput UpdateAuthServer(UpdateAuthServerInput input)
        {
            return _manager.UpdateAuthServer(input);
        }

        [HttpPost]
        [Route("AssignUserFullControl")]
        [IsInternalAPI]
        public AssignUserFullControlOutput AssignUserFullControl(AssignUserFullControlInput input)
        {
            return _manager.AssignUserFullControl(input);
        }
    }
}
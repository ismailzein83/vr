using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Security.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "RegisteredApplication")]
    public class RegisteredApplicationController : Vanrise.Web.Base.BaseAPIController
    {
        private RegisteredApplicationManager _manager = new RegisteredApplicationManager();

        [HttpGet]
        [Route("GetRegisteredApplicationsInfo")]
        public IEnumerable<RegisteredApplicationInfo> GetRegisteredApplicationsInfo(string filter = null)
        {
            RegisteredApplicationFilter deserializedFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<RegisteredApplicationFilter>(filter) : null;
            return _manager.GetRegisteredApplicationsInfo(deserializedFilter);
        }

        [HttpGet]
        [Route("GetRemoteRegisteredApplicationsInfo")]
        public IEnumerable<RegisteredApplicationInfo> GetRemoteRegisteredApplicationsInfo(Guid securityProviderId, string serializedFilter = null)
        {
            return _manager.GetRemoteRegisteredApplicationsInfo(securityProviderId, serializedFilter);
        }

        [HttpPost]
        [Route("RegisterApplication")]
        public RegisterApplicationOutput RegisterApplication(RegisterApplicationInput input)
        {
            return _manager.RegisterApplication(input.ApplicationName, input.ApplicationURL);
        }
    }
}
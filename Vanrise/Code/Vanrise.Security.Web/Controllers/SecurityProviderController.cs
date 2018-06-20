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
    [RoutePrefix(Constants.ROUTE_PREFIX + "SecurityProvider")]
    public class SecurityProviderController : Vanrise.Web.Base.BaseAPIController
    {
        private SecurityProviderManager _manager = new SecurityProviderManager();

        [IsAnonymous]
        [HttpGet]
        [Route("GetSecurityProvidersInfo")]
        public IEnumerable<SecurityProviderInfo> GetSecurityProvidersInfo(string filter = null)
        {
            SecurityProviderFilter deserializedFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<SecurityProviderFilter>(filter) : null;
            return _manager.GetSecurityProvidersInfo(deserializedFilter);
        }

        [IsAnonymous]
        [HttpGet]
        [Route("GetRemoteSecurityProvidersInfo")]
        public IEnumerable<SecurityProviderInfo> GetRemoteSecurityProvidersInfo(Guid connectionId, string serializedFilter = null)
        {
            return _manager.GetRemoteSecurityProvidersInfo(connectionId, serializedFilter);
        }


        [IsAnonymous]
        [HttpGet]
        [Route("GetSecurityProviderInfobyId")]
        public SecurityProviderInfo GetSecurityProviderInfobyId(Guid securityProviderId)
        {
            return _manager.GetSecurityProviderInfobyId(securityProviderId);
        }

        [HttpGet]
        [Route("GetSecurityProviderbyId")]
        public SecurityProvider GetSecurityProviderbyId(Guid securityProviderId)
        {
            return _manager.GetSecurityProviderbyId(securityProviderId);
        }

        [HttpGet]
        [Route("GetSecurityProviderConfigs")]
        public IEnumerable<SecurityProviderConfigs> GetSecurityProviderConfigs()
        {
            return _manager.GetSecurityProviderConfigs();
        }
    }
}
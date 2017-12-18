using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Common.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "VRExclusiveSessionType")]
    [JSONWithTypeAttribute]
    public class VRExclusiveSessionTypeController : BaseAPIController
    {
        VRExclusiveSessionManager _manager = new VRExclusiveSessionManager();

        [HttpGet]
        [Route("GetVRExclusiveSessionTypeExtendedSettingsConfigs")]
        public IEnumerable<VRExclusiveSessionTypeExtendedSettingsConfig> GetVRExclusiveSessionTypeExtendedSettingsConfigs()
        {
            return _manager.GetVRExclusiveSessionTypeExtendedSettingsConfigs();
        }


        [HttpGet]
        [Route("GetVRExclusiveSessionTypeInfos")]
        public object GetVRExclusiveSessionTypeInfos(string filter = null)
        {
            VRExclusiveSessionTypeInfoFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<VRExclusiveSessionTypeInfoFilter>(filter) : null;
            return _manager.GetVRExclusiveSessionTypeInfos(deserializedFilter);
        }
        [HttpPost]
        [Route("TryTakeSession")]
        public object TryTakeSession(VRExclusiveSessionTryTakeInput input)
        {       
            if(!_manager.DoesUserHaveTakeAccess(input.SessionTypeId))
            {
                return GetUnauthorizedResponse();
            }
            else
                return _manager.TryTakeSession(input);
        }

        [HttpPost]
        [Route("TryKeepSession")]
        public VRExclusiveSessionTryKeepOutput TryKeepSession(VRExclusiveSessionTryKeepInput input)
        {
            return _manager.TryKeepSession(input);
        }

        [HttpPost]
        [Route("ReleaseSession")]
        public void ReleaseSession(VRExclusiveSessionReleaseInput input)
        {
            _manager.ReleaseSession(input);
        }

        [HttpPost]
        [Route("GetSessionLockHeartbeatIntervalInSeconds")]
        public int GetSessionLockHeartbeatIntervalInSeconds()
        {
            var configManager = new ConfigManager();
            return configManager.GetSessionLockHeartbeatIntervalInSeconds();
        }

    }
}
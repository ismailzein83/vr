//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Http;
//using Vanrise.Common.Business;
//using Vanrise.Entities;
//using Vanrise.Web.Base;

//namespace Vanrise.Common.Web.Controllers
//{
//    [RoutePrefix(Constants.ROUTE_PREFIX + "VRExclusiveSessionType")]
//    [JSONWithTypeAttribute]
//    public class VRExclusiveSessionTypeController : BaseAPIController
//    {
//        VRExclusiveSessionManager _manager = new VRExclusiveSessionManager();

//        [HttpGet]
//        [Route("GetVRExclusiveSessionTypeExtendedSettingsConfigs")]
//        public IEnumerable<VRExclusiveSessionTypeExtendedSettingsConfig> GetVRExclusiveSessionTypeExtendedSettingsConfigs()
//        {
//            return _manager.GetVRExclusiveSessionTypeExtendedSettingsConfigs();
//        }

//        [HttpPost]
//        [Route("TryTakeSession")]
//        public VRExclusiveSessionTryTakeOutput TryTakeSession(VRExclusiveSessionTryTakeInput input)
//        {
//            return _manager.TryTakeSession(input);
//        }

//        [HttpPost]
//        [Route("TryKeepSession")]
//        public VRExclusiveSessionTryKeepOutput TryKeepSession(VRExclusiveSessionTryKeepInput input)
//        {
//            return _manager.TryKeepSession(input);
//        }

//        [HttpPost]
//        [Route("ReleaseSession")]
//        public void TryKeepSession(VRExclusiveSessionReleaseInput input)
//        {
//            _manager.ReleaseSession(input);
//        }
//    }
//}
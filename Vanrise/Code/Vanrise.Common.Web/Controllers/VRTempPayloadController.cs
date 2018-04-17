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
    [RoutePrefix(Constants.ROUTE_PREFIX + "VRTempPayload")]
    [JSONWithType]
    public class VRTempPayloadController:BaseAPIController
    {
        [HttpPost]
        [Route("AddVRTempPayload")]
        public Vanrise.Entities.InsertOperationOutput<Guid> AddVRTempPayload(VRTempPayload vrTempPayload)
        {
            VRTempPayloadManager manager = new VRTempPayloadManager();
            return manager.AddVRTempPayload(vrTempPayload);
        }
        [HttpGet]
        [Route("GetVRTempPayload")]
        public VRTempPayload GetVRTempPayload(Guid vrTempPayloadId)
        {
            VRTempPayloadManager manager = new VRTempPayloadManager();
            return manager.GetVRTempPayload(vrTempPayloadId);
        }
    }
}
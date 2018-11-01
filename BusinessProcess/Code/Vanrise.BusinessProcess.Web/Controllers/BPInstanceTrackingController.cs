using System.Collections.Generic;
using System.Web.Http;
using Vanrise.BusinessProcess.Business;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Web.Base;

namespace Vanrise.BusinessProcess.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "BPInstanceTracking")]
    public class BPInstanceTrackingController : BaseAPIController
    {
        [HttpPost]
        [Route("GetUpdated")]
        public BPTrackingUpdateOutput GetUpdated(BPTrackingUpdateInput input)
        {
            BPInstanceTrackingManager manager = new BPInstanceTrackingManager();
            return manager.GetUpdated(input);
        }

        [HttpPost]
        [Route("GetBeforeId")]
        public List<BPTrackingMessageDetail> GetBeforeId(BPTrackingBeforeIdInput input)
        {
            BPInstanceTrackingManager manager = new BPInstanceTrackingManager();
            return manager.GetBeforeId(input);
        }
    }
}
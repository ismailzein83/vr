using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Vanrise.BusinessProcess.Business;
using Vanrise.BusinessProcess.Client;
using Vanrise.BusinessProcess.Entities;
using Vanrise.BusinessProcess.Extensions;
using Vanrise.BusinessProcess.Extensions.WFTaskAction.Arguments;
using Vanrise.BusinessProcess.Web.ModelMappers;
using Vanrise.BusinessProcess.Web.Models;
using Vanrise.Common;
using Vanrise.Runtime.Business;
using Vanrise.Runtime.Entities;
using Vanrise.Web.Base;

namespace Vanrise.BusinessProcess.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "BPInstanceTracking")]
    public class BPInstanceTrackingController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredBPInstanceTracking")]
        public object GetFilteredBPInstanceTracking(Vanrise.Entities.DataRetrievalInput<BPTrackingQuery> input)
        {
            BPInstanceTrackingManager manager = new BPInstanceTrackingManager();
            return GetWebResponse(input, manager.GetFilteredBPInstanceTracking(input));
        }


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
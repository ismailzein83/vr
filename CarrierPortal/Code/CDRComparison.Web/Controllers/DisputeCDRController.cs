using CDRComparison.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;

namespace CDRComparison.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "DisputeCDR")]
    public class DisputeCDRController : BaseAPIController
    {
        DisputeCDRManager _manager = new DisputeCDRManager();

        [HttpPost]
        [Route("GetFilteredDisputeCDRs")]
        public object GetFilteredDisputeCDRs(Vanrise.Entities.DataRetrievalInput<object> input)
        {
            return GetWebResponse(input, _manager.GetFilteredDisputeCDRs(input));
        }
    }
}
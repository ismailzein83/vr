using CDRComparison.Business;
using CDRComparison.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;

namespace CDRComparison.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "PartialMatchCDR")]
    public class PartialMatchCDRController : BaseAPIController
    {
        PartialMatchCDRManager _manager = new PartialMatchCDRManager();

        [HttpPost]
        [Route("GetFilteredPartialMatchCDRs")]
        public object GetFilteredPartialMatchCDRs(Vanrise.Entities.DataRetrievalInput<PartialMatchCDRQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredPartialMatchCDRs(input));
        }
    }
}
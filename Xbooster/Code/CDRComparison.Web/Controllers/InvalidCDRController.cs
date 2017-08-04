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
    [RoutePrefix(Constants.ROUTE_PREFIX + "InvalidCDR")]
    public class InvalidCDRController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredInvalidCDRs")]
        public object GetFilteredInvalidCDRs(Vanrise.Entities.DataRetrievalInput<InvalidCDRQuery> input)
        {
            return GetWebResponse(input, new InvalidCDRManager().GetFilteredInvalidCDRs(input));
        }
    }
}
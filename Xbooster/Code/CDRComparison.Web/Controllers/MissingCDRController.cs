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
    [RoutePrefix(Constants.ROUTE_PREFIX + "MissingCDR")]
    public class MissingCDRController : BaseAPIController
    {
        MissingCDRManager _manager = new MissingCDRManager();

        [HttpPost]
        [Route("GetFilteredMissingCDRs")]
        public object GetFilteredMissingCDRs(Vanrise.Entities.DataRetrievalInput<MissingCDRQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredMissingCDRs(input));
        }
    }
}
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
    [RoutePrefix(Constants.ROUTE_PREFIX + "CDR")]
    public class CDRComparison_CDRController : BaseAPIController
    {
        CDRManager _manager = new CDRManager();

        [HttpPost]
        [Route("GetFilteredCDRs")]
        public object GetFilteredCDRs(Vanrise.Entities.DataRetrievalInput<CDRQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredCDRs(input));
        }
    }
}
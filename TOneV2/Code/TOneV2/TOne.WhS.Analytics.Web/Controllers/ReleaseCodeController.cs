using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.Analytics.Business;
using TOne.WhS.Analytics.Entities;

namespace TOne.WhS.Analytics.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "ReleaseCode")]
    public class WhS_ReleaseCodeController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        [Route("GetAllFilteredReleaseCodes")]
        public Object GetAllFilteredReleaseCodes(Vanrise.Entities.DataRetrievalInput<ReleaseCodeQuery> input)
        {
            ReleaseCodeManager manager = new ReleaseCodeManager();
            return GetWebResponse(input, manager.GetAllFilteredReleaseCodes(input), "Release Codes");

        }
    }
}
using CP.WhS.Business;
using CP.WhS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace CP.WhS.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "BIDsASKs")]
    [JSONWithTypeAttribute]
    public class BIDsASKsController : BaseAPIController
    {
        BIDsASKsManager bidsASKsManager = new BIDsASKsManager();

        [HttpPost]
        [Route("GetFilteredBIDs")]
        public object GetFilteredBIDs(DataRetrievalInput<BIDsQuery> input)
        {
            return GetWebResponse(input, bidsASKsManager.GetFilteredBIDs(input));
        }

        [HttpPost]
        [Route("GetFilteredASKs")]
        public object GetFilteredASKs(DataRetrievalInput<ASKsQuery> input)
        {
            return GetWebResponse(input, bidsASKsManager.GetFilteredASKs(input));
        }

    }
}
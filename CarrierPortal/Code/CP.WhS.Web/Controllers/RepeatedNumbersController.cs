using CP.WhS.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.Analytics.Entities;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace CP.WhS.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "RepeatedNumbers")]
    [JSONWithTypeAttribute]
    public class RepeatedNumbersController : BaseAPIController
    {
        RepeatedNumbersManager _repeatedNumbersManager = new RepeatedNumbersManager();
        [HttpPost]
        [Route("GetFilteredRepeatedNumbers")]
        public object GetFilteredRepeatedNumbers(DataRetrievalInput<RepeatedNumberQuery> input)
        {
            return GetWebResponse(input, _repeatedNumbersManager.GetFilteredBlockedAttempts(input));
        }
    }
}
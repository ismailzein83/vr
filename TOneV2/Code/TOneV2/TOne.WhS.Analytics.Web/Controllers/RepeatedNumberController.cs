using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.Analytics.Business;
using TOne.WhS.Analytics.Entities;

namespace TOne.WhS.Analytics.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "RepeatedNumber")]
    public class WhS_RepeatedNumberController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        [Route("GetAllFilteredRepeatedNumbers")]
        public Object GetAllFilteredRepeatedNumbers(Vanrise.Entities.DataRetrievalInput<RepeatedNumberQuery> input)
        {
            RepeatedNumberManager manager = new RepeatedNumberManager();
            return GetWebResponse(input, manager.GetAllFilteredRepeatedNumbers(input), "Repeated Numbers");
        }
    }
}
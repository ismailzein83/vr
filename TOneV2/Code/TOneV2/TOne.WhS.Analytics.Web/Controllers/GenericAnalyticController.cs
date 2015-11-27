using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TOne.WhS.Analytics.Business;
using TOne.WhS.Analytics.Entities;
using Vanrise.Entities;

namespace TOne.WhS.Analytics.Web.Controllers
{
   [RoutePrefix(Constants.ROUTE_PREFIX + "GenericAnalytic")]
    public class WhS_GenericAnalyticController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        [Route("GetFiltered")]
        public Object GetFiltered(Vanrise.Entities.DataRetrievalInput<GenericAnalyticQuery> input)
        {
            GenericAnalyticManager manager = new GenericAnalyticManager();
            return GetWebResponse(input, manager.GetFiltered(input));
        }
    }
}
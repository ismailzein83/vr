using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Demo.Module.Business;
using Demo.Module.Entities;
using Vanrise.Entities;

namespace Demo.Module.Web.Controllers
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
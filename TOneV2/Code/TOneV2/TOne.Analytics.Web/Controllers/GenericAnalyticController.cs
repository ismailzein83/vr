using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TOne.Analytics.Business;
using TOne.Analytics.Entities;
using Vanrise.Entities;

namespace TOne.Analytics.Web.Controllers
{
    public class GenericAnalyticController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        public Object GetFiltered(Vanrise.Entities.DataRetrievalInput<AnalyticQuery> input)
        {
            GenericAnalyticManager manager = new GenericAnalyticManager();
            return GetWebResponse(input, manager.GetFiltered(input));
        }
    }
}
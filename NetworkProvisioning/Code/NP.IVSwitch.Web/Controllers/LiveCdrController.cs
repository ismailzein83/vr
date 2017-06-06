using NP.IVSwitch.Business;
using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;
namespace NP.IVSwitch.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "LiveCdr")]
    public class LiveCdrController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredLiveCdrs")]
        public object GetFilteredLiveCdrs(Vanrise.Entities.DataRetrievalInput<LiveCdrQuery> input)
        {
            LiveCdrManager manager = new LiveCdrManager();
            return GetWebResponse(input, manager.GetFilteredLiveCdrs(input));
        }

    }
}
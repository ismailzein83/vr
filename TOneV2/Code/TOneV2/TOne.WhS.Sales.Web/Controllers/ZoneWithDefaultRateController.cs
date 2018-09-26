using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.Sales.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "ZoneWithDefaultRate")]
    public class ZoneWithDefaultRateController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredZones")]
        public object GetFilteredZones(Vanrise.Entities.DataRetrievalInput<ZoneWithDefaultRateQuery> input)
        {
            ZoneWithDefaultRateManager manager = new ZoneWithDefaultRateManager();
            return GetWebResponse(input, manager.GetFilteredZones(input), "Zones With Default Rate");
        }
    }
}
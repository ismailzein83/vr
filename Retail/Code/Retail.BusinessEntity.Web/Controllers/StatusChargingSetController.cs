using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;


namespace Retail.BusinessEntity.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "StatusChargingSet")]
    [JSONWithTypeAttribute]
    public class StatusChargingSetController
    {
        StatusChargingManager _manager = new StatusChargingManager();
        [HttpPost]
        [Route("GetFilteredStatusChargingSet")]
        public object GetFilteredStatusChargingSet(Vanrise.Entities.DataRetrievalInput<StatusDefinitionQuery> input)
        {
            return null; // GetWebResponse(input, _manager.GetFilteredStatusDefinitions(input));
        }
    }
}
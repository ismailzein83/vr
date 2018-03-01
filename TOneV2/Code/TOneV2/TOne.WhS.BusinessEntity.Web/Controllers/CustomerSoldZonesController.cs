using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.BusinessEntity.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "CustomerSoldZones")]
    public class CustomerSoldZonesController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredCustomerSoldZones")]
        public object GetFilteredCustomerSoldZones(Vanrise.Entities.DataRetrievalInput<CustomerSoldZonesQuery> input)
        {
            return GetWebResponse(input, new CustomerSoldZonesManager().GetFilteredCustomerSoldZones(input));
        }
    }
}
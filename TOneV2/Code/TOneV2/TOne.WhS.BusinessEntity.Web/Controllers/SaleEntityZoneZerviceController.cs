using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.BusinessEntity.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "SaleEntityZoneService")]
    public class WhSBE_SaleEntityZoneServiceController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredSaleEntityZoneServices")]
        public object GetFilteredSaleEntityZoneServices(Vanrise.Entities.DataRetrievalInput<SaleEntityZoneServiceQuery> input)
        {
            SaleEntityServiceManager manager = new SaleEntityServiceManager();
            return GetWebResponse(input, manager.GetFilteredSaleEntityZoneServices(input), "Zone Services");
        }
    }

}
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
    [RoutePrefix(Constants.ROUTE_PREFIX + "OtherSaleRate")]
    public class OtherSaleRateController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        [Route("GetOtherSaleRates")]
        public IEnumerable<SaleRateDetail> GetOtherSaleRates(OtherSaleRateQuery query)
        {
            return new OtherSaleRateManager().GetOtherSaleRates(query);
        }
    }
}
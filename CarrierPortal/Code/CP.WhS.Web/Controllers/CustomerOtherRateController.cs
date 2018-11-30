using CP.WhS.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.Analytics.Entities;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;
using Vanrise.Web.Base;
using TOne.WhS.BusinessEntity.APIEntities;

namespace CP.WhS.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "CustomerOtherRate")]
    [JSONWithTypeAttribute]
    public class CustomerOtherRateController : BaseAPIController
    {
        [HttpPost]
        [Route("GetCustomerOtherRates")]
        public IEnumerable<SaleRateDetail> GetCustomerOtherRates(OtherSaleRateQuery query)
		{
				return new CustomerOtherRateManager().GetCustomerOtherRates(query);
        }
    }

}
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
    [RoutePrefix(Constants.ROUTE_PREFIX + "CustomerRate")]
    [JSONWithTypeAttribute]
    public class CustomerRateController : BaseAPIController
    {
		[HttpPost]
		[Route("GetFilteredCustomerRates")]
		public object GetFilteredCustomerRates(Vanrise.Entities.DataRetrievalInput<SaleRateQuery> input)
		{
			CustomerRateManager manager = new CustomerRateManager();
			return GetWebResponse(input, manager.GetFilteredCustomerRates(input), "Sale Rates");
		}
	}

}
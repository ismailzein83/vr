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
    [RoutePrefix(Constants.ROUTE_PREFIX + "CustomerRateHistory")]
    [JSONWithTypeAttribute]
    public class CustomerRateHistoryController : BaseAPIController
    {
		[HttpPost]
		[Route("GetFilteredCustomerRateHistoryRecords")]
		public object GetFilteredCustomerRateHistoryRecords(Vanrise.Entities.DataRetrievalInput<SaleRateHistoryQuery> input)
		{
			return GetWebResponse(input, new CustomerRateHistoryManager().GetFilteredCustomerRateHistoryRecords(input), "Sale Rate History");
		}
	}

}
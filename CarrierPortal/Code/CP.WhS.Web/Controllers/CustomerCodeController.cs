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
using CP.WhS.Entities;

namespace CP.WhS.Web.Controllers
{
	[RoutePrefix(Constants.ROUTE_PREFIX + "CustomerCode")]
	[JSONWithTypeAttribute]
	public class CustomerCodeController : BaseAPIController
	{
		[HttpPost]
		[Route("GetRemoteFilteredCustomerCodes")]
		public object GetRemoteFilteredCustomerCodes(Vanrise.Entities.DataRetrievalInput<SaleCodeQueryHandlerInfoWrapper> input)
		{
			CustomerCodeManager manager = new CustomerCodeManager();
			return GetWebResponse(input, manager.GetRemoteFilteredCustomerCodes(input), "Sale Codes");
		}

	}
}
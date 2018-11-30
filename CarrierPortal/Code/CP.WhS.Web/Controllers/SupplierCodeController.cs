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
	[RoutePrefix(Constants.ROUTE_PREFIX + "SupplierCode")]
	[JSONWithTypeAttribute]
	public class SupplierCodeController : BaseAPIController
	{
		SupplierCodeManager _supplierCodeManager = new SupplierCodeManager();
		[HttpPost]
		[Route("GetFilteredSupplierCodes")]
		public object GetFilteredSupplierCodes(Vanrise.Entities.DataRetrievalInput<SupplierCodeQuery> input)
		{
			return GetWebResponse(input, _supplierCodeManager.GetFilteredSupplierCodes(input), "Supplier Codes");
		}
	}

}
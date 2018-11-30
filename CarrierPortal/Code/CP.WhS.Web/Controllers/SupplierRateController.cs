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
    [RoutePrefix(Constants.ROUTE_PREFIX + "SupplierRate")]
    [JSONWithTypeAttribute]
    public class SupplierRateController : BaseAPIController
    {
		SupplierRateManager _supplierRateManager = new SupplierRateManager();
        [HttpPost]
        [Route("GetSupplierRateQueryHandlerInfo")]
        public object GetSupplierRateQueryHandlerInfo(DataRetrievalInput<SupplierRateQueryHandlerInfo> input)
        {
			return GetWebResponse(input, _supplierRateManager.GetSupplierRateQueryHandlerInfo(input), "Supplier Rates");
        }
    }

}
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
    [RoutePrefix(Constants.ROUTE_PREFIX + "SupplierRateHistory")]
    [JSONWithTypeAttribute]
    public class SupplierRateHistoryController : BaseAPIController
    {
		SupplierRateHistoryManager _supplierRateManager = new SupplierRateHistoryManager();
        [HttpPost]
        [Route("GetSupplierRateHistoryQueryHandlerInfo")]
        public object GetSupplierRateHistoryQueryHandlerInfo(DataRetrievalInput<SupplierRateHistoryQueryHandlerInfo> input)
        {
			return GetWebResponse(input, _supplierRateManager.GetSupplierRateHistoryQueryHandlerInfo(input), "Supplier Rates History");
        }
    }

}
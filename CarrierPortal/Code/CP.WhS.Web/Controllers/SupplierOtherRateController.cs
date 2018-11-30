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
    [RoutePrefix(Constants.ROUTE_PREFIX + "SupplierOtherRate")]
    [JSONWithTypeAttribute]
    public class SupplierOtherRateController : BaseAPIController
    {
		SupplierOtherRateManager _supplierRateManager = new SupplierOtherRateManager();
        [HttpPost]
        [Route("GetFilteredSupplierOtherRates")]
        public object GetFilteredSupplierOtherRates(DataRetrievalInput<SupplierOtherRateQueryWrapper> input)
        {
			return GetWebResponse(input, _supplierRateManager.GetFilteredSupplierOtherRates(input), "Other Rates");
        }
    }

}
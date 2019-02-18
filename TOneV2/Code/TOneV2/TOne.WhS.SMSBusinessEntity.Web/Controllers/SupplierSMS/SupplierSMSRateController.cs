using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.SMSBusinessEntity.Business;
using TOne.WhS.SMSBusinessEntity.Entities;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.SMSBusinessEntity.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "SupplierSMSRate")]

    public class SupplierSMSRateController : BaseAPIController
    {
        SupplierSMSRateManager _supplierSMSRateManager = new SupplierSMSRateManager();

        [HttpPost]
        [Route("GetFilteredSupplierSMSRate")]
        public object GetFilteredSupplierSMSRate(DataRetrievalInput<SupplierSMSRateQuery> input)
        {
            return GetWebResponse(input, _supplierSMSRateManager.GetFilteredSupplierSMSRate(input), "Supplier SMS Rates");
        }
    }
}
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
    [RoutePrefix(Constants.ROUTE_PREFIX + "SupplierSMSPriceList")]
    public class SupplierSMSPriceListController : BaseAPIController
    {
        SupplierSMSPriceListManager _customerSMSPriceListManager = new SupplierSMSPriceListManager();

        [HttpPost]
        [Route("GetFilteredSupplierSMSPriceLists")]
        public object GetFilteredSupplierSMSPriceLists(DataRetrievalInput<SupplierSMSPriceListQuery> input)
        {
            return GetWebResponse(input, _customerSMSPriceListManager.GetFilteredSupplierSMSPriceLists(input), "Supplier SMS Price Lists");
        }
    }
}
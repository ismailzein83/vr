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
    [RoutePrefix(Constants.ROUTE_PREFIX + "CustomerSMSPriceList")]
    public class CustomerSMSPriceListController : BaseAPIController
    {
        CustomerSMSPriceListManager _customerSMSPriceListManager = new CustomerSMSPriceListManager();

        [HttpPost]
        [Route("GetFilteredCustomerSMSPriceLists")]
        public object GetFilteredCustomerSMSPriceLists(DataRetrievalInput<CustomerSMSPriceListQuery> input)
        {
            return GetWebResponse(input, _customerSMSPriceListManager.GetFilteredCustomerSMSPriceLists(input), "Customer SMS Price Lists");
        }
    }
}
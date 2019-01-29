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
    [RoutePrefix(Constants.ROUTE_PREFIX + "CustomerSMSRate")]

    public class CustomerSMSRateController : BaseAPIController
    {
        CustomerSMSRateManager _customerSMSRateManager = new CustomerSMSRateManager();

        [HttpPost]
        [Route("GetFilteredCustomerSMSRate")]
        public object GetFilteredCustomerSMSRate(DataRetrievalInput<CustomerSMSRateQuery> input)
        {
            return GetWebResponse(input, _customerSMSRateManager.GetFilteredCustomerSMSRate(input), "Customer SMS Rates");
        }
    }
}
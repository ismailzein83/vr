using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.BusinessEntity.Business;
using TOne.BusinessEntity.Entities;
using Vanrise.Web.Base;

namespace TOne.BusinessEntity.Web.Controllers
{
    public class CustomerTariffController : BaseAPIController
    {
        [HttpPost]
        public object GetFilteredCustomerTariffs(Vanrise.Entities.DataRetrievalInput<CustomerTariffQuery> input)
        {
            CustomerTariffManager manager = new CustomerTariffManager();
            return GetWebResponse(input, manager.GetFilteredCustomerTariffs(input));
        }
    }
}
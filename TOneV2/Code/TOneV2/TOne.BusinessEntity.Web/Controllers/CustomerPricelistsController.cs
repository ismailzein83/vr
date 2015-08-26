using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.BusinessEntity.Business;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Web.Controllers
{
    public class CustomerPricelistsController : Vanrise.Web.Base.BaseAPIController
    {
        
        [HttpPost]
        public object GetCustomerPriceLists(Vanrise.Entities.DataRetrievalInput<string> input)
        {
            CustomerPricelistsManager manager = new CustomerPricelistsManager();
            return GetWebResponse(input, manager.GetCustomerPriceLists(input));
        }
        [HttpPost]
        public object GetCustomerPriceListDetails(Vanrise.Entities.DataRetrievalInput<int> input)
        {
            BasePricelistManager<CustomerPriceListDetail> manager = new BasePricelistManager<CustomerPriceListDetail>();
            return GetWebResponse(input, manager.GetPriceListDetails(input));
        }
    }
}
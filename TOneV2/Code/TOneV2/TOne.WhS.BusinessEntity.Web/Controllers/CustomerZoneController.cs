using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.BusinessEntity.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "CustomerZone")]
    public class CustomerZoneController : BaseAPIController
    {
        [HttpGet]
        [Route("GetCustomerZones")]
        public CustomerZones GetCustomerZones(int customerId)
        {
            CustomerZoneManager manager = new CustomerZoneManager();
            return manager.GetCustomerZones(customerId, DateTime.Now, false);
        }

        [HttpPost]
        [Route("GetFilteredSaleZonesToSell")]
        public object GetFilteredSaleZonesToSell(Vanrise.Entities.DataRetrievalInput<SaleZonesToSellQuery> input)
        {
            CustomerZoneManager manager = new CustomerZoneManager();
            return manager.GetFilteredSaleZonesToSell(input);
        }

        [HttpPost]
        [Route("AddCustomerZones")]
        public TOne.Entities.InsertOperationOutput<CustomerZones> AddCustomerZones(CustomerZones customerZones)
        {
            CustomerZoneManager manager = new CustomerZoneManager();
            return manager.AddCustomerZones(customerZones);
        }
    }
}
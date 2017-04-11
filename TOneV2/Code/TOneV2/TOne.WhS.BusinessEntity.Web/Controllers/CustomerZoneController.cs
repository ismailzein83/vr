﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Web.Base;
using Vanrise.Entities;

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

        [HttpGet]
        [Route("GetCountriesToSell")]
        public IEnumerable<Vanrise.Entities.Country> GetCountriesToSell(int customerId)
        {
            CustomerZoneManager manager = new CustomerZoneManager();
            return manager.GetCountriesToSell(customerId);
        }

        [HttpPost]
        [Route("AddCustomerZones")]
        public InsertOperationOutput<CustomerZones> AddCustomerZones(CustomerZones customerZones)
        {
            CustomerZoneManager manager = new CustomerZoneManager();
            return manager.AddCustomerZones(customerZones);
        }
    }
}
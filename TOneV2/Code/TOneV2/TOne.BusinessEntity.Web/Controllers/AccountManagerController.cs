﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TOne.BusinessEntity.Business;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Web.Controllers
{
    public class AccountManagerController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpGet]
        public List<AccountManagerCarrier> GetCarriers(int userId)
        {
            AccountManagerManager manager = new AccountManagerManager();
            return manager.GetCarriers(userId);
        }

        [HttpGet]
        public List<AssignedCarrier> GetAssignedCarriers(int managerId, bool withDescendants, CarrierType carrierType)
        {
            AccountManagerManager manager = new AccountManagerManager();
            return manager.GetAssignedCarriers(managerId, withDescendants, carrierType);
        }

        [HttpPost]
        public object GetAssignedCarriersFromTempTable(Vanrise.Entities.DataRetrievalInput<AssignedCarrierQuery> input)
        {
            AccountManagerManager manager = new AccountManagerManager();
            return GetWebResponse(input, manager.GetAssignedCarriersFromTempTable(input));
        }

        [HttpGet]
        public int? GetLinkedOrgChartId()
        {
            AccountManagerManager manager = new AccountManagerManager();
            return manager.GetLinkedOrgChartId();
        }

        [HttpGet]
        public Vanrise.Entities.UpdateOperationOutput<object> UpdateLinkedOrgChart(int orgChartId)
        {
            AccountManagerManager manager = new AccountManagerManager();
            return manager.UpdateLinkedOrgChart(orgChartId);
        }

        [HttpPost]
        public Vanrise.Entities.UpdateOperationOutput<object> AssignCarriers(UpdatedAccountManagerCarrier[] updatedCarriers)
        {
            AccountManagerManager manager = new AccountManagerManager();
            return manager.AssignCarriers(updatedCarriers);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TOne.BusinessEntity.Business;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Web.Controllers
{
    public class AccountManagerController : ApiController
    {
        [HttpGet]
        public List<AccountManagerCarrier> GetCarriers(int userId, int from, int to)
        {
            AccountManagerManager manager = new AccountManagerManager();
            return manager.GetCarriers(userId, from, to);
        }

        [HttpGet]
        public List<AssignedCarrier> GetAssignedCarriers(int managerId)
        {
            AccountManagerManager manager = new AccountManagerManager();
            return manager.GetAssignedCarriers(managerId);
        }

        [HttpGet]
        public List<AssignedCarrier> GetAssignedCarriersWithDesc(int managerId, int orgChartId)
        {
            AccountManagerManager manager = new AccountManagerManager();
            return manager.GetAssignedCarriersWithDesc(managerId, orgChartId);
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.Web;

namespace TOne.WhS.BusinessEntity.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "AccountManager")]
    public class WhS_BE_AccountManagerController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpGet]
        [Route("GetCarriers")]
        public List<AccountManagerCarrier> GetCarriers(int userId)
        {
            AccountManagerManager manager = new AccountManagerManager();
            return manager.GetCarriers(userId);
        }

        [HttpGet]
        [Route("GetAssignedCarriers")]
        public List<AssignedCarrier> GetAssignedCarriers(int managerId, bool withDescendants, CarrierAccountType carrierType)
        {
            AccountManagerManager manager = new AccountManagerManager();
            return manager.GetAssignedCarriers(managerId, withDescendants, carrierType);
        }

        [HttpPost]
        [Route("GetAssignedCarriersFromTempTable")]
        public object GetAssignedCarriersFromTempTable(Vanrise.Entities.DataRetrievalInput<AssignedCarrierQuery> input)
        {
            AccountManagerManager manager = new AccountManagerManager();
            return GetWebResponse(input, manager.GetAssignedCarriersFromTempTable(input));
        }

        [HttpGet]
        [Route("GetLinkedOrgChartId")]
        public int? GetLinkedOrgChartId()
        {
            AccountManagerManager manager = new AccountManagerManager();
            return manager.GetLinkedOrgChartId();
        }

        [HttpGet]
        [Route("UpdateLinkedOrgChart")]
        public Vanrise.Entities.UpdateOperationOutput<object> UpdateLinkedOrgChart(int orgChartId)
        {
            AccountManagerManager manager = new AccountManagerManager();
            return manager.UpdateLinkedOrgChart(orgChartId);
        }

        [HttpPost]
        [Route("AssignCarriers")]
        public Vanrise.Entities.UpdateOperationOutput<object> AssignCarriers(UpdatedAccountManagerCarrier[] updatedCarriers)
        {
            AccountManagerManager manager = new AccountManagerManager();
            return manager.AssignCarriers(updatedCarriers);
        }
    }
}

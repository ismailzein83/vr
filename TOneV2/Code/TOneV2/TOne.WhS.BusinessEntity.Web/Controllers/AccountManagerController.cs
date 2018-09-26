using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.Web;
using Vanrise.Web.Base;

namespace TOne.WhS.BusinessEntity.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "AccountManager")]
    public class WhS_BE_AccountManagerController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredAssignedCarriers")]
        public object GetFilteredAssignedCarriers(Vanrise.Entities.DataRetrievalInput<AssignedCarrierQuery> input)
        {
            AccountManagerManager manager = new AccountManagerManager();
            return GetWebResponse(input, manager.GetFilteredAssignedCarriers(input), "Assigned Carriers");
        }
        
        [HttpGet]
        [Route("GetAssignedCarriersDetail")]
        public IEnumerable<AssignedCarrierDetail> GetAssignedCarriersDetail(int managerId, bool withDescendants, CarrierAccountType carrierType)
        {
            AccountManagerManager manager = new AccountManagerManager();
            return manager.GetAssignedCarrierDetails(managerId, withDescendants, carrierType);
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

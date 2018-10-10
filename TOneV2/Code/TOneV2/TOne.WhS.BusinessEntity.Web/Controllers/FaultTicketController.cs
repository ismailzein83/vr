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
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "FaultTicket")]
    public class FaultTicketController : Vanrise.Web.Base.BaseAPIController
    {
        FaultTicketManager _faultTicketManager = new FaultTicketManager();

        [HttpPost]
        [Route("GetCustomerFaultTicketDetails")]
        public CustomerFaultTicketSettingsDetails GetCustomerFaultTicketDetails(CustomerFaultTicketSettingsInput customerFaultTicketInput)
        {
            return _faultTicketManager.GetCustomerFaultTicketDetails(customerFaultTicketInput);
        }
        [HttpPost]
        [Route("GetSupplierFaultTicketDetails")]
        public SupplierFaultTicketSettingsDetails GetSupplierFaultTicketDetails(SupplierFaultTicketSettingsInput supplierFaultTicketInput)
        {
            return _faultTicketManager.GetSupplierFaultTicketDetails(supplierFaultTicketInput);
        }

        [HttpGet]
        [Route("GetAccountManagerName")]
        public string GetAccountManagerName(int accountId)
        {
           return _faultTicketManager.GetAccountManagerName(accountId);
        }
    }
}
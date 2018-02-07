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
        [HttpPost]
        [Route("GetCustomerFaultTicketDetails")]
        public CustomerFaultTicketSettingsDetails GetCustomerFaultTicketDetails(CustomerFaultTicketSettingsInput customerFaultTicketInput)
        {
            FaultTicketManager faultTicketManager = new FaultTicketManager();
            return faultTicketManager.GetCustomerFaultTicketDetails(customerFaultTicketInput);
        }
        [HttpPost]
        [Route("GetSupplierFaultTicketDetails")]
        public SupplierFaultTicketSettingsDetails GetSupplierFaultTicketDetails(SupplierFaultTicketSettingsInput supplierFaultTicketInput)
        {
            FaultTicketManager faultTicketManager = new FaultTicketManager();
            return faultTicketManager.GetSupplierFaultTicketDetails(supplierFaultTicketInput);
        }
    }
}
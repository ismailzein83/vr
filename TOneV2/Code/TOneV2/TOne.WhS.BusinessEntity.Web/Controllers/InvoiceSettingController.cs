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
    [RoutePrefix(Constants.ROUTE_PREFIX + "InvoiceSetting")]
    public class InvoiceSettingController : Vanrise.Web.Base.BaseAPIController
    {
        ConfigManager _manager = new ConfigManager();
        
        [HttpGet]
        [Route("GetInvoiceSettingsInfo")]
        public IEnumerable<CustomerInvoiceSettingInfo> GetInvoiceSettingsInfo()
        {
            return _manager.GetCustomerInvoiceSettingInfo();
        }
    }
}
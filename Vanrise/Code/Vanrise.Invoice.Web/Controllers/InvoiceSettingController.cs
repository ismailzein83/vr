using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Invoice.Business;
using Vanrise.Invoice.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Invoice.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "InvoiceSetting")]
    [JSONWithTypeAttribute]
    public class VR_Invoice_InvoiceSettingController : BaseAPIController
    {
        InvoiceSettingManager _manager = new InvoiceSettingManager();
        InvoiceTypeManager _typeManager = new InvoiceTypeManager();

        [HttpGet]
        [Route("GetInvoiceSetting")]
        public InvoiceSetting GetInvoiceSetting(Guid invoiceSettingId)
        {
            return _manager.GetInvoiceSetting(invoiceSettingId);
        }
        
        [HttpPost]
        [Route("GetFilteredInvoiceSettings")]
        public object GetFilteredInvoiceSettings(Vanrise.Entities.DataRetrievalInput<InvoiceSettingQuery> input)
        {
            if (!_typeManager.DoesUserHaveViewSettingsAccess(input.Query.InvoiceTypeId))
                return GetUnauthorizedResponse();
            return GetWebResponse(input, _manager.GetFilteredInvoiceSettings(input));
        }
        [HttpGet]
        [Route("DoesUserHaveAddSettingsAccess")]
        public bool DoesUserHaveAddSettingsAccess(Guid invoiceTypeId)
        {
            return _typeManager.DoesUserHaveAddSettingsAccess(invoiceTypeId);
        }

        [HttpPost]
        [Route("AddInvoiceSetting")]
        public object AddInvoiceSetting(InvoiceSetting invoiceSetting)
        {
            if (!DoesUserHaveAddSettingsAccess(invoiceSetting.InvoiceTypeId))
                return GetUnauthorizedResponse();
            return _manager.AddInvoiceSetting(invoiceSetting);
        }

        [HttpGet]
        [Route("DoesUserHaveEditSettingsAccess")]
        public bool DoesUserHaveEditSettingsAccess(Guid invoiceTypeId)
        {
            return _typeManager.DoesUserHaveEditSettingsAccess(invoiceTypeId);
        }

        [HttpPost]
        [Route("UpdateInvoiceSetting")]
        public object UpdateInvoiceSetting(InvoiceSetting invoiceSetting)
        {
            if (!DoesUserHaveEditSettingsAccess(invoiceSetting.InvoiceTypeId))
                return GetUnauthorizedResponse();
            return _manager.UpdateInvoiceSetting(invoiceSetting);
        }
        [HttpGet]
        [Route("SetInvoiceSettingDefault")]
        public object SetInvoiceSettingDefault(Guid invoiceSettingId, Guid invoiceTypeId)
        {
            if (!DoesUserHaveEditSettingsAccess(invoiceTypeId))
                return GetUnauthorizedResponse();
            return _manager.SetInvoiceSettingDefault(invoiceSettingId);
        }
             
        [HttpGet]
        [Route("GetInvoiceSettingsInfo")]
        public IEnumerable<InvoiceSettingInfo> GetInvoiceSettingsInfo(string filter = null)
        {
            InvoiceSettingFilter serializedFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<InvoiceSettingFilter>(filter) : null;
            return _manager.GetInvoiceSettingsInfo(serializedFilter);
        }
      
        [HttpGet]
        [Route("GetOverridableInvoiceSetting")]
        public List<InvoiceSettingPartUISection> GetOverridableInvoiceSetting(Guid invoiceSettingId)
        {
            return _manager.GetOverridableInvoiceSetting(invoiceSettingId);
        }
    }
}
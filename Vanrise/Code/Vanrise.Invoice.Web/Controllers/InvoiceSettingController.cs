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
            return GetWebResponse(input, _manager.GetFilteredInvoiceSettings(input));
        }

        [HttpPost]
        [Route("AddInvoiceSetting")]
        public Vanrise.Entities.InsertOperationOutput<InvoiceSettingDetail> AddInvoiceSetting(InvoiceSetting invoiceSetting)
        {
            return _manager.AddInvoiceSetting(invoiceSetting);
        }

        [HttpPost]
        [Route("UpdateInvoiceSetting")]
        public Vanrise.Entities.UpdateOperationOutput<InvoiceSettingDetail> UpdateInvoiceSetting(InvoiceSetting invoiceSetting)
        {
            return _manager.UpdateInvoiceSetting(invoiceSetting);
        }
       
        [HttpGet]
        [Route("GetInvoiceSettingsInfo")]
        public IEnumerable<InvoiceSettingInfo> GetInvoiceSettingsInfo(string filter = null)
        {
            InvoiceSettingFilter serializedFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<InvoiceSettingFilter>(filter) : null;
            return _manager.GetInvoiceSettingsInfo(serializedFilter);
        }
    }
}
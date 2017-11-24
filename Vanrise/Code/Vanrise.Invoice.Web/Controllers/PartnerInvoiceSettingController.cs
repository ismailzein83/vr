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
    [RoutePrefix(Constants.ROUTE_PREFIX + "PartnerInvoiceSetting")]
    [JSONWithTypeAttribute]

    public class PartnerInvoiceSettingController:BaseAPIController
    {
        PartnerInvoiceSettingManager _manager = new PartnerInvoiceSettingManager();
        InvoiceSettingManager _settingsManager = new InvoiceSettingManager();

        [HttpGet]
        [Route("GetPartnerInvoiceSetting")]
        public PartnerInvoiceSetting GetPartnerInvoiceSetting(Guid partnerInvoiceSettingId)
        {
            return _manager.GetPartnerInvoiceSetting(partnerInvoiceSettingId);
        }
        [HttpPost]
        [Route("GetFilteredPartnerInvoiceSettings")]
        public object GetFilteredPartnerInvoiceSettings(Vanrise.Entities.DataRetrievalInput<PartnerInvoiceSettingQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredPartnerInvoiceSettings(input));
        }
        [HttpGet]
        [Route("DoesUserHaveAssignPartnerAccess")]
        public bool DoesUserHaveAssignPartnerAccess(Guid invoiceSettingId)
        {
            return _settingsManager.DoesUserHaveAssignPartnerAccess(invoiceSettingId);
        }
        [HttpPost]
        [Route("AddPartnerInvoiceSetting")]
        public object AddPartnerInvoiceSetting(PartnerInvoiceSettingToAdd partnerInvoiceSettingToAdd)
        {
            if (!DoesUserHaveAssignPartnerAccess(partnerInvoiceSettingToAdd.InvoiceSettingID))
                return GetUnauthorizedResponse();
            return _manager.AddPartnerInvoiceSetting(partnerInvoiceSettingToAdd);
        }
        [HttpPost]
        [Route("UpdatePartnerInvoiceSetting")]
        public object UpdatePartnerInvoiceSetting(PartnerInvoiceSettingToEdit partnerInvoiceSettingToEdit)
        {
            if (!DoesUserHaveAssignPartnerAccess(partnerInvoiceSettingToEdit.InvoiceSettingID))
                return GetUnauthorizedResponse();
            return _manager.UpdatePartnerInvoiceSetting(partnerInvoiceSettingToEdit);
        }
        [HttpGet]
        [Route("DeletePartnerInvoiceSetting")]
        public object DeletePartnerInvoiceSetting(Guid partnerInvoiceSettingId , Guid invoiceSettingID)
        {
            if (!DoesUserHaveAssignPartnerAccess(invoiceSettingID))
                return GetUnauthorizedResponse();
            return _manager.DeletePartnerInvoiceSetting(partnerInvoiceSettingId);
        }
    }
}
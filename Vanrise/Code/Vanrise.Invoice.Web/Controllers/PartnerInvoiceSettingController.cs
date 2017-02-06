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
        [HttpPost]
        [Route("AddPartnerInvoiceSetting")]
        public Vanrise.Entities.InsertOperationOutput<PartnerInvoiceSettingDetail> AddPartnerInvoiceSetting(PartnerInvoiceSetting partnerInvoiceSetting)
        {
            return _manager.AddPartnerInvoiceSetting(partnerInvoiceSetting);
        }
        [HttpPost]
        [Route("UpdatePartnerInvoiceSetting")]
        public Vanrise.Entities.UpdateOperationOutput<PartnerInvoiceSettingDetail> UpdatePartnerInvoiceSetting(PartnerInvoiceSetting partnerInvoiceSetting)
        {
            return _manager.UpdatePartnerInvoiceSetting(partnerInvoiceSetting);
        }
        [HttpGet]
        [Route("DeletePartnerInvoiceSetting")]
        public Vanrise.Entities.DeleteOperationOutput<object> DeletePartnerInvoiceSetting(Guid partnerInvoiceSettingId)
        {
            return _manager.DeletePartnerInvoiceSetting(partnerInvoiceSettingId);
        }
    }
}
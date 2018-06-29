using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TOne.WhS.Invoice.Entities;
using TOne.WhS.Invoice.Business;
using Vanrise.Web.Base;
using System.Web.Http;
using Vanrise.Invoice.Entities;
using Vanrise.Entities;

namespace TOne.WhS.Invoice.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "WhSInvoice")]
    public class WhSInvoiceController:BaseAPIController
    {

        [HttpPost]
        [Route("CompareInvoices")]
        public object CompareInvoices(Vanrise.Entities.DataRetrievalInput<InvoiceComparisonInput> input)
        {
            try
            {
                InvoiceManager manager = new InvoiceManager();
                if (!manager.DoesUserUserHaveCompareInvoiceAccess(input.Query.InvoiceTypeId))
                    return GetUnauthorizedResponse();
                return GetWebResponse(input, manager.CompareInvoices(input));
            }
            catch (Exception ex)
            {
                return new CompareInvoiceOperationOutput<object>
                {
                    Message = ex.Message,
                    ShowExactMessage = true,
                    Result = InsertOperationResult.Failed
                };
            }
        }

        [HttpPost]
        [Route("UpdateOriginalInvoiceData")]
        public object UpdateOriginalInvoiceData(OriginalInvoiceDataInput input)
        {
            InvoiceManager manager = new InvoiceManager();
            if (!manager.DoesUserHaveUpdateOriginalInvoiceDataAccess(input.InvoiceId))
                return GetUnauthorizedResponse();
            return manager.UpdateOriginalInvoiceData(input);
        }

        [HttpGet]
        [Route("GetOriginalInvoiceDataRuntime")]
        public OriginalInvoiceDataRuntime GetOriginalInvoiceDataRuntime(long invoiceId)
        {
            InvoiceManager manager = new InvoiceManager();
            return manager.GetOriginalInvoiceDataRuntime(invoiceId);
        }
        [HttpGet]
        [Route("GetInvoiceDetails")]
        public ComparisonInvoiceDetail GetInvoiceDetails(long invoiceId)
        {
            InvoiceManager manager = new InvoiceManager();
            return manager.GetInvoiceDetails(invoiceId);
        }
        
    }
}
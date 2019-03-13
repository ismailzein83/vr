using Retail.Interconnect.Business;
using Retail.Interconnect.Entities;
using Retail.Interconnect.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Retail.Voice.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "InterconnectInvoiceCompareController")]
    public class WhSInvoiceCompareController : BaseAPIController
    {

        [HttpPost]
        [Route("CompareVoiceInvoices")]
        public object CompareVoiceInvoices(Vanrise.Entities.DataRetrievalInput<InvoiceComparisonVoiceInput> input)
        {
            try
            {
                InvoiceCompareManager manager = new InvoiceCompareManager();
                if (!manager.DoesUserUserHaveCompareInvoiceAccess(input.Query.InvoiceTypeId))
                    return GetUnauthorizedResponse();
                return GetWebResponse(input, manager.CompareVoiceInvoices(input), "Compare Invoices");
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
        [Route("CompareSMSInvoices")]
        public object CompareSMSInvoices(Vanrise.Entities.DataRetrievalInput<InvoiceComparisonSMSInput> input)
        {
            try
            {
                InvoiceCompareManager manager = new InvoiceCompareManager();
                if (!manager.DoesUserUserHaveCompareInvoiceAccess(input.Query.InvoiceTypeId))
                    return GetUnauthorizedResponse();
                return GetWebResponse(input, manager.CompareSMSInvoices(input), "Compare Invoices");
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

    }
}
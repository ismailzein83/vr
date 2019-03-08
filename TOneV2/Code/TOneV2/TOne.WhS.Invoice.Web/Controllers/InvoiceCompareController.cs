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
using TOne.WhS.Invoice.Business.Extensions;

namespace TOne.WhS.Invoice.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "WhSInvoiceCompare")]
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
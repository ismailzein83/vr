﻿using System;
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
    [RoutePrefix(Constants.ROUTE_PREFIX + "WhSInvoice")]
    public class WhSInvoiceController : BaseAPIController
    {
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
        public OriginalInvoiceDataRuntime GetOriginalInvoiceDataRuntime(long invoiceId, InvoiceCarrierType invoiceCarrierType)
        {
            InvoiceManager manager = new InvoiceManager();
            return manager.GetOriginalInvoiceDataRuntime(invoiceId, invoiceCarrierType);
        }

        [HttpGet]
        [Route("GetInvoiceDetails")]
        public ComparisonInvoiceDetail GetInvoiceDetails(long invoiceId, InvoiceCarrierType invoiceCarrierType)
        {
            InvoiceManager manager = new InvoiceManager();
            return manager.GetInvoiceDetails(invoiceId, invoiceCarrierType);
        }

        [HttpGet]
        [Route("DoesInvoiceReportExist")]
        public bool DoesInvoiceReportExist(bool isCustomer)
        {
            InvoiceManager manager = new InvoiceManager();
            return manager.DoesInvoiceReportExist(isCustomer);
        }
		[HttpGet]
		[Route("GetFinancialAccountCurrencyDescription")]
		public string GetFinancialAccountCurrencyDescription(int partnerId)
		{
			InvoiceManager manager = new InvoiceManager();
			return manager.GetFinancialAccountCurrencyDescription(partnerId);
		}

	}
}
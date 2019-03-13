using Retail.Interconnect.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace Retail.Interconnect.Business
{
    public class InterconnectInvoiceManager
    {
        public ComparisonInvoiceDetail GetInvoiceDetails(long invoiceId, InvoiceCarrierType invoiceCarrierType)
        {
            CurrencyManager currencyManager = new CurrencyManager();
            Vanrise.Invoice.Business.InvoiceManager invoiceManager = new Vanrise.Invoice.Business.InvoiceManager();
            var invoice = invoiceManager.GetInvoiceDetail(invoiceId);
            invoice.ThrowIfNull("invoice", invoiceId);

            return new ComparisonInvoiceDetail()
            {
                To = invoice.PartnerName,
                ToDate = invoice.Entity.ToDate,
                DueDate = invoice.Entity.DueDate,
                Calls = invoice.Entity.Details.TotalNumberOfCalls,
                FromDate = invoice.Entity.FromDate,
                IssuedDate = invoice.Entity.IssueDate,
                SerialNumber = invoice.Entity.SerialNumber,
                Duration = invoice.Entity.Details.Duration,
                TotalAmount = invoice.Entity.Details.TotalInvoiceAmount,
                TotalNumberOfSMS = invoice.Entity.Details.TotalNumberOfSMS,
                IsLocked = invoice.Lock,
                IsPaid = invoice.Paid,
                IssuedBy = invoice.UserName,
                PartnerId = invoice.Entity.PartnerId,
                Currency = currencyManager.GetCurrencySymbol(invoice.Entity.Details.InterconnectCurrencyId)
            };
        }

        public bool DoesUserHaveUpdateOriginalInvoiceDataAccess(long invoiceId)
        {
            return new Vanrise.Invoice.Business.InvoiceManager().DoesUserHaveGenerateAccess(invoiceId);

        }

        public bool DoesInvoiceReportExist(bool isCustomer)
        {
            string physicalPath = "";
            if (isCustomer)
                physicalPath = VRWebContext.MapVirtualToPhysicalPath("~/Client/Modules/Retail_Interconnect/Reports/CustomerCompareInvoiceReport.rdlc");
            else
                physicalPath = VRWebContext.MapVirtualToPhysicalPath("~/Client/Modules/Retail_Interconnect/Reports/SupplierCompareInvoiceReport.rdlc");
            return Utilities.PhysicalPathExists(physicalPath);
        }
    }
}

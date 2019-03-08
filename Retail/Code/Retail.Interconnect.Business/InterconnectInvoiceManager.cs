//using Retail.Interconnect.Entities;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Vanrise.Common;
//using Vanrise.Common.Business;

//namespace Retail.Interconnect.Business
//{
//    public class InterconnectInvoiceManager
//    {
//        public ComparisonInvoiceDetail GetInvoiceDetails(long invoiceId, InvoiceCarrierType invoiceCarrierType)
//        {
//            CurrencyManager currencyManager = new CurrencyManager();
//            Vanrise.Invoice.Business.InvoiceManager invoiceManager = new Vanrise.Invoice.Business.InvoiceManager();
//            var invoice = invoiceManager.GetInvoiceDetail(invoiceId);
//            invoice.ThrowIfNull("invoice", invoiceId);

//            return new ComparisonInvoiceDetail()
//            {
//                To = invoice.PartnerName,
//                ToDate = invoice.Entity.ToDate,
//                DueDate = invoice.Entity.DueDate,
//                Calls = invoice.Entity.Details.TotalNumberOfCalls,
//                FromDate = invoice.Entity.FromDate,
//                IssuedDate = invoice.Entity.IssueDate,
//                SerialNumber = invoice.Entity.SerialNumber,
//                Duration = invoice.Entity.Details.Duration,
//                TotalAmount = invoice.Entity.Details.TotalInvoiceAmount,
//                TotalNumberOfSMS = invoice.Entity.Details.TotalNumberOfSMS,
//                IsLocked = invoice.Lock,
//                IsPaid = invoice.Paid,
//                IssuedBy = invoice.UserName,
//                PartnerId = invoice.Entity.PartnerId,
//                Currency = GetCurrency(invoiceCarrierType, currencyManager, invoice)
//            };
//        }

//        private static string GetCurrency(InvoiceCarrierType invoiceCarrierType, CurrencyManager currencyManager, Vanrise.Invoice.Entities.InvoiceDetail invoice)
//        {
//            string currency = "";
//            switch (invoiceCarrierType)
//            {
//                case InvoiceCarrierType.Customer:
//                    currency = currencyManager.GetCurrencySymbol(invoice.Entity.Details.SaleCurrencyId);
//                    break;
//                case InvoiceCarrierType.Supplier:
//                    currency = currencyManager.GetCurrencySymbol(invoice.Entity.Details.SupplierCurrencyId);
//                    break;
//            }
//            return currency;
//        }

//        public bool DoesUserHaveUpdateOriginalInvoiceDataAccess(long invoiceId)
//        {
//            return new Vanrise.Invoice.Business.InvoiceManager().DoesUserHaveGenerateAccess(invoiceId);

//        }
//    }
//}

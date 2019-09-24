using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMExtended.Main.SOMAPI;

namespace BPMExtended.Main.Business
{
    public class InvoiceManager
    {

        #region Public
        public InvoiceEntity GetInvoiceById(string invoiceId)
        {
            var item = new InvoiceEntity();
            using (SOMClient client = new SOMClient())
            {
                var invoice = client.Get<Invoice>(String.Format("api/SOM.ST/Billing/GetInvoiceDetails?InvoiceId={0}", invoiceId));
                item = InvoiceToEntityMapper(invoice);
            }
            return item;
        }
        public List<InvoiceDetail> GetInvoices(string customerId)
        {
            //TODO:Get invoices
            // List<InvoiceDetail> invoices =  RatePlanMockDataGenerator.GetInvoices(customerId);

            //foreach (InvoiceDetail invoice in invoices)
            //{
            //    invoice.CollectionStatus = GetCollectionStatus(invoice.InvoiceCode);
            //    invoice.InvoiceInstallmentFlag = GetInvoiceInstallmentFlag(invoice.InvoiceId);
            //    invoice.FinancialDisputes = GetFinancialDisputes(invoice.InvoiceId);
            //}
            //            return invoices;

            var invoicesDetailItems = new List<InvoiceDetail>();
            using (SOMClient client = new SOMClient())
            {
                List<Invoice> items = client.Get<List<Invoice>>(String.Format("api/SOM.ST/Billing/GetCustomerInvoices?CustomerId={0}", customerId));
                foreach (var item in items)
                {
                    var detailItem = InvoiceToDetailMapper(item);
                    invoicesDetailItems.Add(detailItem);
                }
            }
            return invoicesDetailItems;
        }

        public List<InvoiceDetail> BillOnDemandInvoice(string customerId)
        {
            //TODO:Get invoices
            // List<InvoiceDetail> invoices =  RatePlanMockDataGenerator.GetInvoices(customerId);

            //foreach (InvoiceDetail invoice in invoices)
            //{
            //    invoice.CollectionStatus = GetCollectionStatus(invoice.InvoiceCode);
            //    invoice.InvoiceInstallmentFlag = GetInvoiceInstallmentFlag(invoice.InvoiceId);
            //    invoice.FinancialDisputes = GetFinancialDisputes(invoice.InvoiceId);
            //}
            //            return invoices;

            var invoicesDetailItems = new List<InvoiceDetail>();
            //invoicesDetailItems.Add(new InvoiceDetail
            //{
            //    CustomerID = "aaaa",
            //    InvoiceCode = "aaaa",
            //    InvoiceId = "aaaa",
            //    InvoiceNumber = "aaaa",
            //    BillCycle = "aaaa",
            //    DueDate = "aaaa",
            //    Resource = "aaaa",
            //    InvoiceAccount = "aaaa",
            //    Amount = "aaaa",
            //    OpenAmount = "aaaa",
            //    PhoneNumber = "aaaa",
            //    URL = "aaaa",
            //    CollectionStatus = true,
            //    InvoiceInstallmentFlag = true,
            //    FinancialDisputes = true
            //});
            //invoicesDetailItems.Add(new InvoiceDetail
            //{
            //    CustomerID = "bbbb",
            //    InvoiceCode = "bbbbb",
            //    InvoiceId = "bbbbbb",
            //    InvoiceNumber = "bbbbbb",
            //    BillCycle = "bbbbbb",
            //    DueDate = "bbbbbb",
            //    Resource = "bbbbb",
            //    InvoiceAccount = "bbbbb",
            //    Amount = "bbbbb",
            //    OpenAmount = "bbbbb",
            //    PhoneNumber = "bbbbb",
            //    URL = "bbbbbb",
            //    CollectionStatus = true,
            //    InvoiceInstallmentFlag = true,
            //    FinancialDisputes = true
            //});
            //invoicesDetailItems.Add(new InvoiceDetail
            //{
            //    CustomerID = "cccc",
            //    InvoiceCode = "cccc",
            //    InvoiceId = "cccc",
            //    InvoiceNumber = "ccccc",
            //    BillCycle = "ccccc",
            //    DueDate = "ccccc",
            //    Resource = "cccccc",
            //    InvoiceAccount = "cccccc",
            //    Amount = "ccccc",
            //    OpenAmount = "ccccc",
            //    PhoneNumber = "ccccc",
            //    URL = "ccccccc",
            //    CollectionStatus = false,
            //    InvoiceInstallmentFlag = false,
            //    FinancialDisputes = false
            //});
            //invoicesDetailItems.Add(new InvoiceDetail
            //{
            //    CustomerID = "ddddd",
            //    InvoiceCode = "ddddd",
            //    InvoiceId = "ddddd",
            //    InvoiceNumber = "ddddd",
            //    BillCycle = "ddddd",
            //    DueDate = "ddddd",
            //    Resource = "ddddd",
            //    InvoiceAccount = "ddddd",
            //    Amount = "ddddd",
            //    OpenAmount = "ddddd",
            //    PhoneNumber = "ddddd",
            //    URL = "ddddd",
            //    CollectionStatus = true,
            //    InvoiceInstallmentFlag = true,
            //    FinancialDisputes = true
            //});
            using (SOMClient client = new SOMClient())
            {
                List<Invoice> items = client.Get<List<Invoice>>(String.Format("api/SOM.ST/Billing/GetCustomerSimulatedInvoices?CustomerId={0}", customerId));
                foreach (var item in items)
                {
                    var detailItem = InvoiceToDetailMapper(item);
                    invoicesDetailItems.Add(detailItem);
                }
            }
            return invoicesDetailItems;
        }


        public List<PaymentPlanDetail> GetPaymentPlansByInvoiceId(string invoiceId)
        {
            var paymentDetails = new List<PaymentPlanDetail>();
            using (SOMClient client = new SOMClient())
            {
                List<PaymentPlan> items = client.Get<List<PaymentPlan>>(String.Format("api/SOM.ST/Billing/GetInvoiceInstallments?InvoiceId={0}", invoiceId));
                foreach (var item in items)
                {
                    var paymentPlanDetail = PaymentPlanToDetailMapper(item);
                    paymentDetails.Add(paymentPlanDetail);
                }
            }
            return paymentDetails;
        }

        public bool IsInvoiceHasInstallment(string invoiceId)
        {
            var hasInstallments = false;
            using (SOMClient client = new SOMClient())
            {
                hasInstallments = client.Get<bool>(String.Format("api/SOM.ST/Billing/IsInvoiceHasInstallment?InvoiceId={0}", invoiceId));
            }
            return hasInstallments;
        }

        public BSCSDocument ReadBillDocument(string documentCode, bool documentRefOnly)
        {
            var document = new BSCSDocument();
            using (SOMClient client = new SOMClient())
            {
                document = client.Get<BSCSDocument>(String.Format("api/SOM.ST/Billing/GetBillDocument?DocumentCode={0}&DocumentRefOnly={1}", documentCode, documentRefOnly));
            }
            return document;
        }


        #endregion

        #region Mappers

        public InvoiceDetail InvoiceToDetailMapper(Invoice item)
        {//Invoice Code, Bill Cycle, phone Number , InvoiceAmount, open amount, Invoice URL
            return new InvoiceDetail
            {
                InvoiceCode = item.DocumentCode,
                InvoiceAccount = item.BillingAccountCode,
                OpenAmount = Convert.ToString(item.OpenAmount),
                Amount = Convert.ToString(item.Amount),
                PhoneNumber = item.DirectoryNumber
            };
        }

        public InvoiceEntity InvoiceToEntityMapper(Invoice item)
        {
            return new InvoiceEntity
            {
                Id = item.Id,
                CustomerId = item.CustomerId,
                BillingAccountCode = item.BillingAccountCode,
                EntryDate = item.EntryDate != null ? item.EntryDate.ToString("MM/dd/yyyy HH:mm") : null,
                DueDate = item.DueDate != null ? item.DueDate.ToString("MM/dd/yyyy HH:mm") : null,
                Amount = Convert.ToString(item.Amount),
                OpenAmount = Convert.ToString(item.OpenAmount),
                DocumentCode = item.DocumentCode
            };
        }

        public PaymentPlanDetail PaymentPlanToDetailMapper(PaymentPlan item)
        {
            return new PaymentPlanDetail()
            {
                Id = item.Id,
                Desc = item.Desc,
                Amount = Convert.ToString(item.Amount),
                EndDate = Convert.ToString(item.EndDate),
                NextInstallmentAmount = Convert.ToString(item.NextInstallmentAmount),
                NextInstallmentDueDate = Convert.ToString(item.NextInstallmentDueDate),
                NextInstallmentInterest = Convert.ToString(item.NextInstallmentInterest),
                StartDate = Convert.ToString(item.StartDate),
                Status = Convert.ToString(item.Status)
            };
        }
        #endregion

    }
    //To be relocated in SOM
    public class BSCSDocument
    {
        public string DocumentData { get; set; }
        public string DocumentFormat { get; set; }
    }
}

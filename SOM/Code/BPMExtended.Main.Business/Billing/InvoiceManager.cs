using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMExtended.Main.SOMAPI;
using Terrasoft.Core.Entities;
using Terrasoft.Core;
using System.Web;

namespace BPMExtended.Main.Business
{
    public class InvoiceManager
    {
        #region User Connection
        public UserConnection BPM_UserConnection
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
        }
        #endregion

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
        public List<InvoiceDetail> GetInvoices(string customerId, int? invoicesDaysBack)
        {
            DateTime fromDate;
            if (invoicesDaysBack.HasValue)

                fromDate = DateTime.Today.AddDays(-invoicesDaysBack.Value);

            else

                fromDate = GetInvoicesFromDate();

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

        public List<BillOnDemandInvoiceDetail> BillOnDemandInvoice(string customerId)
        {

            var invoicesDetailItems = new List<BillOnDemandInvoiceDetail>();

            using (SOMClient client = new SOMClient())
            {
                List<Invoice> items = client.Get<List<Invoice>>(String.Format("api/SOM.ST/Billing/GetCustomerSimulatedInvoices?CustomerId={0}", customerId));
                foreach (var item in items)
                {
                    var detailItem = BillOnDemandInvoiceToDetailMapper(item);
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
        {
            return new InvoiceDetail
            {
                ContractId = item.ContractId,
                InvoiceCode = item.DocumentCode,
                InvoiceDate = Convert.ToString(item.EntryDate),
                DueDate = Convert.ToString(item.DueDate),
                OriginalAmount = Convert.ToString(item.OriginalAmount),
                OpenAmount = Convert.ToString(item.OpenAmount),
                BillDispute = GetBillDisputeByEnumValue(item.BillDispute),
                CurrencyCode = item.Currency,
            };
        }
        public string GetBillDisputeByEnumValue(int enumValue)
        {
            string contractStatusDescription = null;
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;

            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StBillDispute");

            esq.AddColumn("StValue");
            esq.AddColumn("Description");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "StValue", enumValue);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                contractStatusDescription = entities[0].GetColumnValue("Description").ToString();
            }

            return contractStatusDescription;
        }
        public DateTime GetInvoicesFromDate()
        {
            int invoicesDaysBack = 0;
            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            DateTime invoiceFromDate = DateTime.Today;
            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StGeneralSettings");

            esq.AddColumn("Id");
            esq.AddColumn("StInvoicesDaysBack");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", "C26F08FF-3A2D-48D4-8EC7-21F59C66BCAF");
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                invoicesDaysBack = (int)entities[0].GetColumnValue("StInvoicesDaysBack");
            }
            if (invoicesDaysBack > 0)
                invoiceFromDate.AddDays(-invoicesDaysBack);
            else
                invoiceFromDate.AddDays(-10);
            return invoiceFromDate;
        }
        public BillOnDemandInvoiceDetail BillOnDemandInvoiceToDetailMapper(Invoice item)
        {
            return new BillOnDemandInvoiceDetail
            {
                InvoiceCode = item.DocumentCode,
                InvoiceAccount = item.BillingAccountCode,
                OpenAmount = Convert.ToString(item.OpenAmount),
                Amount = Convert.ToString(item.OriginalAmount),
                PhoneNumber = item.DirectoryNumber,
                DueDate=item.DueDate.ToString(),
                CustomerID=item.CustomerId,

            };
        }

        public InvoiceEntity InvoiceToEntityMapper(Invoice item)
        {
            return new InvoiceEntity
            {
                ContractId = item.ContractId,
                InvoiceCode = item.DocumentCode,
                InvoiceDate = Convert.ToString(item.EntryDate),
                DueDate = Convert.ToString(item.DueDate),
                OriginalAmount = Convert.ToString(item.OriginalAmount),
                OpenAmount = Convert.ToString(item.OpenAmount),
                BillDispute = GetBillDisputeByEnumValue(item.BillDispute),
                CurrencyCode = item.Currency,
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public class GenerateInvoiceInputItem
    {
        public long? InvoiceId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public dynamic CustomSectionPayload { get; set; }

    }
    public class GenerateInvoiceOutput
    {
        public bool IsSucceeded { get; set; }
        public Invoice Invoice { get; set; }
        public DateTime FromDate  { get; set; }
        public DateTime ToDate { get; set; }
        public InvoiceGenerationMessageOutput Message { get; set; }

    }
    public enum GenerateInvoiceResult { Succeeded = 0, Failed = 1, NoData = 2 }
    public class GenerateInvoiceInput
    {
        public List<GenerateInvoiceInputItem> Items { get; set; }
        public Guid InvoiceActionId { get; set; }
        public Guid InvoiceTypeId { get; set; }
        public string PartnerId { get; set; }
        public DateTime IssueDate { get; set; }
        public Boolean IsAutomatic { get; set; }
    }
    public class PreparedGenerateInvoiceInput
    {
        public Func<Entities.Invoice, bool> ActionAfterGenerateInvoice { get; set; } 
        public IEnumerable<GeneratedInvoiceBillingTransaction> BillingTransactions { get; set; }
        public List<GeneratedInvoiceItemSet> InvoiceItemSets { get; set; }
        public Entities.Invoice Invoice { get; set; }
        public List<long> InvoiceToSettleIds { get; set; }
        public long? InvoiceIdToDelete { get; set; }
        public Guid? SplitInvoiceGroupId { get; set; }
    }
    public class GenerateInvoiceInputToSave
    {
        public Func<Entities.Invoice, bool> ActionAfterGenerateInvoice { get; set; }

        public Func<Entities.Invoice,bool> ActionBeforeGenerateInvoice { get; set; }
        public IEnumerable<Vanrise.AccountBalance.Entities.BillingTransaction> MappedTransactions { get; set; }
        public Dictionary<string, List<string>> ItemSetNameStorageDic { get; set; }
        public List<GeneratedInvoiceItemSet> InvoiceItemSets { get; set; }
        public Entities.Invoice Invoice { get; set; }
        public List<long> InvoiceToSettleIds { get; set; }
        public long? InvoiceIdToDelete { get; set; }
        public Guid? SplitInvoiceGroupId { get; set; }

    }
    public class ReGenerateInvoiceInput
    {
        public long? InvoiceId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public dynamic CustomSectionPayload { get; set; }
        public Guid InvoiceActionId { get; set; }
        public Guid InvoiceTypeId { get; set; }
        public string PartnerId { get; set; }
        public DateTime IssueDate { get; set; }
        public Boolean IsAutomatic { get; set; }
    }
}
 
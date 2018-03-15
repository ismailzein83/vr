using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public class Invoice
    {
        public long InvoiceId { get; set; }

        public Guid InvoiceTypeId { get; set; }
        public long? SettlementInvoiceId { get; set; }
        public string PartnerId { get; set; }
        public int UserId { get; set; }
        public string SerialNumber { get; set; }

        public DateTime FromDate { get; set; }
        public Guid InvoiceSettingId { get; set; }
        public DateTime ToDate { get; set; }

        public DateTime IssueDate { get; set; }
        public DateTime? LockDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime CreatedTime { get; set; }
        public dynamic Details { get; set; }
        public DateTime? PaidDate { get; set; }
        public string Note { get; set; }
        public string SourceId { get; set; }
        public Boolean IsAutomatic { get; set; }
        public DateTime? SentDate { get; set; }
        public Guid? SplitInvoiceGroupId { get; set; }
        public InvoiceSettings Settings { get; set; }
        public Invoice() { }
        public IEnumerable<Invoice> GetInvoiceRDLCSchema()
        {
            return null;
        }
    } 
    public class InvoiceSettings
    {
        public long? FileId { get; set; }
    }
}

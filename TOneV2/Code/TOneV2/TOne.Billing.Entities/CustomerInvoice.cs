using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Billing.Entities
{
   public class CustomerInvoice
    {
        public int InvoiceId { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string SupplierId { get; set; }
        public string SupplierName { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string SerialNumber { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public string TimeZone { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? PaidDate { get; set; }
        public int NumberOfCalls { get; set; }
        public decimal Duration { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyId { get; set; }
        public string IsLocked { get; set; }
        public string IsPaid { get; set; }
        public string IsAutomatic { get; set; }
        public string IsSent { get; set; }
        public string InvoiceNotes { get; set; }
    }
}

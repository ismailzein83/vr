using System;

namespace BPMExtended.Main.SOMAPI
{
    public class Invoice
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string BillingAccountCode { get; set; }
        public DateTime EntryDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal InvoiceAmount { get; set; }
        public decimal OpenAmount { get; set; }
    }
}

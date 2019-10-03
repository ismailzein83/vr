using BPMExtended.Main.Entities;
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
        public decimal OriginalAmount { get; set; }
        public decimal OpenAmount { get; set; }
        public string DocumentCode { get; set; }
        public string LinePath { get; set; }
        public string DirectoryNumber { get; set; }
        public string ContractId { get; set; }
        public string InvoiceDate { get; set; }
        public string InvoiceNumber { get; set; }
        public string Currency { get; set; }
        public int BillDispute { get; set; }
    }
}
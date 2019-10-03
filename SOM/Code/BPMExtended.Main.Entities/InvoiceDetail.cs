using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public class InvoiceDetail
    {
        public string ContractId { get; set; }
        public string CustomerId { get; set; }
        public string InvoiceCode { get; set; }
        public string InvoiceDate { get; set; }
        public string DueDate { get; set; }
        public string OriginalAmount { get; set; }
        public string OpenAmount { get; set; }
        public string BillDispute { get; set; }
        public string CurrencyCode { get; set; }
    }
    public class BillOnDemandInvoiceDetail
    {
        public string CustomerID { get; set; }
        public string InvoiceCode { get; set; }
        public string InvoiceId { get; set; }
        public string InvoiceNumber { get; set; }
        public string BillCycle { get; set; }
        public string DueDate { get; set; }
        public string Resource { get; set; }
        public string InvoiceAccount { get; set; }
        public string Amount { get; set; }
        public string OpenAmount { get; set; }
        public string PhoneNumber { get; set; }
        public string URL { get; set; }
        public bool CollectionStatus { get; set; }
        public bool InvoiceInstallmentFlag { get; set; }
        public bool FinancialDisputes { get; set; }
    }
}

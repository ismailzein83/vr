using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public class InvoiceEntity
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
}

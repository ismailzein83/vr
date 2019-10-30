using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public class InvoiceAmountData
    {
        public string Id { get; set; }
        public string InvoiceCode { get; set; }
        public string DocumentType { get; set; }
        public DateTime EntryDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal OpenAmount { get; set; }
        public decimal AdditionalFees { get; set; }
        public decimal LateFee { get; set; }
        public string Currency { get; set; }
    }
}

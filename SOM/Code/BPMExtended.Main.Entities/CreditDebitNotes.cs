using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public class CreditDebitNotes
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string DocumentType { get; set; }
        public string BillingAccountCode { get; set; }
        public DateTime EntryDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Amount { get; set; }
        public decimal OpenAmount { get; set; }
    }
}

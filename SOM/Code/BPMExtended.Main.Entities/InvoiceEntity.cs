using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public class InvoiceEntity
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string BillingAccountCode { get; set; }
        public string EntryDate { get; set; }
        public string DueDate { get; set; }
        public string Amount { get; set; }
        public string OpenAmount { get; set; }
        public string DocumentCode { get; set; }
    }
}

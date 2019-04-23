using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public class PaymentDetail //User Story
    {
        public string PaymentCode { get; set; } // Code
        public string InvoiceCode { get; set; }//??
        public string PaymentDate { get; set; } // EntryDate?
        public string CashierUserName { get; set; } //GLAccount?
        public string PaymentType { get; set; } // PaymentMethodId (Id to Invoice/Cash) Enum
        public string CustomerID { get; set; }//??
    }
}
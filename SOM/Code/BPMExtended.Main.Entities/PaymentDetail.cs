using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public class PaymentDetail
    {
        public string PaymentCode { get; set; }
        public string InvoiceCode { get; set; }
        public string PaymentDate { get; set; }
        public string CashierUserName { get; set; }
        public string PaymentType { get; set; }
        public string CustomerID { get; set; }
    }
}

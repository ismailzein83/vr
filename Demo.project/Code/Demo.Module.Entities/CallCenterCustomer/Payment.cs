using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public class Payment
    {
        public long PaymentId { get; set; }
        public string Notes { get; set; }
        public string ReferenceNumber { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public DateTime PaymentDate { get; set; }

    }
}

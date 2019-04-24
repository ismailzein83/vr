using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public class PaymentData
    {
        public List<ServicePayment> Fees { get; set; }
        public List<ServicePayment> Services { get; set; }
        public List<ServicePayment> Deposits { get; set; }
        public bool IsPaid { get; set; }
    }
}

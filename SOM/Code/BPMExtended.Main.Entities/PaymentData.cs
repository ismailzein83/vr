using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public class PaymentData
    {
        public List<SaleService> Fees { get; set; }
        public List<VASService> Services { get; set; }
        public List<DepositDocument> Deposits { get; set; }
        public bool IsPaid { get; set; }
    }
}

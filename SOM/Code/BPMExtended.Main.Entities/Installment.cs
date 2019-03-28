using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public class Installment
    {
        public string Id { get; set; }
        public string PaymentPlanId { get; set; }
        public string Date { get; set; }
        public string Amount { get; set; }
        public string Currency { get; set; }

    }
}

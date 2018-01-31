using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public class TelephoneServiceDetail
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsMain { get; set; }
        public string Package { get; set; }
        public decimal SubscriptionFee { get; set; }
        public decimal UsageFee { get; set; }
    }
}

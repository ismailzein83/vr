using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMExtended.Main.Entities;

namespace BPMExtended.Main.Entities
{
    public class CommonInputArgument
    {
        public string ContactId { get; set; }
        public string AccountId { get; set; }
        public string ContractId { get; set; }
        public string CustomerId { get; set; }
        public string RequestId { get; set; }
        public string SwitchId { get; set; }
        public PaymentData PaymentData { get; set; }
    }
}

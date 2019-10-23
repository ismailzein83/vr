using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMExtended.Main.Entities;

namespace BPMExtended.Main.SOMAPI
{
    public class ADSLSubscriptionRequestInput
    {
        public CommonInputArgument CommonInputArgument { get; set; }
        public PaymentData PaymentData { get; set; }
        public string LinePathId { get; set; }
        public string ContractId { get; set; }
        public List<ContractService> ContractServices { get; set; }
        public string RequestId { get; set; }

    }
}

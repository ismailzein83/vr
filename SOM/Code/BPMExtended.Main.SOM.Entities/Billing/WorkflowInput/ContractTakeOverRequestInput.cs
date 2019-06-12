using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMExtended.Main.Entities;

namespace BPMExtended.Main.SOMAPI
{
    public class ContractTakeOverRequestInput
    {
        public string LinePathId { get; set; }
        public string ADSLContractId { get; set; }
        public string NewCustomerId { get; set; }
        public string OldUserName { get; set; }
        public string NewUserName { get; set; }
        public string NewPassword { get; set; }
        public string SubType { get; set; }
        public PaymentData PaymentData { get; set; }
        public CommonInputArgument CommonInputArgument { get; set; }

    }
}

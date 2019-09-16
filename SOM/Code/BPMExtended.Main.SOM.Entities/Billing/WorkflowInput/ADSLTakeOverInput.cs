using BPMExtended.Main.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.SOMAPI
{
    public class ADSLTakeOverInput
    {
        public CommonInputArgument CommonInputArgument { get; set; }
        public string NewCustomerId { get; set; }
        public string NewTelephonyContractId { get; set; }
        public string LinePathId { get; set; }
        public string CSO { get; set; }
        public string NewUserName { get; set; }
        public string NewPassword { get; set; }
        public PaymentData PaymentData { get; set; }
    }
}

using BPMExtended.Main.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.SOMAPI
{
    public class ADSLLineTerminationRequestInput
    {
        public CommonInputArgument CommonInputArgument { get; set; }
        public PaymentData PaymentData { get; set; }
        public string TelephonyContractId { get; set; }
        public string LinePathId { get; set; }
        public string Reason { get; set; }

    }
}

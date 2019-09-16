using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMExtended.Main.Entities;

namespace BPMExtended.Main.SOMAPI
{
    public class ADSLLineMovingRequestInput
    {
        public CommonInputArgument CommonInputArgument { get; set; }
        public string OldTelephonyContractId { get; set; }
        public string NewTelephonyContractId { get; set; }
        public string OldDSLAM { get; set; }
        public string NewDSLAM { get; set; }
        PaymentData PaymentData { get; set; }
        public Address Address { get; set; }

    }
}

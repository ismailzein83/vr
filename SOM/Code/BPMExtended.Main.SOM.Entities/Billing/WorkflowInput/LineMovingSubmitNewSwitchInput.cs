using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMExtended.Main.Entities;

namespace BPMExtended.Main.SOMAPI
{
    public class LineMovingSubmitNewSwitchInput
    {
        public CommonInputArgument CommonInputArgument { get; set; }
        public string NewPhoneNumber { get; set; }
        public string OldPhoneNumber { get; set; }
        public string OldTelLinePathId { get; set; }
        public string NewTelLinePathId { get; set; }
        public string NewADSLLinePathId { get; set; }
        public List<ServiceData> NotApplicableServices { get; set; }
        public bool SameSwitch { get; set; }
        public Address Address { get; set; }
        public bool HasADSL { get; set; }
        public PaymentData PaymentData { get; set; }
        public string ADSLContractId { get; set; }

    }
}

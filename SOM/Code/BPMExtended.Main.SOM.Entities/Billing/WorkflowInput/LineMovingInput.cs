using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMExtended.Main.Entities;

namespace BPMExtended.Main.SOMAPI
{
    public class LineMovingInput
    {
        public CommonInputArgument CommonInputArgument { get; set; }
        public string OldLinePathId { get; set; }
        public string NewLinePathId { get; set; }
        public bool IsVPN { get; set; }
        public bool SameSwitch { get; set; }
        public Address Address { get; set; }
        public PaymentData PaymentData { get; set; }
        public string ContractId { get; set; }
        public string RequestId { get; set; }
        public string LinePathId { get; set; }
        public string NewLinePath { get; set; }
        public string ADSLContractId { get; set; }
        public bool SameMDF { get; set; }
        public bool HasADSL { get; set; }
    }
}

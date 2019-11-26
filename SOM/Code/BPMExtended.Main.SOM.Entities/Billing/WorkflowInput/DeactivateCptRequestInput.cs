using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMExtended.Main.Entities;

namespace BPMExtended.Main.SOMAPI
{
    public class DeactivateCptRequestInput
    {
        public CommonInputArgument CommonInputArgument { get; set; }
        public string CPTServiceId { get; set; }
        public string LinePathId { get; set; }
        public string CPTNumber { get; set; }
    }
    public class FinalizeDeactivateCptRequestInput
    {
        public CommonInputArgument CommonInputArgument { get; set; }
        public string CPTServiceId { get; set; }
        public string CPTId { get; set; }
        public PaymentData PaymentData { get; set; }
    }
}

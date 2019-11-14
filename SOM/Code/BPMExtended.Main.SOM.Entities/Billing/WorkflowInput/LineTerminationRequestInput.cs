using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMExtended.Main.Entities;

namespace BPMExtended.Main.SOMAPI
{
    public class LineTerminationRequestInput
    {
        public CommonInputArgument CommonInputArgument { get; set; }
        public PaymentData PaymentData { get; set; }
        public string LinePathId { get; set; }
        public string OldLinePathId { get; set; }
        public bool HasADSL { get; set; }
        public string ADSLContract { get; set; }
        public string Reason { get; set; }
        public bool IsVPN { get; set; }

    }
}

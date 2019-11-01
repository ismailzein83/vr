using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMExtended.Main.Entities;

namespace BPMExtended.Main.SOMAPI
{
    public class AddProffessionalDIInput
    {
        public CommonInputArgument CommonInputArgument { get; set; }
        public PaymentData PaymentData { get; set; }
        public List<ServiceData> ServicesList { get; set; }
        public long AddressSequence { get; set; }
        public string GivenName { get; set; }
        public string BusinessType { get; set; }
    }
}

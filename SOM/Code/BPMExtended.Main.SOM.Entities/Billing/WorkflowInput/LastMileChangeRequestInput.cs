using BPMExtended.Main.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.SOMAPI
{
    public class LastMileChangeRequestInput
    {
        public string OldLinePathId { get; set; }
        public string NewLinePathId { get; set; }
        public CommonInputArgument CommonInputArgument { get; set; }
        public PaymentData PaymentData { get; set; }
        public Address Address { get; set; }
    }
}

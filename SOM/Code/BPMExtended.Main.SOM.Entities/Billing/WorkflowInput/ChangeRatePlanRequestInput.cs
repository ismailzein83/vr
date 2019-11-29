using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMExtended.Main.Entities;

namespace BPMExtended.Main.SOMAPI
{
    public class ChangeRatePlanRequestInput
    {
        public CommonInputArgument CommonInputArgument { get; set; }
        public PaymentData PaymentData { get; set; }
        public string NewRatePlanId { get; set; }
        public string LinePathId { get; set; }
        public List<ServicePackage> ServicesToRemove { get; set; }

    }
}

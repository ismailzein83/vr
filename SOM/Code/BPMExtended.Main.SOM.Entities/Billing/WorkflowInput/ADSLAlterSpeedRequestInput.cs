using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMExtended.Main.Entities;

namespace BPMExtended.Main.SOMAPI
{
    public class ADSLAlterSpeedRequestInput
    {
        public CommonInputArgument CommonInputArgument { get; set; }
        public PaymentData PaymentData { get; set; }
        public string NewRatePlan { get; set; }
        public string LinePathId{ get; set; }
        public string ContractId { get; set; }
        public string RequestId { get; set; }
        public List<ServicePackage> ServicesAreUnavailableInNewRP { get; set; }
        public List<ServicePackage> ServicesAreInDifferentPKGInNewRP { get; set; }
        public List<ServicePackage> ServicesAreCoreInNewRP { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMExtended.Main.Entities;

namespace BPMExtended.Main.SOMAPI
{
    public class ChangePhoneNumberRequestInput
    {
        public CommonInputArgument CommonInputArgument { get; set; }
        public string OldPhoneNumber { get; set; }
        public string NewPhoneNumber { get; set; }
        public string OldRatePlanId { get; set; }
        public string NewRatePlanId { get; set; }
        public string LinePathId { get; set; }
        public string OldDeviceId { get; set; }
        public List<SaleService> ServicesToRemove { get; set; }
        public PaymentData PaymentData { get; set; }

    }

}
    
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public class TelephonyContractOnHoldInput
    {
        public CommonInputArgument CommonInputArgument { get; set; }
        public string RatePlanId { get; set; }
        public string CSO { get; set; }
        public string SubType { get; set; }
        public string City { get; set; }
        public string ServiceResource { get; set; }

        public string PhoneNumber { get; set; }

        public string LinePathId { get; set; }
        public List<ContractService> ContractServices { get; set; }

    }
}

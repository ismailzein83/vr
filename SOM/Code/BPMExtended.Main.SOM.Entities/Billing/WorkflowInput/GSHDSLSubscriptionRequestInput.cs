using BPMExtended.Main.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.SOMAPI
{
    public class GSHDSLSubscriptionRequestInput
    {
        public CommonInputArgument CommonInputArgument { get; set; }
        public string LinePathId { get; set; }
        public string ContractId { get; set; }

        public List<ContractService> ContractServices { get; set; }
        public string RequestId { get; set; }
        public bool IsVPN { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

    }
}

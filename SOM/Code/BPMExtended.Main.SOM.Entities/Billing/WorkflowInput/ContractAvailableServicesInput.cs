using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.SOMAPI
{
    public class ContractAvailableServicesInput
    {
        public string RatePlanId { get; set; }
        public string ContractId { get; set; }
        public string LinePathId { get; set; }
        public List<string> ExcludedPackages { get; set; }
    }
}

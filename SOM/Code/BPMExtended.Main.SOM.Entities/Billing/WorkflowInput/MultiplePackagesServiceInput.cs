using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.SOMAPI
{
    public class MultiplePackagesServiceInput
    {
        public string RatePlanId { get; set; }
        public string SwitchId { get; set; }
        public List<string> ExcludedPackages { get; set; }

    }

}

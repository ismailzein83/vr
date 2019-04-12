using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public class ExcludePackagesServiceInput
    {
        public string RatePlanId { get; set; }
        public List<string> ExcludePackages { get; set; }

    }

}

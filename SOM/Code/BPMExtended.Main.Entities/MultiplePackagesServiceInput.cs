using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public class MultiplePackagesServiceInput
    {
        public string RatePlanId { get; set; }
        public List<string> Packages { get; set; }

    }

}

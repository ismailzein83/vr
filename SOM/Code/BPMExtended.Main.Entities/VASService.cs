using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public class VASService
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsNetwork { get; set; }
        public bool NeedProvisioning { get; set; }
    }
}

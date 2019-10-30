using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public class ServiceConsistensyCatalog
    {
        public Dictionary<string, List<ProhibitedService>> ProhibitedServices { get; set; }
        public Dictionary<string, List<RequiredService>> RequiredServices { get; set; }
    }

    public class UnconsistentServices
    {
        public Dictionary<string, string> RequiredServices { get; set; }
        public Dictionary<string,string> ProhibitedServices { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordAnalysis.Entities
{
    public class SwitchInterconnection
    {
        public List<SwitchInterconnectionTrunk> Trunks { get; set; }
        public List<SwitchInterconnectionTrunkGroups> TrunkGroups { get; set; }
    }

    public class SwitchInterconnectionTrunk
    {
        public string Trunk { get; set; }
    }

    public class SwitchInterconnectionTrunkGroups
    {
        public string Name { get; set; }
        public List<SwitchInterconnectionTrunk> Trunks { get; set; }

    }
}

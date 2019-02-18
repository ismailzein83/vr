using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordAnalysis.Entities
{
    public class C4SwitchInterconnection
    {
        public List<C4SwitchInterconnectionTrunk> Trunks { get; set; }
        public List<C4SwitchInterconnectionTrunkGroups> TrunkGroups { get; set; }
    }

    public class C4SwitchInterconnectionTrunk
    {
        public Guid TrunkId { get; set; }
        public string TrunkName { get; set; }
    }

    public class C4SwitchInterconnectionTrunkGroups
    {
        public string Name { get; set; }
        public List<C4SwitchInterconnectionTrunk> Trunks { get; set; }

    }
}

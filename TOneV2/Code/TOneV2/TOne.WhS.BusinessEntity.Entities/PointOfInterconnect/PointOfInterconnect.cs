using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class PointOfInterconnectEntity
    {
        public long PointOfInterconnectEntityId { get; set; }
        public string Name { get; set; }
        public int SwitchId { get; set; }
        public PointOfInterconnect Settings { get; set; }
    }
    public class PointOfInterconnect
    {
        public List<PointOfInterconnectTrunk> Trunks { get; set; }
    }
    public class PointOfInterconnectTrunk
    {   
        public string Trunk { get; set; }
    }
}

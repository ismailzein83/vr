using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class PointOfInterconnect
    {
        public List<PointOfInterconnectTrunk> Trunks { get; set; }
    }
    public class PointOfInterconnectTrunk
    {   
        public string Trunk { get; set; }
    }
}

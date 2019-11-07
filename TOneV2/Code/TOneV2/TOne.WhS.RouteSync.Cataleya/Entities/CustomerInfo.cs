using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.RouteSync.Cataleya.Entities
{
    public class CustomerInfo
    {
        public int CarrierId { get; set; }
        public int ZoneID { get; set; }
        public List<InTrunk> InTrunks { get; set; }
        public int? Version { get; set; }
    }
}

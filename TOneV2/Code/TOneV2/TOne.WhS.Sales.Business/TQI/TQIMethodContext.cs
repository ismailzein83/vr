using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Business
{
    public class TQIMethodContext : ITQIMethodContext
    {
        public RPRouteDetailByZone Route { get; set; }
        public int LongPrecision { get; set; }
        public decimal Rate { get; set; }
    }
}

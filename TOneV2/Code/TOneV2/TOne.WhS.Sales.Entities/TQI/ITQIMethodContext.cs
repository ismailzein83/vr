using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Sales.Entities
{
    public interface ITQIMethodContext
    {
        RPRouteDetailByZone Route { get; }
        int LongPrecision { get; }
        Decimal Rate { set; }
    }
}

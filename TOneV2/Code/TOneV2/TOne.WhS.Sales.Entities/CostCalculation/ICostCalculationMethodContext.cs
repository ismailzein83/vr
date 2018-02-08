using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Sales.Entities
{
    public interface ICostCalculationMethodContext
    {
        IEnumerable<long> ZoneIds { get; }

        RPRouteDetail Route { get; }

        int? NumberOfOptions { get; }

        Decimal Cost { set; }

        object CustomObject { get; set; }
    }
}

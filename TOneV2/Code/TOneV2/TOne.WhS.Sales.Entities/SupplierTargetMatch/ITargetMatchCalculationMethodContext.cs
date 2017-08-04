using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Sales.Entities
{
    public interface ITargetMatchCalculationMethodContext
    {
        RPRouteDetail RPRouteDetail { get; }

        RPRouteOptionDetail RPRouteOptionDetail { set; }
        decimal MarginValue { get; }
        MarginType MarginType { get; }

    }
}

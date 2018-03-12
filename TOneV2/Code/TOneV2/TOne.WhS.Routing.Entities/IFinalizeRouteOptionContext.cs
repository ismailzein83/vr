using System.Collections.Generic;

namespace TOne.WhS.Routing.Entities
{
    public interface IFinalizeRouteOptionContext
    {
        List<RouteOption> RouteOptions { get; set; }

        int? NumberOfOptionsInSettings { get; }
    }
}

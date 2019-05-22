using System.Collections.Generic;

namespace TOne.WhS.Routing.Entities
{
    public interface ICompleteSupplierDataRoute
    {
        List<List<RouteOption>> GetAvailableRouteOptionsList();
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.LCR.Entities.Routing;

namespace TOne.LCR.Data
{
    public interface IOldRoutingDataManager : IDataManager
    {
        List<RouteInfo> GetRoutes(string customerId, string target, TargetType targetType, int routesCount, int lcrCount);
    }
}

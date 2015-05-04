using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;

namespace TOne.LCR.Data.SQL
{
    public class OldRoutingDataManager : BaseTOneDataManager, IOldRoutingDataManager
    {
        public List<Entities.Routing.RouteInfo> GetRoutes(string customerId, string target, Entities.Routing.TargetType targetType, int routesCount, int lcrCount)
        {
            return new List<Entities.Routing.RouteInfo>();
        }


        const string query = @""; 
    }
}

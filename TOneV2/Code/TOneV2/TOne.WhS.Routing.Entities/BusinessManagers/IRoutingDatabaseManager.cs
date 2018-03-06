using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public interface IRoutingDatabaseManager : IRoutingManagerFactory
    {
        DateTime? GetLatestRoutingDatabaseEffectiveTime(RoutingProcessType routingProcessType, RoutingDatabaseType routingDatabaseType);
    }
}

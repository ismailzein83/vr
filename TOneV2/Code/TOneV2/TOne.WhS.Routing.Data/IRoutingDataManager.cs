using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Data
{
    public interface IRoutingDataManager : IDataManager
    {
        RoutingDatabase RoutingDatabase { get; set; }
        void FinalizeCustomerRouteDatabase(Action<string> trackStep, int commandTimeoutInSeconds);
    }
}

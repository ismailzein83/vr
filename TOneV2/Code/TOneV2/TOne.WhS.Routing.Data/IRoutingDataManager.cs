using System;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Data
{
    public interface IRoutingDataManager : IDataManager
    {
        RoutingDatabase RoutingDatabase { get; set; }
        void FinalizeCustomerRouteDatabase(Action<string> trackStep, int commandTimeoutInSeconds, int? maxDOP);
        void FinalizeRoutingProcess(IFinalizeRouteContext context, Action<string> trackStep);
    }
}

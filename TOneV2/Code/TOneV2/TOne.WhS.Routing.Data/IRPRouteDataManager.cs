using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;
using Vanrise.Data;

namespace TOne.WhS.Routing.Data
{
    public interface IRPRouteDataManager : IDataManager, IBulkApplyDataManager<RPRoute>, IRoutingDataManager
    {
        void ApplyProductRouteForDB(object preparedProductRoute);

        Vanrise.Entities.BigResult<Entities.RPRoute> GetFilteredRPRoutes(Vanrise.Entities.DataRetrievalInput<Entities.RPRouteQuery> input);

        Dictionary<Guid, IEnumerable<RPRouteOption>> GetRouteOptions(int routingProductId, long saleZoneId);

        Dictionary<int, RPRouteOptionSupplier> GetRouteOptionSuppliers(int routingProductId, long saleZoneId);

        IEnumerable<RPRoute> GetRPRoutes(IEnumerable<RPZone> rpZones);

        void FinalizeProductRoute(Action<string> trackStep, int commandTimeoutInSeconds);
    }

}

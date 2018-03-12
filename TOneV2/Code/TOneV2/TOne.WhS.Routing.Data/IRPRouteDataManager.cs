using System;
using System.Collections.Generic;
using TOne.WhS.Routing.Entities;
using Vanrise.Data;

namespace TOne.WhS.Routing.Data
{
    public interface IRPRouteDataManager : IDataManager, IBulkApplyDataManager<RPRoute>, IRoutingDataManager
    {
        void ApplyProductRouteForDB(object preparedProductRoute);

        IEnumerable<RPRoute> GetFilteredRPRoutesByZone(Vanrise.Entities.DataRetrievalInput<RPRouteQueryByZone> input);

        Dictionary<Guid, IEnumerable<RPRouteOption>> GetRouteOptions(int routingProductId, long saleZoneId);

        Dictionary<int, RPRouteOptionSupplier> GetRouteOptionSuppliers(int routingProductId, long saleZoneId);

        IEnumerable<RPRoute> GetRPRoutes(IEnumerable<RPZone> rpZones);

        void FinalizeProductRoute(Action<string> trackStep, int commandTimeoutInSeconds, int? maxDOP);

        IEnumerable<RPRouteByCode> GetFilteredRPRoutesByCode(Vanrise.Entities.DataRetrievalInput<RPRouteQueryByCode> input);
    }
}
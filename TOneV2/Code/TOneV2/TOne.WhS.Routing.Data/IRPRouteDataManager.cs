using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;
using Vanrise.Data;

namespace TOne.WhS.Routing.Data
{
    public interface IRPRouteDataManager : IDataManager, IBulkApplyDataManager<RPRouteByCustomer>, IRoutingDataManager
    {
        IEnumerable<RoutingCustomerInfo> RoutingCustomerInfo { get; set; }
        void ApplyProductRouteForDB(object preparedProductRoute);

        IEnumerable<BaseRPRoute> GetFilteredRPRoutesByZone(Vanrise.Entities.DataRetrievalInput<RPRouteQueryByZone> input);

        Dictionary<Guid, IEnumerable<RPRouteOption>> GetRouteOptions(int routingProductId, long saleZoneId, int? customerId);

        Dictionary<int, RPRouteOptionSupplier> GetRouteOptionSuppliers(int routingProductId, long saleZoneId);

        IEnumerable<BaseRPRoute> GetRPRoutes(IEnumerable<RPZone> rpZones, int? customerId);

        void FinalizeProductRoute(Action<string> trackStep, int commandTimeoutInSeconds, int? maxDOP);

        IEnumerable<RPRouteByCode> GetFilteredRPRoutesByCode(Vanrise.Entities.DataRetrievalInput<RPRouteQueryByCode> input);
    }
}
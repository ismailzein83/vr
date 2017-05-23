using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Data
{
    public interface IRoutingDataManager : IDataManager
    {
        RoutingDatabase RoutingDatabase { get; set; }

        void FinalizeCustomerRouteDatabase(Action<string> trackStep, int commandTimeoutInSeconds, int? maxDOP);

        void FinalizeRoutingProcess(IFinalizeRouteContext context, Action<string> trackStep);

        void StoreCarrierAccounts(List<CarrierAccountInfo> carrierAccounts);

        void StoreSaleZones(List<SaleZone> saleZones);
    }
}
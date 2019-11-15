using System.Collections.Generic;
using TOne.WhS.Routing.Entities;
using Vanrise.Data;

namespace TOne.WhS.Routing.Data
{
    public interface ICustomerRouteMarginStagingDataManager : IDataManager, IBulkApplyDataManager<CustomerRouteMarginStaging>
    {
        void CreateCustomerRouteMarginStagingTable(RoutingDatabaseType routingDatabaseType);

        void InsertCustomerRouteMarginStagingListToDB(RoutingDatabaseType routingDatabaseType, List<CustomerRouteMarginStaging> customerRouteMarginStagingList);

        List<CustomerRouteMarginStaging> GetCustomerRouteMarginStagingList(RoutingDatabaseType routingDatabaseType); 

        void DropTable(RoutingDatabaseType routingDatabaseType);
    }
}
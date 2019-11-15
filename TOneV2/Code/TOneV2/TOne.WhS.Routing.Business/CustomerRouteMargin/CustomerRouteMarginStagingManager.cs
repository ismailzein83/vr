using System.Collections.Generic;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business
{
    public class CustomerRouteMarginStagingManager
    {
        public void CreateCustomerRouteMarginStagingTable(RoutingDatabaseType routingDatabaseType)
        {
            ICustomerRouteMarginStagingDataManager dataManager = RoutingDataManagerFactory.GetDataManager<ICustomerRouteMarginStagingDataManager>();
            dataManager.CreateCustomerRouteMarginStagingTable(routingDatabaseType);
        }

        public void InsertCustomerRouteMarginStagingListToDB(RoutingDatabaseType routingDatabaseType, List<CustomerRouteMarginStaging> customerRouteMarginStagingList)
        {
            ICustomerRouteMarginStagingDataManager dataManager = RoutingDataManagerFactory.GetDataManager<ICustomerRouteMarginStagingDataManager>();
            dataManager.InsertCustomerRouteMarginStagingListToDB(routingDatabaseType, customerRouteMarginStagingList);
        }

        public List<CustomerRouteMarginStaging> GetCustomerRouteMarginStagingList(RoutingDatabaseType routingDatabaseType)
        {
            ICustomerRouteMarginStagingDataManager dataManager = RoutingDataManagerFactory.GetDataManager<ICustomerRouteMarginStagingDataManager>();
            return dataManager.GetCustomerRouteMarginStagingList(routingDatabaseType);
        }

        public void DropTable(RoutingDatabaseType routingDatabaseType) 
        {
            ICustomerRouteMarginStagingDataManager dataManager = RoutingDataManagerFactory.GetDataManager<ICustomerRouteMarginStagingDataManager>();
            dataManager.DropTable(routingDatabaseType);
        }
    }
}
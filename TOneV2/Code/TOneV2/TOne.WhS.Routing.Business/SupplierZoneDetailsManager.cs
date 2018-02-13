using System;
using System.Collections.Generic;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business
{
    public class SupplierZoneDetailsManager
    {
        public IEnumerable<SupplierZoneDetail> GetSupplierZoneDetailsByCode(string code)
        {
            ISupplierZoneDetailsDataManager supplierZoneDetailsDataManager = RoutingDataManagerFactory.GetDataManager<ISupplierZoneDetailsDataManager>();
            supplierZoneDetailsDataManager.RoutingDatabase = new RoutingDatabaseManager().GetLatestRoutingDatabase(RoutingProcessType.CustomerRoute, RoutingDatabaseType.Current);

            return supplierZoneDetailsDataManager.GetSupplierZoneDetailsByCode(code);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Data.SQL
{
    public class CarrierAccountDataManager : RoutingDataManager, ICarrierAccountDataManager
    {
        
        
        RoutingCustomerInfo RoutingCustomerInfoMapper(IDataReader reader)
        {
            return new RoutingCustomerInfo()
            {
                CustomerId = (int)reader["CustomerId"]
            };
        }

        RoutingSupplierInfo RoutingSupplierInfoMapper(IDataReader reader)
        {
            return new RoutingSupplierInfo()
            {
                SupplierId = (int)reader["SupplierId"]
            };
        }
    }
}

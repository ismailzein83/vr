using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public interface ICustomerZoneDetailsManager : IRoutingManagerFactory
    {
        IEnumerable<CustomerZoneDetail> GetCustomerZoneDetailsByZoneIdsAndCustomerIds(List<long> saleZoneIds, List<int> customerIds);
    }
}

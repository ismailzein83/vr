using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.RouteSync.Cataleya.Entities
{
    public class FinalCustomerData
    {
        public FinalCustomerData()
        {
            CustomerIdentificationsToAdd = new List<CustomerIdentification>();
            CustomerIdentificationsToDelete = new List<CustomerIdentification>();
        }

        public List<CustomerIdentification> CustomerIdentificationsToAdd { get; set; }
        public List<CustomerIdentification> CustomerIdentificationsToDelete { get; set; }
        public CarrierAccountMapping CarrierAccountMappingToAdd { get; set; }
        public CarrierAccountMapping CarrierAccountMappingToUpdate { get; set; }
    }
}
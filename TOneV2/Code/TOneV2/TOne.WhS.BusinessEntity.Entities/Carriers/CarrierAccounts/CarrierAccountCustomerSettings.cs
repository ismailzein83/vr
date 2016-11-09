using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{

    public class CarrierAccountCustomerSettings
    {
        public int? DefaultRoutingProductId { get; set; }
        public  RoutingStatus RoutingStatus { get; set; }
        public int TimeZoneId { get; set; }
        public bool IsAToZ { get; set; }
    }
}

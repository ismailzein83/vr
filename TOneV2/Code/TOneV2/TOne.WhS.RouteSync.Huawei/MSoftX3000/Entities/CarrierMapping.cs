using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.RouteSync.Huawei.Entities
{
    public class CarrierMapping
    {
        public int CarrierId { get; set; }

        public CustomerMapping CustomerMapping { get; set; }

        public SupplierMapping SupplierMapping { get; set; }
    }
     
    public class CustomerMapping
    {
        public string RSSN { get; set; }

        public string CSCName { get; set; }

        public int DNSet { get; set; } 
    }
      
    public class SupplierMapping
    {
        public string RouteName { get; set; }

        public string ISUP { get; set; }
    }
}
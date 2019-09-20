using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.RouteSync.Huawei.SoftX3000.Entities
{
    public class CarrierMapping
    {
        public int CarrierId { get; set; }

        public CustomerMapping CustomerMapping { get; set; }

        public SupplierMapping SupplierMapping { get; set; }
    }

    public class CustomerMapping
    {
        public int RSSC { get; set; }

        public int DNSet { get; set; }
    }

    public class SupplierMapping
    {
        public int SRT { get; set; }
    }
}
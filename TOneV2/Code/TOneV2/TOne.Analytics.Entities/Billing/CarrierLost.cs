using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class CarrierLost
    {
        public string CustomerID { get; set; }
        public string SupplierID { get; set; }
        public int SaleZoneID { get; set; }
        public int CostZoneID { get; set; }
        public decimal Duration { get; set; }
        public double CostNet { get; set; }
        public double SaleNet { get; set; }
    }
}

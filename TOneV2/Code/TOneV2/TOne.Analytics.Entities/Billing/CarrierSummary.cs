using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class CarrierSummary
    {

        public string SupplierID { get; set; }
        public string CustomerID { get; set; }
        public decimal? SaleDuration { get; set; }
        public decimal? CostDuration { get; set; }
        public double? CostNet { get; set; }
        public double? SaleNet { get; set; }
        public double? CostCommissionValue { get; set; }
        public double? SaleCommissionValue { get; set; }
        public double? CostExtraChargeValue { get; set; }
        public double? SaleExtraChargeValue { get; set; }

    }
}
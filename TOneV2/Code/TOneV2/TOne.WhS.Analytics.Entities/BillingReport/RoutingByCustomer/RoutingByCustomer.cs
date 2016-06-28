using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Analytics.Entities.BillingReport.RoutingByCustomer
{
    public class RoutingByCustomer
    {
        public DateTime CallDate { get; set; }

        public int SaleZone { get; set; }

        public int CostZone { get; set; }

        public string CustomerID { get; set; }

        public string SupplierID { get; set; }

        public double SaleRate { get; set; }

        public double CostRate { get; set; }

        public decimal SaleDuration { get; set; }

        public decimal CostDuration { get; set; }

        public double SaleNet { get; set; }

        public double CostNet { get; set; }

    }
}

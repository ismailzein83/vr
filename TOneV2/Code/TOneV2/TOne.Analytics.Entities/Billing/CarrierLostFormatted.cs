using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class CarrierLostFormatted
    {
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string SupplierID { get; set; }
        public string SupplierName { get; set; }
        public int SaleZoneID { get; set; }
        public string SaleZoneName { get; set; }
        public int CostZoneID { get; set; }
        public string CostZoneName { get; set; }
        public decimal Duration { get; set; }
        public string DurationFormatted { get; set; }
        public double CostNet { get; set; }
        public string CostNetFormatted { get; set; }
        public double SaleNet { get; set; }
        public string SaleNetFormatted { get; set; }
        public string Margin { get; set; }
        public string Percentage { get; set; }

    }
}

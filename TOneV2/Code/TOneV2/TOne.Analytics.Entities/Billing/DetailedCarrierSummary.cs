using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class DetailedCarrierSummary
    {
        public DateTime CallDate { get; set; }
        public string CustomerID { get; set; }
        public string SupplierID { get; set; }
        public int CostZoneID { get; set; }
        public int SaleZoneID { get; set; }
        public string Cost_Currency { get; set; }
        public string Sale_Currency { get; set; }
        public double Cost_Nets { get; set; }
        public double Sale_Nets { get; set; }
        public double Sale_Rate { get; set; }
        public double Cost_Rate { get; set; }
        public decimal SaleDuration { get; set; }
        public decimal CostDuration { get; set; }
        public string SaleZoneName { get; set; }
        public int SaleRateChange { get; set; }
        public DateTime SaleRateEffectiveDate { get; set; }
        public double SaleAmount { get; set; }
        public int ZoneID { get; set; }
        public string CostZoneName { get; set; }
        public string CostRateChange { get; set; }
        public DateTime CostRateEffectiveDate { get; set; }
        public double CostAmount { get; set; }
        public double profit { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class DetailedCarrierSummary
    {
        public string CustomerID { get; set; }
        public int SaleZoneID { get; set; }
        public string SaleZoneName { get; set; }
        public decimal SaleDuration { get; set; }
        public double SaleRate { get; set; }
        public Int16 SaleRateChange { get; set; }
        public DateTime SaleRateEffectiveDate { get; set; }
        public double SaleAmount { get; set; }
        public string SupplierID { get; set; }
        public int CostZoneID { get; set; }
        public string CostZoneName { get; set; }
        public decimal CostDuration { get; set; }
        public double CostRate { get; set; }
        public Int16 CostRateChange { get; set; }
        public DateTime CostRateEffectiveDate { get; set; }
        public double CostAmount { get; set; }
        public double Profit { get; set; }

    }
}

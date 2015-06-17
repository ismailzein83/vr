using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class DetailedCarrierSummaryFormatted
    {
        public string CustomerID { get; set; }
        public string Customer { get; set; }
        public int SaleZoneID { get; set; }
        public string SaleZoneName { get; set; }
        public decimal SaleDuration { get; set; }
        public string SaleDurationFormatted { get; set; }
        public double SaleRate { get; set; }
        public string SaleRateFormatted { get; set; }
        public int SaleRateChange { get; set; }
        public string SaleRateChangeFormatted { get; set; }
        public DateTime SaleRateEffectiveDate { get; set; }
        public double SaleAmount { get; set; }
        public string SaleAmountFormatted { get; set; }
        public string SupplierID { get; set; }
        public string Supplier { get; set; }
        public int CostZoneID { get; set; }
        public string CostZoneName { get; set; }
        public decimal CostDuration { get; set; }
        public string CostDurationFormatted { get; set; }
        public double CostRate { get; set; }
        public string CostRateFormatted { get; set; }
        public int CostRateChange { get; set; }
        public string CostRateChangeFormatted { get; set; }
        public DateTime CostRateEffectiveDate { get; set; }
        public double CostAmount { get; set; }
        public string CostAmountFormatted { get; set; }
        public double Profit { get; set; }
        public string ProfitFormatted { get; set; }
        public string ProfitPercentage { get; set; }
    }
}

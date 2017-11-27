using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Analytics.Entities.BillingReport
{
    public class CarrierSummaryFormatted
    {
        public string SupplierID { get; set; }
        public string Supplier { get; set; }
        public string CustomerID { get; set; }
        public string Customer { get; set; }
        public string SaleZoneName { get; set; }
        public string CostZoneName { get; set; }
        public double SaleAmount { get; set; }
        public string SaleAmountFormatted { get; set; }
        public double CostAmount { get; set; }
        public string CostAmountFormatted { get; set; }
        public double SaleRate { get; set; }
        public string SaleRateFormatted { get; set; }
        public double CostRate { get; set; }
        public int? SaleRateType { get; set; }
        public int? CostRateType { get; set; }
        public string SaleRateTypeFormatted { get; set; }
        public string CostRateTypeFormatted { get; set; }
        public string CostRateFormatted { get; set; }
        public decimal? SaleDuration { get; set; }
        public string SaleDurationFormatted { get; set; }
        public decimal? CostDuration { get; set; }
        public string CostDurationFormatted { get; set; }
        public double? CostNet { get; set; }
        public string CostNetFormatted { get; set; }
        public double? SaleNet { get; set; }
        public string SaleNetFormatted { get; set; }
        public double? CostCommissionValue { get; set; }
        public string CostCommissionValueFormatted { get; set; }
        public double? SaleCommissionValue { get; set; }
        public string SaleCommissionValueFormatted { get; set; }
        public double? CostExtraChargeValue { get; set; }
        public string CostExtraChargeValueFormatted { get; set; }
        public double? SaleExtraChargeValue { get; set; }
        public string SaleExtraChargeValueFormatted { get; set; }
        public double? Profit { get; set; }
        public string ProfitFormatted { get; set; }
        public string ProfitPercentage { get; set; }
        public string SaleRateChange { get; set; }
        public string SaleRateChangeFormatted { get; set; }
        public string CostRateChange { get; set; }
        public string CostRateChangeFormatted { get; set; }
        public decimal? AvgMin { get; set; }
        public string AvgMinFormatted { get; set; }
        public DateTime? SaleRateEffectiveDate { get; set; }
        public DateTime? CostRateEffectiveDate { get; set; }

        public CarrierSummaryFormatted() { }
        public IEnumerable<CarrierSummaryFormatted> GetCarrierSummaryRDLCSchema()
        {
            return null;
        }
    }
}

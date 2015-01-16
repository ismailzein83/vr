using System;

namespace TABS.DTO
{
    public class DTO_BillingStats
    {
        public DateTime CallDate { get; set; }
        public CarrierAccount Customer { get; set; }
        public CarrierAccount Supplier { get; set; }
        public Zone CostZone { get; set; }
        public Zone SaleZone { get; set; }
        public Currency Cost_Currency { get; set; }
        public Currency Sale_Currency { get; set; }
        public decimal Durations { get; set; }
        public int NumberOfCalls { get; set; }
        public string FirstCallTime { get; set; }
        public string LastCallTime { get; set; }
        public decimal MinDuration { get; set; }
        public decimal MaxDuration { get; set; }
        public decimal AvgDuration { get; set; }
        public double Cost_Nets { get; set; }
        public double Cost_Discounts { get; set; }
        public double Cost_Commissions { get; set; }
        public double Cost_ExtraCharges { get; set; }
        public double Sale_Nets { get; set; }
        public double Sale_Discounts { get; set; }
        public double Sale_Commissions { get; set; }
        public double Sale_ExtraCharges { get; set; }
        public decimal SaleDuration { get; set; }
        public double Sale_Rate { get; set; }
        public double Cost_Rate { get; set; }
        public short SaleRateType { get; set; }
        public short CostRateType { get; set; }
        public decimal CostDuration { get; set; }
    }
}

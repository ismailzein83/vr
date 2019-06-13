using System;
using System.Collections.Generic;

namespace TOne.WhS.Deal.Entities
{
    public class SwapDealAnalysisOutbound
    {
        public OutboundCollection SwapDealAnalysisOutbounds { get; set; }
    }
    public class OutboundCollection : List<Outbound>
    {

    }
    public class Outbound
    {
        public int CountryId { get; set; }
        public decimal CurrentRate { get; set; }
        public int DailyVolume { get; set; }
        public int Volume { get; set; }
        public decimal DealRate { get; set; }
        public SwapDealAnalysisOutboundItemRateCalcMethod ItemCalculationMethod { get; set; }
        public Guid CalculationMethodId { get; set; }
        public List<long> SupplierZoneIds { get; set; }
        public string GroupName { get; set; }
        public decimal Revenue { get; set; }
        public decimal Savings { get; set; }
    }
}

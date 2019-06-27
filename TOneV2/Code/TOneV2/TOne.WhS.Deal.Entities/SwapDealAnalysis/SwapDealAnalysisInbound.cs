using System;
using System.Collections.Generic;

namespace TOne.WhS.Deal.Entities
{
    public class SwapDealAnalysisInbound
    {
        public InboundCollection SwapDealAnalysisInbounds { get; set; }
    }
    public class InboundCollection : List<Inbound>
    {

    }
    public class Inbound
    {
        public int ZoneGroupNumber { get; set; }
        public int CountryId { get; set; }
        public string GroupName { get; set; }
        public decimal CurrentRate { get; set; }
        public int DailyVolume { get; set; }
        public int Volume { get; set; }
        public decimal DealRate { get; set; }
        public Decimal? RateProfit { get; set; }
        public Decimal? Profit { get; set; } 
        public Decimal Revenue { get; set; }
        public SwapDealAnalysisInboundItemRateCalcMethod ItemCalculationMethod { get; set; }
        public Guid CalculationMethodId { get; set; }
        public List<long> SaleZoneIds { get; set; }

    }
}

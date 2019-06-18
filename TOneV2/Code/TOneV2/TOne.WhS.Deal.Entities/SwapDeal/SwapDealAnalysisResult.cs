using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Deal.Entities
{
    public class SwapDealAnalysisResult
    {
        public List<SwapDealAnalysisResultInbound> Inbounds { get; set; }

        public List<SwapDealAnalysisResultOutbound> Outbounds { get; set; }

        public int DealPeriodInDays { get; set; }

        public Decimal TotalCostRevenue { get; set; }

        public Decimal TotalSaleRevenue { get; set; }

        public Decimal? TotalCostMargin { get; set; }

        public Decimal? TotalSaleMargin { get; set; }

        public Decimal? OverallProfit { get; set; }

        public Decimal? Margins { get; set; }

        public Decimal OverallRevenue { get; set; }
    }

    public class SwapDealAnalysisResultInbound
    {
        public string GroupName { get; set; }
        public int Volume { get; set; }
        public int DailyVolume { get; set; }
        public Decimal? DealRate { get; set; }
        public Decimal? CurrentRate { get; set; }
        public Decimal? RateProfit { get; set; }
        public Decimal? Profit { get; set; }
        public Decimal Revenue { get; set; }
        public Guid CalculationMethodId { get; set; }
        public SwapDealAnalysisInboundItemRateCalcMethod ItemCalculationMethod { get; set; }
        public List<long> SaleZoneIds { get; set; }
        public int CountryId { get; set; }
    }

    public class SwapDealAnalysisResultOutbound
    {
        public string GroupName { get; set; }
        public int Volume { get; set; }
        public Decimal? DealRate { get; set; }
        public int DailyVolume { get; set; }
        public Decimal? CurrentRate { get; set; }
        public Decimal RateSavings { get; set; }
        public Decimal Savings { get; set; }
        public Decimal Revenue { get; set; }
        public Guid CalculationMethodId { get; set; }
        public SwapDealAnalysisOutboundItemRateCalcMethod ItemCalculationMethod { get; set; }
        public List<long> SupplierZoneIds { get; set; }
        public int CountryId { get; set; }
    }
}

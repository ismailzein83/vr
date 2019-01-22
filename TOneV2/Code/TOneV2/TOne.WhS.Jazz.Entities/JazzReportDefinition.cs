using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Jazz.Entities
{
    public enum ReportDefinitionDirectionEnum  { In = 0,Out = 1 }
    public enum RateCalculationTypeEnum { FullRate = 0,FixedRate = 1,PartialRate = 2 }
    public class JazzReportDefinition
    {
        public Guid JazzReportDefinitionId { get; set; }
        public string Name  { get; set; }
        public ReportDefinitionDirectionEnum Direction { get; set; }
        public int SwitchId { get; set; }
        public AmountCalculation AmountCalculation { get; set; }
        public bool DivideByMarket { get; set; }
        public MarketSettings MarketSettings { get; set; }
        public bool DivideByRegion { get; set; }
        public RegionSettings RegionSettings { get; set; }
        public bool CreateTax { get; set; }
        public decimal TaxPercentage { get; set; }
    }
    public class AmountCalculation
    {
        public RateCalculationTypeEnum RateCalculationType { get; set; }
        public decimal? FixedRateValue { get; set; }
    }
    public class MarketSettings
    {
        public List<MarketOption> MarketOptions { get; set; }

    }
    public class MarketOption
    {
        public Guid MarketCodeId { get; set; }
        public Guid CustomerTypeCodeId { get; set; }
        public decimal Percentage { get; set; }
    }
    public class RegionSettings
    {
        public List<RegionOption> RegionOptions { get; set; }

    }
    public class RegionOption
    {
        public Guid RegionCodeId { get; set; }
        public decimal Percentage { get; set; }
    }
}

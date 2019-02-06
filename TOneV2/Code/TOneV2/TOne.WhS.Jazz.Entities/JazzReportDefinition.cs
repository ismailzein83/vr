using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
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
        public JazzReportDefinitionSettings Settings { get; set; }
        public bool IsEnabled { get; set; }

    }
    public class JazzReportDefinitionSettings
    {
        public AmountCalculation AmountCalculation { get; set; }
        public MarketSettings MarketSettings { get; set; }
        public RegionSettings RegionSettings { get; set; }
        public bool CreateTax { get; set; }
        public decimal TaxPercentage { get; set; }
        public RecordFilter TaxFilter { get; set; }
        public RecordFilter ReportFilter { get; set; }
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
namespace TOne.WhS.Jazz.Entities
{
    public enum ReportDefinitionDirection  { In = 0,Out = 1 }
    public enum AmountType { FixedRate = 1, PartialAmount = 2 }
    public enum AmountMeasureType { AMT=0}
    public enum TaxOption { TaxMeasure=1,ZeroTax=2}
    public class JazzReportDefinition
    {
        public Guid JazzReportDefinitionId { get; set; }
        public string Name { get; set; }
        public ReportDefinitionDirection Direction { get; set; }
        public int SwitchId { get; set; }
        public TaxOption? TaxOption { get; set; }
        public AmountType? AmountType { get; set; }
        public int? CurrencyId { get; set; }
        public AmountMeasureType? AmountMeasureType { get; set; }
        public decimal? SplitRateValue { get; set; }
        public JazzReportDefinitionSettings Settings { get; set; }
        public bool IsEnabled { get; set; }

    }
    public class JazzReportDefinitionSettings
    {
        public MarketSettings MarketSettings { get; set; }
        public RegionSettings RegionSettings { get; set; }
        public RecordFilterGroup ReportFilter { get; set; }

    }


    public class MarketSettings
    {
        public List<MarketOption> MarketOptions { get; set; }

    }
    public class MarketOption
    {
        public Guid MarketId { get; set; }
        public Guid CustomerTypeId { get; set; }
        public decimal Percentage { get; set; }
    }
    public class RegionSettings
    {
        public List<RegionOption> RegionOptions { get; set; }

    }
    public class RegionOption
    {
        public Guid RegionId { get; set; }
        public decimal Percentage { get; set; }
    }
}

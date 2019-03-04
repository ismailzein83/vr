using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
namespace TOne.WhS.Jazz.Entities
{
    public enum ReportDefinitionDirectionEnum  { In = 0,Out = 1 }
    public enum AmountTypeEnum { FixedRate = 1, PartialAmount = 2 }
    public enum AmountMeasureTypeEnum { AMT=0}
    public enum TaxOptionEnum { TaxMeasure=1,ZeroTax=2}
    public class JazzReportDefinition
    {
        public Guid JazzReportDefinitionId { get; set; }
        public string Name { get; set; }
        public ReportDefinitionDirectionEnum Direction { get; set; }
        public int SwitchId { get; set; }
        public TaxOptionEnum? TaxOption { get; set; }
        public AmountTypeEnum? AmountType { get; set; }
        public int? CurrencyId { get; set; }
        public AmountMeasureTypeEnum? AmountMeasureType { get; set; }
        public decimal? SplitRateValue { get; set; }
        public JazzReportDefinitionSettings Settings { get; set; }
        public bool IsEnabled { get; set; }

    }
    public class JazzReportDefinitionSettings
    {
        public MarketSettings MarketSettings { get; set; }
        public RegionSettings RegionSettings { get; set; }
        public RecordFilter ReportFilter { get; set; }

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Jazz.Entities;

namespace TOne.WhS.Jazz.BP.Activities
{
    public class JazzReport
    {
        public string ReportName { get; set; }
        public ReportDefinitionDirectionEnum Direction { get; set; }

        public List<JazzReportData> ReportData { get; set; }
    }
    public class JazzReportData
    {
        public int CarrierAccountId { get; set; }
        public string CarrierAccountName { get; set; }
        public decimal Duration { get; set; }
        public decimal Amount { get; set; }

        public decimal Amount2 { get; set; }
        public List<JazzReportMarket> Markets { get; set; }
      
    }
    public class JazzReportMarket
    {
        public Guid MarketId { get; set; }
        public string MarketName { get; set; }
        public Guid CustomerTypeId { get; set; }
        public string CustomerTypeName { get; set; }
        public decimal MarketValue { get; set; }
        public decimal MarketValue2 { get; set; }
        public decimal Percentage { get; set; }
        public List<JazzReportRegion> Regions { get; set; }
    }
    public class JazzReportRegion
    {
        public Guid RegionId { get; set; }
        public string RegionName { get; set; }
        public decimal RegionValue { get; set; }
        public decimal RegionValue2 { get; set; }
        public decimal Percentage { get; set; }

    }
}

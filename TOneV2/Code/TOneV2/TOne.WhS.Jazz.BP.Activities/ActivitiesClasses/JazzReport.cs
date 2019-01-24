using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Jazz.BP.Activities
{
    public class JazzReport
    {
        public string ReportName { get; set; }
        public List<JazzReportData> ReportData { get; set; }
    }
    public class JazzReportData
    {
        public int CarrierAccountId { get; set; }
        public decimal Duration { get; set; }
        public decimal Amount { get; set; }
        public Guid MarketId { get; set; }
        public Guid RegionId { get; set; }
        public decimal MarketValue { get; set; }
        public decimal RegionValue { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Entities;

namespace TOne.Analytics.Business.BillingReports
{
    public class ZoneProfitReportGenerator : TOne.Entities.IReportGenerator
    {
        public Dictionary<string, System.Collections.IEnumerable> GenerateDataSources(TOne.Entities.ReportParameters parameters)
        {
            BillingStatisticManager manager = new BillingStatisticManager();
            List<ZoneProfitFormatted> zoneProfit = manager.GetZoneProfit(parameters.FromTime, parameters.ToTime, true);
            Dictionary<string, System.Collections.IEnumerable> dataSources = new Dictionary<string, System.Collections.IEnumerable>();
            dataSources.Add("ZoneProfit", zoneProfit);
            return dataSources;
        }
    }
}

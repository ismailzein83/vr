using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Data;
using TOne.Analytics.Entities;

namespace TOne.Analytics.Business
{
    public class TrafficMonitorManager
    {
        public Vanrise.Entities.IDataRetrievalResult<GroupSummary<TrafficStatistic>> GetTrafficStatisticSummary(Vanrise.Entities.DataRetrievalInput<TrafficStatisticSummaryInput> input)
        {
            ITrafficMonitorDataManager dataManager = AnalyticsDataManagerFactory.GetDataManager<ITrafficMonitorDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetTrafficStatisticSummary(input));
        }

        public IEnumerable<TrafficStatistic> GetTrafficStatistics(TrafficStatisticGroupKeys filterByColumn, string columnFilterValue, DateTime from, DateTime to)
        {
            ITrafficMonitorDataManager dataManager = AnalyticsDataManagerFactory.GetDataManager<ITrafficMonitorDataManager>();
            return dataManager.GetTrafficStatistics(filterByColumn, columnFilterValue, from, to);
        }
    }
}

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
    public class TrafficStatisticManager
    {
        public TrafficStatisticSummaryBigResult GetTrafficStatisticSummary(Vanrise.Entities.DataRetrievalInput<TrafficStatisticSummaryInput> input)
        {
            ITrafficStatisticDataManager dataManager = AnalyticsDataManagerFactory.GetDataManager<ITrafficStatisticDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetTrafficStatisticSummary(input)) as TrafficStatisticSummaryBigResult;
        }

        public IEnumerable<TrafficStatistic> GetTrafficStatistics(TrafficStatisticGroupKeys filterByColumn, string columnFilterValue, DateTime from, DateTime to)
        {
            ITrafficStatisticDataManager dataManager = AnalyticsDataManagerFactory.GetDataManager<ITrafficStatisticDataManager>();
            return dataManager.GetTrafficStatistics(filterByColumn, columnFilterValue, from, to);
        }
    }
}

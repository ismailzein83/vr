using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TOne.Analytics.Business;
using TOne.Analytics.Entities;

namespace TOne.Analytics.Web.Controllers
{
    public class TrafficMonitorController : Vanrise.Web.Base.BaseAPIController
    {
        private readonly AnalyticsManager _analyticsManager;
        public TrafficMonitorController()
        {
            _analyticsManager = new AnalyticsManager();
        }

        #region Mobile

        [HttpGet]
        public List<TOne.Analytics.Entities.TopNDestinationView> GetTopNDestinations(DateTime fromDate, DateTime toDate, int from, int to, string sortOrder = "DESC"
            , char groupByCodeGroup = 'N', char showSupplier = 'N', string customerID = null, string supplierID = null, int? switchID = null, string codeGroup = null, int? topCount = null)
        {
            string orderTarget = "Quantity";
            return _analyticsManager.GetTopNDestinations(fromDate, toDate, sortOrder, customerID, supplierID, switchID, groupByCodeGroup, codeGroup, showSupplier, orderTarget, from, to, topCount);
        }
       
        #endregion

        #region Traffic Statistic

        [HttpPost]
        public object GetTrafficStatisticSummary(Vanrise.Entities.DataRetrievalInput<TrafficStatisticSummaryInput> input)
        {
            System.Threading.Thread.Sleep(1000);
            TrafficMonitorManager manager = new TrafficMonitorManager();
            return GetWebResponse(input,manager.GetTrafficStatisticSummary(input));
        }
        [HttpGet]
        public IEnumerable<TrafficStatistic> GetTrafficStatistics(TrafficStatisticGroupKeys filterByColumn, string columnFilterValue, DateTime from, DateTime to)
        {
            TrafficMonitorManager manager = new TrafficMonitorManager();
            return manager.GetTrafficStatistics(filterByColumn, columnFilterValue, from, to);
        }
 

        [HttpPost]
        public object GetReleaseCodeStatistics(Vanrise.Entities.DataRetrievalInput<TrafficStatisticSummaryInput> input)
        {
            System.Threading.Thread.Sleep(1000);
            TrafficMonitorManager manager = new TrafficMonitorManager();
            return GetWebResponse(input, manager.GetReleaseCodeStatistics(input));
        }

        


        #endregion

    }


    #region Argument Classes
    public class GetTrafficStatisticSummaryInput
    {
        public string TempTableKey { get; set; }
        public GenericFilter Filter { get; set; }

        public bool WithSummary { get; set; }

        public TrafficStatisticGroupKeys[] GroupKeys { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int FromRow { get; set; }
        public int ToRow { get; set; }
        public TrafficStatisticMeasures OrderBy { get; set; }
        public bool IsDescending { get; set; }
        public List<string> Headers { get; set; }
    }
    #endregion
}

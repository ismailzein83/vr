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
    public class AnalyticsController : Vanrise.Web.Base.BaseAPIController
    {
        private readonly AnalyticsManager _analyticsManager;
        public AnalyticsController()
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
        public TrafficStatisticSummaryBigResult GetTrafficStatisticSummary(GetTrafficStatisticSummaryInput input)
        {
            System.Threading.Thread.Sleep(1000);
            TrafficStatisticManager manager = new TrafficStatisticManager();
            return manager.GetTrafficStatisticSummary(input.TempTableKey, input.Filter, input.WithSummary, input.GroupKeys, input.From, input.To, input.FromRow, input.ToRow, input.OrderBy, input.IsDescending);
        }
        [HttpGet]
        public IEnumerable<TrafficStatistic> GetTrafficStatistics(TrafficStatisticGroupKeys filterByColumn, string columnFilterValue, DateTime from, DateTime to)
        {
            TrafficStatisticManager manager = new TrafficStatisticManager();
            return manager.GetTrafficStatistics(filterByColumn, columnFilterValue, from, to);
        }
        [HttpPost]
        public HttpResponseMessage ExportTrafficStatisticSummary(GetTrafficStatisticSummaryInput input)
        {
            TrafficStatisticManager manager = new TrafficStatisticManager();
            TrafficStatisticSummaryBigResult records = manager.GetTrafficStatisticSummary(input.TempTableKey, input.Filter, input.WithSummary, input.GroupKeys, input.From, input.To, input.FromRow, input.ToRow, input.OrderBy, input.IsDescending);
            var groupKeys = Enum.GetNames(typeof(TrafficStatisticGroupKeys));


            return manager.ExportTrafficStatisticSummary(records, input.Headers,input.GroupKeys);
        }

        #endregion

    }


    #region Argument Classes
    public class GetTrafficStatisticSummaryInput
    {
        public string TempTableKey { get; set; }
        public TrafficStatisticFilter Filter { get; set; }

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TOne.Analytics.Business;
using TOne.Analytics.Entities;

namespace TOne.Web.Online.Controllers
{
    public class AnalyticsController : ApiController
    {
        private readonly AnalyticsManager _analyticsManager;
        public AnalyticsController()
        {
            _analyticsManager = new AnalyticsManager();
        }

        [HttpGet]
        public List<TOne.Analytics.Entities.TopNDestinationView> GetTopNDestinations(DateTime fromDate, DateTime toDate, int from, int to, string sortOrder = "DESC"
            , char groupByCodeGroup = 'N', char showSupplier = 'N', string customerID = null, string supplierID = null, int? switchID = null, string codeGroup = null, int? topCount = null)
        {
            string orderTarget = "Quantity";
            return _analyticsManager.GetTopNDestinations(fromDate, toDate, sortOrder, customerID, supplierID, switchID, groupByCodeGroup, codeGroup, showSupplier, orderTarget, from, to, topCount);
        }

        [HttpGet]
        public List<TOne.Analytics.Entities.TopNDestinationView> GetTopNDestinationsQuality(DateTime fromDate, DateTime toDate, int from, int to, string sortOrder = "DESC", char groupByCodeGroup = 'N'
            , char showSupplier = 'N', string customerID = null, string supplierID = null, int? switchID = null, string codeGroup = null, int? topCount = null)
        {
            string orderTarget = "Quality";
            return _analyticsManager.GetTopNDestinations(fromDate, toDate, sortOrder, customerID, supplierID, switchID, groupByCodeGroup, codeGroup, showSupplier, orderTarget, from, to, topCount);
        }

        [HttpGet]
        public List<TOne.Analytics.Entities.AlertView> GetAlerts(int from, int to, int? topCount = null, int? alertLevel = null, char showHiddenAlerts = 'N', string tag = null, string source = null, int? userID = null)
        {

            return _analyticsManager.GetAlerts(from, to, topCount, showHiddenAlerts, alertLevel, tag, source, userID);
        }

        [HttpGet]
        public List<TOne.Analytics.Entities.CarrierRateView> GetRates(TOne.BusinessEntity.Entities.CarrierType carrierType, DateTime effectiveOn, string carrierID, int from, int to, string code = null, string zoneName = null, string codeGroup = null)
        {
            return _analyticsManager.GetRates(carrierType.ToString(), effectiveOn, carrierID, codeGroup, code, zoneName, from, to);
        }
        [HttpPost]
        public bool UpdateRateServiceFlag([FromBody]string parameters)
        {
            return _analyticsManager.UpdateRateServiceFlag(parameters);
        }
        [HttpGet]
        public List<TOne.Analytics.Entities.CarrierSummaryView> GetCarrierSummary(TOne.BusinessEntity.Entities.CarrierType carrierType, DateTime fromDate, DateTime toDate, int from, int to, char groupByProfile = 'N', string customerID = null, string supplierID = null, int? topCount = null)
        {
            return _analyticsManager.GetCarrierSummary(carrierType.ToString(), fromDate, toDate, customerID, supplierID, topCount, groupByProfile, from, to);
        }

        [HttpGet]
        public List<TOne.Analytics.Entities.TopCarriersView> GetTopCustomers(DateTime fromDate, DateTime toDate, int topCount)
        {
            return _analyticsManager.GetTopCustomers(fromDate, toDate, topCount);
        }

        [HttpGet]
        public List<TOne.Analytics.Entities.TopCarriersView> GetTopSuppliers(DateTime fromDate, DateTime toDate, int topCount)
        {
            return _analyticsManager.GetTopSuppliers(fromDate, toDate, topCount);
        }

        [HttpGet]
        public List<TOne.Analytics.Entities.ProfitByWeekDayView> GetLastTwoWeeksProfit(DateTime fromDate, DateTime toDate)
        {
            //if (toDate.Subtract(fromDate).TotalDays > 14)
            //   throw new  HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Date Range must be 14 days or less"));

            return _analyticsManager.GetLastWeeksProfit(fromDate, toDate);
        }

        [HttpGet]
        public TOne.Analytics.Entities.TrafficSummaryView GetSummary(DateTime fromDate, DateTime toDate)
        {
            return _analyticsManager.GetSummary(fromDate, toDate);
        }

        [HttpGet]
        public TOne.Analytics.Entities.TrafficSummaryView GetSummaryOneDay(DateTime day)
        {
            return _analyticsManager.GetSummary(day, day.AddDays(1));
        }
    }
}

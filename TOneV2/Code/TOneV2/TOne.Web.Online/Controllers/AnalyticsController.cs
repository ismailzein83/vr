using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TOne.Analytics.Business;

namespace TOne.Web.Online.Controllers
{
    public class AnalyticsController : ApiController
    {
        private readonly AnalyticsManager _analyticsManager;
        public AnalyticsController()
        {
            _analyticsManager = new AnalyticsManager() ;
        }

        [HttpGet]
        public List<TOne.Analytics.Entities.TopNDestinationView> GetTopNDestinations(int topCount, DateTime fromDate, DateTime toDate
            , int from, int to, string sortOrder = "DESC", char groupByCodeGroup = 'N', char showSupplier = 'N', string customerID = null, string supplierID = null, int? switchID = null, string codeGroup = null)
        {
            string orderTarget = "Quantity";
            return _analyticsManager.GetTopNDestinations(topCount, fromDate, toDate, sortOrder, customerID, supplierID, switchID, groupByCodeGroup, codeGroup, showSupplier, orderTarget, from, to);
        }

        [HttpGet]
        public List<TOne.Analytics.Entities.TopNDestinationView> GetTopNDestinationsQuality(int topCount, DateTime fromDate, DateTime toDate, string sortOrder, char groupByCodeGroup
            , char showSupplier, int from, int to, string customerID = null, string supplierID = null, int? switchID = null, string codeGroup = null)
        {
            string orderTarget = "Quality";
            return _analyticsManager.GetTopNDestinations(topCount, fromDate, toDate, sortOrder, customerID, supplierID, switchID, groupByCodeGroup, codeGroup, showSupplier, orderTarget, from, to);
        }

        [HttpGet]
        public List<TOne.Analytics.Entities.AlertView> GetAlerts(int topCount, int alertLevel, char showHiddenAlerts = 'N', string tag = null, string source = null, int? userID = null)
        {
            
            return _analyticsManager.GetAlerts(topCount, showHiddenAlerts, alertLevel, tag, source, userID);
        }

        [HttpGet]
        public List<TOne.Analytics.Entities.CarrierRateView> GetRates(int carrierType, DateTime effectiveOn, string carrierID, int from, int to,string codeGroup = null)
        {
            TOne.Entities.CarrierType type = (TOne.Entities.CarrierType)carrierType;
            return _analyticsManager.GetRates(type.ToString(), effectiveOn, carrierID, codeGroup, from, to);
        }

        [HttpGet]
        public List<TOne.Analytics.Entities.CarrierSummaryView> GetCarrierSummary(int carrierType, DateTime fromDate, DateTime toDate, int topCount, char groupByProfile = 'N', string customerID = null, string supplierID = null)
        {
            TOne.Entities.CarrierType type = (TOne.Entities.CarrierType)carrierType;
            return _analyticsManager.GetCarrierSummary(type.ToString(), fromDate, toDate, customerID, supplierID, topCount, groupByProfile);
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
            if (toDate.Subtract(fromDate).TotalDays > 14)
               throw new  HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Date Range must be 14 days or less"));
                
            return _analyticsManager.GetLastWeeksProfit(fromDate, toDate);
        }

        [HttpGet]
        public TOne.Analytics.Entities.TrafficSummaryView GetSummary(DateTime fromDate, DateTime toDate)
        {
            return _analyticsManager.GetSummary(fromDate, toDate);
        }

        [HttpGet]
        public TOne.Analytics.Entities.TrafficSummaryView  GetSummaryOneDay(DateTime day)
        {
            return _analyticsManager.GetSummary(day, day.AddDays(1));
        }
    }
}

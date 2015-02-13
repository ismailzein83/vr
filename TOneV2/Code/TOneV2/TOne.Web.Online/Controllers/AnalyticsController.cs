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
        public List<TOne.Analytics.Entities.TopNDestinationView> GetTopNDestinations(int topCount, DateTime fromDate, DateTime toDate, string sortOrder, char groupByCodeGroup
            , char showSupplier, int from, int to, string customerID = null, string supplierID = null, int? switchID = null, string codeGroup = null)
        {
           
            return _analyticsManager.GetTopNDestinations(topCount, fromDate, toDate, sortOrder, customerID, supplierID, switchID, groupByCodeGroup, codeGroup, showSupplier, from, to);
        }

        [HttpGet]
        public List<TOne.Analytics.Entities.AlertView> GetAlerts(int topCount, char showHiddenAlerts, int alertLevel, string tag = null, string source = null, int? userID = null)
        {
            
            return _analyticsManager.GetAlerts(topCount, showHiddenAlerts, alertLevel, tag, source, userID);
        }

        [HttpGet]
        public List<TOne.Analytics.Entities.CarrierRateView> GetRates(string carrierType, DateTime effectiveOn, string carrierID, int from, int to,string codeGroup = null)
        {
            return _analyticsManager.GetRates(carrierType, effectiveOn, carrierID, codeGroup, from, to);
        }

        [HttpGet]
        public List<TOne.Analytics.Entities.CarrierSummaryView> GetCarrierSummary(string carrierType, DateTime fromDate, DateTime toDate, int topCount, char groupByProfile, string customerID = null, string supplierID = null)
        {
            return _analyticsManager.GetCarrierSummary(carrierType, fromDate, toDate, customerID, supplierID, topCount, groupByProfile);
        }
    }
}

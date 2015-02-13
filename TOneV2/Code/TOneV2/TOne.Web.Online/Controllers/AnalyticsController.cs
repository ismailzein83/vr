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
        public List<TOne.Analytics.Entities.TopNDestinationView> GetTopNDestinations(int topCount, DateTime fromDate, DateTime toDate, string sortOrder, string customerID, string supplierID, int? switchID, char groupByCodeGroup
            , string codeGroup, char showSupplier, int from, int to)
        {
           
            return _analyticsManager.GetTopNDestinations(topCount, fromDate, toDate, sortOrder, customerID, supplierID, switchID, groupByCodeGroup, codeGroup, showSupplier, from, to);
        }

        [HttpGet]
        public List<TOne.Analytics.Entities.AlertView> GetAlerts(int topCount, char showHiddenAlerts, int alertLevel, string tag, string source, int? userID)
        {
            
            return _analyticsManager.GetAlerts(topCount, showHiddenAlerts, alertLevel, tag, source, userID);
        }
    }
}

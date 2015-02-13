using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Data;

namespace TOne.Analytics.Business
{
    public class AnalyticsManager
    {
        public List<Entities.TopNDestinationView> GetTopNDestinations(int topCount, DateTime fromDate, DateTime toDate, string sortOrder, string customerID, string supplierID, int? switchID, char groupByCodeGroup, string codeGroup, char showSupplier, int from, int to)
        {
            IAnalyticsDataManager datamanager = AnalyticsDataManagerFactory.GetDataManager<IAnalyticsDataManager>();

            return datamanager.GetTopNDestinations(topCount, fromDate, toDate, sortOrder, customerID, supplierID, switchID, groupByCodeGroup, codeGroup, showSupplier, from, to);
        }

        public List<Entities.AlertView> GetAlerts(int topCount, char showHiddenAlerts, int alertLevel, string tag, string source, int? userID)
        {
            IAnalyticsDataManager datamanager = AnalyticsDataManagerFactory.GetDataManager<IAnalyticsDataManager>();
            List<Entities.Alert> alerts = datamanager.GetAlerts(topCount, showHiddenAlerts, alertLevel, tag, source, userID);
            return CreateAlertViews(alerts);
        }

        private List<Entities.AlertView> CreateAlertViews(List<Entities.Alert> alerts)
        {
            List<Entities.AlertView> alertViews = new List<Entities.AlertView>();
            foreach (var alert in alerts)
            {
                Entities.AlertView alertView = new Entities.AlertView(alert);
                alertViews.Add(alertView);
            }
            return alertViews;
        }
    }
}

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
        private readonly IAnalyticsDataManager _datamanager;
        public AnalyticsManager()
        {
            _datamanager = AnalyticsDataManagerFactory.GetDataManager<IAnalyticsDataManager>();
        }
        public List<Entities.TopNDestinationView> GetTopNDestinations(int topCount, DateTime fromDate, DateTime toDate, string sortOrder, string customerID, string supplierID, int? switchID, char groupByCodeGroup, string codeGroup, char showSupplier, int from, int to)
        {
           // IAnalyticsDataManager datamanager = AnalyticsDataManagerFactory.GetDataManager<IAnalyticsDataManager>();

            return _datamanager.GetTopNDestinations(topCount, fromDate, toDate, sortOrder, customerID, supplierID, switchID, groupByCodeGroup, codeGroup, showSupplier, from, to);
        }

        public List<Entities.AlertView> GetAlerts(int topCount, char showHiddenAlerts, int alertLevel, string tag, string source, int? userID)
        {
            //IAnalyticsDataManager datamanager = AnalyticsDataManagerFactory.GetDataManager<IAnalyticsDataManager>();
            List<Entities.Alert> alerts = _datamanager.GetAlerts(topCount, showHiddenAlerts, alertLevel, tag, source, userID);
            return CreateAlertViews(alerts);
        }

        public List<Entities.CarrierRateView> GetRates(string carrierType, DateTime effectiveOn, string carrierID, string codeGroup, int from, int to)
        {
            //IAnalyticsDataManager datamanager = AnalyticsDataManagerFactory.GetDataManager<IAnalyticsDataManager>();
            return _datamanager.GetRates(carrierType, effectiveOn, carrierID, codeGroup, from, to);
        }

        public List<Entities.CarrierSummaryView> GetCarrierSummary(string carrierType, DateTime fromDate, DateTime toDate, string customerID, string supplierID, int topCount, char groupByProfile)
        {
            return _datamanager.GetCarrierSummary(carrierType, fromDate, toDate, customerID, supplierID, topCount, groupByProfile);
        }

        #region Private Methods
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

        #endregion
    }
}

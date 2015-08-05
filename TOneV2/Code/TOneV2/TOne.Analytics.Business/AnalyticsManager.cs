using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Data;
using TOne.Analytics.Entities;
using TOne.BusinessEntity.Business;

namespace TOne.Analytics.Business
{
    public class AnalyticsManager
    {
        private readonly IAnalyticsDataManager _datamanager;
        public AnalyticsManager()
        {
            _datamanager = AnalyticsDataManagerFactory.GetDataManager<IAnalyticsDataManager>();
        }
        public List<Entities.TopNDestinationView> GetTopNDestinations(DateTime fromDate, DateTime toDate, string sortOrder, string customerID, string supplierID, int? switchID, char groupByCodeGroup, string codeGroup, char showSupplier, string orderTarget, int from, int to, int? topCount)
        {
            toDate = toDate.AddDays(1).AddSeconds(-1);
            return _datamanager.GetTopNDestinations(fromDate, toDate, sortOrder, customerID, supplierID, switchID, groupByCodeGroup, codeGroup, showSupplier, orderTarget, from, to, topCount);
        }

        public List<Entities.AlertView> GetAlerts(int from, int to, int? topCount, char showHiddenAlerts, int? alertLevel, string tag, string source, int? userID)
        {
            List<Entities.Alert> alerts = _datamanager.GetAlerts(from, to, topCount, showHiddenAlerts, alertLevel, tag, source, userID);
            return CreateAlertViews(alerts);
        }

        public List<Entities.CarrierRateView> GetRates(string carrierType, DateTime effectiveOn, string carrierID, string codeGroup, string code, string zoneName, int from, int to)
        {
            List<Entities.CarrierRateView> lst = _datamanager.GetRates(carrierType, effectiveOn, carrierID, codeGroup, code, zoneName, from, to);
            foreach (Entities.CarrierRateView CR in lst)
            {
                switch (CR.ChangeID)
                {
                    case -1: CR.Change = "images/down-green.png";
                        break;
                    case 1: CR.Change = "images/up-red.png";
                        break;
                    case 2: CR.Change = "images/bookmark-256.png";
                        break;
                    default: CR.Change = "";
                        break;
                }
            }
            FlaggedServiceManager flaggedServiceManager = new FlaggedServiceManager();
            if (lst != null)
                flaggedServiceManager.AssignFlaggedServiceInfo(lst);
            return lst;
        }

        public List<Entities.CarrierSummaryView> GetCarrierSummary(string carrierType, DateTime fromDate, DateTime toDate, string customerID, string supplierID, int? topCount, char groupByProfile, int from, int to)
        {
            toDate = toDate.AddDays(1).AddSeconds(-1);
            return _datamanager.GetCarrierSummary(carrierType, fromDate, toDate, customerID, supplierID, groupByProfile, topCount, from, to);
        }

        public List<Entities.TopCarriersView> GetTopCustomers(DateTime fromDate, DateTime toDate, int topCount)
        {
            toDate = toDate.AddDays(1).AddSeconds(-1);
            return _datamanager.GetTopCustomers(fromDate, toDate, topCount);
        }

        public List<Entities.TopCarriersView> GetTopSuppliers(DateTime fromDate, DateTime toDate, int topCount)
        {
            toDate = toDate.AddDays(1).AddSeconds(-1);
            return _datamanager.GetTopSupplier(fromDate, toDate, topCount);
        }

        public List<Entities.ProfitByWeekDayView> GetLastWeeksProfit(DateTime fromDate, DateTime toDate)
        {
            toDate = toDate.AddDays(1).AddSeconds(-1);
            List<Entities.ProfitByDay> dailyProfit = _datamanager.GetLastWeeksProfit(fromDate, toDate);

            return CreateProfityWeekDayViews(dailyProfit);
        }

        public Entities.TrafficSummaryView GetSummary(DateTime fromDate, DateTime toDate)
        {
            return _datamanager.GetSummary(fromDate, toDate);
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

        private List<Entities.ProfitByWeekDayView> CreateProfityWeekDayViews(List<Entities.ProfitByDay> dailyProfit)
        {
            List<Entities.ProfitByWeekDayView> result = new List<Entities.ProfitByWeekDayView>();

            foreach (var dailyRecord in dailyProfit)
            {
                Entities.ProfitByWeekDayView weekDayItem = result.Where(c => c.DayNumber == dailyRecord.DayNumber).FirstOrDefault();
                if (weekDayItem == null)
                {
                    weekDayItem = new Entities.ProfitByWeekDayView();
                    weekDayItem.DayNumber = dailyRecord.DayNumber;
                    weekDayItem.DayOfWeek = dailyRecord.DayOfWeek;
                    weekDayItem.ProfitWeek1 = dailyRecord.Profit;
                    result.Add(weekDayItem);
                }
                else
                {
                    if (weekDayItem.ProfitWeek2 == null)
                        weekDayItem.ProfitWeek2 = dailyRecord.Profit;
                    else if (weekDayItem.ProfitWeek3 == null)
                        weekDayItem.ProfitWeek3 = dailyRecord.Profit;
                    else if (weekDayItem.ProfitWeek4 == null)
                        weekDayItem.ProfitWeek4 = dailyRecord.Profit;
                }
            }
            return result;
        }

        #endregion
    }
}

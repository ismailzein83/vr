using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using TOne.Analytics.Business;
using TOne.Analytics.Entities;


namespace TOne.Analytics.Web.Controllers
{

    public class BillingStatisticsController : Vanrise.Web.Base.BaseAPIController
    {
        private readonly BillingStatisticManager __billingStatisticsManager;

        public BillingStatisticsController()
        {

            __billingStatisticsManager = new BillingStatisticManager();
        }

        public List<string> GetTest(string name)
        {
            List<string> l = new List<string>();
            l.Add("TEST 1" + name);
            l.Add("TEST 2" + name);
            l.Add("TEST 3" + name);
            l.Add("TEST 4" + name);
            return l;
        }

        public List<ZoneProfitFormatted> GetZoneProfit(DateTime date1, DateTime date2)
        {
            return __billingStatisticsManager.GetZoneProfit(date1, date2, true);
        }

        [HttpGet]
        public List<BillingStatistic> GetBillingStatistics(DateTime fromDate, DateTime toDate)
        {

            return __billingStatisticsManager.GetBillingStatistics(fromDate, toDate);
        }

        public List<VariationReportsData> GetVariationReportsData(DateTime selectedDate, int periodCount, string periodTypeValue, int variationReportOptionValue)
        {
            return __billingStatisticsManager.GetVariationReportsData(selectedDate, periodCount, periodTypeValue,variationReportOptionValue);
        }
         
        [HttpGet]
        public string GetVariationReportQuery(DateTime selectedDate, int periodCount, string periodTypeValue, string variationReportOptionValue)
        {
           return __billingStatisticsManager.GetVariationReportQuery(selectedDate, periodCount, (TimePeriod)Enum.Parse(typeof(TimePeriod), periodTypeValue), (VariationReportOptions)Enum.Parse(typeof(VariationReportOptions), variationReportOptionValue));
            
        }
    }
}
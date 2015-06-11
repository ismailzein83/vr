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

       //public List<VariationReports> GetVariationReportsData(DateTime selectedDate, int periodCount, string periodTypeValue)
        //{
        //    return __billingStatisticsManager.GetVariationReportsData(selectedDate, periodCount, periodTypeValue);
        //}
        public StringBuilder GetVariationReportsData(DateTime selectedDate, int periodCount, string periodTypeValue, int variationReportOptionValue)
        {
            return __billingStatisticsManager.GetVariationReportsData(selectedDate, periodCount, periodTypeValue,variationReportOptionValue);
        }

        public List<VariationReportsData> GetVariationReportsFinalData(DateTime selectedDate, int periodCount, string periodTypeValue)
        {
            List<VariationReports> variationReports = new List<VariationReports>();//__billingStatisticsManager.GetVariationReportsData(selectedDate, periodCount, periodTypeValue);
            List<VariationReportsData> variationReportsData = new List<VariationReportsData>();


            VariationReportsData current = null;

            foreach (var item in variationReports.OrderBy(v => v.CarrierAccountID))
            {
                if (current == null || current.CarrierAccountID != item.CarrierAccountID)
                {
                    current = new VariationReportsData
                    {
                        CarrierAccountID = item.CarrierAccountID,
                        Name = item.Name,
                        TotalDurationsPerDate = new List<TotalDurationPerDate>()
                    };
                    variationReportsData.Add(current);
                }
                current.TotalDurationsPerDate.Add(new TotalDurationPerDate(item.CallDate, item.TotalDuration));
            }

            variationReportsData.OrderBy(v => v.TotalDurationsPerDate.OrderBy(d => d.CallDate));

            foreach (var item in variationReportsData)
            {
                decimal average = 0;
                double CurrentDayValue = item.TotalDurationsPerDate.Where(t => t.CallDate == selectedDate).SingleOrDefault() != null ? double.Parse(item.TotalDurationsPerDate.Where(t => t.CallDate == selectedDate).SingleOrDefault().CallDate.ToString()) : 0;
                double PrevDayValue = item.TotalDurationsPerDate.Where(t => t.CallDate == (selectedDate.AddDays(-1))).SingleOrDefault() != null ? double.Parse(item.TotalDurationsPerDate.Where(t => t.CallDate == (selectedDate.AddDays(-1))).SingleOrDefault().CallDate.ToString()) : 0;
                foreach (var totalDurations in item.TotalDurationsPerDate)
                    average += totalDurations.TotalDuration;
                average = average / periodCount;
                item.PeriodTypeValueAverage = average;
                item.PeriodTypeValuePercentage = Convert.ToDecimal((CurrentDayValue - Convert.ToDouble(average)) / (average == 0 ? double.MaxValue : Convert.ToDouble(average))) * 100;
                item.PreviousPeriodTypeValuePercentage = Convert.ToDecimal((CurrentDayValue - PrevDayValue) / (PrevDayValue == 0 ? double.MaxValue : PrevDayValue)) * 100;

            }
            return variationReportsData;
        }
 
    
    }
}
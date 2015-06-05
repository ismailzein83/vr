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
    
    public class BillingStatisticsController : Vanrise.Web.Base.BaseAPIController
    {
        private readonly BillingStatisticManager __billingStatisticsManager;

        public BillingStatisticsController() {

            __billingStatisticsManager = new BillingStatisticManager();
        }

        public List<string>  GetTest(string name) {
            List<string> l = new List<string>();
            l.Add("TEST 1" + name);
            l.Add("TEST 2" + name);
            l.Add("TEST 3" + name);
            l.Add("TEST 4" + name);
            return l ;
        }
       
        public List<ZoneProfitFormatted> GetZoneProfit(DateTime date1,DateTime date2) {
            return __billingStatisticsManager.GetZoneProfit(date1,date2, true);
        }

        [HttpGet]
        public List<BillingStatistic> GetBillingStatistics(DateTime fromDate, DateTime toDate)
        {

            return __billingStatisticsManager.GetBillingStatistics(fromDate,toDate);
        }

        [HttpGet]

        public List<VariationReports> GetVariationReportsData(DateTime selectedDate, int periodCount, string periodTypeValue)
        {
            return __billingStatisticsManager.GetVariationReportsData(selectedDate, periodCount, periodTypeValue);
        }

        [HttpGet]
        public List<VariationReportsData> GetVariationReportsFinalData(DateTime selectedDate, int periodCount, string periodTypeValue)
        {
           // List<VariationReports> variationReports = __billingStatisticsManager.GetVariationReportsData(selectedDate, periodCount, periodTypeValue);
            List<VariationReportsData> variationReportsData = new List<VariationReportsData>();
            
           
            //VariationReportsData current = null;

            //foreach (var item in  variationReports.OrderBy(v => v.CarrierAccountID))
            //{ 
            //    if(current == null || current.CarrierAccountID != item.CarrierAccountID)
            //    {
            //        current = new VariationReportsData
            //        {
            //            CarrierAccountID = item.CarrierAccountID,
            //            Name = item.Name,
            //            TotalDurationsPerDate = new List<TotalDurationPerDate>()
            //        };
            //        variationReportsData.Add(current);
            //    }
            //    current.TotalDurationsPerDate.Add(new TotalDurationPerDate(item.CallDate, item.TotalDuration));
            //}
            //foreach (VariationReportsData item in variationReportsData)
            //{
            //    double average = 0;
            //    double CurrentDayValue = item.TotalDurationsPerDate[0].CallDate == null ? 0 : double.Parse(item.TotalDurationsPerDate[0].CallDate.ToString());
            //    double PrevDayValue = item.TotalDurationsPerDate[1] == null ? 0 : double.Parse(item.TotalDurationsPerDate[1].ToString());

            //    for (int i = 3; i <= 3 + periodCount; i++)
            //    {
            //        if (i != 4)
            //        {
            //            average += item[i] == DBNull.Value ? 0 : double.Parse(item[i].ToString());
            //            if (Totals[i] == double.MinValue) Totals[i] = (double)0m;
            //            Totals[i] += row[i] == DBNull.Value ? 0 : double.Parse(row[i].ToString());
            //        }
            //    }
            //    average = average / Days;
            //    row[periodtypeText + " AVG"] = average;
            //    row["Prev " + periodtypeValue + " %"] = ((CurrentDayValue - PrevDayValue) / (PrevDayValue == 0 ? double.MaxValue : PrevDayValue)); // * 100
            //    row[periodtypeText + " %"] = ((CurrentDayValue - average) / (average == 0 ? double.MaxValue : average));// * 100
            //}
            //usedforsorting = periodtypeText + " AVG Desc";

            //VariationResults.DefaultView.Sort = usedforsorting;  //"Days AVG Desc" ;
            return variationReportsData;

        }
    }
}
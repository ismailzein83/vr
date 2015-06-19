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
          
        [HttpGet]
        public List<VariationReportsData> GetVariationReport(DateTime selectedDate, int periodCount, string periodTypeValue, string variationReportOptionValue)
        {
            return __billingStatisticsManager.GetVariationReportsData(selectedDate, periodCount, (TimePeriod)Enum.Parse(typeof(TimePeriod), periodTypeValue), (VariationReportOptions)Enum.Parse(typeof(VariationReportOptions), variationReportOptionValue));
            
        }
    }
}
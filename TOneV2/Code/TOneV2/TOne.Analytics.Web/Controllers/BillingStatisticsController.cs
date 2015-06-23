﻿using System;
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
        public VariationReportResult GetVariationReport(DateTime selectedDate, int periodCount, TimePeriod timePeriod, VariationReportOptions variationReportOption, int fromRow, int toRow)
        {
            return __billingStatisticsManager.GetVariationReportsData(selectedDate, periodCount, timePeriod, variationReportOption, fromRow, toRow);
            
        }
    }
}
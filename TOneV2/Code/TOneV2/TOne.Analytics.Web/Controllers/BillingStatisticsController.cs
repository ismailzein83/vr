using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using TOne.Analytics.Business;
using TOne.Analytics.Entities;
using Vanrise.Entities;


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
        public VariationReportResult GetVariationReport(DateTime selectedDate, int periodCount, TimePeriod timePeriod, VariationReportOptions variationReportOption, int fromRow, int toRow, EntityType entityType, string entityID, GroupingBy groupingBy)
        {
            if (variationReportOption == VariationReportOptions.InOutBoundMinutes || variationReportOption == VariationReportOptions.InOutBoundAmount)
            {
                return __billingStatisticsManager.GetInOutVariationReportsData(selectedDate, periodCount, timePeriod, variationReportOption, entityType, entityID, groupingBy, fromRow, toRow);

            }
            else
                return __billingStatisticsManager.GetVariationReportsData(selectedDate, periodCount, timePeriod, variationReportOption, fromRow, toRow, entityType, entityID, groupingBy);
        }

        public List<VolumeTraffic> GetTrafficVolumes(DateTime fromDate, DateTime toDate, string customerId, string supplierId, string zoneId, int attempts, VolumeReportsTimePeriod timePeriod)
        {
            return __billingStatisticsManager.GetTrafficVolumes(fromDate, toDate, customerId, supplierId, zoneId, attempts, timePeriod);
        }

        public List<TOne.Analytics.Entities.ZoneInfo> GetDestinationTrafficVolumes(DateTime fromDate, DateTime toDate, string customerId, string supplierId, int zoneId, int attempts, VolumeReportsTimePeriod timePeriod, int topDestination, bool isDuration)
        {
            return __billingStatisticsManager.GetDestinationTrafficVolumes(fromDate, toDate, customerId, supplierId, zoneId, attempts, timePeriod, topDestination,isDuration);
        }

         [HttpGet]
        public List<InOutVolumeTraffic> CompareInOutTraffic(DateTime fromDate, DateTime toDate, string customerId, VolumeReportsTimePeriod timePeriod, bool showChartsInPie)
        {
            return __billingStatisticsManager.CompareInOutTraffic(fromDate, toDate, customerId, timePeriod, showChartsInPie);

        }

         public object ExportInOutTraffic(ExportInOutTraffic input)
         {
             return GetExcelResponse(__billingStatisticsManager.ExportInOutTraffic(input.FromDate, input.ToDate, input.CustomerId, input.TimePeriod));
         }

        [HttpPost]
        public object ExportCarrierProfile(ExportCarrierProfileInput input)
        {
            return GetExcelResponse(__billingStatisticsManager.ExportCarrierProfile(input.FromDate, input.ToDate, input.CustomerId, input.TopDestination));
        }

    }

    public class ExportCarrierProfileInput
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int TopDestination { get; set; }
        public string CustomerId { get; set; }
    }

    public class ExportInOutTraffic
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string CustomerId { get; set; }
        public VolumeReportsTimePeriod TimePeriod { get; set; }
    }
}
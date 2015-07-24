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
        public VariationReportResult GetVariationReport(DateTime selectedDate, int periodCount, TimePeriod timePeriod, VariationReportOptions variationReportOption, int fromRow, int toRow, EntityType entityType, string entityID,GroupingBy groupingBy)
        {
            if (variationReportOption == VariationReportOptions.InOutBoundMinutes || variationReportOption == VariationReportOptions.InOutBoundAmount)
            {
                return __billingStatisticsManager.GetInOutVariationReportsData(selectedDate, periodCount, timePeriod, variationReportOption, entityType, entityID, groupingBy,fromRow,toRow);
              
            }
            else
                return __billingStatisticsManager.GetVariationReportsData(selectedDate, periodCount, timePeriod, variationReportOption, fromRow, toRow, entityType, entityID, groupingBy);
        }

        public List<VolumeTraffic> GetTrafficVolumes(DateTime fromDate, DateTime toDate, string customerId, string supplierId, string zoneId, int attempts, VolumeReportsTimePeriod timePeriod)
        {
            return __billingStatisticsManager.GetTrafficVolumes(fromDate, toDate, customerId, supplierId, zoneId, attempts, timePeriod);
        }

        public List<DestinationVolumeTraffic> GetDestinationTrafficVolumes(DateTime fromDate, DateTime toDate, string customerId, string supplierId, int zoneId, int attempts, VolumeReportsTimePeriod timePeriod, int topDestination)
        {
            return __billingStatisticsManager.GetDestinationTrafficVolumes(fromDate, toDate, customerId, supplierId, zoneId, attempts, timePeriod, topDestination);
        } 

        [HttpGet]
        public HttpResponseMessage Export(string fromDate, string toDate, string customerId, string topDestination)
        {
            DateTime from = DateTime.ParseExact(fromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime to = DateTime.ParseExact(toDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            int topDest = 0;
            int.TryParse(topDestination, out topDest);

            return __billingStatisticsManager.ExportSupplierCostDetails(from, to, customerId, topDest);
        }
    }
}
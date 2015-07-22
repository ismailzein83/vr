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

        public List<VolumeTrafficResult> GetVolumeReportData(DateTime fromDate, DateTime toDate, string selectedCustomers, string selectedSuppliers, string selectedZones, int attempts, string selectedTimePeriod, VolumeReportsOptions selectedTrafficReport)
        {
            return __billingStatisticsManager.GetVolumeReportData(fromDate, toDate, selectedCustomers, selectedSuppliers, selectedZones, attempts, selectedTimePeriod, selectedTrafficReport);
        }

        public List<VolumeTraffic> GetTrafficVolumes(DateTime fromDate, DateTime toDate, string selectedCustomers, string selectedSuppliers, string selectedZones, int attempts, VolumeReportsTimePeriodEnum selectedTimePeriod)
        {

            return __billingStatisticsManager.GetTrafficVolumes(fromDate, toDate, selectedCustomers, selectedSuppliers, selectedZones, attempts, selectedTimePeriod);
        }

        [HttpGet]
        public HttpResponseMessage Export(string fromDate, string toDate)
        {
            DateTime from = DateTime.ParseExact(fromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime to = DateTime.ParseExact(toDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            return __billingStatisticsManager.ExportSupplierCostDetails(from, to, 0, 0);
        }
    }
}
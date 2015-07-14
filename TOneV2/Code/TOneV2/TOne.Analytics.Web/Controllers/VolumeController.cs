using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.Analytics.Business;
using TOne.Analytics.Entities;


namespace TOne.Analytics.Web.Controllers
{
    public class VolumeController : Vanrise.Web.Base.BaseAPIController
    {

        private readonly VolumeReportsManager __volumeReportsManager;

        public VolumeController()
        {

            __volumeReportsManager = new VolumeReportsManager();
        }
          
        [HttpGet]
        public List<VolumeTrafficResult> GetVolumeReportData(DateTime fromDate, DateTime toDate, string selectedCustomers, string selectedSuppliers, string selectedZones, int attempts, string selectedTimePeriod, VolumeReportsOptions selectedTrafficReport)
        {
            return __volumeReportsManager.GetVolumeReportData(fromDate, toDate, selectedCustomers, selectedSuppliers, selectedZones, attempts, selectedTimePeriod, selectedTrafficReport);
        }
    }
}
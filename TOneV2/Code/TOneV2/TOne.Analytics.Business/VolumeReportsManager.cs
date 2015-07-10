using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Data;
using TOne.Analytics.Entities;

namespace TOne.Analytics.Business
{
    public class VolumeReportsManager 
    {
        private readonly IVolumeReportsDataManager _datamanager;
        public VolumeReportsManager() {
            _datamanager = AnalyticsDataManagerFactory.GetDataManager<IVolumeReportsDataManager>();
        }

        public List<VolumeTraffic> GetVolumeReportData(DateTime fromDate, DateTime toDate, string customerID, string supplierID, string ZoneID, int attempts, string selectedTimePeriod)
        {
            return _datamanager.GetVolumeReportData(fromDate, toDate, customerID, supplierID, ZoneID, attempts, selectedTimePeriod);

        }
    }
}

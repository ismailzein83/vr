using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Entities;

namespace TOne.Analytics.Data
{
    public interface IVolumeReportsDataManager : IDataManager
    {
        List<VolumeTraffic> GetVolumeReportData(DateTime fromDate, DateTime toDate, string customerID, string supplierID, string ZoneID, int attempts, string selectedTimePeriod);
    }
}

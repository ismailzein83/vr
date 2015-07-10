using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.Analytics.Entities;
using System.Data;

namespace TOne.Analytics.Data.SQL
{
    public class VolumeReportsDataManager : BaseTOneDataManager,IVolumeReportsDataManager
    {
        public List<VolumeTraffic> GetVolumeReportData(DateTime fromDate, DateTime toDate, string customerId, string supplierId, string ZoneId, int attempts, string selectedTimePeriod)
        {

            return GetItemsSP("rpt_Volumes_DailyTraffic", (reader) => VolumeReportsMapper(reader),
                   fromDate,
                   toDate,
                   (customerId == null || customerId == "") ? null : customerId,
                   (supplierId == null || supplierId == "") ? null : supplierId,
                   (ZoneId == null || ZoneId == "")? (object)DBNull.Value : ZoneId,
                   attempts,
                   selectedTimePeriod);
        
        }

        private VolumeTraffic VolumeReportsMapper(IDataReader reader)
        {

            VolumeTraffic instance = new VolumeTraffic
            {
                  Attempts = reader["Attempts"]!= DBNull.Value ? (int) reader["Attempts"] :0 ,
                  Duration = GetReaderValue<decimal>(reader, "Duration")
            };

            return instance;
        }

    }
}

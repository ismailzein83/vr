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
        public List<VolumeTrafficResult> GetVolumeReportData(DateTime fromDate, DateTime toDate, string customerId, string supplierId, string ZoneId, int attempts, string selectedTimePeriod, VolumeReportsOptions selectedTrafficReport)
        {
            switch (selectedTrafficReport) {
                case VolumeReportsOptions.TrafficVolumes: return  GetTrafficVolumeSummary(fromDate, toDate, customerId, supplierId, ZoneId, attempts, selectedTimePeriod);
                    break;
                case VolumeReportsOptions.CompareInOutTraffic: return GetTrafficVolumeSummary(fromDate, toDate, customerId, supplierId, ZoneId, attempts, selectedTimePeriod);
                    break;
                default: return new List<VolumeTrafficResult>();
            
            }        
        }

        private List<VolumeTrafficResult> GetTrafficVolumeSummary(DateTime fromDate, DateTime toDate, string customerId, string supplierId, string ZoneId, int attempts, string selectedTimePeriod)
        {
            //Get Traffic Volume Summary
            VolumeTraffic result = GetItemSP("rpt_Volumes_DailyTraffic", (reader) => VolumeReportsMapper(reader),
                   fromDate,
                   toDate,
                   (customerId == null || customerId == "") ? null : customerId,
                   (supplierId == null || supplierId == "") ? null : supplierId,
                   (ZoneId == null || ZoneId == "") ? (object)DBNull.Value : ZoneId,
                   attempts,
                   selectedTimePeriod);
            List<decimal> values = new List<decimal>();
            values.Add(result.Attempts);
            values.Add(result.Duration);

            List<VolumeTrafficResult> resultList = new List<VolumeTrafficResult>();
            resultList.Add(new VolumeTrafficResult() { Values = values });
            return resultList;
        
        
        }

        private List<VolumeTrafficResult> CompareInOutTraffic(DateTime fromDate, DateTime toDate, string customerId, string supplierId, string ZoneId, int attempts, string selectedTimePeriod) {
            
           // Compare in / out traffic for a selected customer 
            VolumeTraffic result = GetItemSP("rpt_Volumes_CompareInOutTraffic", (reader) => VolumeReportsMapper(reader),
                   fromDate,
                   toDate,
                   (customerId == null || customerId == "") ? null : customerId,
                   selectedTimePeriod);
            List<decimal> values = new List<decimal>();
            values.Add(result.Attempts);
            values.Add(result.Duration);

            List<VolumeTrafficResult> resultList = new List<VolumeTrafficResult>();
            resultList.Add(new VolumeTrafficResult() { Values = values });
            return resultList;
        
        
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;

namespace TOne.Analytics.Data.SQL
{
    public class AnalyticsDataManager : BaseTOneDataManager, IAnalyticsDataManager
    {
        public List<Entities.TopNDestinationView> GetTopNDestinations(int topCount, DateTime fromDate, DateTime toDate, string sortOrder, string customerID, string supplierID, int? switchID, char groupByCodeGroup, string codeGroup, char showSupplier, int from, int to)
        {
            return GetItemsSP("Analytics.sp_Traffic_TopNDestination", (reader) =>
            {
                return new Entities.TopNDestinationView
                {
                    OurZoneID = Convert.ToInt32(reader["OurZoneID"]),
                    ZoneName = reader["Name"] as string,
                    SupplierID = reader["SupplierID"] as string,
                    Attempts = Convert.ToInt32(reader["Attempts"]),
                    DurationInMinutes = Convert.ToDecimal(reader["DurationsInMinutes"]),
                    ASR = Convert.ToDecimal(reader["ASR"]),
                    SuccessfulAttempts = Convert.ToInt32(reader["SuccessfulAttempt"]),
                    DeliveredASR = Convert.ToDecimal(reader["DeliveredASR"]),
                    AveragePDD = Convert.ToDecimal(reader["AveragePDD"]),
                    CodeGroup = reader["CodeGroupName"] as string
                };
            },
                topCount, fromDate, toDate, sortOrder, customerID, supplierID, switchID, groupByCodeGroup, codeGroup, showSupplier, from, to, "TopNDestinationTemp");
        }


        public List<Entities.Alert> GetAlerts(int topCount, char showHiddenAlerts, int alertLevel, string tag, string source, int? userID)
        {
            return GetItemsSP("Analytics.sp_alerts_getalerts", (reader) =>
            {
                return new Entities.Alert
                {
                    ID = Convert.ToInt32(reader["ID"]),
                    Created = Convert.ToDateTime(reader["Created"]),
                    Source = reader["Source"] as string,
                    Level = (Entities.AlertLevel)Convert.ToInt32(reader["Level"]),
                    Progress = (Entities.AlertProgress)Convert.ToInt32(reader["Progress"]),
                    Tag = reader["Tag"] as string,
                    Description = reader["Description"] as string
                };
            }, topCount, showHiddenAlerts, alertLevel, tag, source, userID);
        }

        //public List<Entities.CarrierSummaryView> GetCarrierSummary(string carrierType, DateTime fromDate, DateTime toDate, string customerID, string supplierID, int topCount, char groupByProfile)
        //{

        //}



    }
}

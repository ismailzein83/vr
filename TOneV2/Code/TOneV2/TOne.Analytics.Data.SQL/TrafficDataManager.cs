using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;

namespace TOne.Analytics.Data.SQL
{
    public class TrafficDataManager : BaseTOneDataManager, ITrafficDataManager
    {
        public List<Entities.TopNDestinationView> GetTopNDestinations(int topCount, DateTime fromDate, DateTime toDate, string sortOrder, string customerID, string supplierID, int? switchID, char groupByCodeGroup, string codeGroup, char showSupplier)
        {
            return GetItemsSP("Traffic.sp_Traffic_TopNDestination", (reader) =>
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
                topCount, fromDate, toDate, sortOrder, customerID, supplierID, switchID, groupByCodeGroup, codeGroup, showSupplier, 1, 10, "anytable");
        }
    }
}

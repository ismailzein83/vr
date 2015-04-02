using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Entities;
using TOne.Data.SQL;
using TOne.Entities;

namespace TOne.Analytics.Data.SQL
{
    public class TrafficStatisticDataManager : BaseTOneDataManager, ITrafficStatisticDataManager
    {
        public BigResult<TrafficStatisticGroupSummary> GetTrafficStatisticSummary(string tempTableKey, TrafficStatisticGroupKeys[] groupKeys, DateTime from, DateTime to, int fromRow, int toRow, TrafficStatisticMeasures orderBy, bool isDescending)
        {
            TempTableName tempTableName = null;
            if (tempTableKey != null)
                tempTableName = GetTempTableName(tempTableKey);
            else
                tempTableName = GenerateTempTableName();
            BigResult<TrafficStatisticGroupSummary> rslt = new BigResult<TrafficStatisticGroupSummary>()
            {
                ResultKey = tempTableName.Key
            };

            CreateTempTableIfNotExists(tempTableName.TableName, groupKeys, from, to);
            int totalDataCount;
            rslt.Data = GetData(tempTableName.TableName, groupKeys, fromRow, toRow, orderBy, isDescending, out totalDataCount);
            rslt.TotalCount = totalDataCount;
            return rslt;
        }

        private IEnumerable<TrafficStatisticGroupSummary> GetData(string tempTableName, TrafficStatisticGroupKeys[] groupKeys, int fromRow, int toRow, TrafficStatisticMeasures orderBy, bool isDescending, out int totalCount)
        {
            string query = String.Format(@"WITH OrderedResult AS (SELECT *, ROW_NUMBER()  OVER ( ORDER BY {1} {2}) AS rowNumber FROM {0})
	                                    SELECT * FROM OrderedResult WHERE rowNumber between @FromRow AND @ToRow", tempTableName, GetColumnName(orderBy), isDescending ? "DESC" : "ASC");

            totalCount = (int)ExecuteScalarText(String.Format("SELECT COUNT(*) FROM {0}", tempTableName), null);
            return GetItemsText(query,
                (reader) =>
                {
                    var obj = new TrafficStatisticGroupSummary
                    {
                        GroupKeyValues = new object[groupKeys.Count()],
                        FirstCDRAttempt = GetReaderValue<DateTime>(reader, "FirstCDRAttempt"),
                        LastCDRAttempt = GetReaderValue<DateTime>(reader, "LastCDRAttempt"),
                        Attempts = GetReaderValue<int>(reader, "Attempts"),
                        DeliveredAttempts = GetReaderValue<int>(reader, "DeliveredAttempts"),
                        SuccessfulAttempts = GetReaderValue<int>(reader, "SuccessfulAttempts"),
                        DurationsInSeconds = GetReaderValue<Decimal>(reader, "DurationsInSeconds"),
                        MaxDurationInSeconds = GetReaderValue<Decimal>(reader, "MaxDurationInSeconds"),
                        PDDInSeconds = GetReaderValue<Decimal>(reader, "PDDInSeconds"),
                        UtilizationInSeconds = GetReaderValue<Decimal>(reader, "UtilizationInSeconds"),
                        NumberOfCalls = GetReaderValue<int>(reader, "NumberOfCalls"),
                        DeliveredNumberOfCalls = GetReaderValue<int>(reader, "DeliveredNumberOfCalls"),
                        PGAD = GetReaderValue<Decimal>(reader, "PGAD")
                    };
                    for (int i = 0; i < groupKeys.Count(); i++)
                    {
                        obj.GroupKeyValues[i] = reader[GetColumnName(groupKeys[i])];
                    }

                    return obj;
                },
                (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@FromRow", fromRow));
                    cmd.Parameters.Add(new SqlParameter("@ToRow", toRow));
                });
        }

        private string GetColumnName(TrafficStatisticMeasures column)
        {
            return column.ToString();
        }

        private string GetColumnName(TrafficStatisticGroupKeys column)
        {
            if (column == TrafficStatisticGroupKeys.OurZone)
                return "ZoneName";
            else
                return column.ToString();
        }

        private void CreateTempTableIfNotExists(string tempTableName, IEnumerable<TrafficStatisticGroupKeys> groupKeys, DateTime from, DateTime to)
        {
            string query = String.Format(@"
                            IF NOT OBJECT_ID('{0}', N'U') IS NOT NULL
	                            BEGIN

                            WITH OurZones AS (SELECT ZoneID, Name FROM Zone z WITH (NOLOCK) WHERE SupplierID = 'SYS'),
		                        AllResult AS
		                        (
			                        SELECT
					                       ts.OurZoneID AS OurZoneID
					                       ,z.Name as ZoneName
                                           ,Min(FirstCDRAttempt) AS FirstCDRAttempt
				                           , Max(ts.LastCDRAttempt) AS LastCDRAttempt
				                           , Sum(ts.Attempts) AS Attempts
				                           , Sum(ts.DeliveredAttempts) AS DeliveredAttempts
				                           , Sum(ts.SuccessfulAttempts) AS SuccessfulAttempts
				                           , Sum(ts.DurationsInSeconds) AS DurationsInSeconds
				                           , Max(ts.MaxDurationInSeconds) AS MaxDurationInSeconds
				                           , AVG(ts.PDDInSeconds) AS PDDInSeconds
				                           , AVG(ts.UtilizationInSeconds) AS UtilizationInSeconds
				                           , Sum(ts.NumberOfCalls) AS NumberOfCalls
				                           , SUM(ts.DeliveredNumberOfCalls) AS DeliveredNumberOfCalls
				                           , AVG(ts.PGAD) AS PGAD

			                        FROM TrafficStats ts WITH(NOLOCK ,INDEX(IX_TrafficStats_DateTimeFirst))
			                        JOIN OurZones z ON ts.OurZoneID = z.ZoneID
			                        WHERE
			                        FirstCDRAttempt BETWEEN @FromDate AND @ToDate
			                        GROUP BY ts.OurZoneID, z.Name
		                        )
		                        SELECT * INTO {0} FROM AllResult
                            END", tempTableName);
            ExecuteNonQueryText(query, (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@FromDate", from));
                    cmd.Parameters.Add(new SqlParameter("@ToDate", to));
                });
        }
    }
}

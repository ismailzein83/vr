﻿using System;
using System.Collections.Generic;
using System.Data;
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

        public IEnumerable<TrafficStatistic> GetTrafficStatistics(TrafficStatisticGroupKeys filterByColumn, string columnFilterValue, DateTime from, DateTime to)
        {
            string query =String.Format( @"                            
			                SELECT
					                ts.OurZoneID
					                ,ts.FirstCDRAttempt
				                    , ts.LastCDRAttempt
				                    , ts.Attempts
				                    , ts.DeliveredAttempts
				                    , ts.SuccessfulAttempts
				                    , ts.DurationsInSeconds
				                    , ts.MaxDurationInSeconds
				                    , ts.PDDInSeconds
				                    , ts.UtilizationInSeconds
				                    , ts.NumberOfCalls
				                    , ts.DeliveredNumberOfCalls
				                    , ts.PGAD

			                FROM TrafficStats ts WITH(NOLOCK ,INDEX(IX_TrafficStats_DateTimeFirst))
			                WHERE {0} AND  ts.FirstCDRAttempt BETWEEN @FromDate AND @ToDate
			                GROUP BY ts.FirstCDRAttempt ", GetColumnFilter(filterByColumn, columnFilterValue));
            return GetItemsText(query, TrafficStatisticMapper,
                (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@FromDate", from));
                    cmd.Parameters.Add(new SqlParameter("@ToDate", to));
                });
        }

        #region Private Methods

        private void CreateTempTableIfNotExists(string tempTableName, IEnumerable<TrafficStatisticGroupKeys> groupKeys, DateTime from, DateTime to)
        {
            string query = String.Format(@"
                            IF NOT OBJECT_ID('{0}', N'U') IS NOT NULL
	                            BEGIN

                            WITH OurZones AS (SELECT ZoneID, Name FROM Zone z WITH (NOLOCK) WHERE SupplierID = 'SYS'),
		                        AllResult AS
		                        (
			                        SELECT
					                       ts.OurZoneID
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
                        GroupKeyValues = new KeyColumn[groupKeys.Count()]
                    };
                    FillTrafficStatisticFromReader(obj, reader);
                    
                    for (int i = 0; i < groupKeys.Count(); i++)
                    {
                        string idColumn;
                        string nameColumn;
                        GetColumnNames(groupKeys[i], out idColumn, out nameColumn);
                        object id = reader[idColumn];
                        obj.GroupKeyValues[i] = new KeyColumn
                        {
                            Id = id != DBNull.Value ? id.ToString() : null,
                            Name = reader[nameColumn] as string
                        };
                    }

                    return obj;
                },
                (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@FromRow", fromRow));
                    cmd.Parameters.Add(new SqlParameter("@ToRow", toRow));
                });
        }

        TrafficStatistic TrafficStatisticMapper(IDataReader reader)
        {
            TrafficStatistic obj = new TrafficStatistic();
            FillTrafficStatisticFromReader(obj, reader);
            return obj;
        }

        private string GetColumnFilter(TrafficStatisticGroupKeys column, string columnFilterValue)
        {
            switch(column)
            {
                case TrafficStatisticGroupKeys.OurZone:
                    return String.Format("{0} = '{1}'", OurZoneIDColumnName, columnFilterValue);
                default: return null;
            }
        }

        void FillTrafficStatisticFromReader(TrafficStatistic trafficStatistics, IDataReader reader)
        {
            trafficStatistics.FirstCDRAttempt = GetReaderValue<DateTime>(reader, "FirstCDRAttempt");
            trafficStatistics.LastCDRAttempt = GetReaderValue<DateTime>(reader, "LastCDRAttempt");
            trafficStatistics.Attempts = GetReaderValue<int>(reader, "Attempts");
            trafficStatistics.DeliveredAttempts = GetReaderValue<int>(reader, "DeliveredAttempts");
            trafficStatistics.SuccessfulAttempts = GetReaderValue<int>(reader, "SuccessfulAttempts");
            trafficStatistics.DurationsInSeconds = GetReaderValue<Decimal>(reader, "DurationsInSeconds");
            trafficStatistics.MaxDurationInSeconds = GetReaderValue<Decimal>(reader, "MaxDurationInSeconds");
            trafficStatistics.PDDInSeconds = GetReaderValue<Decimal>(reader, "PDDInSeconds");
            trafficStatistics.UtilizationInSeconds = GetReaderValue<Decimal>(reader, "UtilizationInSeconds");
            trafficStatistics.NumberOfCalls = GetReaderValue<int>(reader, "NumberOfCalls");
            trafficStatistics.DeliveredNumberOfCalls = GetReaderValue<int>(reader, "DeliveredNumberOfCalls");
            trafficStatistics.PGAD = GetReaderValue<Decimal>(reader, "PGAD");
        }

        private string GetColumnName(TrafficStatisticMeasures column)
        {
            return column.ToString();
        }

        private void GetColumnNames(TrafficStatisticGroupKeys column, out string idColumn, out string nameColumn)
        {
            switch (column)
            {
                case TrafficStatisticGroupKeys.OurZone:
                    idColumn = OurZoneIDColumnName;
                    nameColumn = OurZoneNameColumnName;
                    break;
                default:
                    idColumn = null;
                    nameColumn = null;
                    break;
            }
        }

        #endregion

        const string OurZoneIDColumnName = "OurZoneID";
        const string OurZoneNameColumnName = "ZoneName";
    }
}

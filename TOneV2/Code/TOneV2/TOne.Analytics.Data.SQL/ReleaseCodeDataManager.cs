using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using TOne.Analytics.Entities;
using TOne.Data.SQL;

namespace TOne.Analytics.Data.SQL
{
    public class ReleaseCodeDataManager : BaseTOneDataManager, IReleaseCodeDataManager
    {
        private TrafficStatisticCommon _trafficStatisticCommon = new TrafficStatisticCommon();


        private string BuildFilter(GenericFilter filter)
        {
            StringBuilder whereBuilder = new StringBuilder();
            _trafficStatisticCommon.AddFilter(whereBuilder, filter.CustomerIds, "CustomerID");
            _trafficStatisticCommon.AddFilter(whereBuilder, filter.SupplierIds, "SupplierID");
            _trafficStatisticCommon.AddFilter(whereBuilder, filter.SwitchIds, "SwitchID");

            if (filter.SwitchIds == null || filter.SwitchIds.Count == 0)
                whereBuilder.Append(" AND CustomerID IS NOT NULL AND CustomerID NOT IN (SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc)");

            return whereBuilder.ToString();
        }

        private void BuildSelect(IEnumerable<TrafficStatisticGroupKeys> groupKeys, StringBuilder groupKeysSelectPart, StringBuilder groupKeysGroupByPart, HashSet<string> joinStatement)
        {
            foreach (var groupKey in groupKeys)
            {
                string columnName;
                string groupByStatement;
                _trafficStatisticCommon.GetColumnStatements(groupKey, out columnName,  joinStatement, out groupByStatement);
                if (groupByStatement == null)
                {
                    groupByStatement = columnName;
                }
                groupKeysSelectPart.Append(columnName);
                groupKeysSelectPart.Append(",");
                if (groupKeysGroupByPart.Length > 0)
                    groupKeysGroupByPart.Append(",");
                groupKeysGroupByPart.Append(groupByStatement);
            }
        }

        private string CreateTempTableIfNotExists(string tempTableName, GenericFilter filter, IEnumerable<TrafficStatisticGroupKeys> groupKeys)
        {
            StringBuilder whereBuilder = new StringBuilder();
            String zoneTemp = String.Empty;
            String joinZone = String.Empty;

            if (filter.CodeGroups != null && filter.CodeGroups.Count > 0)
            {
                if (filter.ZoneIds != null && filter.ZoneIds.Count > 0)
                {
                    zoneTemp =
                        String.Format(
                            " SELECT ZoneID INTO #ZONES FROM Zone z WITH (NOLOCK) WHERE CodeGroup IN  ('{0}') OR ZoneID IN ('{1}')  ",
                            String.Join("', '", filter.CodeGroups), String.Join("', '", filter.ZoneIds));
                    joinZone = " INNER JOIN #ZONES z ON OurZoneID = z.ZoneID ";
                }
                else
                {
                    zoneTemp =
                        String.Format(
                            " SELECT ZoneID INTO #ZONES FROM Zone z WITH (NOLOCK) WHERE CodeGroup IN  ('{0}')  ",
                            String.Join("', '", filter.CodeGroups));
                    joinZone = " INNER JOIN #ZONES z ON OurZoneID = z.ZoneID ";
                }
            }
            else
            {
                if (filter.ZoneIds != null && filter.ZoneIds.Count > 0)
                {
                    zoneTemp =
                        String.Format(
                            " SELECT ZoneID INTO #ZONES FROM Zone z WITH (NOLOCK) WHERE ZoneID IN ('{0}')  ", String.Join("', '", filter.ZoneIds));
                    joinZone = " INNER JOIN #ZONES z ON OurZoneID = z.ZoneID ";
                }
            }
            StringBuilder groupKeysSelectPart = new StringBuilder();
            StringBuilder groupKeysGroupByPart = new StringBuilder();
            
            groupKeysGroupByPart.Append("ReleaseCode , ReleaseSource");
            BuildSelect(groupKeys, groupKeysSelectPart, groupKeysGroupByPart, null);
            StringBuilder queryBuilder = new StringBuilder();
            groupKeysSelectPart.Append(" ReleaseCode , ReleaseSource , ");
            queryBuilder.AppendFormat(@"
                            IF NOT OBJECT_ID('{2}', N'U') IS NOT NULL
                                BEGIN 
                                
                                {0}
                                
                                SELECT  SwitchID,OurZoneID,SupplierID, ReleaseCode, ReleaseSource,DurationInSeconds, Attempt, Port_out, Port_in
                                Into #BillingTemp
                                FROM Billing_CDR_INVALID WITH(NOLOCK,INDEX(IX_Billing_CDR_InValid_Attempt))  {1}
                                WHERE Attempt  BETWEEN @FromDate AND @ToDate  {3}


                                INSERT INTO  #BillingTemp
                                SELECT SwitchID,OurZoneID,SupplierID, ReleaseCode, ReleaseSource,DurationInSeconds, Attempt, Port_out, Port_in
                                FROM Billing_CDR_Main WITH(NOLOCK)  {1}
                                WHERE Attempt  BETWEEN @FromDate AND @ToDate  {3}
                                
                                Declare @TotalAttempts bigint

                                Select @TotalAttempts=COUNT(*) from #BillingTemp
            
                                DECLARE @ShowNameSuffix nvarchar(1)
                                SET @ShowNameSuffix= (select SP.BooleanValue from SystemParameter SP where Name like 'ShowNameSuffix')                                    

                                ;With PrimaryResult AS (
	
	                                SELECT   {4} 
                                          SUM(DurationInSeconds) / 60.0 AS DurationsInMinutes, 
                                          Count(*) Attempts,
		                                  SUM( CASE WHEN DurationInSeconds > 0 THEN 1 ELSE 0 END) SuccessfulAttempts, 
                                          Min(Attempt) FirstAttempt,
		                                  Max(Attempt) LastAttempt 
	                                FROM #BillingTemp BT WITH(NOLOCK)  
	                                Group By   {5}

	                             ),Suppliers AS
                                 (
                                    Select (CASE WHEN  @ShowNameSuffix ='Y' 
                                            THEN (case when A.NameSuffix!='' THEN  P.Name+'('+A.NameSuffix+')' else P.Name end ) ELSE (P.Name ) END ) AS SupplierName
			                            ,A.CarrierAccountID as SupplierID  from CarrierAccount A LEFT JOIN CarrierProfile P on P.ProfileID=A.ProfileID
                                 ),CTEResult As(
	                                SELECT  {4} 
			                                Sum(DurationsInMinutes) DurationsInMinutes,
			                                Sum(Attempts) Attempts,
			                                Sum(SuccessfulAttempts) SuccessfulAttempts,
			                                Sum(Attempts) - Sum(SuccessfulAttempts) FailedAttempts,
			                                Min(FirstAttempt) FirstAttempt,	
			                                Max(LastAttempt) LastAttempt
	                                From PrimaryResult pr
                                    GROUP BY {5}
                                )

                            SELECT * INTO {2}  FROM CTEResult

                            END", zoneTemp, joinZone, tempTableName, BuildFilter(filter), groupKeysSelectPart, groupKeysGroupByPart);
            return queryBuilder.ToString();
        }

        public GenericSummaryBigResult<ReleaseCodeStatistic> GetReleaseCodeStatistic(Vanrise.Entities.DataRetrievalInput<TrafficStatisticSummaryInput> input)
        {
            string columnId;
            _trafficStatisticCommon.GetColumnNames(input.Query.GroupKeys[0], out columnId);

            Dictionary<string, string> mapper = new Dictionary<string, string>
            {
                {"Data.Attempts", "Attempts"},
                {"Zone", "OurZoneID"},
                {"Customer", "CustomerID"},
                {"Supplier", "SupplierID"},
                {"Code Group", "CodeGroupID"},
                {"Switch", "SwitchID"},
                {"GateWay In", "GateWayInName"},
                {"GateWay Out", "GateWayOutName"},
                {"Port In", "Port_IN"},
                {"Port Out", "Port_OUT"},
                {"Code Sales", "OurCode"},
                {"Code Buy", "SupplierCode"},
                {"GroupKeyValues[0].Name", columnId}
            };

            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQueryText(CreateTempTableIfNotExists(tempTableName, input.Query.Filter,input.Query.GroupKeys), (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@FromDate", input.Query.From));
                    cmd.Parameters.Add(new SqlParameter("@ToDate", input.Query.To));
                });
            };
            GenericSummaryBigResult<ReleaseCodeStatistic> rslt = RetrieveData(input, createTempTableAction, (reader) =>
            {
                var obj = new GroupSummary<ReleaseCodeStatistic>
                {
                    GroupKeyValues = new KeyColumn[input.Query.GroupKeys.Count()],
                    Data = FillReleaseCodeStatisticFromReader(reader)
                };


                for (int i = 0; i < input.Query.GroupKeys.Count(); i++)
                {
                    string idColumn;

                    string nameColumn = null;
                    if (input.Query.GroupKeys[i] == TrafficStatisticGroupKeys.CodeGroup)
                        nameColumn = _trafficStatisticCommon.GetConstName(TrafficStatisticGroupKeys.CodeGroup);
                    else if (input.Query.GroupKeys[i] == TrafficStatisticGroupKeys.GateWayIn)
                        nameColumn = _trafficStatisticCommon.GetConstName(TrafficStatisticGroupKeys.GateWayIn);
                    else if (input.Query.GroupKeys[i] == TrafficStatisticGroupKeys.GateWayOut)
                        nameColumn = _trafficStatisticCommon.GetConstName(TrafficStatisticGroupKeys.GateWayOut);

                    _trafficStatisticCommon.GetColumnNames(input.Query.GroupKeys[i], out idColumn);
                    object id = reader[idColumn];
                    obj.GroupKeyValues[i] = new KeyColumn
                    {
                        Id = id != DBNull.Value ? id.ToString() : "N/A",
                        Name = nameColumn != null && reader[nameColumn] is string ? reader[nameColumn] as string : "N/A"
                    };
                }
                return obj;
            }, mapper, new GenericSummaryBigResult<ReleaseCodeStatistic>()) as GenericSummaryBigResult<ReleaseCodeStatistic>;

            _trafficStatisticCommon.FillBEProperties(rslt, input.Query.GroupKeys);
            return rslt;

        }

        ReleaseCodeStatistic FillReleaseCodeStatisticFromReader(IDataReader reader)
        {
            ReleaseCodeStatistic releaseCodeStatistics = new ReleaseCodeStatistic
            {
                Attempts = GetReaderValue<int>(reader, "Attempts"),
                FailedAttempts = GetReaderValue<int>(reader, "FailedAttempts"),
                DurationsInMinutes = GetReaderValue<Decimal>(reader, "DurationsInMinutes") ,
                ReleaseCode = reader["ReleaseCode"] as string,
                FirstAttempt = GetReaderValue<DateTime>(reader, "FirstAttempt"),
                LastAttempt = GetReaderValue<DateTime>(reader, "LastAttempt"),
                //PortIn = reader["PortIn"] as string,
                //PortOut = reader["PortOut"] as string,
                ReleaseSource = reader["ReleaseSource"] as string
            };
            return releaseCodeStatistics;
        }

    }
}

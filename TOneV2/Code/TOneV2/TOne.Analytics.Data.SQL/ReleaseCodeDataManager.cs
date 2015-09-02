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

        private String GetFilterCondition(GenericFilter filter)
        {
            StringBuilder whereBuilder = new StringBuilder();
            _trafficStatisticCommon.AddFilter(whereBuilder, filter.CustomerIds, "CustomerID");
            _trafficStatisticCommon.AddFilter(whereBuilder, filter.SupplierIds, "SupplierID");
            _trafficStatisticCommon.AddFilter(whereBuilder, filter.SwitchIds, "SwitchID");

            if (filter.SwitchIds == null || filter.SwitchIds.Count == 0)
                whereBuilder.Append(" AND CustomerID IS NOT NULL AND CustomerID NOT IN (SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc)");

            return whereBuilder.ToString();
        }

        private String GetJoin(GenericFilter filter, IEnumerable<TrafficStatisticGroupKeys> groupKeys , out String select)
        {
            String result = "";
            select = "";
            if ((filter.CodeGroups != null && filter.CodeGroups.Count > 0) || (filter.ZoneIds != null && filter.ZoneIds.Count > 0))
            {
                result = " LEFT JOIN #ZONES z ON OurZoneID = z.ZoneID ";
            }

            foreach (var groupKey in groupKeys)
            {
                switch (groupKey)
                {
                    case TrafficStatisticGroupKeys.GateWayIn:
                        select += " cscIn.GateWayName as GateWayInName ,";
                        result += " Left JOIN #SwitchConnectivity cscIn  ON (','+cscIn.Details+',' LIKE '%,'+ Port_IN +',%' ) AND(BL.SwitchID = cscIn.SwitchID) AND CustomerID =cscIn.CarrierAccount  ";
                        break;
                    case TrafficStatisticGroupKeys.GateWayOut:
                        select += " cscOut.GateWayName as GateWayOutName ,";
                        result += " Left JOIN #SwitchConnectivity cscOut ON  (','+cscOut.Details+',' LIKE '%,'+ Port_OUT +',%') AND (BL.SwitchID = cscOut.SwitchID)  AND SupplierID  =cscOut.CarrierAccount  ";
                        break;
                }
            }
            if (! String.IsNullOrWhiteSpace(select)) select = select.Remove(select.Length - 1);

            return result;
        }

        private String GetTemp(GenericFilter filter, IEnumerable<TrafficStatisticGroupKeys> groupKeys)
        {
            String result = "";
            if (filter.CodeGroups != null && filter.CodeGroups.Count > 0)
            {
                if (filter.ZoneIds != null && filter.ZoneIds.Count > 0)
                    result = String.Format("SELECT ZoneID INTO #ZONES FROM Zone z WITH (NOLOCK) WHERE CodeGroup IN  ('{0}') OR ZoneID IN ('{1}')  ",
                            String.Join("', '", filter.CodeGroups), String.Join("', '", filter.ZoneIds));
                else
                result = String.Format(
                    " SELECT ZoneID INTO #ZONES FROM Zone z WITH (NOLOCK) WHERE CodeGroup IN  ('{0}')  ",
                    String.Join("', '", filter.CodeGroups));
            }
            else if (filter.ZoneIds != null && filter.ZoneIds.Count > 0)
            {
                result =  String.Format(" SELECT ZoneID INTO #ZONES FROM Zone z WITH (NOLOCK) WHERE ZoneID IN ('{0}')  ", String.Join("', '", filter.ZoneIds));
            }

            if (groupKeys.ToList().Contains(TrafficStatisticGroupKeys.GateWayOut) ||
                groupKeys.ToList().Contains(TrafficStatisticGroupKeys.GateWayIn))
            {
                result += @"        SELECT csc.CarrierAccountID AS  CarrierAccount
                                       ,csc.SwitchID AS SwitchID
                                       ,csc.Details AS Details
                                       ,csc.BeginEffectiveDate AS BeginEffectiveDate
                                       ,csc.EndEffectiveDate AS EndEffectiveDate
                                       ,csc.[Name] AS GateWayName
                                     INTO #SwitchConnectivity
                                     FROM   CarrierSwitchConnectivity csc WITH(NOLOCK)
                                     WHERE (csc.EndEffectiveDate IS null) ";
            }

            return result;
        }

        private String GetGroupBy(IEnumerable<TrafficStatisticGroupKeys> groupKeys, out HashSet<string> joinStatement)
        {
            List<String> result = new List<string>();
            joinStatement = new HashSet<string>();
            foreach (var groupKey in groupKeys)
            {
                switch (groupKey)
                {
                    case TrafficStatisticGroupKeys.OurZone:
                        result.Add("OurZoneID");
                        break;
                    case TrafficStatisticGroupKeys.CustomerId:
                        result.Add("CustomerId");
                        break;
                    case TrafficStatisticGroupKeys.SupplierId:
                        result.Add("BT.SupplierID");
                        break;
                    case TrafficStatisticGroupKeys.Switch:
                        result.Add("SwitchID");
                        break;
                    case TrafficStatisticGroupKeys.GateWayOut:
                        result.Add("GateWayOutName");
                        break;
                    case TrafficStatisticGroupKeys.GateWayIn:
                        result.Add("GateWayInName");
                        break;
                    case TrafficStatisticGroupKeys.PortOut:
                        result.Add("Port_out");
                        break;
                    case TrafficStatisticGroupKeys.PortIn:
                        result.Add("Port_in");
                        break;
                    case TrafficStatisticGroupKeys.CodeGroup:
                        result.Add("CodeGroup");
                        joinStatement.Add("LEFT JOIN Zone as z ON BT.OurZoneID = z.ZoneID");
                        break;
                }
            }
            return string.Join(",", result);
        }

        public String GetQuery(string tempTableName, GenericFilter filter, IEnumerable<TrafficStatisticGroupKeys> groupKeys)
        {
            CustomQuery queryCdrInvalid = new CustomQuery
            {
                SelectStatement = "SELECT BL.SwitchID,OurZoneID,CustomerID,SupplierID, ReleaseCode, ReleaseSource,DurationInSeconds, Attempt, Port_out, Port_in",
                InsertStatement ="Into #BillingTemp",
                FromStatement = "FROM Billing_CDR_INVALID as BL WITH(NOLOCK,INDEX(IX_Billing_CDR_InValid_Attempt))",
                WhereStatement = String.Format("WHERE Attempt  BETWEEN @FromDate AND @ToDate  {0}", GetFilterCondition(filter)),
            };
            String select;
            queryCdrInvalid.JoinStatement.Add(GetJoin(filter, groupKeys, out select));
            if(! String.IsNullOrWhiteSpace(select)) 
                queryCdrInvalid.SelectStatement += " ," + select;

            CustomQuery queryCdrMain = new CustomQuery
            {
                SelectStatement = "INSERT Into #BillingTemp " + queryCdrInvalid.SelectStatement,
                FromStatement = "FROM Billing_CDR_Main as BL WITH(NOLOCK)",
                WhereStatement = queryCdrInvalid.WhereStatement,
                JoinStatement = queryCdrInvalid.JoinStatement,
            };

            String queryShowNameSuffix = @"Declare @TotalAttempts bigint
                               Select @TotalAttempts=COUNT(*) from #BillingTemp
                               DECLARE @ShowNameSuffix nvarchar(1)
                               SET @ShowNameSuffix= (select SP.BooleanValue from SystemParameter SP where Name like 'ShowNameSuffix')";


            HashSet<String> join = new HashSet<string>();
            String groupBy = GetGroupBy(groupKeys, out join);

            CustomQuery queryPrimaryResult = new CustomQuery()
            {
                SelectStatement = String.Format(@"SELECT SUM(DurationInSeconds) / 60.0 AS DurationsInMinutes, 
                                           Count(*) Attempts, 
                                           SUM( CASE WHEN DurationInSeconds > 0 THEN 1 ELSE 0 END) SuccessfulAttempts, 
                                           Min(Attempt) FirstAttempt,
                                           Max(Attempt) LastAttempt , ReleaseCode,ReleaseSource , {0}", groupBy),
                FromStatement = "FROM #BillingTemp BT ",
                GroupByStatement = "GROUP BY ReleaseCode,ReleaseSource , " + groupBy,
                JoinStatement = join
            };

            String querySuppliers = @"Select (CASE WHEN  @ShowNameSuffix ='Y' THEN (case when A.NameSuffix!='' THEN  P.Name+'('+A.NameSuffix+')' else P.Name end ) ELSE (P.Name ) END ) AS SupplierName ,A.CarrierAccountID as SupplierID  
                                      from CarrierAccount A LEFT JOIN CarrierProfile P on P.ProfileID=A.ProfileID";

            CustomQuery queryCteResult = new CustomQuery()
            {
                SelectStatement = String.Format(@"SELECT 
                                            Sum(DurationsInMinutes) DurationsInMinutes,
			                                Sum(Attempts) Attempts,
			                                Sum(SuccessfulAttempts) SuccessfulAttempts,
			                                Sum(Attempts) - Sum(SuccessfulAttempts) FailedAttempts,
			                                Min(FirstAttempt) FirstAttempt,	
			                                Max(LastAttempt) LastAttempt,
			                                0 Percentage,
                                            ReleaseCode,ReleaseSource , {0}", groupBy.Replace("BT.SupplierID", "SupplierID")),
                FromStatement = "FROM PrimaryResult pr ",
                GroupByStatement = "GROUP BY ReleaseCode,ReleaseSource , " + groupBy.Replace("BT.SupplierID", "SupplierID"),
            };

            String queryResult = BuildTempTable(tempTableName,(query)=>
            {
                query.Append(GetTemp(filter, groupKeys));
                query.Append(queryCdrInvalid.Build());
                query.Append(queryCdrMain.Build());
                query.Append(queryShowNameSuffix);
                query.Append(";With ");
                query.Append(BuildCte("PrimaryResult",queryPrimaryResult.Build()));
                query.Append(",");
                query.Append(BuildCte("Suppliers", querySuppliers));
                query.Append(",");
                query.Append(BuildCte("CTEResult", queryCteResult.Build()));
            }
            );

            return queryResult;
        }

        public String BuildCte(String cteName, String cteBody)
        {
            StringBuilder query = new StringBuilder();
            query.AppendFormat(" {0} AS ( ", cteName);
            query.Append(cteBody);
            query.AppendFormat(")");
            return query.ToString();
        }

        public String BuildTempTable(String tableName, Action<StringBuilder> prepareQuery)
        {
            StringBuilder query = new StringBuilder();
            query.AppendFormat(" IF NOT OBJECT_ID('{0}', N'U') IS NOT NULL BEGIN ", tableName);
            prepareQuery(query);
            query.AppendFormat("SELECT * INTO {0}  FROM CTEResult ", tableName);
            query.AppendFormat("END");
            return query.ToString();
        }

        private class CustomQuery
        {
            public CustomQuery()
            {
                JoinStatement = new HashSet<string>();
            }

            public String SelectStatement { get; set; }

            public String FromStatement { get; set; }

            public String InsertStatement { get; set; }

            public String WhereStatement { get; set; }

            public String GroupByStatement { get; set; }

            public HashSet<string> JoinStatement { get; set; }

            public String Build()
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine(SelectStatement);
                query.AppendLine(InsertStatement);
                query.Append(FromStatement);
                query.AppendLine(string.Join(" ", JoinStatement));
                query.AppendLine(WhereStatement);
                query.AppendLine(GroupByStatement);
                return query.ToString();
            }
        }

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

        public void GetColumnStatements(TrafficStatisticGroupKeys groupKey, out string columnName, HashSet<string> joinStatement, out string groupByStatement)
        {
            _trafficStatisticCommon.GetColumnNames(groupKey, out columnName);
            switch (groupKey)
            {
                case TrafficStatisticGroupKeys.OurZone:
                    joinStatement = null;
                    groupByStatement = null;
                    break;
                case TrafficStatisticGroupKeys.CustomerId:
                    joinStatement = null;
                    groupByStatement = null;
                    break;

                case TrafficStatisticGroupKeys.SupplierId:
                    joinStatement = null;
                    groupByStatement = null;
                    break;
                case TrafficStatisticGroupKeys.Switch:
                    columnName = String.Format("ts.SwitchID as {0}", _trafficStatisticCommon.GetConstName(TrafficStatisticGroupKeys.Switch));
                    groupByStatement = "ts.SwitchID";
                    joinStatement = null;
                    break;
                case TrafficStatisticGroupKeys.CodeGroup:
                    columnName = String.Format(" z.CodeGroup as {0}", _trafficStatisticCommon.GetConstName(TrafficStatisticGroupKeys.CodeGroup));
                    joinStatement.Add("LEFT JOIN Zone as z ON BT.OurZoneID = z.ZoneID");
                    groupByStatement = _trafficStatisticCommon.GetConstName(TrafficStatisticGroupKeys.CodeGroup);
                    break;
                default:
                    columnName = null;
                    groupByStatement = null;
                    break;
            }
        }

        private void BuildSelect(IEnumerable<TrafficStatisticGroupKeys> groupKeys, StringBuilder groupKeysSelectPart, StringBuilder groupKeysGroupByPart, HashSet<string> joinStatement)
        {
            foreach (var groupKey in groupKeys)
            {
                string columnName;
                string groupByStatement;
                GetColumnStatements(groupKey, out columnName,  joinStatement, out groupByStatement);
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
            HashSet<string> joinStatement = new HashSet<string>();
            BuildSelect(groupKeys, groupKeysSelectPart, groupKeysGroupByPart, joinStatement);
            
            StringBuilder queryBuilder = new StringBuilder();
            groupKeysSelectPart.Append(" ReleaseCode , ReleaseSource , ");
            queryBuilder.AppendFormat(@"
                            IF NOT OBJECT_ID('{2}', N'U') IS NOT NULL
                                BEGIN 
                                
                                {0}
                                
                                SELECT  SwitchID,OurZoneID,CustomerID,SupplierID, ReleaseCode, ReleaseSource,DurationInSeconds, Attempt, Port_out, Port_in
                                Into #BillingTemp
                                FROM Billing_CDR_INVALID WITH(NOLOCK,INDEX(IX_Billing_CDR_InValid_Attempt))  {1}
                                WHERE Attempt  BETWEEN @FromDate AND @ToDate  {3}


                                INSERT INTO  #BillingTemp
                                SELECT SwitchID,OurZoneID,CustomerID,SupplierID, ReleaseCode, ReleaseSource,DurationInSeconds, Attempt, Port_out, Port_in
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
	                                FROM #BillingTemp BT WITH(NOLOCK) {6} 
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

                            END", zoneTemp, joinZone, tempTableName, BuildFilter(filter), groupKeysSelectPart, groupKeysGroupByPart, string.Join(" ", joinStatement), groupKeysSelectPart);
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
                {"GroupKeyValues[0].Name", columnId},
                {"GroupKeyValues[3].Name","CodeGroup"}
            };

            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQueryText(GetQuery(tempTableName, input.Query.Filter, input.Query.GroupKeys), (cmd) =>
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
                        nameColumn = "CodeGroup";
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

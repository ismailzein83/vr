using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Entities;
using TOne.BusinessEntity.Business;
using TOne.BusinessEntity.Entities;
using TOne.Data.SQL;
using TOne.Entities;
using Vanrise.Data.SQL;
using Vanrise.Entities; 

namespace TOne.Analytics.Data.SQL
{
    public class TrafficStatisticDataManager : BaseTOneDataManager, ITrafficStatisticDataManager
    {
        public TrafficStatisticSummaryBigResult GetTrafficStatisticSummary(Vanrise.Entities.DataRetrievalInput<TrafficStatisticSummaryInput> input)
        {
            Dictionary<string, string> mapper = new Dictionary<string, string>();
            string columnId;
            GetColumnNames(input.Query.GroupKeys[0], out columnId);
            mapper.Add("GroupKeyValues[0].Name", columnId);
            string tempTable=null;
            Action<string> createTempTableAction = (tempTableName) =>
            {
                tempTable = tempTableName;
                ExecuteNonQueryText(CreateTempTableIfNotExists(tempTableName, input.Query.Filter, input.Query.GroupKeys), (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@FromDate", input.Query.From));
                    cmd.Parameters.Add(new SqlParameter("@ToDate", input.Query.To));
                });
            };
            TrafficStatisticSummaryBigResult rslt = RetrieveData(input, createTempTableAction, (reader) =>
            {
                var obj = new TrafficStatisticGroupSummary
                {
                    GroupKeyValues = new KeyColumn[input.Query.GroupKeys.Count()]
                };
                FillTrafficStatisticFromReader(obj, reader);

                for (int i = 0; i < input.Query.GroupKeys.Count(); i++)
                {
                    string idColumn;

                    string nameColumn=null;
                    if (input.Query.GroupKeys[i] == TrafficStatisticGroupKeys.CodeGroup)
                        nameColumn = CodeGroupNameColumnName;
                    else if(input.Query.GroupKeys[i] == TrafficStatisticGroupKeys.GateWayIn)
                        nameColumn = GateWayIDColumnName;
                    else if (input.Query.GroupKeys[i] == TrafficStatisticGroupKeys.GateWayOut)
                        nameColumn = GateWayIDColumnName;

                    GetColumnNames(input.Query.GroupKeys[i], out idColumn);
                    object id = reader[idColumn];
                    obj.GroupKeyValues[i] = new KeyColumn
                    {
                        Id = id != DBNull.Value ? id.ToString() : null,
                        Name = nameColumn!=null?reader[nameColumn] as string:null
                    };
                }

                return obj;
            }, mapper, new TrafficStatisticSummaryBigResult()) as TrafficStatisticSummaryBigResult;

            FillBEProperties(rslt, input.Query.GroupKeys);
               if (input.Query.WithSummary)
                   rslt.Summary = GetSummary(tempTable);

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
			                ORDER BY ts.FirstCDRAttempt ", GetColumnFilter(filterByColumn, columnFilterValue));
            return GetItemsText(query, TrafficStatisticMapper,
                (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@FromDate", from));
                    cmd.Parameters.Add(new SqlParameter("@ToDate", to));
                });
        }
   

        #region Private Methods

        private string CreateTempTableIfNotExists(string tempTableName, TrafficStatisticFilter filter, IEnumerable<TrafficStatisticGroupKeys> groupKeys)
        {
            StringBuilder whereBuilder = new StringBuilder();
            StringBuilder queryBuilder = new StringBuilder(@"
                            IF NOT OBJECT_ID('#TEMPTABLE#', N'U') IS NOT NULL
	                            BEGIN with
                                OurZones AS (SELECT ZoneID, Name, CodeGroup FROM Zone z WITH (NOLOCK) WHERE SupplierID = 'SYS'),
SwitchConnectivity AS
            (
                SELECT csc.CarrierAccountID AS  CarrierAccount
                      ,csc.SwitchID AS SwitchID
                      ,csc.Details AS Details
                      ,csc.BeginEffectiveDate AS BeginEffectiveDate
                      ,csc.EndEffectiveDate AS EndEffectiveDate
                      ,csc.[Name] AS GateWayName
                 FROM   CarrierSwitchConnectivity csc WITH(NOLOCK)--, INDEX(IX_CSC_CarrierAccount))
                WHERE (csc.EndEffectiveDate IS null)
                       
           
            ),
                                     AllResult AS(
			                        SELECT
                                            #SELECTPART#
                                           Min(FirstCDRAttempt) AS FirstCDRAttempt
				                           , Sum(ts.Attempts) AS Attempts
                                            , Sum(ts.SuccessfulAttempts) AS SuccessfulAttempts
				                           , Sum(ts.Attempts-ts.SuccessfulAttempts) AS FailedAttempts
				                           , Sum(ts.DeliveredAttempts) AS DeliveredAttempts
				                           , Sum(ts.DurationsInSeconds) AS DurationsInSeconds
				                           , AVG(ts.PDDInSeconds) AS PDDInSeconds
				                           , AVG(ts.UtilizationInSeconds) AS UtilizationInSeconds
				                           , Sum(ts.NumberOfCalls) AS NumberOfCalls
				                           , SUM(ts.DeliveredNumberOfCalls) AS DeliveredNumberOfCalls
                                           , Sum(ts.CeiledDuration) AS CeiledDuration
                                           , case when Sum(ts.SuccessfulAttempts) > 0 then CONVERT(DECIMAL(10,2),Sum(ts.DurationsInSeconds)/(60.0*Sum(ts.SuccessfulAttempts))) ELSE 0 end as ACD
                                           
                                           , DATEADD(ms,-datepart(ms,Max(LastCDRAttempt)),Max(LastCDRAttempt)) as LastCDRAttempt

                                           ,CONVERT(DECIMAL(10,2),Max (ts.MaxDurationInSeconds)/60.0) as MaxDurationInSeconds
				                           ,CONVERT(DECIMAL(10,2),Avg(ts.PGAD)) as PGAD
                                           ,CONVERT(DECIMAL(10,2),Avg(ts.PDDinSeconds)) as AveragePDD

                                       
			                        FROM TrafficStats ts WITH(NOLOCK ,INDEX(IX_TrafficStats_DateTimeFirst))
                                  #JOINPART#
			                        WHERE
			                        FirstCDRAttempt BETWEEN @FromDate AND @ToDate
                                    #FILTER#
			                        GROUP BY #GROUPBYPART#)
                            SELECT * INTO #TEMPTABLE# FROM AllResult
                            END");
            StringBuilder groupKeysSelectPart = new StringBuilder();
            StringBuilder groupKeysGroupByPart = new StringBuilder();
            HashSet<string> joinStatement=new HashSet<string> ();
            //  string joinStatement = null;
              AddFilterToQuery(filter, whereBuilder, joinStatement);
            foreach(var groupKey in groupKeys)
            {
                string columnName;
                string groupByStatement;
                GetColumnStatements(groupKey, out columnName, joinStatement, out groupByStatement);
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
          
            queryBuilder.Replace("#TEMPTABLE#", tempTableName);

            if (joinStatement.Count>0)
            queryBuilder.Replace("#JOINPART#",string.Join(" ",joinStatement));
            else
                queryBuilder.Replace("#JOINPART#", null);
            queryBuilder.Replace("#FILTER#", whereBuilder.ToString());
            queryBuilder.Replace("#SELECTPART#", groupKeysSelectPart.ToString());
            queryBuilder.Replace("#GROUPBYPART#", groupKeysGroupByPart.ToString());
            return queryBuilder.ToString();
        }

        private void AddFilterToQuery(TrafficStatisticFilter filter, StringBuilder whereBuilder, HashSet<string> joinStatement)
        {
            AddFilter(whereBuilder, filter.SwitchIds, "ts.SwitchId");
            AddFilter(whereBuilder, filter.CustomerIds, "ts.CustomerID");
            AddFilter(whereBuilder, filter.SupplierIds, "ts.SupplierID");
            if (filter.CodeGroups != null && filter.CodeGroups.Count > 0)
            {
                joinStatement.Add(OurZonesJoinQuery);
                AddFilter(whereBuilder, filter.CodeGroups, "z.CodeGroup");

            }

            AddFilter(whereBuilder, filter.PortIn, "ts.Port_IN");
            AddFilter(whereBuilder, filter.PortOut, "ts.Port_OUT");
            AddFilter(whereBuilder, filter.ZoneIds, "ts.OurZoneID");
            AddFilter(whereBuilder, filter.SupplierZoneId, "ts.SupplierZoneID");
            if (filter.GateWayIn != null && filter.GateWayIn.Count > 0)
            {
                joinStatement.Add( GateWayInJoinQuery);
                AddFilter(whereBuilder, filter.GateWayIn, "cscIn.GateWayName");

            }
            if (filter.GateWayOut != null && filter.GateWayOut.Count > 0)
            {
                joinStatement.Add(GateWayOutJoinQuery);
                AddFilter(whereBuilder, filter.GateWayOut, "cscOut.GateWayName");

            }
           
        }

        void AddFilter<T>(StringBuilder whereBuilder, IEnumerable<T> values, string column)
        {
            if (values != null && values.Count() > 0)
            {
                if (typeof(T) == typeof(string))
                    whereBuilder.AppendFormat("AND {0} IN ('{1}')", column, String.Join("', '", values));
                else
                    whereBuilder.AppendFormat("AND {0} IN ({1})", column, String.Join(", ", values));
            }
        }
       
        private void FillBEProperties(TrafficStatisticSummaryBigResult  TrafficStatisticData, TrafficStatisticGroupKeys[] groupKeys)
        {
            BusinessEntityInfoManager manager = new BusinessEntityInfoManager();
            
            foreach (TrafficStatisticGroupSummary data in TrafficStatisticData.Data)
            {

                for (int i = 0; i < groupKeys.Length;i++ )
                {
                    TrafficStatisticGroupKeys groupKey = groupKeys[i];
                    string Id = data.GroupKeyValues[i].Id;
                    switch (groupKey)
                    {
                        case TrafficStatisticGroupKeys.OurZone:
                            data.GroupKeyValues[i].Name = manager.GetZoneName(Convert.ToInt32(Id));break;
                        case TrafficStatisticGroupKeys.CustomerId:
                            if(Id!=null)
                              data.GroupKeyValues[i].Name = manager.GetCarrirAccountName(Id);break;
                        case TrafficStatisticGroupKeys.SupplierId:
                            if (Id != null)
                            data.GroupKeyValues[i].Name = manager.GetCarrirAccountName(Id);break;
                        case TrafficStatisticGroupKeys.Switch:
                            data.GroupKeyValues[i].Name = manager.GetSwitchName(Convert.ToInt32(Id));break;
                        case TrafficStatisticGroupKeys.CodeGroup:
                            break;
                        case TrafficStatisticGroupKeys.SupplierZoneId:
                            data.GroupKeyValues[i].Name = manager.GetZoneName(Convert.ToInt32(Id));break;
                        case TrafficStatisticGroupKeys.PortIn:
                           data.GroupKeyValues[i].Name=Id;break;
                        case TrafficStatisticGroupKeys.PortOut:
                           data.GroupKeyValues[i].Name =Id;break;
                        case TrafficStatisticGroupKeys.GateWayIn:
                            data.GroupKeyValues[i].Name =Id;break;
                        default: break;
                    }

                }
              
            }
        }
        private TrafficStatistic GetSummary(string tempTableName)
        {
            String query = String.Format(@"SELECT
					                        Min(FirstCDRAttempt) AS FirstCDRAttempt
				                           , Max(ts.LastCDRAttempt) AS LastCDRAttempt
				                           , Sum(ts.Attempts) AS Attempts
				                           , Sum(ts.DeliveredAttempts) AS DeliveredAttempts
                                            , Sum(ts.SuccessfulAttempts) AS SuccessfulAttempts
				                            , Sum(ts.Attempts-ts.SuccessfulAttempts) AS FailedAttempts
				                           , Sum(ts.DurationsInSeconds) AS DurationsInSeconds
, Sum(ts.CeiledDuration) AS CeiledDuration
 , case when Sum(ts.SuccessfulAttempts) > 0 then CONVERT(DECIMAL(10,2),Sum(ts.DurationsInSeconds)/(60.0*Sum(ts.SuccessfulAttempts))) ELSE 0 end as ACD
				                           , Max(ts.MaxDurationInSeconds) AS MaxDurationInSeconds
				                           , AVG(ts.PDDInSeconds) AS PDDInSeconds
				                           , AVG(ts.UtilizationInSeconds) AS UtilizationInSeconds
				                           , Sum(ts.NumberOfCalls) AS NumberOfCalls
				                           , SUM(ts.DeliveredNumberOfCalls) AS DeliveredNumberOfCalls
, DATEADD(ms,-datepart(ms,Max(LastCDRAttempt)),Max(LastCDRAttempt)) as LastCDRAttempt
				                           , AVG(ts.PGAD) AS PGAD FROM {0} ts", tempTableName);
            return GetItemText(query, TrafficStatisticMapper, null);
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
                case TrafficStatisticGroupKeys.CustomerId:
                    return String.Format("{0} = '{1}'", CustomerIDColumnName, columnFilterValue);
                case TrafficStatisticGroupKeys.SupplierId:
                    return String.Format("{0} = '{1}'", SupplierIDColumnName, columnFilterValue);
                case TrafficStatisticGroupKeys.Switch:
                    return String.Format("{0} = '{1}'", SwitchIdColumnName, columnFilterValue);
                case TrafficStatisticGroupKeys.PortIn:
                    return String.Format("{0} = '{1}'", Port_INColumnName, columnFilterValue);
                case TrafficStatisticGroupKeys.PortOut:
                    return String.Format("{0} = '{1}'", Port_OutColumnName, columnFilterValue);
                case TrafficStatisticGroupKeys.CodeGroup:
                    return String.Format("{0} = '{1}'", CodeGroupIDColumnName, columnFilterValue);
                case TrafficStatisticGroupKeys.SupplierZoneId:
                    return String.Format("{0} = '{1}'", SupplierZoneIDColumnName, columnFilterValue);
                case TrafficStatisticGroupKeys.GateWayIn:
                    return String.Format("{0} = '{1}'", GateWayIDColumnName, columnFilterValue);
                case TrafficStatisticGroupKeys.GateWayOut:
                    return String.Format("{0} = '{1}'", GateWayIDColumnName, columnFilterValue);
                default: return null;
            }
        }

        void FillTrafficStatisticFromReader(TrafficStatistic trafficStatistics, IDataReader reader)
        {
            trafficStatistics.FirstCDRAttempt = GetReaderValue<DateTime>(reader, "FirstCDRAttempt");
            trafficStatistics.LastCDRAttempt = GetReaderValue<DateTime>(reader, "LastCDRAttempt");
            trafficStatistics.Attempts = GetReaderValue<int>(reader, "Attempts");
            trafficStatistics.FailedAttempts = GetReaderValue<int>(reader, "FailedAttempts");
            trafficStatistics.DeliveredAttempts = GetReaderValue<int>(reader, "DeliveredAttempts");
            trafficStatistics.SuccessfulAttempts = GetReaderValue<int>(reader, "SuccessfulAttempts");
            trafficStatistics.DurationsInMinutes = GetReaderValue<Decimal>(reader, "DurationsInSeconds") / 60;
            trafficStatistics.MaxDurationInMinutes = GetReaderValue<Decimal>(reader, "MaxDurationInSeconds") / 60;
            trafficStatistics.CeiledDuration = GetReaderValue<long>(reader, "CeiledDuration");
            trafficStatistics.ACD = GetReaderValue<Decimal>(reader, "ACD");
            trafficStatistics.PDDInSeconds = GetReaderValue<Decimal>(reader, "PDDInSeconds");
            trafficStatistics.UtilizationInSeconds = GetReaderValue<Decimal>(reader, "UtilizationInSeconds");
            trafficStatistics.NumberOfCalls = GetReaderValue<int>(reader, "NumberOfCalls");
            trafficStatistics.DeliveredNumberOfCalls = GetReaderValue<int>(reader, "DeliveredNumberOfCalls");
            trafficStatistics.PGAD = GetReaderValue<Decimal>(reader, "PGAD");
        }

        private string GetColumnName(TrafficStatisticMeasures column)
        {
            switch(column)
            {
              case TrafficStatisticMeasures.FirstCDRAttempt:return "FirstCDRAttempt";
              case TrafficStatisticMeasures.LastCDRAttempt: return "LastCDRAttempt";
              case TrafficStatisticMeasures.Attempts: return "Attempts";
              case TrafficStatisticMeasures.FailedAttempts: return "FailedAttempts";
              case TrafficStatisticMeasures.SuccessfulAttempts: return "SuccessfulAttempts";
              case TrafficStatisticMeasures.DurationsInMinutes: return "DurationsInSeconds";
              case TrafficStatisticMeasures.MaxDurationInMinutes: return "MaxDurationsInSeconds";
              case TrafficStatisticMeasures.PDDInSeconds: return "PDDInSeconds";
              case TrafficStatisticMeasures.UtilizationInSeconds: return "UtilizationInSeconds";
              case TrafficStatisticMeasures.NumberOfCalls:return "NumberOfCalls";
              case TrafficStatisticMeasures.DeliveredNumberOfCalls: return "DeliveredNumberOfCalls";
              case TrafficStatisticMeasures.PGAD: return "PGAD";
                default: return column.ToString();
            }            
        }

        private void GetColumnNames(TrafficStatisticGroupKeys column, out string idColumn)
        {
            switch (column)
            {
                case TrafficStatisticGroupKeys.OurZone:
                    idColumn = OurZoneIDColumnName;
                    break;
                case TrafficStatisticGroupKeys.CustomerId:
                    idColumn = CustomerIDColumnName;
                    break;

                case TrafficStatisticGroupKeys.SupplierId:
                    idColumn = SupplierIDColumnName;
                    break;
                case TrafficStatisticGroupKeys.Switch:
                    idColumn = SwitchIdColumnName;
                    break;
                case TrafficStatisticGroupKeys.PortIn:
                    idColumn = Port_INColumnName;
                    break;
                case TrafficStatisticGroupKeys.PortOut:
                    idColumn = Port_OutColumnName;
                    break;
                case TrafficStatisticGroupKeys.CodeGroup:
                    idColumn = CodeGroupIDColumnName;
                    break;
                case TrafficStatisticGroupKeys.SupplierZoneId:
                    idColumn = SupplierZoneIDColumnName;
                    break;
                case TrafficStatisticGroupKeys.GateWayIn:
                    idColumn = GateWayIDColumnName;
                    break;
                case TrafficStatisticGroupKeys.GateWayOut:
                    idColumn = GateWayIDColumnName;
                    break;
                default:
                    idColumn = null;
                    break;
            }
        }

        private void GetColumnStatements(TrafficStatisticGroupKeys groupKey, out string columnName, HashSet<string> joinStatement, out string groupByStatement)
        {
            GetColumnNames(groupKey, out columnName);
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
                    joinStatement = null;
                    groupByStatement = null; 
                    break;
                  case TrafficStatisticGroupKeys.CodeGroup:
                    columnName = String.Format("z.CodeGroup as {0}, c.Name {1}", CodeGroupIDColumnName, CodeGroupNameColumnName);
                    joinStatement.Add(String.Format("{0} LEFT JOIN CodeGroup c ON z.CodeGroup=c.Code", OurZonesJoinQuery));
                    groupByStatement = "z.CodeGroup, c.Name";
                    break;
                  case TrafficStatisticGroupKeys.PortIn:
                    joinStatement = null;
                    groupByStatement = null; 
                    break;
                  case TrafficStatisticGroupKeys.PortOut:
                    joinStatement = null;
                    groupByStatement = null; 
                    break;
                  case TrafficStatisticGroupKeys.SupplierZoneId:
                    joinStatement = null;
                    groupByStatement = null; 
                    break;
                  case TrafficStatisticGroupKeys.GateWayIn:
                    joinStatement.Add(GateWayInJoinQuery);
                    columnName = String.Format("cscIn.GateWayName as {0}", GateWayIDColumnName);
                    groupByStatement = "cscIn.GateWayName";
                    break;
                  case TrafficStatisticGroupKeys.GateWayOut:
                    joinStatement.Add(GateWayOutJoinQuery);
                    columnName = String.Format("cscOut.GateWayName as {0}", GateWayIDColumnName);
                    groupByStatement = "cscOut.GateWayName";
                    break;
                  default:
                    columnName = null;
                    groupByStatement = null;
                    break;
            }
        }

        #endregion
        const string SwitchIdColumnName = "SwitchId";
        const string OurZoneIDColumnName = "OurZoneID";
        const string CustomerIDColumnName = "CustomerID";
        const string SupplierIDColumnName = "SupplierID";
        const string Port_INColumnName = "Port_IN";
        const string Port_OutColumnName = "Port_OUT";
        const string CodeGroupIDColumnName = "CodeGroupID";
        const string CodeGroupNameColumnName = "CodeGroupName";
        const string SupplierZoneIDColumnName = "SupplierZoneID";
        const string GateWayIDColumnName = "GateWayName";
        const string OurZonesJoinQuery = " LEFT JOIN OurZones z ON ts.OurZoneID = z.ZoneID";
        const string GateWayInJoinQuery = "Left JOIN SwitchConnectivity cscIn  ON (','+cscIn.Details+',' LIKE '%,'+ts.Port_IN +',%' ) AND(TS.SwitchID = cscIn.SwitchID) AND ts.CustomerID =cscIn.CarrierAccount ";
        const string GateWayOutJoinQuery = "Left JOIN SwitchConnectivity cscOut ON  (','+cscOut.Details+',' LIKE '%,'+ts.Port_OUT +',%') AND (TS.SwitchID = cscOut.SwitchID)  AND ts.SupplierID  =cscOut.CarrierAccount  ";
    }
}

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
    public class TrafficMonitorDataManager : BaseTOneDataManager, ITrafficMonitorDataManager
    {
        private TrafficStatisticCommon trafficStatisticCommon = new TrafficStatisticCommon();
        public GenericSummaryBigResult<TrafficStatistic> GetTrafficStatisticSummary(Vanrise.Entities.DataRetrievalInput<TrafficStatisticSummaryInput> input)
        {
            Dictionary<string, string> mapper = new Dictionary<string, string>();
            string columnId;
            trafficStatisticCommon.GetColumnNames(input.Query.GroupKeys[0], out columnId);

            mapper.Add("Data.Attempts", "Attempts");
            mapper.Add("GroupKeyValues[0].Name", columnId);
            mapper.Add("Zone", "OurZoneID");
            mapper.Add("Customer", "CustomerID");
            mapper.Add("Supplier", "SupplierID");
            mapper.Add("Code Group", "CodeGroupID");
            mapper.Add("Switch", "SwitchID");
            mapper.Add("GateWay In", "GateWayInName");
            mapper.Add("GateWay Out", "GateWayOutName");
            mapper.Add("Port In", "Port_IN");
            mapper.Add("Port Out", "Port_OUT");
            mapper.Add("Code Sales", "OurCode");
            mapper.Add("Code Buy", "SupplierCode");

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
            GenericSummaryBigResult<TrafficStatistic> rslt = RetrieveData(input, createTempTableAction, (reader) =>
            {
                var obj = new GroupSummary<TrafficStatistic>
                {
                    GroupKeyValues = new KeyColumn[input.Query.GroupKeys.Count()], 
                    Data= FillTrafficStatisticFromReader(reader)
                };

               
                for (int i = 0; i < input.Query.GroupKeys.Count(); i++)
                {
                    string idColumn;

                    string nameColumn=null;
                    if (input.Query.GroupKeys[i] == TrafficStatisticGroupKeys.CodeGroup)
                        nameColumn = trafficStatisticCommon.GetConstName(TrafficStatisticGroupKeys.CodeGroup);
                    else if(input.Query.GroupKeys[i] == TrafficStatisticGroupKeys.GateWayIn)
                        nameColumn = trafficStatisticCommon.GetConstName(TrafficStatisticGroupKeys.GateWayIn);
                    else if (input.Query.GroupKeys[i] == TrafficStatisticGroupKeys.GateWayOut)
                        nameColumn = trafficStatisticCommon.GetConstName(TrafficStatisticGroupKeys.GateWayOut);

                    trafficStatisticCommon.GetColumnNames(input.Query.GroupKeys[i], out idColumn);
                    object id = reader[idColumn];
                    obj.GroupKeyValues[i] = new KeyColumn
                    {
                        Id = id != DBNull.Value ? id.ToString() : "N/A",
                        Name = nameColumn != null && reader[nameColumn] as string != null ? reader[nameColumn] as string : "N/A"
                    };
                }
                return obj;
            }, mapper, new GenericSummaryBigResult<TrafficStatistic>()) as GenericSummaryBigResult<TrafficStatistic>;

            trafficStatisticCommon.FillBEProperties<TrafficStatistic>(rslt, input.Query.GroupKeys);
            if (input.Query.WithSummary)
                rslt.Summary = GetSummary(tempTable);
               return rslt;

        }
     

        public IEnumerable<TrafficStatistic> GetTrafficStatistics(TrafficStatisticGroupKeys filterByColumn, string columnFilterValue, DateTime from, DateTime to)
        {
            string columnName;
            HashSet<string> joinStatement=new HashSet<string> ();
            string groupByStatement;
            trafficStatisticCommon.GetColumnStatements(filterByColumn, out columnName, joinStatement, out groupByStatement);
            string query = String.Format(@"BEGIN with 
                             OurZones AS (SELECT ZoneID, Name, CodeGroup  FROM Zone z WITH (NOLOCK) WHERE SupplierID = 'SYS'),
                              AllResult AS(  
			                SELECT
					                {0}    
				                    , ts.Attempts as Attempts
				                    , ts.DurationsInSeconds AS DurationsInSeconds

			                FROM TrafficStats ts WITH(NOLOCK ,INDEX(IX_TrafficStats_DateTimeFirst)) {1}
			                WHERE {2} AND  ts.FirstCDRAttempt BETWEEN @FromDate AND @ToDate 
			                 )
                           SELECT * FROM AllResult ORDER BY Attempts END  ", columnName, joinStatement.Count != 0 ? string.Join(" ", joinStatement) : null,trafficStatisticCommon.GetColumnFilter(filterByColumn, columnFilterValue));
            return GetItemsText(query, TrafficStatisticChartMapper,
                (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@FromDate", from));
                    cmd.Parameters.Add(new SqlParameter("@ToDate", to));
                });
        }
   

        #region Private Methods
        TrafficStatistic TrafficStatisticChartMapper( IDataReader reader)
        {

            TrafficStatistic trafficStatistics = new TrafficStatistic
            {
                Attempts = GetReaderValue<int>(reader, "Attempts"),
                DurationsInMinutes = GetReaderValue<Decimal>(reader, "DurationsInSeconds") / 60
            };
            return trafficStatistics;
            
        }

        private string CreateTempTableIfNotExists(string tempTableName, GenericFilter filter, IEnumerable<TrafficStatisticGroupKeys> groupKeys)
        {
            StringBuilder whereBuilder = new StringBuilder();
            string tableName = trafficStatisticCommon.GetTableName(groupKeys, filter);
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
                                            ,#ABRSELECTPART#
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

                                       
			                        FROM #TABLENAME# 
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
            trafficStatisticCommon.AddFilterToQuery(filter, whereBuilder, joinStatement);
            foreach(var groupKey in groupKeys)
            {
                string columnName;
                string groupByStatement;
               trafficStatisticCommon.GetColumnStatements(groupKey, out columnName, joinStatement, out groupByStatement);
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
            queryBuilder.Replace("#TABLENAME#", tableName);
            
            if (joinStatement.Count>0)
            queryBuilder.Replace("#JOINPART#",string.Join(" ",joinStatement));
            else
                queryBuilder.Replace("#JOINPART#", null);
            queryBuilder.Replace("#FILTER#", whereBuilder.ToString());
            string neededSelectStatment = trafficStatisticCommon.GetNeededStatment(filter, groupKeys);
            queryBuilder.Replace("#ABRSELECTPART#", neededSelectStatment);
            queryBuilder.Replace("#SELECTPART#", groupKeysSelectPart.ToString());
            queryBuilder.Replace("#GROUPBYPART#", groupKeysGroupByPart.ToString());
            return queryBuilder.ToString();
        }



  
        private TrafficStatistic GetSummary(string tempTableName)
        {
            String query = String.Format(@"SELECT
					                        Min(FirstCDRAttempt) AS FirstCDRAttempt
				                           , Max(ts.LastCDRAttempt) AS LastCDRAttempt
                                            ,SUM(ts.ASR) AS ASR
                                            ,SUM(ts.ABR) AS ABR
                                            ,SUM(ts.NER) AS NER
				                           , Sum(ts.Attempts) AS Attempts
				                           , Sum(ts.DeliveredAttempts) AS DeliveredAttempts
                                            , Sum(ts.SuccessfulAttempts) AS SuccessfulAttempts
				                            , Sum(ts.Attempts-ts.SuccessfulAttempts) AS FailedAttempts
				                           , Sum(ts.DurationsInSeconds) AS DurationsInSeconds
                                            , Sum(ts.CeiledDuration) AS CeiledDuration
                                    , SUM(ts.ACD) ACD
				                           , Max(ts.MaxDurationInSeconds) AS MaxDurationInSeconds
				                           , AVG(ts.PDDInSeconds) AS PDDInSeconds
				                           , AVG(ts.UtilizationInSeconds) AS UtilizationInSeconds
				                           , Sum(ts.NumberOfCalls) AS NumberOfCalls
				                           , SUM(ts.DeliveredNumberOfCalls) AS DeliveredNumberOfCalls
                                        , Max(ts.LastCDRAttempt) as LastCDRAttempt
				                           , AVG(ts.PGAD) AS PGAD FROM {0} ts", tempTableName);
            return GetItemText(query, TrafficStatisticMapper, null);


        }
        TrafficStatistic TrafficStatisticMapper(IDataReader reader)
        {
            return FillTrafficStatisticFromReader(reader); ;
        }
        TrafficStatistic FillTrafficStatisticFromReader(IDataReader reader)
        {
            TrafficStatistic trafficStatistics = new TrafficStatistic();
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
            trafficStatistics.ABR = GetReaderValue<Decimal>(reader, "ABR");
            trafficStatistics.ASR = GetReaderValue<Decimal>(reader, "ASR");
            trafficStatistics.NER = GetReaderValue<Decimal>(reader, "NER");
            return trafficStatistics;
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
       


        #endregion

       
       
       
    }
}

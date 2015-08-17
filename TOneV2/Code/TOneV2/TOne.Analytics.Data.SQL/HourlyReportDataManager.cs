using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Entities;
using TOne.Data.SQL;

namespace TOne.Analytics.Data.SQL
{
    public class HourlyReportDataManager : BaseTOneDataManager, IHourlyReportDataManager
    {
        private TrafficStatisticCommon trafficStatisticCommon = new TrafficStatisticCommon();
        public GenericSummaryBigResult<HourlyReport> GetHourlyReportData(Vanrise.Entities.DataRetrievalInput<HourlyReportInput> input)
        {
            Dictionary<string, string> mapper = new Dictionary<string, string>();
            string columnId;
            trafficStatisticCommon.GetColumnNames(input.Query.GroupKeys[0], out columnId);
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
            mapper.Add(" Data.Hour", "Hour");

           
            string tempTable = null;
            Action<string> createTempTableAction = (tempTableName) =>
            {
                tempTable = tempTableName;
                ExecuteNonQueryText(CreateTempTableIfNotExists(tempTableName, input.Query.Filter, input.Query.GroupKeys), (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@FromDate", input.Query.From));
                    cmd.Parameters.Add(new SqlParameter("@ToDate", input.Query.To));
                });
            };
            GenericSummaryBigResult<HourlyReport> rslt = RetrieveData(input, createTempTableAction, (reader) =>
            {
                var obj = new GroupSummary<HourlyReport>
                {
                    GroupKeyValues = new KeyColumn[input.Query.GroupKeys.Count()],
                    Data = FillHourlyReportFromReader(reader)
                };


                for (int i = 0; i < input.Query.GroupKeys.Count(); i++)
                {
                    string idColumn;

                    string nameColumn = null;
                    if (input.Query.GroupKeys[i] == TrafficStatisticGroupKeys.CodeGroup)
                        nameColumn = trafficStatisticCommon.GetConstName(TrafficStatisticGroupKeys.CodeGroup);
                    else if (input.Query.GroupKeys[i] == TrafficStatisticGroupKeys.GateWayIn)
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
            }, mapper, new GenericSummaryBigResult<HourlyReport>()) as GenericSummaryBigResult<HourlyReport>;
            trafficStatisticCommon.FillBEProperties<HourlyReport>(rslt, input.Query.GroupKeys);
           // FillBEProperties(rslt, input.Query.GroupKeys);
            if (input.Query.WithSummary)
                rslt.Summary = GetSummary(tempTable);
            //GenericSummaryBigResult<HourlyReport> rslt = new GenericSummaryBigResult<HourlyReport>();
            return rslt;

        }
        private HourlyReport GetSummary(string tempTableName)
        {
            String query = String.Format(@"SELECT
                                            SUM(Hour) AS [Hour],
                                           Max(Date) as [Date],
				                           Sum(Attempts) as Attempts,
                                           Sum(DurationsInMinutes) as DurationsInMinutes,        
                                            SUM(ASR) AS ASR,
                                             Max(LastAttempt) as LastAttempt,
                                            Sum(FailedAttempts) AS FailedAttempts,
                                            Sum(ACD) AS ACD,    
                                            Sum(NER) AS NER,    
                                            SUM(DeliveredASR)as DeliveredASR,          
	                                    	MAX(MaxDuration) as MaxDuration,
                                           Sum(SuccessfulAttempt) as SuccessfulAttempt,
                                          AVG(UtilizationInMinutes) as UtilizationInMinutes  FROM {0} ts", tempTableName);
            return GetItemText(query, HourlyReportMapper, null);


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
                                          
                                           datepart(hour,LastCDRAttempt) AS [Hour],
                                           dateadd(dd,0, datediff(dd,0,LastCDRAttempt)) AS [Date],
				                           Sum(Attempts) as Attempts,
                                           Sum(DurationsInSeconds)/60.0 as DurationsInMinutes, 
                                            #NeededSelectPart#,
                                            case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
                                            case when sum(attempts)>0 then Sum(deliveredAttempts)*100.0 / Sum(Attempts) 
		                                    else 0 end as DeliveredASR, 
                                            Sum(Attempts - SuccessfulAttempts) AS FailedAttempts,
	                                    	MAX(DurationsInSeconds) / 60.0 as MaxDuration,
                                           Max(LastCDRAttempt) as LastAttempt,
                                           Sum(SuccessfulAttempts) as SuccessfulAttempt,
                                          Sum(UtilizationInSeconds)/60.0 as UtilizationInMinutes                                      
			                              FROM #TABLENAME# 
                                           #JOINPART#
			                               WHERE
			                               FirstCDRAttempt BETWEEN @FromDate AND @ToDate
                                         #FILTER#
			                           GROUP BY #GROUPBYPART#,datepart(hour,LastCDRAttempt),dateadd(dd,0, datediff(dd,0,LastCDRAttempt)))
                                       SELECT * INTO #TEMPTABLE# FROM AllResult  ORDER BY [Hour] ,[Date]
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

            if (joinStatement.Count > 0)
                queryBuilder.Replace("#JOINPART#", string.Join(" ", joinStatement));
            else
                queryBuilder.Replace("#JOINPART#", null);
            queryBuilder.Replace("#FILTER#", whereBuilder.ToString());
            string neededSelectStatment = trafficStatisticCommon.GetNeededStatment(filter, groupKeys);
            queryBuilder.Replace("#NeededSelectPart#", neededSelectStatment);
            queryBuilder.Replace("#SELECTPART#", groupKeysSelectPart.ToString());
            queryBuilder.Replace("#GROUPBYPART#", groupKeysGroupByPart.ToString());
            return queryBuilder.ToString();
        }

          HourlyReport HourlyReportMapper(IDataReader reader)
          {
              return FillHourlyReportFromReader(reader); 
          }
          HourlyReport FillHourlyReportFromReader(IDataReader reader)
          {
              HourlyReport hourlyReport = new HourlyReport();
              hourlyReport.Hour = GetReaderValue<int>(reader, "Hour");
              hourlyReport.Date = GetReaderValue<DateTime>(reader, "Date");
              hourlyReport.Attempts = GetReaderValue<int>(reader, "Attempts");
              hourlyReport.DurationsInMinutes = GetReaderValue<Decimal>(reader, "DurationsInMinutes") ;
              hourlyReport.ASR = GetReaderValue<Decimal>(reader, "ASR");
              hourlyReport.ACD = GetReaderValue<Decimal>(reader, "ACD");
              hourlyReport.NER = GetReaderValue<Decimal>(reader, "NER");         
              hourlyReport.DeliveredASR = GetReaderValue<Decimal>(reader, "DeliveredASR");
              hourlyReport.FailedAttempts = GetReaderValue<int>(reader, "FailedAttempts");
              hourlyReport.MaxDurationInMinutes = GetReaderValue<Decimal>(reader, "MaxDuration");
              hourlyReport.LastAttempt = GetReaderValue<DateTime>(reader, "LastAttempt");
              hourlyReport.SuccessfulAttempt = GetReaderValue<int>(reader, "SuccessfulAttempt");
              hourlyReport.UtilizationInMinutes = GetReaderValue<Decimal>(reader, "UtilizationInMinutes");
              return hourlyReport;
          }
        
         
          
    }
}

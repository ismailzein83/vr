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
        public TrafficStatisticSummaryBigResult<TrafficStatistic> GetTrafficStatisticSummary(Vanrise.Entities.DataRetrievalInput<TrafficStatisticSummaryInput> input)
        {
            Dictionary<string, string> mapper = new Dictionary<string, string>();
            string columnId;
            GetColumnNames(input.Query.GroupKeys[0], out columnId);

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
            TrafficStatisticSummaryBigResult<TrafficStatistic> rslt = RetrieveData(input, createTempTableAction, (reader) =>
            {
                var obj = new TrafficStatisticGroupSummary<TrafficStatistic>
                {
                    GroupKeyValues = new KeyColumn[input.Query.GroupKeys.Count()], 
                    Data= FillTrafficStatisticFromReader(reader)
                };

               
                for (int i = 0; i < input.Query.GroupKeys.Count(); i++)
                {
                    string idColumn;

                    string nameColumn=null;
                    if (input.Query.GroupKeys[i] == TrafficStatisticGroupKeys.CodeGroup)
                        nameColumn = CodeGroupNameColumnName;
                    else if(input.Query.GroupKeys[i] == TrafficStatisticGroupKeys.GateWayIn)
                        nameColumn = GateWayInIDColumnName;
                    else if (input.Query.GroupKeys[i] == TrafficStatisticGroupKeys.GateWayOut)
                        nameColumn = GateWayOutIDColumnName;

                    GetColumnNames(input.Query.GroupKeys[i], out idColumn);
                    object id = reader[idColumn];
                    obj.GroupKeyValues[i] = new KeyColumn
                    {
                        Id = id != DBNull.Value ? id.ToString() : "N/A",
                        Name = nameColumn != null && reader[nameColumn] as string != null ? reader[nameColumn] as string : "N/A"
                    };
                }
                return obj;
            }, mapper, new TrafficStatisticSummaryBigResult<TrafficStatistic>()) as TrafficStatisticSummaryBigResult<TrafficStatistic>;
         
            FillBEProperties(rslt, input.Query.GroupKeys);
            if (input.Query.WithSummary)
                rslt.Summary = GetSummary(tempTable);
               return rslt;

        }
     

        public IEnumerable<TrafficStatistic> GetTrafficStatistics(TrafficStatisticGroupKeys filterByColumn, string columnFilterValue, DateTime from, DateTime to)
        {
            string columnName;
            HashSet<string> joinStatement=new HashSet<string> ();
            string groupByStatement;
            GetColumnStatements(filterByColumn, out columnName, joinStatement, out groupByStatement);
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
                           SELECT * FROM AllResult ORDER BY Attempts END  ", columnName, joinStatement.Count != 0 ? string.Join(" ", joinStatement) : null, GetColumnFilter(filterByColumn, columnFilterValue));
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

        private string CreateTempTableIfNotExists(string tempTableName, TrafficStatisticFilter filter, IEnumerable<TrafficStatisticGroupKeys> groupKeys)
        {
            StringBuilder whereBuilder = new StringBuilder();
            string tableName = GetTableName(groupKeys, filter);
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
            queryBuilder.Replace("#TABLENAME#", tableName);
            
            if (joinStatement.Count>0)
            queryBuilder.Replace("#JOINPART#",string.Join(" ",joinStatement));
            else
                queryBuilder.Replace("#JOINPART#", null);
            queryBuilder.Replace("#FILTER#", whereBuilder.ToString());
            string neededSelectStatment = GetNeededStatment(filter, groupKeys);
            queryBuilder.Replace("#ABRSELECTPART#", neededSelectStatment);
            queryBuilder.Replace("#SELECTPART#", groupKeysSelectPart.ToString());
            queryBuilder.Replace("#GROUPBYPART#", groupKeysGroupByPart.ToString());
            return queryBuilder.ToString();
        }
        private string GetTableName(IEnumerable<TrafficStatisticGroupKeys> groupKeys, TrafficStatisticFilter filter)
        {
            foreach (var groupKey in groupKeys)
            {
                if (groupKey == TrafficStatisticGroupKeys.CodeBuy || groupKey == TrafficStatisticGroupKeys.CodeSales)
                    return "TrafficStatsByCode ts WITH(NOLOCK ,INDEX(IX_TrafficStatsByCode_DateTimeFirst))";
            }
            if (filter.CodeSales != null && filter.CodeSales.Count > 0 || filter.CodeSales != null && filter.CodeSales.Count > 0)
                return "TrafficStatsByCode ts WITH(NOLOCK ,INDEX(IX_TrafficStatsByCode_DateTimeFirst))";
            return "TrafficStats ts WITH(NOLOCK ,INDEX(IX_TrafficStats_DateTimeFirst)) ";
        }
        private void AddFilterToQuery(TrafficStatisticFilter filter, StringBuilder whereBuilder, HashSet<string> joinStatement)
        {
            AddFilter(whereBuilder, filter.SwitchIds, "ts.SwitchID");
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
            AddFilter(whereBuilder, filter.CodeBuy, "ts.SupplierCode");
            AddFilter(whereBuilder, filter.CodeSales, "ts.OurCode");
           
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

        private void FillBEProperties(TrafficStatisticSummaryBigResult<TrafficStatistic> TrafficStatisticData, TrafficStatisticGroupKeys[] groupKeys)
        {
            BusinessEntityInfoManager manager = new BusinessEntityInfoManager();

            foreach (TrafficStatisticGroupSummary<TrafficStatistic> data in TrafficStatisticData.Data)
            {

                for (int i = 0; i < groupKeys.Length;i++ )
                {
                    TrafficStatisticGroupKeys groupKey = groupKeys[i];
                    string Id = data.GroupKeyValues[i].Id;
                    switch (groupKey)
                    {
                        case TrafficStatisticGroupKeys.OurZone:
                            if (Id != "N/A")
                                  data.GroupKeyValues[i].Name = manager.GetZoneName(Convert.ToInt32(Id));                       
                             break;
                          
                        case TrafficStatisticGroupKeys.CustomerId:
                             if (Id != "N/A")
                               data.GroupKeyValues[i].Name = manager.GetCarrirAccountName(Id);
                          break;   

                        case TrafficStatisticGroupKeys.SupplierId:
                          if (Id != "N/A")
                                    data.GroupKeyValues[i].Name = manager.GetCarrirAccountName(Id);
                          break; 
                        case TrafficStatisticGroupKeys.Switch:
                          if (Id != "N/A")
                                    data.GroupKeyValues[i].Name = manager.GetSwitchName(Convert.ToInt32(Id));
                          break; 
                        case TrafficStatisticGroupKeys.SupplierZoneId:
                          if (Id != "N/A")
                            data.GroupKeyValues[i].Name = manager.GetZoneName(Convert.ToInt32(Id));break;                            
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
                    return String.Format("{0} = '{1}'", GateWayInIDColumnName, columnFilterValue);
                case TrafficStatisticGroupKeys.GateWayOut:
                    return String.Format("{0} = '{1}'", GateWayOutIDColumnName, columnFilterValue);
                case TrafficStatisticGroupKeys.CodeSales:
                    return String.Format("{0} = '{1}'", CodeSalesIDColumnName, columnFilterValue);
                case TrafficStatisticGroupKeys.CodeBuy:
                    return String.Format("{0} = '{1}'", CodeBuyIDColumnName, columnFilterValue);
                default: return null;
            }
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
                    idColumn = GateWayInIDColumnName;
                    break;
                case TrafficStatisticGroupKeys.GateWayOut:
                    idColumn = GateWayOutIDColumnName;
                    break;
                case TrafficStatisticGroupKeys.CodeSales:
                    idColumn = CodeSalesIDColumnName;
                    break;
                case TrafficStatisticGroupKeys.CodeBuy:
                    idColumn = CodeBuyIDColumnName;
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
                    columnName = String.Format("ts.SwitchID as {0}", SwitchIdColumnName);
                    groupByStatement = "ts.SwitchID";
                    joinStatement = null;
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
                    columnName = String.Format("cscIn.GateWayName as {0}", GateWayInIDColumnName);
                    groupByStatement = "cscIn.GateWayName";
                    break;
                  case TrafficStatisticGroupKeys.GateWayOut:
                    joinStatement.Add(GateWayOutJoinQuery);
                    columnName = String.Format("cscOut.GateWayName as {0}", GateWayOutIDColumnName);
                    groupByStatement = "cscOut.GateWayName";
                    break;
                  case TrafficStatisticGroupKeys.CodeBuy:
                    joinStatement = null;
                    groupByStatement = null;
                    break;
                  case TrafficStatisticGroupKeys.CodeSales:
                    joinStatement = null;
                    groupByStatement = null;
                    break;
                  default:
                    columnName = null;
                    groupByStatement = null;
                    break;
            }
        }
        public string GetNeededStatment(TrafficStatisticFilter filter, IEnumerable<TrafficStatisticGroupKeys>  groupKeys)
        {
            StringBuilder neededSelectStatement = new StringBuilder();
            foreach (TrafficStatisticGroupKeys groupKey in groupKeys)
            {
                if (groupKey == TrafficStatisticGroupKeys.SupplierId || groupKey == TrafficStatisticGroupKeys.PortOut || groupKey == TrafficStatisticGroupKeys.GateWayOut || filter.SupplierIds.Count != 0)
                {
                    neededSelectStatement.Append(" Case WHEN SUM(ts.Attempts)>0 THEN CONVERT(DECIMAL(10,2),SUM(ts.SuccessfulAttempts)*100.0/Sum(ts.Attempts)) ELSE 0 END AS ABR ");
                    neededSelectStatement.Append(", Case WHEN (Sum(ts.Attempts)-Sum(case when ts.SupplierID is null then ts.Attempts else 0 end))>0 THEN CONVERT(DECIMAL(10,2),SUM(ts.SuccessfulAttempts)*100.0/(Sum(ts.Attempts)-Sum(case when ts.SupplierID is null then ts.Attempts else 0 end))) ELSE 0 END AS ASR ");
                    neededSelectStatement.Append(" ,Case WHEN (Sum(ts.Attempts)-Sum(case when ts.SupplierID is null then ts.Attempts else 0 end))>0 THEN CONVERT(DECIMAL(10,2),SUM(ts.DeliveredNumberOfCalls)*100.0/(Sum(ts.Attempts)-Sum(case when ts.SupplierID is null then ts.Attempts else 0 end))) ELSE 0 END AS NER ");
                    return neededSelectStatement.ToString();
                }
               
            }

             neededSelectStatement.Append(" Case WHEN SUM(ts.NumberOfCalls)>0 THEN CONVERT(DECIMAL(10,2),SUM(ts.SuccessfulAttempts)*100.0/Sum(ts.NumberOfCalls)) ELSE 0 END AS ABR ");
             neededSelectStatement.Append(" ,Case WHEN (Sum(ts.NumberOfCalls)-Sum(case when ts.SupplierID is null then ts.Attempts else 0 end))>0 THEN CONVERT(DECIMAL(10,2),SUM(ts.SuccessfulAttempts)*100.0/(Sum(ts.NumberOfCalls)-Sum(case when ts.SupplierID is null then ts.Attempts else 0 end))) ELSE 0 END AS ASR ");
             neededSelectStatement.Append(" ,Case WHEN (Sum(ts.NumberOfCalls)-Sum(case when ts.SupplierID is null then ts.Attempts else 0 end))>0 THEN CONVERT(DECIMAL(10,2),SUM(ts.DeliveredNumberOfCalls)*100.0/(Sum(ts.NumberOfCalls)-Sum(case when ts.SupplierID is null then ts.Attempts else 0 end))) ELSE 0 END AS NER ");
             return neededSelectStatement.ToString();
        }

        #endregion
        const string SwitchIdColumnName = "SwitchID";
        const string OurZoneIDColumnName = "OurZoneID";
        const string CustomerIDColumnName = "CustomerID";
        const string SupplierIDColumnName = "SupplierID";
        const string Port_INColumnName = "Port_IN";
        const string Port_OutColumnName = "Port_OUT";
        const string CodeGroupIDColumnName = "CodeGroup";
        const string CodeGroupNameColumnName = "CodeGroupName";
        const string SupplierZoneIDColumnName = "SupplierZoneID";
        const string GateWayInIDColumnName = "GateWayInName";
        const string GateWayOutIDColumnName = "GateWayOutName";

        const string CodeBuyIDColumnName = "SupplierCode";
        const string CodeSalesIDColumnName = "OurCode";
        
        const string OurZonesJoinQuery = " LEFT JOIN  OurZones z ON ts.OurZoneID = z.ZoneID";
        const string GateWayInJoinQuery = "Left JOIN SwitchConnectivity cscIn  ON (','+cscIn.Details+',' LIKE '%,'+ts.Port_IN +',%' ) AND(ts.SwitchID = cscIn.SwitchID) AND ts.CustomerID =cscIn.CarrierAccount ";
        const string GateWayOutJoinQuery = "Left JOIN SwitchConnectivity cscOut ON  (','+cscOut.Details+',' LIKE '%,'+ts.Port_OUT +',%') AND (ts.SwitchID = cscOut.SwitchID)  AND ts.SupplierID  =cscOut.CarrierAccount  ";
    }
}

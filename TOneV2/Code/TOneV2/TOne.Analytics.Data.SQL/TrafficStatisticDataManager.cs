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
        public TrafficStatisticSummaryBigResult GetTrafficStatisticSummary(string tempTableKey, TrafficStatisticFilter filter, bool withSummary, TrafficStatisticGroupKeys[] groupKeys, DateTime from, DateTime to, int fromRow, int toRow, TrafficStatisticMeasures orderBy, bool isDescending)
        {
            TempTableName tempTableName = null;
            if (tempTableKey != null)
                tempTableName = GetTempTableName(tempTableKey);
            else
                tempTableName = GenerateTempTableName();
            TrafficStatisticSummaryBigResult rslt = new TrafficStatisticSummaryBigResult()
            {
                ResultKey = tempTableName.Key
            };
            CreateTempTableIfNotExists(tempTableName.TableName, filter, groupKeys, from, to);
            int totalDataCount;
            rslt.Data = GetData(tempTableName.TableName, groupKeys, fromRow, toRow, orderBy, isDescending, out totalDataCount);
            rslt.TotalCount = totalDataCount;
            if (withSummary)
                rslt.Summary = GetSummary(tempTableName.TableName);
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

        private void CreateTempTableIfNotExists(string tempTableName, TrafficStatisticFilter filter, IEnumerable<TrafficStatisticGroupKeys> groupKeys, DateTime from, DateTime to)
        {
            StringBuilder whereBuilder = new StringBuilder();
            StringBuilder queryBuilder = new StringBuilder(@"
                            IF NOT OBJECT_ID('#TEMPTABLE#', N'U') IS NOT NULL
	                            BEGIN
                            WITH
		                        AllResult AS
		                        (
			                        SELECT
                                            #SELECTPART#
                                           Min(FirstCDRAttempt) AS FirstCDRAttempt
				                           , Sum(ts.Attempts) AS Attempts
				                           , Sum(ts.DeliveredAttempts) AS DeliveredAttempts
				                           
				                           , Sum(ts.DurationsInSeconds) AS DurationsInSeconds
				                           , AVG(ts.PDDInSeconds) AS PDDInSeconds
				                           , AVG(ts.UtilizationInSeconds) AS UtilizationInSeconds
				                           , Sum(ts.NumberOfCalls) AS NumberOfCalls
				                           , SUM(ts.DeliveredNumberOfCalls) AS DeliveredNumberOfCalls

                                           , Sum(ts.SuccessfulAttempts) AS SuccessfulAttempts
                                           , DATEADD(ms,-datepart(ms,Max(LastCDRAttempt)),Max(LastCDRAttempt)) as LastCDRAttempt
                                           ,CONVERT(DECIMAL(10,2),Max (ts.MaxDurationInSeconds)/60.0) as MaxDurationInSeconds
				                           ,CONVERT(DECIMAL(10,2),Avg(ts.PGAD)) as PGAD
                                           ,CONVERT(DECIMAL(10,2),Avg(ts.PDDinSeconds)) as AveragePDD


			                        FROM TrafficStats ts WITH(NOLOCK ,INDEX(IX_TrafficStats_DateTimeFirst))
                                 
			                        WHERE
			                        FirstCDRAttempt BETWEEN @FromDate AND @ToDate
                                    #FILTER#
			                        GROUP BY #GROUPBYPART#
		                        )
		                        SELECT * INTO #TEMPTABLE# FROM AllResult
                            END");
            StringBuilder groupKeysSelectPart = new StringBuilder();
            StringBuilder groupKeysGroupByPart = new StringBuilder();
            foreach(var groupKey in groupKeys)
            {
                string columnName;
                GetColumnNames(groupKey, out columnName);
               
               groupKeysSelectPart.Append(columnName);
               groupKeysSelectPart.Append(",");
               if (groupKeysGroupByPart.Length > 0)
                    groupKeysGroupByPart.Append(",");
               groupKeysGroupByPart.Append(columnName);
            }
            queryBuilder.Replace("#TEMPTABLE#", tempTableName);
            AddFilterToQuery(filter, whereBuilder);
            queryBuilder.Replace("#FILTER#", whereBuilder.ToString());
            queryBuilder.Replace("#SELECTPART#", groupKeysSelectPart.ToString());
            queryBuilder.Replace("#GROUPBYPART#", groupKeysGroupByPart.ToString());
            ExecuteNonQueryText(queryBuilder.ToString(), (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@FromDate", from));
                    cmd.Parameters.Add(new SqlParameter("@ToDate", to));
                });
        }

        private void AddFilterToQuery(TrafficStatisticFilter filter, StringBuilder whereBuilder)
        {
            AddFilter(whereBuilder, filter.SwitchIds, "ts.SwitchId");
            AddFilter(whereBuilder, filter.CustomerIds, "ts.CustomerID");
            AddFilter(whereBuilder, filter.SupplierIds, "ts.SupplierID");
            ZoneManager zoneManager = new ZoneManager();
            if (filter.CodeGroups != null)
            {
                List<int> zoneIds = zoneManager.GetCodeGroupZones(filter.CodeGroups);
                AddFilter(whereBuilder, zoneIds, "ts.OurZoneID");
            }
        
            AddFilter(whereBuilder, filter.PortIn, "ts.Port_IN");
            AddFilter(whereBuilder, filter.PortOut, "ts.Port_OUT");
            AddFilter(whereBuilder, filter.ZoneIds, "ts.OurZoneID");
            AddFilter(whereBuilder, filter.SupplierZoneId, "ts.SupplierZoneID");
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
        private IEnumerable<TrafficStatisticGroupSummary> GetData(string tempTableName, TrafficStatisticGroupKeys[] groupKeys, int fromRow, int toRow, TrafficStatisticMeasures orderBy, bool isDescending, out int totalCount)
        {
            string orderByColumnName = GetColumnName(orderBy);
            IEnumerable<TrafficStatisticGroupSummary> trafficStatisticrData = base.GetDataFromTempTable<TrafficStatisticGroupSummary>(tempTableName, fromRow, toRow, orderByColumnName, isDescending,
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
                       GetColumnNames(groupKeys[i], out idColumn);
                       object id = reader[idColumn];
                       obj.GroupKeyValues[i] = new KeyColumn
                       {
                           Id = id != DBNull.Value ? id.ToString() : null,
                       };
                   }

                   return obj;
               },
                out totalCount);
            FillBEProperties(trafficStatisticrData, groupKeys);
            return trafficStatisticrData;
        }
        private void FillBEProperties(IEnumerable<TrafficStatisticGroupSummary> TrafficStatisticData, TrafficStatisticGroupKeys[] groupKeys)
        {
            BusinessEntityInfoManager manager = new BusinessEntityInfoManager();
            foreach (TrafficStatisticGroupSummary data in TrafficStatisticData)
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
                            ZoneManager zoneManager = new ZoneManager();
                            string codeGroupName = zoneManager.GetZone((Convert.ToInt32(Id))).CodeGroupName;
                            if (codeGroupName!=null)
                                data.GroupKeyValues[i].Name = codeGroupName;
                            break;
                        case TrafficStatisticGroupKeys.SupplierZoneId:
                            data.GroupKeyValues[i].Name = manager.GetZoneName(Convert.ToInt32(Id));break;
                        case TrafficStatisticGroupKeys.PortIn:
                           data.GroupKeyValues[i].Name=Id;break;
                        case TrafficStatisticGroupKeys.PortOut:
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
				                           , Sum(ts.DurationsInSeconds) AS DurationsInSeconds
				                           , Max(ts.MaxDurationInSeconds) AS MaxDurationInSeconds
				                           , AVG(ts.PDDInSeconds) AS PDDInSeconds
				                           , AVG(ts.UtilizationInSeconds) AS UtilizationInSeconds
				                           , Sum(ts.NumberOfCalls) AS NumberOfCalls
				                           , SUM(ts.DeliveredNumberOfCalls) AS DeliveredNumberOfCalls
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
            trafficStatistics.DurationsInMinutes = GetReaderValue<Decimal>(reader, "DurationsInSeconds") / 60;
            trafficStatistics.MaxDurationInMinutes = GetReaderValue<Decimal>(reader, "MaxDurationInSeconds") / 60;
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
              case TrafficStatisticMeasures.DeliveredAttempts: return "DeliveredAttempts";
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
                default:
                    idColumn = null;
                    break;
            }
        }

        //private void GetColumnStatements(TrafficStatisticGroupKeys column, out string selectStatement, out string groupByStatement)
        //{
        //    switch (column)
        //    {
        //        case TrafficStatisticGroupKeys.OurZone:
        //            selectStatement = String.Format(" ts.OurZoneID as {0},  ", OurZoneIDColumnName);
        //            groupByStatement = "ts.OurZoneID";
        //            break;
        //        case TrafficStatisticGroupKeys.CustomerId:
        //             selectStatement = String.Format(" ts.CustomerID as {0}, ", CustomerIDColumnName);
        //            groupByStatement = "ts.CustomerID";
        //            break;

        //        case TrafficStatisticGroupKeys.SupplierId:
        //            selectStatement = String.Format(" ts.SupplierID as {0}, ", SupplierIDColumnName);
        //            groupByStatement = "ts.SupplierID";
        //            break;
        //        case TrafficStatisticGroupKeys.Switch:
        //            selectStatement = String.Format(" ts.SwitchId as {0}, ", SwitchIdColumnName);
        //            groupByStatement = "ts.SwitchId";
        //            break;
        //        case TrafficStatisticGroupKeys.CodeGroup:
        //            selectStatement = String.Format("ts.OurZoneID as {0}, ", CodeGroupIDColumnName);
        //            groupByStatement = "ts.OurZoneID";
        //            break;
        //        case TrafficStatisticGroupKeys.PortIn:
        //            selectStatement = String.Format(" ts.Port_IN as {0}, ", Port_INColumnName);
        //            groupByStatement = "ts.Port_IN";
        //            break;
        //        case TrafficStatisticGroupKeys.PortOut:
        //            selectStatement = String.Format(" ts.Port_OUT as {0}, ", Port_OutColumnName);
        //            groupByStatement = "ts.Port_OUT";
        //            break;
        //        case TrafficStatisticGroupKeys.SupplierZoneId:
        //            selectStatement = String.Format(" ts.SupplierZoneID as {0}, ", SupplierZoneIDColumnName);
        //            groupByStatement = "ts.SupplierZoneID";
        //            break;
        //        default:
        //            selectStatement = null;
        //            groupByStatement = null;
        //            break;
        //    }
        //}

        #endregion
        const string SwitchIdColumnName = "SwitchId";
        const string OurZoneIDColumnName = "OurZoneID";
        const string CustomerIDColumnName = "CustomerID";
        const string SupplierIDColumnName = "SupplierID";
        const string Port_INColumnName = "Port_IN";
        const string Port_OutColumnName = "Port_OUT";
        const string CodeGroupIDColumnName = "CodeGroupID";
        const string SupplierZoneIDColumnName = "SupplierZoneID";
    }
}

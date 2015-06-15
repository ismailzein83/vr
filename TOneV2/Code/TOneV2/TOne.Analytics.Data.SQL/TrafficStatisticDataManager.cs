using System;
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

                            WITH OurZones AS (SELECT ZoneID, Name, CodeGroup FROM Zone z WITH (NOLOCK) WHERE SupplierID = 'SYS'),
                           
                            CarrierInfo AS
                            (
                                Select P.Name + ' (' + A.NameSuffix + ')' AS Name, A.CarrierAccountID AS CarrierAccountID from CarrierAccount A LEFT JOIN CarrierProfile P on P.ProfileID=A.ProfileID
                            ),
		                        AllResult AS
		                        (
			                        SELECT
                                            #SELECTPART#
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
				                           , AVG(ts.PGAD) AS PGAD

			                        FROM TrafficStats ts WITH(NOLOCK ,INDEX(IX_TrafficStats_DateTimeFirst))
                                    #JOINPART# 
			                        WHERE
			                        FirstCDRAttempt BETWEEN @FromDate AND @ToDate
                                    #FILTER#
			                        GROUP BY #GROUPBYPART#
		                        )
		                        SELECT * INTO #TEMPTABLE# FROM AllResult
                            END");
            StringBuilder groupKeysSelectPart = new StringBuilder();
            StringBuilder groupKeysJoinPart = new StringBuilder();
            StringBuilder groupKeysGroupByPart = new StringBuilder();
            foreach(var groupKey in groupKeys)
            {
                string selectStatement ;
                string joinStatement ;
                string groupByStatement;
                GetColumnStatements(groupKey, out selectStatement, out joinStatement, out groupByStatement);
                groupKeysSelectPart.Append(selectStatement);
                groupKeysJoinPart.Append(joinStatement);
               if (groupKeysGroupByPart.Length > 0)
                    groupKeysGroupByPart.Append(",");
                groupKeysGroupByPart.Append(groupByStatement);
            }

            queryBuilder.Replace("#TEMPTABLE#", tempTableName);
            AddFilterToQuery(filter, whereBuilder);
            queryBuilder.Replace("#FILTER#", whereBuilder.ToString());
            queryBuilder.Replace("#SELECTPART#", groupKeysSelectPart.ToString());
            queryBuilder.Replace("#JOINPART#", groupKeysJoinPart.ToString());
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
            AddFilter(whereBuilder, filter.CodeGroup, "z.CodeGroup");
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
                case TrafficStatisticMeasures.DurationsInMinutes: return "DurationsInSeconds";
                case TrafficStatisticMeasures.MaxDurationInMinutes: return "MaxDurationsInSeconds";
                default: return column.ToString();
            }            
        }

        private void GetColumnNames(TrafficStatisticGroupKeys column, out string idColumn, out string nameColumn)
        {
            switch (column)
            {
                case TrafficStatisticGroupKeys.OurZone:
                    idColumn = OurZoneIDColumnName;
                    nameColumn = OurZoneNameColumnName;
                    break;
                case TrafficStatisticGroupKeys.CustomerId:
                    idColumn = CustomerIDColumnName;
                    nameColumn = CustomerNameColumnName;
                    break;

                case TrafficStatisticGroupKeys.SupplierId:
                    idColumn = SupplierIDColumnName;
                    nameColumn = SupplierNameColumnName;
                    break;
                case TrafficStatisticGroupKeys.Switch:
                    idColumn = SwitchIdColumnName;
                    nameColumn = SwitchNameColumnName;
                    break;
                case TrafficStatisticGroupKeys.PortIn:
                    idColumn = Port_INColumnName;
                    nameColumn = Port_INColumnName;
                    break;
                case TrafficStatisticGroupKeys.PortOut:
                    idColumn = Port_OutColumnName;
                    nameColumn = Port_OutColumnName;
                    break;
                case TrafficStatisticGroupKeys.CodeGroup:
                    idColumn = CodeGroupIDColumnName;
                    nameColumn = CodeGroupNameColumnName;
                    break;
                case TrafficStatisticGroupKeys.SupplierZoneId:
                    idColumn = SupplierZoneIDColumnName;
                    nameColumn = SupplierZoneNameColumnName;
                    break;
                default:
                    idColumn = null;
                    nameColumn = null;
                    break;
            }
        }

        private void GetColumnStatements(TrafficStatisticGroupKeys column, out string selectStatement, out string joinStatement, out string groupByStatement)
        {
            switch (column)
            {
                case TrafficStatisticGroupKeys.OurZone:
                    selectStatement = String.Format(" ts.OurZoneID as {0}, z.Name as {1}, ", OurZoneIDColumnName, OurZoneNameColumnName);
                    joinStatement = " LEFT JOIN OurZones z ON ts.OurZoneID = z.ZoneID";
                    groupByStatement = "ts.OurZoneID, z.Name";
                    break;
                case TrafficStatisticGroupKeys.CustomerId:
                    selectStatement = String.Format(" ts.CustomerID as {0}, cust.Name as {1}, ", CustomerIDColumnName, CustomerNameColumnName);
                    joinStatement = " LEFT JOIN CarrierInfo cust ON ts.CustomerID = cust.CarrierAccountID";
                    groupByStatement = "ts.CustomerID, cust.Name";
                    break;

                case TrafficStatisticGroupKeys.SupplierId:
                    selectStatement = String.Format(" ts.SupplierID as {0}, supp.Name as {1}, ", SupplierIDColumnName, SupplierNameColumnName);
                    joinStatement = " LEFT JOIN CarrierInfo supp ON ts.SupplierID = supp.CarrierAccountID";
                    groupByStatement = "ts.SupplierID, supp.Name";
                    break;
                case TrafficStatisticGroupKeys.Switch:
                    selectStatement = String.Format(" ts.SwitchId as {0}, swit.Name as {1}, ", SwitchIdColumnName, SwitchNameColumnName);
                    joinStatement = " LEFT JOIN Switch swit ON ts.SwitchId = swit.SwitchID";
                    groupByStatement = "ts.SwitchId, swit.Name";
                    break;
                case TrafficStatisticGroupKeys.CodeGroup:
                    selectStatement = String.Format(" z.CodeGroup as {0}, c.Name as {1}, ", CodeGroupIDColumnName, CodeGroupNameColumnName);
                    joinStatement = " LEFT JOIN OurZones z ON ts.OurZoneID = z.ZoneID LEFT JOIN CodeGroup c ON z.CodeGroup=c.Code";
                    groupByStatement = "z.CodeGroup, c.Name";
                    break;
                case TrafficStatisticGroupKeys.PortIn:
                    selectStatement = String.Format(" ts.Port_IN as {0}, ", Port_INColumnName);
                    joinStatement = null;
                    groupByStatement = "ts.Port_IN";
                    break;
                case TrafficStatisticGroupKeys.PortOut:
                    selectStatement = String.Format(" ts.Port_OUT as {0}, ", Port_OutColumnName);
                    joinStatement = null;
                    groupByStatement = "ts.Port_OUT";
                    break;
                case TrafficStatisticGroupKeys.SupplierZoneId:
                    selectStatement = String.Format(" ts.SupplierZoneID as {0}, supplierZones.Name as {1}, ", SupplierZoneIDColumnName, SupplierZoneNameColumnName);
                    joinStatement = " LEFT JOIN Zone supplierZones WITH (NOLOCK) ON ts.SupplierZoneID = supplierZones.ZoneID";
                    groupByStatement = "ts.SupplierZoneID,  supplierZones.Name";
                    break;
                default:
                    selectStatement = null;
                    joinStatement = null;
                    groupByStatement = null;
                    break;
            }
        }

        #endregion
        const string SwitchIdColumnName = "SwitchId";
        const string SwitchNameColumnName = "SwitchName";
        const string OurZoneIDColumnName = "OurZoneID";
        const string OurZoneNameColumnName = "ZoneName";
        const string CustomerIDColumnName = "CustomerID";
        const string CustomerNameColumnName = "CustomerName";
        const string SupplierIDColumnName = "SupplierID";
        const string SupplierNameColumnName = "SupplierName";
        const string Port_INColumnName = "PortIn";
        const string Port_OutColumnName = "PortOut";
        const string CodeGroupIDColumnName = "CodeGroupID";
        const string CodeGroupNameColumnName = "CodeGroupName";
        const string SupplierZoneIDColumnName = "SupplierZoneID";
        const string SupplierZoneNameColumnName = "SupplierZoneName";
       
    }
}

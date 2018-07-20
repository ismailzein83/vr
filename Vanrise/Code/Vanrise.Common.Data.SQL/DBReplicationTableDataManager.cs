﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Vanrise.Data.SQL;
using Vanrise.Entities;

namespace Vanrise.Common.Data.SQL
{
    public class DBReplicationTableDataManager : BaseSQLDataManager
    {
        string _sourceConnectionString;
        public DBReplicationTableDataManager(string connectionString)
            : base(connectionString, false)
        {
            _sourceConnectionString = connectionString;
        }

        public void MigrateData(IDBReplicationTableMigrateDataContext context)
        {
            List<string> queryFilters = new List<string>();

            bool splitByDate = false;
            if (!string.IsNullOrEmpty(context.FilterDateTimeColumn))
            {
                splitByDate = true;
                queryFilters.Add(string.Format("({0} >= @FromTime)", context.FilterDateTimeColumn));
                queryFilters.Add(string.Format("({0} < @ToTime)", context.FilterDateTimeColumn));
            }

            string concatColumns = string.Join<string>(",", context.Columns);
            string sourceTableNameWithSchema = string.Format("[{0}].[{1}]", context.TableSchema, context.SourceTableName);

            List<DateTimeRange> dateTimeRanges = new List<DateTimeRange>() { new DateTimeRange() { From = context.FromTime, To = context.ToTime } };
            if (splitByDate)
            {
                TimeSpan dbReplicationTimeInterval = new TimeSpan(context.NumberOfDaysPerInterval, 0, 0, 0);
                dateTimeRanges = Vanrise.Common.Utilities.GenerateDateTimeRanges(context.FromTime, context.ToTime, dbReplicationTimeInterval).ToList();
            }

            int totalCount = 0;
            foreach (DateTimeRange dateTimeRange in dateTimeRanges)
            {
                context.WriteInformation(string.Format("Data Replication for table {0} from {1} to {2} started", GetFullTableName(context.SourceTableName, null, context.TableSchema), dateTimeRange.From.ToString("yyyy-MM-dd"), dateTimeRange.To.ToString("yyyy-MM-dd")));

                StringBuilder strBuilder = new StringBuilder();
                strBuilder.AppendFormat("SELECT {0} FROM {1} WITH(NOLOCK)", concatColumns, sourceTableNameWithSchema);

                if (splitByDate)
                    strBuilder.Append(string.Format(" WHERE {0} ", string.Join(" AND ", queryFilters)));

                DataTable datable = new DataTable();
                using (SqlConnection sourceConnection = new SqlConnection(_sourceConnectionString))
                {
                    sourceConnection.Open();
                    SqlCommand cmd = new SqlCommand(strBuilder.ToString(), sourceConnection);

                    if (splitByDate)
                    {
                        var fromParam = new SqlParameter("@FromTime", SqlDbType.DateTime);
                        fromParam.Value = dateTimeRange.From;
                        cmd.Parameters.Add(fromParam);

                        var toParam = new SqlParameter("@ToTime", SqlDbType.DateTime);
                        toParam.Value = dateTimeRange.To;
                        cmd.Parameters.Add(toParam);
                    }

                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd))
                    {
                        sqlDataAdapter.Fill(datable);
                    }
                }

                using (SqlBulkCopy bulkcopy = new SqlBulkCopy(context.TargetConnectionString, SqlBulkCopyOptions.KeepIdentity))
                {
                    bulkcopy.DestinationTableName = GetFullTableName(context.TargetTempTableName, null, context.TableSchema);
                    bulkcopy.WriteToServer(datable);
                }
                totalCount += datable.Rows.Count;
                context.WriteInformation(string.Format("Data Replication for table {0} from {1} to {2} done. Rows copied: {3}", GetFullTableName(context.SourceTableName, null, context.TableSchema), dateTimeRange.From.ToString("yyyy-MM-dd"), dateTimeRange.To.ToString("yyyy-MM-dd"), datable.Rows.Count));
            }

            if (splitByDate)
                context.WriteInformation(string.Format("Data Replication for table {0} from {1} to {2} done. Total rows copied: {3}", GetFullTableName(context.SourceTableName, null, context.TableSchema), context.FromTime.ToString("yyyy-MM-dd"), context.ToTime.ToString("yyyy-MM-dd"), totalCount));
        }

        private string GetFullTableName(string tableName, string tableSuffix = null, string tableSchema = null)
        {
            string result = tableName;

            if (!string.IsNullOrEmpty(tableSuffix))
                result = string.Format("{0}{1}", tableName, tableSuffix);

            if (!string.IsNullOrEmpty(tableSchema))
                result = string.Format("{0}.{1}", tableSchema, result);

            return result;
        }
    }
}
//using System;
//using System.Collections.Generic;
//using System.Data.SqlClient;
//using System.Text;
//using Vanrise.Data.SQL;
//using Vanrise.Entities;
//using System.Linq;

//namespace Vanrise.Common.Data.SQL
//{
//    public class DBReplicationTableDataManager : BaseSQLDataManager
//    {
//        public DBReplicationTableDataManager(string connectionString)
//            : base(connectionString, false)
//        {

//        }

//        public void MigrateData(IDBReplicationTableMigrateDataContext context)
//        {
//            bool useAuxTable = false;
//            string fullTargetTempTableName = string.Format("[{0}].[{1}].[{2}].[{3}]", context.TargetLinkedServerName, context.TargetDatabaseName, context.TableSchema, context.TargetTempTableName);

//            string fullTargetAuxTableName = null;
//            if (!string.IsNullOrEmpty(context.TargetAuxTableName))
//            {
//                useAuxTable = true;
//                fullTargetAuxTableName = string.Format("[{0}].[{1}].[{2}].[{3}]", context.TargetLinkedServerName, context.TargetDatabaseName, context.TableSchema, context.TargetAuxTableName);
//            }

//            List<string> queryFilters = new List<string>();

//            bool splitByDate = false;
//            if (!string.IsNullOrEmpty(context.FilterDateTimeColumn))
//            {
//                splitByDate = true;
//                queryFilters.Add(string.Format("({0} >= @FromTime)", context.FilterDateTimeColumn));
//                queryFilters.Add(string.Format("({0} < @ToTime)", context.FilterDateTimeColumn));
//            }

//            StringBuilder queryBuilder = new StringBuilder();
//            string concatColumns = string.Join<string>(",", context.Columns);


//            string sourceTableNameWithSchema = string.Format("[{0}].[{1}]", context.TableSchema, context.SourceTableName);
//            queryBuilder.AppendFormat("INSERT INTO {0} WITH (TABLOCK) ({1}) SELECT {1} FROM {2} WITH(NOLOCK)", !useAuxTable ? fullTargetTempTableName : fullTargetAuxTableName, concatColumns, sourceTableNameWithSchema);

//            if (queryFilters.Count > 0)
//                queryBuilder.Append(string.Format(" WHERE {0} ", string.Join(" AND ", queryFilters)));

//            DateTime from = context.FromTime;
//            DateTime to = context.ToTime;

//            List<DateTimeRange> dateTimeRanges = new List<DateTimeRange>() { new DateTimeRange() { From = context.FromTime, To = context.ToTime } };
//            if (splitByDate)
//            {
//                TimeSpan dbReplicationTimeInterval = new TimeSpan(context.NumberOfDaysPerInterval, 0, 0, 0);
//                dateTimeRanges = Vanrise.Common.Utilities.GenerateDateTimeRanges(context.FromTime, context.ToTime, dbReplicationTimeInterval).ToList();
//            }

//            foreach (DateTimeRange dateTimeRange in dateTimeRanges)
//            {
//                if (splitByDate)
//                    context.WriteInformation(string.Format("Data Replication for table {0} from {1} to {2} started", GetFullTableName(context.SourceTableName, null, context.TableSchema), dateTimeRange.From, dateTimeRange.To));

//                ExecuteNonQueryText(queryBuilder.ToString(), (cmd) =>
//                {
//                    if (splitByDate)
//                    {
//                        cmd.Parameters.Add(new SqlParameter("@FromTime", dateTimeRange.From));
//                        cmd.Parameters.Add(new SqlParameter("@ToTime", dateTimeRange.To));
//                    }
//                });

//                if (splitByDate)
//                    context.WriteInformation(string.Format("Data Replication for table {0} from {1} to {2} done", GetFullTableName(context.SourceTableName, null, context.TableSchema), dateTimeRange.From.ToString("yyyy-MM-dd"), dateTimeRange.To.ToString("yyyy-MM-dd")));
//            }

//            if (useAuxTable)
//            {
//                string targetTempTableNameWithSchema = string.Format("[{0}].[{1}]", context.TableSchema, context.TargetTempTableName);
//                string targetAuxTableNameWithSchema = string.Format("[{0}].[{1}]", context.TableSchema, context.TargetAuxTableName);
//                string queryCopyData = string.Format(@"DECLARE @RunStoredProcSQL VARCHAR(1000);
//                                        SET @RunStoredProcSQL = 'EXEC {0}.[dbo].CopyData @SourceTableName=''{1}'',@TargetTableName=''{2}'',@Columns=''{3}''';
//                                        EXEC (@RunStoredProcSQL) AT [{4}];", context.TargetDatabaseName, targetAuxTableNameWithSchema, targetTempTableNameWithSchema, concatColumns, context.TargetLinkedServerName);

//                ExecuteNonQueryText(queryCopyData, (cmd) =>
//                {

//                });
//            }
//        }

//        private string GetFullTableName(string tableName, string tableSuffix = null, string tableSchema = null)
//        {
//            string result = tableName;

//            if (!string.IsNullOrEmpty(tableSuffix))
//                result = string.Format("{0}{1}", tableName, tableSuffix);

//            if (!string.IsNullOrEmpty(tableSchema))
//                result = string.Format("{0}.{1}", tableSchema, result);

//            return result;
//        }
//    }
//}
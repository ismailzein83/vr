using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Vanrise.Data.RDB;
using Vanrise.Entities;
using Vanrise.Integration.Entities;
using Vanrise.Common;

namespace Vanrise.Integration.Data.RDB
{
    public class DataSourceLogDataManager : IDataSourceLogDataManager
    {
        static string TABLE_NAME = "integration_DataSourceLog";
        static string TABLE_ALIAS = "vrDataSourceLog";
        const string COL_ID = "ID";
        const string COL_DataSourceId = "DataSourceId";
        const string COL_Severity = "Severity";
        const string COL_Message = "Message";
        const string COL_ImportedBatchId = "ImportedBatchId";
        const string COL_LogEntryTime = "LogEntryTime";


        static DataSourceLogDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_DataSourceId, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Severity, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_Message, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_ImportedBatchId, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_LogEntryTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "integration",
                DBTableName = "DataSourceLog",
                Columns = columns
            });
        }

        #region Private Methods
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("Integration", "BusinessProcessTrackingDBConnStringKey", "BusinessProcessTrackingDBConnStringKey");
        }

        #endregion

        #region Mappers

        DataSourceLog DataSourceLogMapper(IRDBDataReader reader)
        {
            DataSourceLog dataSourceLog = new DataSourceLog
            {
                ID = reader.GetInt(COL_ID),
                DataSourceId = reader.GetGuid(COL_DataSourceId),
                Severity = (LogEntryType)reader.GetInt(COL_Severity),
                Message = reader.GetString(COL_Message),
                LogEntryTime = reader.GetDateTime(COL_LogEntryTime)
            };

            return dataSourceLog;
        }

        #endregion

        #region IDataSourceLogDataManager

        public void InsertEntry(Vanrise.Entities.LogEntryType entryType, string message, Guid dataSourceId, long? importedBatchId, string logTimeSpan)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.Column(COL_Severity).Value((int)entryType);
            insertQuery.Column(COL_Message).Value(message);
            insertQuery.Column(COL_DataSourceId).Value(dataSourceId);
            if (importedBatchId.HasValue)
                insertQuery.Column(COL_ImportedBatchId).Value(importedBatchId.Value);
            insertQuery.Column(COL_LogEntryTime).Value(logTimeSpan);

            queryContext.ExecuteNonQuery();
        }

        public List<DataSourceLog> GetFilteredDataSourceLogs(DataSourceLogQuery query)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, query.Top, true);
            selectQuery.SelectColumns().Columns(COL_ID, COL_DataSourceId, COL_Severity, COL_Message, COL_LogEntryTime);
            var where = selectQuery.Where();
            if (query.DataSourceId.HasValue)
                where.EqualsCondition(COL_DataSourceId).Value(query.DataSourceId.Value);
            if (query.Severities != null)
                where.ListCondition(COL_Severity, RDBListConditionOperator.IN, query.Severities.MapRecords(x => (int)x));
            if (query.From.HasValue)
                where.GreaterOrEqualCondition(COL_LogEntryTime).Value(query.From.Value);
            if (query.To.HasValue)
                where.LessOrEqualCondition(COL_LogEntryTime).Value(query.To.Value);
            return queryContext.GetItems(DataSourceLogMapper);
        }

        #endregion

    }
}

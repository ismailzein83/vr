using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Vanrise.Data.RDB;
using Vanrise.Entities;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Data.RDB
{
    public class DataSourceDataManager : IDataSourceDataManager
    {
        static string TABLE_NAME = "integration_DataSource";
        static string TABLE_ALIAS = "vrDataSource";

        const string COL_ID = "ID";
        const string COL_Name = "Name";
        const string COL_AdapterID = "AdapterID";
        const string COL_AdapterState = "AdapterState";
        const string COL_TaskId = "TaskId";
        const string COL_Settings = "Settings";
        const string COL_LastModifiedTime = "LastModifiedTime";
        const string COL_CreatedTime = "CreatedTime";


        static DataSourceDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 100 });
            columns.Add(COL_AdapterID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_AdapterState, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 1000 });
            columns.Add(COL_TaskId, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "integration",
                DBTableName = "DataSource",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime

            });
        }

        #region Private Methods
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("Integration", "IntegrationConfigDBConnStringKey", "IntegrationConfigDBConnStringKey");
        }

        public DataSource DataSourceMapper(IRDBDataReader reader)
        {
            DataSource DataSource = new DataSource
            {
                DataSourceId = reader.GetGuid(COL_ID),
                Name = reader.GetString(COL_Name),
                AdapterTypeId = reader.GetGuid(COL_AdapterID),
                AdapterState = Vanrise.Common.Serializer.Deserialize<BaseAdapterState>(reader.GetString(COL_AdapterState)),
                TaskId = reader.GetGuid(COL_TaskId),
                Settings = Vanrise.Common.Serializer.Deserialize<DataSourceSettings>(reader.GetString(COL_Settings))
            };
            return DataSource;
        }

        #endregion


        #region IDataSourceManager
        public List<DataSource> GetAllDataSources()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            return queryContext.GetItems(DataSourceMapper);
        }

        public bool AreDataSourcesUpdated(ref object lastReceivedDataInfo)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref lastReceivedDataInfo);
        }

        public bool AddDataSource(DataSource dataSource)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.Column(COL_ID).Value(dataSource.DataSourceId);
            insertQuery.Column(COL_Name).Value(dataSource.Name);
            insertQuery.Column(COL_AdapterID).Value(dataSource.AdapterTypeId);
            if (dataSource.AdapterState != null)
                insertQuery.Column(COL_AdapterState).Value(Vanrise.Common.Serializer.Serialize(dataSource.AdapterState));
            insertQuery.Column(COL_TaskId).Value(dataSource.TaskId);
            if (dataSource.Settings != null)
                insertQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(dataSource.Settings));
            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool UpdateDataSource(DataSource dataSource)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_Name).Value(dataSource.Name);
            updateQuery.Column(COL_AdapterID).Value(dataSource.AdapterTypeId);
            if (dataSource.Settings != null)
                updateQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(dataSource.Settings));
            else
                updateQuery.Column(COL_Settings).Null();
            updateQuery.Where().EqualsCondition(COL_ID).Value(dataSource.DataSourceId);
            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool DeleteDataSource(Guid dataSourceId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(TABLE_NAME);
            deleteQuery.Where().EqualsCondition(COL_ID).Value(dataSourceId);
            return queryContext.ExecuteNonQuery() > 0;
        }


        public bool UpdateTaskId(Guid dataSourceId, Guid taskId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_TaskId).Value(taskId);
            updateQuery.Where().EqualsCondition(COL_ID).Value(dataSourceId);
            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool UpdateAdapterState(Guid dataSourceId, Entities.BaseAdapterState adapterState)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            if (adapterState != null)
                updateQuery.Column(COL_AdapterState).Value(Vanrise.Common.Serializer.Serialize(adapterState));
            else
                updateQuery.Column(COL_AdapterState).Null();
            updateQuery.Where().EqualsCondition(COL_ID).Value(dataSourceId);
            return queryContext.ExecuteNonQuery() > 0;
        }

        #endregion


    }
}

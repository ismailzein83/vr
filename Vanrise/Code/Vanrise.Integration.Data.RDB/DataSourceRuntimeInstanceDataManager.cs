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
    public class DataSourceRuntimeInstanceDataManager : IDataSourceRuntimeInstanceDataManager
    {
        static string TABLE_NAME = "integration_DataSourceRuntimeInstance";
        static string TABLE_ALIAS = "vrDataSourceRuntimeInstance";
        const string COL_ID = "ID";
        const string COL_DataSourceID = "DataSourceID";
        const string COL_CreatedTime = "CreatedTime";


        static DataSourceRuntimeInstanceDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_DataSourceID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "integration",
                DBTableName = "DataSourceRuntimeInstance",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime
            });
        }

        #region Private Methods
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("Integration", "IntegrationConfigDBConnStringKey", "IntegrationConfigDBConnStringKey");
        }

        #endregion

        #region Mappers

        DataSourceRuntimeInstance DataSourceRuntimeInstanceMapper(IRDBDataReader reader)
        {
            return new DataSourceRuntimeInstance
            {

                DataSourceRuntimeInstanceId = reader.GetGuid(COL_ID),
                DataSourceId = reader.GetGuid(COL_DataSourceID),
                CreatedTime = reader.GetDateTime(COL_CreatedTime)
            };
        }

        #endregion

        #region IDataSourceRuntimeInstanceDataManager

        public void AddNewInstance(Guid runtimeInstanceId, Guid dataSourceId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.Column(COL_ID).Value(runtimeInstanceId);
            insertQuery.Column(COL_DataSourceID).Value(dataSourceId);
            queryContext.ExecuteNonQuery();
        }

        public void TryAddNewInstance(Guid runtimeInstanceId, Guid dataSourceId, int maxNumberOfParallelInstances)
        {

            var selectQueryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = selectQueryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS);
            selectQuery.SelectAggregates().Count("Count");
            selectQuery.Where().EqualsCondition(COL_DataSourceID).Value(dataSourceId);

            var count = selectQueryContext.ExecuteScalar().IntValue;
            if (count >= maxNumberOfParallelInstances)
                return;

            var insertQueryContext = new RDBQueryContext(GetDataProvider());

            var insertQuery = insertQueryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.Column(COL_ID).Value(runtimeInstanceId);
            insertQuery.Column(COL_DataSourceID).Value(dataSourceId);

            insertQueryContext.ExecuteNonQuery();

        }
        public List<DataSourceRuntimeInstance> GetAll()
        {
            var selectQueryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = selectQueryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS,null,true);
            selectQuery.SelectColumns().Columns(COL_ID, COL_DataSourceID,COL_CreatedTime);
            return selectQueryContext.GetItems(DataSourceRuntimeInstanceMapper);
        }


        public bool IsStillExist(Guid dsRuntimeInstanceId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, 1, true);
            selectQuery.SelectColumns().Column(COL_ID);

            var where = selectQuery.Where();
            where.EqualsCondition(COL_ID).Value(dsRuntimeInstanceId);

            return queryContext.ExecuteScalar().NullableLongValue.HasValue;
        }
        public void DeleteInstance(Guid dsRuntimeInstanceId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(TABLE_NAME);
            deleteQuery.Where().EqualsCondition(COL_ID).Value(dsRuntimeInstanceId);
        }
        public bool DoesAnyDSRuntimeInstanceExist(Guid dataSourceId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, 1, true);
            selectQuery.SelectColumns().Column(COL_ID);

            var where = selectQuery.Where();
            where.EqualsCondition(COL_DataSourceID).Value(dataSourceId);

            return queryContext.ExecuteScalar().NullableLongValue.HasValue;
        }

        #endregion

    }
}

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
    public class DataSourceStateDataManager : IDataSourceStateDataManager
    {
        static string TABLE_NAME = "integration_DataSourceState";
        static string TABLE_ALIAS = "vrDataSourceState";
        const string COL_DataSourceID = "DataSourceID";
        const string COL_State = "State";
        const string COL_LockedByProcessID = "LockedByProcessID";


        static DataSourceStateDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_DataSourceID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_State, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_LockedByProcessID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "integration",
                DBTableName = "DataSourceState",
                Columns = columns,
                IdColumnName = COL_DataSourceID
            });
        }

        #region Private Methods
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("Integration", "IntegrationConfigDBConnStringKey", "IntegrationConfigDBConnString");
        }

        #endregion

        #region Mappers



        #endregion

        #region IDataSourceStateDataManager

        public string GetDataSourceState(Guid dataSourceId)
        {
            var selectQueryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = selectQueryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS);
            selectQuery.SelectColumns().Columns(COL_State);
            selectQuery.Where().EqualsCondition(COL_DataSourceID).Value(dataSourceId);
            return selectQueryContext.ExecuteScalar().StringValue;
        }

        public void InsertOrUpdateDataSourceState(Guid dataSourceId, string dataSourceState)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            var where = updateQuery.Where();
            where.EqualsCondition(COL_DataSourceID).Value(dataSourceId);
            updateQuery.Column(COL_State).Value(dataSourceState);
            if (queryContext.ExecuteNonQuery() <= 0)
            {
                var insertQuery = queryContext.AddInsertQuery();
                insertQuery.IntoTable(TABLE_NAME);
                var ifNotExist = insertQuery.IfNotExists(TABLE_ALIAS);
                ifNotExist.EqualsCondition(COL_DataSourceID).Value(dataSourceId);
                insertQuery.Column(COL_DataSourceID).Value(dataSourceId);
                insertQuery.Column(COL_State).Value(dataSourceState);
                queryContext.ExecuteNonQuery();
            }
        }

        #endregion

    }
}

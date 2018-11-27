using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.GenericData.Data.SQL;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Data.RDB
{
    public class DataStoreDataManager : IDataStoreDataManager
    {
        #region RDB
        static string TABLE_NAME = "genericdata_DataStore";
        static string TABLE_ALIAS = "ds";
        const string COL_ID = "ID";
        const string COL_Name = "Name";
        const string COL_Settings = "Settings";
        const string COL_CreatedTime = "CreatedTime";


        static DataStoreDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 1000 });
            columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "genericdata",
                DBTableName = "DataStore",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime
            });
        }
        #endregion
        #region Private Methods
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("VR_GenericData_DataStore", "ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey");
        }
        #endregion

        #region Mappers
        DataStore DataStoreMapper(IRDBDataReader reader)
        {
            return new DataStore()
            {
                DataStoreId = reader.GetGuid(COL_ID),
                Name = reader.GetString(COL_Name),
                Settings = Vanrise.Common.Serializer.Deserialize<DataStoreSettings>(reader.GetString(COL_Settings))
            };
        }
        #endregion

        public bool AddDataStore(DataStore dataStore)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            var ifNotExists = insertQuery.IfNotExists(TABLE_ALIAS);
            ifNotExists.EqualsCondition(COL_Name).Value(dataStore.Name);
            insertQuery.Column(COL_ID).Value(dataStore.DataStoreId);
            insertQuery.Column(COL_Name).Value(dataStore.Name);
            insertQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(dataStore.Settings));
            insertQuery.Column(COL_CreatedTime).DateNow();
            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool AreDataStoresUpdated(ref object updateHandle)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DataStore> GetDataStores()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().Columns(COL_ID, COL_Name, COL_Settings);
            selectQuery.Sort().ByColumn(COL_Name, RDBSortDirection.ASC);
            return queryContext.GetItems<DataStore>(DataStoreMapper);
        }

        public bool UpdateDataStore(DataStore dataStore)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            var ifNotExists = updateQuery.IfNotExists(TABLE_ALIAS);
            ifNotExists.NotEqualsCondition(COL_ID).Value(dataStore.DataStoreId);
            ifNotExists.EqualsCondition(COL_Name).Value(dataStore.Name);
            updateQuery.Column(COL_Name).Value(dataStore.Name);
            updateQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(dataStore.Settings));
            updateQuery.Where().EqualsCondition(COL_ID).Value(dataStore.DataStoreId);
            return queryContext.ExecuteNonQuery() > 0;
        }
    }
}

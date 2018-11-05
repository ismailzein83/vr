using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Data.RDB
{
    public class DataRecordStorageDataManager : IDataRecordStorageDataManager
    {
        #region RDB
        static string TABLE_NAME = "genericdata_DataRecordStorage";
        static string TABLE_ALIAS = "drs";
        const string COL_ID = "ID";
        const string COL_Name = "Name";
        const string COL_DataRecordTypeID = "DataRecordTypeID";
        const string COL_DataStoreID = "DataStoreID";
        const string COL_Settings = "Settings";
        const string COL_State = "State";
        const string COL_CreatedTime = "CreatedTime";


        static DataRecordStorageDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 1000 });
            columns.Add(COL_DataRecordTypeID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_DataStoreID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_State, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "genericdata",
                DBTableName = "DataRecordStorage",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime
            });
        }
        #endregion

        #region Private Methods
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("VR_GenericData_DataRecordStorage", "ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey");
        }
        #endregion

        #region Mappers
        DataRecordStorage DataRecordStorageMapper(IRDBDataReader reader)
        {
            var state = reader.GetStringWithEmptyHandling(COL_State);
            return new DataRecordStorage()
            {
                DataRecordStorageId = reader.GetGuid(COL_ID),
                Name = reader.GetString(COL_Name),
                DataRecordTypeId = reader.GetGuid(COL_DataRecordTypeID),
                DataStoreId = reader.GetGuid(COL_DataStoreID),
                Settings = Vanrise.Common.Serializer.Deserialize<DataRecordStorageSettings>(reader.GetString(COL_Settings)),
                State = state != null ? Vanrise.Common.Serializer.Deserialize<DataRecordStorageState>(state) : null
            };
        }
        #endregion

        #region IDataRecordStorageDataManager

        public bool AddDataRecordStorage(DataRecordStorage dataRecordStorage)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            var ifNotExists = insertQuery.IfNotExists(TABLE_ALIAS);
            ifNotExists.EqualsCondition(COL_Name).Value(dataRecordStorage.Name);
            insertQuery.Column(COL_ID).Value(dataRecordStorage.DataRecordStorageId);
            insertQuery.Column(COL_Name).Value(dataRecordStorage.Name);
            insertQuery.Column(COL_DataRecordTypeID).Value(dataRecordStorage.DataRecordTypeId);
            insertQuery.Column(COL_DataStoreID).Value(dataRecordStorage.DataStoreId);
            insertQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(dataRecordStorage.Settings));
            insertQuery.Column(COL_CreatedTime).DateNow();
            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool AreDataRecordStoragesUpdated(ref object updateHandle)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DataRecordStorage> GetDataRecordStorages()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().Columns(COL_ID, COL_Name, COL_DataRecordTypeID, COL_DataStoreID, COL_Settings, COL_State);
            selectQuery.Sort().ByColumn(COL_Name, RDBSortDirection.ASC);
            return queryContext.GetItems<DataRecordStorage>(DataRecordStorageMapper);
        }

        public bool UpdateDataRecordStorage(DataRecordStorage dataRecordStorage)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            var ifNotExists = updateQuery.IfNotExists(TABLE_ALIAS);
            ifNotExists.NotEqualsCondition(COL_ID).Value(dataRecordStorage.DataRecordStorageId);
            ifNotExists.EqualsCondition(COL_Name).Value(dataRecordStorage.Name);
            updateQuery.Column(COL_Name).Value(dataRecordStorage.Name);
            updateQuery.Column(COL_DataRecordTypeID).Value(dataRecordStorage.DataRecordTypeId);
            updateQuery.Column(COL_DataStoreID).Value(dataRecordStorage.DataStoreId);
            updateQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(dataRecordStorage.Settings));
            updateQuery.Where().EqualsCondition(COL_ID).Value(dataRecordStorage.DataRecordStorageId);
            return queryContext.ExecuteNonQuery() > 0;
        }
        #endregion
    }
}

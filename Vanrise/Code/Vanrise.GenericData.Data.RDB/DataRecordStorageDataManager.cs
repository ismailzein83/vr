﻿using System;
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
        const string COL_LastModifiedTime = "LastModifiedTime";

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
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "genericdata",
                DBTableName = "DataRecordStorage",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime
            });
        }
        #endregion

        #region Private Methods
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("VR_GenericData", "ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey");
        }
        #endregion

        #region Mappers
        DataRecordStorage DataRecordStorageMapper(IRDBDataReader reader)
        {
            return new DataRecordStorage()
            {
                DataRecordStorageId = reader.GetGuid(COL_ID),
                Name = reader.GetString(COL_Name),
                DataRecordTypeId = reader.GetGuid(COL_DataRecordTypeID),
                DataStoreId = reader.GetGuid(COL_DataStoreID),
                Settings = Vanrise.Common.Serializer.Deserialize<DataRecordStorageSettings>(reader.GetString(COL_Settings)),
                State = Vanrise.Common.Serializer.Deserialize<DataRecordStorageState>(reader.GetString(COL_State))
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
            if(dataRecordStorage.Settings!=null)
                insertQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(dataRecordStorage.Settings));
            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool AreDataRecordStoragesUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        public IEnumerable<DataRecordStorage> GetDataRecordStorages()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            selectQuery.Sort().ByColumn(COL_Name, RDBSortDirection.ASC);
            return queryContext.GetItems(DataRecordStorageMapper);
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
            if (dataRecordStorage.Settings != null)
                updateQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(dataRecordStorage.Settings));
            else
                updateQuery.Column(COL_Settings).Null();
            updateQuery.Where().EqualsCondition(COL_ID).Value(dataRecordStorage.DataRecordStorageId);
            return queryContext.ExecuteNonQuery() > 0;
        }
        #endregion
    }
}

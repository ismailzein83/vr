using System;
using System.Collections.Generic;
using System.Data;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.Common.Data.RDB
{
    public class ProcessStateDataManager : IProcessStateDataManager
    {

        #region RDB 

        public static string TABLE_NAME = "common_ProcessState";
        public static string TABLE_ALIAS = "common_ProcessState";
        public const string COL_UniqueName = "UniqueName";
        public const string COL_Settings = "Settings";
        const string COL_LastModifiedTime = "LastModifiedTime";
        const string COL_CreatedTime = "CreatedTime";

        static ProcessStateDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_UniqueName, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 255 });
            columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "common",
                DBTableName = "ProcessState",
                Columns = columns,
                IdColumnName = COL_UniqueName,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime,
            });
        }

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("VR_CommonTransaction", "VRCommonTransactionDBConnStringKey", "TransactionDBConnString");
        }

        #endregion


        #region Public Methods

        public List<ProcessState> GetProcessStates()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            return queryContext.GetItems(ProcessStateEntityMapper);
        }

        public bool InsertOrUpdate(string processStateUniqueName, ProcessStateSettings settings)
        {
            int? isInsertedOrUpdated;

            var queryContext = new RDBQueryContext(GetDataProvider());

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(settings));
            updateQuery.Column(COL_LastModifiedTime).Value(DateTime.Now);
            var where = updateQuery.Where();
            where.EqualsCondition(COL_UniqueName).Value(processStateUniqueName);

            isInsertedOrUpdated = queryContext.ExecuteScalar().NullableIntValue;

            if (!isInsertedOrUpdated.HasValue)
            {
                var query2Context = new RDBQueryContext(GetDataProvider());
                var insertQuery = query2Context.AddInsertQuery();
                insertQuery.IntoTable(TABLE_NAME);
                insertQuery.Column(COL_UniqueName).Value(processStateUniqueName);
                insertQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(settings));

                isInsertedOrUpdated = query2Context.ExecuteScalar().NullableIntValue;
            }

            return isInsertedOrUpdated.HasValue;
        }

        #endregion

        #region Private Methods
        private ProcessState ProcessStateEntityMapper(IRDBDataReader reader)
        {
            return new ProcessState()
            {
                UniqueName = reader.GetString("UniqueName"),
                Settings = Vanrise.Common.Serializer.Deserialize<ProcessStateSettings>(reader.GetString("Settings"))
            };
        }
        #endregion
    }
}
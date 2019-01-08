using System;
using System.Collections.Generic;
using Vanrise.Security.Entities;
using Vanrise.Data.RDB;

namespace Vanrise.Security.Data.RDB
{
    public class RequiredPermissionSetDataManager : IRequiredPermissionSetDataManager
    {
        #region RDB
        static string TABLE_NAME = "sec_RequiredPermissionSet";
        static string TABLE_ALIAS = "set";
        const string COL_ID = "ID";
        const string COL_Module = "Module";
        const string COL_RequiredPermissionString = "RequiredPermissionString";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_LastModifiedTime = "LastModifiedTime";

        static RequiredPermissionSetDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_Module, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 255 });
            columns.Add(COL_RequiredPermissionString, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 255 });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "sec",
                DBTableName = "RequiredPermissionSet",
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
            return RDBDataProviderFactory.CreateProvider("VR_Sec", "SecurityDBConnStringKey", "SecurityDBConnString");
        }
        #endregion

        #region Mappers
        RequiredPermissionSet RequiredPermissionSetMapper(IRDBDataReader reader)
        {
            return new RequiredPermissionSet
            {
                RequiredPermissionSetId = reader.GetInt(COL_ID),
                Module = reader.GetString(COL_Module),
                RequiredPermissionString = reader.GetString(COL_RequiredPermissionString)
            };
        }
        #endregion

        #region IRequiredPermissionSetDataManager
        public int AddIfNotExists(string module, string requiredPermissionString)
        {
            var selectQueryContext1 = new RDBQueryContext(GetDataProvider());
            var selectQuery1 = selectQueryContext1.AddSelectQuery();
            selectQuery1.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery1.SelectColumns().AllTableColumns(TABLE_ALIAS);
            var where1 = selectQuery1.Where();
            where1.EqualsCondition(COL_RequiredPermissionString).Value(requiredPermissionString);
            where1.EqualsCondition(COL_Module).Value(module);
            var item = selectQueryContext1.GetItem(RequiredPermissionSetMapper);
            if (item == null)
            {
                var insertQueryContext = new RDBQueryContext(GetDataProvider());
                var insertQuery = insertQueryContext.AddInsertQuery();
                insertQuery.IntoTable(TABLE_NAME);
                insertQuery.Column(COL_Module).Value(module);
                insertQuery.Column(COL_RequiredPermissionString).Value(requiredPermissionString);
                insertQueryContext.ExecuteNonQuery();
            }
            var selectQueryContext2 = new RDBQueryContext(GetDataProvider());
            var selectQuery2 = selectQueryContext2.AddSelectQuery();
            selectQuery2.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery2.SelectColumns().Column(COL_ID);
            var where2 = selectQuery2.Where();
            where2.EqualsCondition(COL_RequiredPermissionString).Value(requiredPermissionString);
            where2.EqualsCondition(COL_Module).Value(module);
            return selectQueryContext2.ExecuteScalar().IntValue;
        }

        public bool AreRequiredPermissionSetsUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        public List<RequiredPermissionSet> GetAll()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            return queryContext.GetItems(RequiredPermissionSetMapper);
        }
        #endregion
    }
}

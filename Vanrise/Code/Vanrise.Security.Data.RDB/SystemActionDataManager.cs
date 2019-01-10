using System;
using System.Collections.Generic;
using Vanrise.Security.Entities;
using Vanrise.Data.RDB;
using Vanrise.Entities;
namespace Vanrise.Security.Data.RDB
{
    public class SystemActionDataManager : ISystemActionDataManager
    {
        #region RDB
        static string TABLE_NAME = "sec_SystemAction";
        static string TABLE_ALIAS = "action";
        const string COL_ID = "ID";
        const string COL_Name = "Name";
        const string COL_RequiredPermissions = "RequiredPermissions";
        const string COL_LastModifiedTime = "LastModifiedTime";

        static SystemActionDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 900 });
            columns.Add(COL_RequiredPermissions, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 1000 });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "sec",
                DBTableName = "SystemAction",
                Columns = columns,
                IdColumnName = COL_ID,
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
        SystemAction SystemActionMapper(IRDBDataReader reader)
        {
            return new SystemAction()
            {
                SystemActionId = reader.GetInt(COL_ID),
                Name = reader.GetString(COL_Name),
                RequiredPermissions = reader.GetString(COL_RequiredPermissions)
            };
        }
        #endregion

        #region ISystemActionDataManager
        public bool AreSystemActionsUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        public IEnumerable<SystemAction> GetSystemActions()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            selectQuery.Sort().ByColumn(COL_Name, RDBSortDirection.ASC);
            return queryContext.GetItems(SystemActionMapper);
        }
        #endregion
    }
}

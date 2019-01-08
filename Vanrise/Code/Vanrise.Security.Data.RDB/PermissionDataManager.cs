using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Data.RDB
{
    public class PermissionDataManager : IPermissionDataManager
    {
        #region RDB

        static string TABLE_NAME = "sec_Permission";
        static string TABLE_ALIAS = "permission";
        const string COL_HolderType = "HolderType";
        const string COL_HolderId = "HolderId";
        const string COL_EntityType = "EntityType";
        const string COL_EntityId = "EntityId";
        const string COL_PermissionFlags = "PermissionFlags";
        const string COL_LastModifiedTime = "LastModifiedTime";

        static PermissionDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_HolderType, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_HolderId, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            columns.Add(COL_EntityType, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_EntityId, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            columns.Add(COL_PermissionFlags, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 1000 });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "sec",
                DBTableName = "Permission",
                Columns = columns,
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
        Permission PermissionMapper(IRDBDataReader reader)
        {
            Permission permission = new Permission
            {
                HolderType = (HolderType)reader.GetInt(COL_HolderType),
                HolderId = reader.GetString(COL_HolderId),
                EntityType = (EntityType)reader.GetInt(COL_EntityType),
                EntityId = reader.GetString(COL_EntityId),
            };
            var serializedPermissionFlags = reader.GetString(COL_PermissionFlags);
            if (!string.IsNullOrEmpty(serializedPermissionFlags))
                permission.PermissionFlags = Common.Serializer.Deserialize<List<PermissionFlag>>(serializedPermissionFlags);
            return permission;
        }
        #endregion

        #region IPermissionDataManager
        public bool ArePermissionsUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        public bool DeletePermission(Permission permission)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(TABLE_NAME);
            var whereQuery = deleteQuery.Where();
            whereQuery.EqualsCondition(COL_HolderType).Value((int)permission.HolderType);
            whereQuery.EqualsCondition(COL_HolderId).Value(permission.HolderId);
            whereQuery.EqualsCondition(COL_EntityType).Value((int)permission.EntityType);
            whereQuery.EqualsCondition(COL_EntityId).Value(permission.EntityId);
            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool DeletePermission(HolderType holderType, string holderId, EntityType entityType, string entityId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(TABLE_NAME);
            var whereQuery = deleteQuery.Where();
            whereQuery.EqualsCondition(COL_HolderType).Value((int)holderType);
            whereQuery.EqualsCondition(COL_HolderId).Value(holderId);
            whereQuery.EqualsCondition(COL_EntityType).Value((int)entityType);
            whereQuery.EqualsCondition(COL_EntityId).Value(entityId);
            return queryContext.ExecuteNonQuery() > 0;
        }

        public IEnumerable<Permission> GetHolderPermissions(HolderType holderType, string holderId)
        {
            var groupDataManager = new GroupDataManager();
            var userDataManager = new UserDataManager();
            string groupAlias = "r";
            string userAlias = "u";
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            var selectColumns = selectQuery.SelectColumns();
            selectColumns.Column(COL_HolderType);
            var holderNameExp = selectColumns.Expression("HolderName").CaseExpression();
            var holderNameCase1 = holderNameExp.AddCase();
            holderNameCase1.When().EqualsCondition(COL_HolderType).Value((int)HolderType.USER);
            holderNameCase1.Then().Column(userAlias, UserDataManager.COL_Name);
            holderNameExp.Else().Column(groupAlias, GroupDataManager.COL_Name);
            selectColumns.Columns(COL_EntityId, COL_EntityType, COL_HolderId, COL_PermissionFlags);
            groupDataManager.SetJoinContext(selectQuery.Join(), TABLE_ALIAS, groupAlias, COL_HolderId, RDBJoinType.Left);
            userDataManager.SetJoinContext(selectQuery.Join(), TABLE_ALIAS, userAlias, COL_HolderId, RDBJoinType.Left);
            var whereQuery = selectQuery.Where();
            whereQuery.EqualsCondition(COL_HolderType).Value((int)holderType);
            whereQuery.EqualsCondition(COL_HolderId).Value(holderId);
            return queryContext.GetItems(PermissionMapper);
        }

        public IEnumerable<Permission> GetPermissions()
        {
            var groupDataManager = new GroupDataManager();
            var userDataManager = new UserDataManager();
            string groupAlias = "r";
            string userAlias = "u";
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            var selectColumns = selectQuery.SelectColumns();
            selectColumns.Column(COL_HolderType);
            var holderNameExp = selectColumns.Expression("HolderName").CaseExpression();
            var holderNameCase1 = holderNameExp.AddCase();
            holderNameCase1.When().EqualsCondition(COL_HolderType).Value((int)HolderType.USER);
            holderNameCase1.Then().Column(userAlias, UserDataManager.COL_Name);
            holderNameExp.Else().Column(groupAlias, GroupDataManager.COL_Name);
            selectColumns.Columns(COL_EntityId, COL_EntityType, COL_HolderId, COL_PermissionFlags);
            groupDataManager.SetJoinContext(selectQuery.Join(), TABLE_ALIAS, groupAlias, COL_HolderId, RDBJoinType.Left);
            userDataManager.SetJoinContext(selectQuery.Join(), TABLE_ALIAS, userAlias, COL_HolderId, RDBJoinType.Left);
            return queryContext.GetItems(PermissionMapper);
        }

        public bool UpdatePermission(Permission permission)
        {
            var selectQueryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = selectQueryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            var whereQuery = selectQuery.Where();
            whereQuery.EqualsCondition(COL_HolderType).Value((int)permission.HolderType);
            whereQuery.EqualsCondition(COL_HolderId).Value(permission.HolderId);
            whereQuery.EqualsCondition(COL_EntityType).Value((int)permission.EntityType);
            whereQuery.EqualsCondition(COL_EntityId).Value(permission.EntityId);
            var permissions = selectQueryContext.GetItems(PermissionMapper);
            if (permissions != null && permissions.Count>0)
            {
                var updateQueryContext = new RDBQueryContext(GetDataProvider());
                var updateQuery = updateQueryContext.AddUpdateQuery();
                updateQuery.FromTable(TABLE_NAME);
                if (permission.PermissionFlags != null)
                    updateQuery.Column(COL_PermissionFlags).Value(Common.Serializer.Serialize(permission.PermissionFlags));
                else
                    updateQuery.Column(COL_PermissionFlags).Null();
                var where = updateQuery.Where();
                where.EqualsCondition(COL_HolderType).Value((int)permission.HolderType);
                where.EqualsCondition(COL_HolderId).Value(permission.HolderId);
                where.EqualsCondition(COL_EntityType).Value((int)permission.EntityType);
                where.EqualsCondition(COL_EntityId).Value(permission.EntityId);
                return updateQueryContext.ExecuteNonQuery() > 0;
            }
            else
            {
                var insertQueryContext = new RDBQueryContext(GetDataProvider());
                var insertQuery = insertQueryContext.AddInsertQuery();
                insertQuery.IntoTable(TABLE_NAME);
                insertQuery.Column(COL_HolderType).Value((int)permission.HolderType);
                insertQuery.Column(COL_HolderId).Value(permission.HolderId);
                insertQuery.Column(COL_EntityType).Value((int)permission.EntityType);
                insertQuery.Column(COL_EntityId).Value(permission.EntityId);
                if (permission.PermissionFlags != null)
                    insertQuery.Column(COL_PermissionFlags).Value(Common.Serializer.Serialize(permission.PermissionFlags));
                return insertQueryContext.ExecuteNonQuery() > 0;
            }
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Security.Entities;
using Vanrise.Entities;
namespace Vanrise.Security.Data.RDB
{
    public class GroupDataManager : IGroupDataManager
    {
        #region RDB
        static string TABLE_NAME = "sec_Group";
        static string TABLE_ALIAS = "group";
        const string COL_ID = "ID";
        const string COL_PSIdentifier = "PSIdentifier";
        internal const string COL_Name = "Name";
        const string COL_Description = "Description";
        const string COL_Settings = "Settings";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_LastModifiedTime = "LastModifiedTime";

        static GroupDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_PSIdentifier, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_Description, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "sec",
                DBTableName = "Group",
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
        Group GroupMapper(IRDBDataReader reader)
        {
            return new Group
            {
                GroupId = reader.GetInt(COL_ID),
                Name = reader.GetString(COL_Name),
                Description = reader.GetString(COL_Description),
                Settings = Common.Serializer.Deserialize<GroupSettings>(reader.GetString(COL_Settings))
            };
        }
        #endregion
        #region IGroupDataManager
        public bool AddGroup(Group role, out int insertedId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.AddSelectGeneratedId();
            insertQuery.IfNotExists(TABLE_ALIAS).EqualsCondition(COL_Name).Value(role.Name);
            insertQuery.Column(COL_Name).Value(role.Name);
            insertQuery.Column(COL_Description).Value(role.Description);
            if (role.Settings != null)
                insertQuery.Column(COL_Settings).Value(Common.Serializer.Serialize(role.Settings));
            var id = queryContext.ExecuteScalar().NullableIntValue;
            if (id.HasValue)
                insertedId = id.Value;
            else
                insertedId = -1;
            return insertedId != -1;
        }

        public bool AreGroupsUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        public void AssignMembers(int roleId, int[] members)
        {

        }

        public List<Group> GetGroups()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            selectQuery.Sort().ByColumn(COL_Name, RDBSortDirection.ASC);
            return queryContext.GetItems(GroupMapper);
        }

        public List<int> GetUserGroups(int userId)
        {
            var userGroupDataManager = new UserGroupDataManager();
            List<int> result = new List<int>();
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            userGroupDataManager.SetSelectQuery(selectQuery, userId);
            queryContext.ExecuteReader(reader =>
            {
                while (reader.Read())
                {
                    result.Add(reader.GetInt(UserGroupDataManager.COL_GroupId));
                }
            });
            return result;
        }

        public bool UpdateGroup(Group role)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            var ifNotExists = updateQuery.IfNotExists(TABLE_ALIAS);
            ifNotExists.EqualsCondition(COL_Name).Value(role.Name);
            ifNotExists.NotEqualsCondition(COL_ID).Value(role.GroupId);
            updateQuery.Column(COL_Name).Value(role.Name);
            updateQuery.Column(COL_Description).Value(role.Description);
            if (role.Settings != null)
                updateQuery.Column(COL_Settings).Value(Common.Serializer.Serialize(role.Settings));
            else
                updateQuery.Column(COL_Settings).Null();
            updateQuery.Where().EqualsCondition(COL_ID).Value(role.GroupId);
            return queryContext.ExecuteNonQuery() > 0;
        }
        #endregion

        #region PermissionDataManager
        public void SetJoinContext(RDBJoinContext joinContext, string table1Alias, string table2Alias, string column1Name, RDBJoinType joinType)
        {
            joinContext.JoinOnEqualOtherTableColumn(joinType, TABLE_NAME, table2Alias, COL_ID, table1Alias, column1Name);
        }
        #endregion
    }
}

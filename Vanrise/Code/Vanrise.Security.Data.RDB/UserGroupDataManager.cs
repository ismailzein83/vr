using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Data.RDB
{
    public class UserGroupDataManager : IUserGroupDataManager
    {
        #region RDB
        static string TABLE_NAME = "sec_UserGroup";
        static string TABLE_ALIAS = "userGroup";
        const string COL_UserId = "UserId";
        const string COL_GroupId = "GroupId";
        const string COL_LastModifiedTime = "LastModifiedTime";

        static UserGroupDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_UserId, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_GroupId, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "sec",
                DBTableName = "UserGroup",
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

        private UserGroup UserGroupMapper(IRDBDataReader reader)
        {
            return new Entities.UserGroup
            {
                UserId = reader.GetInt(COL_UserId),
                GroupId = reader.GetInt(COL_GroupId)
            };
        }
        #endregion

        public bool AreUserGroupUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        public IEnumerable<UserGroup> GetAllUserGroupEntities()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            return queryContext.GetItems(UserGroupMapper);
        }
    }
}

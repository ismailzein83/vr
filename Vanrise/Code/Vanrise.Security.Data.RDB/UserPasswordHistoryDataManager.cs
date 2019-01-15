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
    public class UserPasswordHistoryDataManager : IUserPasswordHistoryDataManager
    {
        #region RDB
        static string TABLE_NAME = "sec_UserPasswordHistory";
        static string TABLE_ALIAS = "userPasswordHistory";
        const string COL_ID = "ID";
        const string COL_UserID = "UserID";
        const string COL_Password = "Password";
        const string COL_IsChangedByAdmin = "IsChangedByAdmin";
        const string COL_CreatedTime = "CreatedTime";


        static UserPasswordHistoryDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_UserID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_Password, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_IsChangedByAdmin, new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "sec",
                DBTableName = "UserPasswordHistory",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime
            });
        }
        #endregion
        #region Mappers
        UserPasswordHistory UserPasswordHistoryMapper(IRDBDataReader reader)
        {
            return new UserPasswordHistory
            {
                Id = reader.GetLong(COL_ID),
                UserId = reader.GetInt(COL_UserID),
                IsChangedByAdmin = reader.GetBoolean(COL_IsChangedByAdmin),
                Password = reader.GetString(COL_Password)
            };
        }
        #endregion
        #region Private Methods
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("VR_Sec", "SecurityDBConnStringKey", "SecurityDBConnString");
        }
        #endregion
        #region IUserPasswordHistoryDataManager
        public bool AddUserPasswordHistory(UserPasswordHistory history, out int insertedId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.AddSelectGeneratedId();
            insertQuery.Column(COL_UserID).Value(history.UserId);
            insertQuery.Column(COL_Password).Value(history.Password);
            insertQuery.Column(COL_IsChangedByAdmin).Value(history.IsChangedByAdmin);
            var id = queryContext.ExecuteScalar().NullableIntValue;
            if (id.HasValue)
            {
                insertedId = id.Value;
                return true;
            }
            else
            {
                insertedId = -1;
                return false;
            }
        }

        public IEnumerable<UserPasswordHistory> GetUserPasswordHistoryByUserId(int userId, int maxRecordsCount)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, maxRecordsCount, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            var whereQuery = selectQuery.Where();
            whereQuery.EqualsCondition(COL_UserID).Value(userId);
            var condition = whereQuery.ConditionIfColumnNotNull(COL_IsChangedByAdmin, RDBConditionGroupOperator.OR);
            condition.EqualsCondition(COL_IsChangedByAdmin).Value(false);
            selectQuery.Sort().ByColumn(COL_ID, RDBSortDirection.DESC);
            return queryContext.GetItems(UserPasswordHistoryMapper);
        }
        #endregion
    }
}

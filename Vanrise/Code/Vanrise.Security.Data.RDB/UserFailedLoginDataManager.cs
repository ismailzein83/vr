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
    public class UserFailedLoginDataManager : IUserFailedLoginDataManager
    {
        #region RDB
        static string TABLE_NAME = "sec_UserFailedLogin";
        static string TABLE_ALIAS = "failedLogin";
        const string COL_ID = "ID";
        const string COL_UserID = "UserID";
        const string COL_FailedResultID = "FailedResultID";
        const string COL_CreatedTime = "CreatedTime";


        static UserFailedLoginDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_UserID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_FailedResultID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "sec",
                DBTableName = "UserFailedLogin",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime
            });
        }

        #endregion

        #region Mappers
        UserFailedLogin UserFailedLoginMapper(IRDBDataReader reader)
        {
            return new UserFailedLogin
            {
                Id = reader.GetLong(COL_ID),
                UserId = reader.GetInt(COL_UserID),
                FailedResultId = reader.GetInt(COL_FailedResultID),
                CreatedTime = reader.GetDateTime(COL_CreatedTime)
            };
        }
        #endregion

        #region Private Methods
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("VR_Sec", "SecurityDBConnStringKey", "SecurityDBConnString");
        }
        #endregion
        #region IUserFailedLoginDataManager
        public bool AddUserFailedLogin(UserFailedLogin failedLogin, out int insertedId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.AddSelectGeneratedId();
            insertQuery.Column(COL_UserID).Value(failedLogin.UserId);
            insertQuery.Column(COL_FailedResultID).Value(failedLogin.FailedResultId);
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

        public bool DeleteUserFailedLogin(int userId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(TABLE_NAME);
            deleteQuery.Where().EqualsCondition(COL_UserID).Value(userId);
            return queryContext.ExecuteNonQuery() > 0;
        }

        public IEnumerable<UserFailedLogin> GetUserFailedLoginByUserId(int userId, DateTime startInterval, DateTime endInterval)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            var whereQuery = selectQuery.Where();
            whereQuery.EqualsCondition(COL_UserID).Value(userId);
            whereQuery.GreaterOrEqualCondition(COL_CreatedTime).Value(startInterval);
            whereQuery.LessOrEqualCondition(COL_CreatedTime).Value(endInterval);
            selectQuery.Sort().ByColumn(COL_ID, RDBSortDirection.DESC);
            return queryContext.GetItems(UserFailedLoginMapper);
        }
        #endregion
    }
}

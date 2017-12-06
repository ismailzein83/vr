using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Data.SQL
{
    public class UserPasswordHistoryDataManager : BaseSQLDataManager, IUserPasswordHistoryDataManager
    {
        public UserPasswordHistoryDataManager()
            : base(GetConnectionStringName("SecurityDBConnStringKey", "SecurityDBConnString"))
        {

        }

        public IEnumerable<UserPasswordHistory> GetUserPasswordHistoryByUserId(int userId, int maxRecordsCount)
        {
            return GetItemsSP("sec.sp_UserPasswordHistory_GetByUserId", UserPasswordHistoryMapper, userId, maxRecordsCount);
        }
        public bool AddUserPasswordHistory(UserPasswordHistory history, out int insertedId)
        {
            object id;

            int recordesEffected = ExecuteNonQuerySP("sec.sp_UserPasswordHistory_Insert", out id, history.UserId, history.Password, history.IsChangedByAdmin);
            insertedId = (recordesEffected > 0) ? (int)id : -1;

            return (recordesEffected > 0);
        }
        UserPasswordHistory UserPasswordHistoryMapper(IDataReader reader)
        {
            return new UserPasswordHistory
            {
                Id = (long)reader["ID"],
                UserId = (int)reader["UserID"],
                IsChangedByAdmin = (bool)reader["IsChangedByAdmin"],
                Password = reader["Password"] as string
            };
        }
    }
}

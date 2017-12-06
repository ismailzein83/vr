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
    public class UserFailedLoginDataManager : BaseSQLDataManager, IUserFailedLoginDataManager
    {
        public UserFailedLoginDataManager()
            : base(GetConnectionStringName("SecurityDBConnStringKey", "SecurityDBConnString"))
        {

        }
        public IEnumerable<UserFailedLogin> GetUserFailedLoginByUserId(int userId, DateTime startInterval, DateTime endInterval)
        {
            return GetItemsSP("sec.sp_UserFailedLogin_GetByUserId", UserFailedLoginMapper, userId, startInterval, endInterval);
        }

        public bool AddUserFailedLogin(UserFailedLogin failedLogin, out int insertedId)
        {
            object id;

            int recordesEffected = ExecuteNonQuerySP("sec.sp_UserFailedLogin_Insert", out id, failedLogin.UserId, failedLogin.FailedResultId);
            insertedId = (recordesEffected > 0) ? (int)id : -1;

            return (recordesEffected > 0);
        }

        public bool DeleteUserFailedLogin(int userId)
        {
            return ExecuteNonQuerySP("[sec].[sp_UserFailedLogin_DeleteByUserId]", userId) > 0;
        }

        UserFailedLogin UserFailedLoginMapper(IDataReader reader)
        {
            return new UserFailedLogin
            {
                Id = (long)reader["ID"],
                UserId = (int)reader["UserID"],
                FailedResultId = (int)reader["FailedResultID"],
                CreatedTime = GetReaderValue<DateTime>(reader, "CreatedTime")
            };
        }
    }
}

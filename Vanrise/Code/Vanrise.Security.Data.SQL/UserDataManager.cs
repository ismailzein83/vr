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
    public class UserDataManager : BaseSQLDataManager, IUserDataManager
    {
        #region ctor
        public UserDataManager()
            : base(GetConnectionStringName("SecurityDBConnStringKey", "SecurityDBConnString"))
        {

        }

        #endregion

        #region Public Methods


        public List<User> GetUsers()
        {
            return GetItemsSP("sec.sp_User_GetAll", UserMapper);
        }

        public string GetUserPassword(int userId)
        {
            return ExecuteScalarSP("sec.sp_User_GetPassword", userId) as string;
        }

        public string GetUserTempPassword(int userId)
        {
            return ExecuteScalarSP("sec.sp_User_GetTempPassword", userId) as string;
        }

        public bool AddUser(User userObject, string tempPassword, out int insertedId)
        {
            object userID;

            int recordesEffected = ExecuteNonQuerySP("sec.sp_User_Insert", out userID, userObject.Name, tempPassword, userObject.Email, userObject.Description, userObject.TenantId, userObject.EnabledTill, userObject.ExtendedSettings != null ? Vanrise.Common.Serializer.Serialize(userObject.ExtendedSettings) : null);
            insertedId = (recordesEffected > 0) ? (int)userID : -1;

            return (recordesEffected > 0);
        }

        public bool UpdateUser(User userObject)
        {
            int recordesEffected = ExecuteNonQuerySP("sec.sp_User_Update", userObject.UserId, userObject.Name, userObject.Email, userObject.Description, userObject.TenantId, userObject.EnabledTill);
            return (recordesEffected > 0);
        }

        public bool DisableUser(int userID)
        {
            int recordesEffected = ExecuteNonQuerySP("sec.sp_User_SetDisable", userID);
            return (recordesEffected > 0);
        }

        public bool EnableUser(int userID)
        {
            int recordesEffected = ExecuteNonQuerySP("sec.sp_User_SetEnable", userID);
            return (recordesEffected > 0);
        }
        public bool UpdateLastLogin(int userID)
        {
            int recordsAffected = (int)ExecuteNonQuerySP("sec.sp_User_UpdateLastLogin", userID);
            return (recordsAffected > 0);
        }
        public bool ResetPassword(int userId, string password)
        {
            return ExecuteNonQuerySP("sec.sp_User_UpdatePassword", userId, password) > 0;
        }

        public bool ActivatePassword(string email, string password, string name)
        {
            return ExecuteNonQuerySP("sec.sp_User_ActivatePassword", email, password, name) > 0;
        }

        public bool UpdateTempPasswordById(int userId, string password, DateTime? passwordValidTill)
        {
            return ExecuteNonQuerySP("sec.sp_User_UpdateTempPasswordById", userId, password, passwordValidTill) > 0;
        }

        public bool UpdateTempPasswordByEmail(string email, string password, DateTime? passwordValidTill)
        {
            return ExecuteNonQuerySP("sec.sp_User_UpdateTempPasswordByEmail", email, password, passwordValidTill) > 0;
        }

        public bool ChangePassword(int userId, string newPassword)
        {
            int recordsAffected = ExecuteNonQuerySP("sec.sp_User_UpdatePassword", userId, newPassword);
            return (recordsAffected > 0);
        }

        public bool EditUserProfile(string name, int userId)
        {
            return ExecuteNonQuerySP("sec.sp_User_UpdateName", userId, name) > 0;
        }

        public bool AreUsersUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("sec.[User]", ref updateHandle);
        }

        #endregion

        #region Mappers

        private User UserMapper(IDataReader reader)
        {
            return new Entities.User
            {
                UserId = Convert.ToInt32(reader["Id"]),
                Name = reader["Name"] as string,
                Email = reader["Email"] as string,
                LastLogin = GetReaderValue<DateTime?>(reader, "LastLogin"),
                EnabledTill = GetReaderValue<DateTime?>(reader, "EnabledTill"),
                Description = reader["Description"] as string,
                TenantId = Convert.ToInt32(reader["TenantId"]),
                ExtendedSettings = reader["ExtendedSettings"] != DBNull.Value ? Vanrise.Common.Serializer.Deserialize<Dictionary<string, object>>(reader["ExtendedSettings"] as string) : null
            };
        }

        #endregion
    }
}

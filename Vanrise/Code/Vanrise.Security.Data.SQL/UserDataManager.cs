﻿using System;
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
            return (string)ExecuteScalarSP("sec.sp_User_GetPassword", userId);
        }

        public bool AddUser(User userObject, string password, out int insertedId)
        {
            object userID;

            int recordesEffected = ExecuteNonQuerySP("sec.sp_User_Insert", out userID, userObject.Name, password, userObject.Email, userObject.Status, userObject.Description, userObject.TenantId);
            insertedId = (recordesEffected > 0) ? (int)userID : -1;

            return (recordesEffected > 0);
        }

        public bool UpdateUser(User userObject)
        {
            int recordesEffected = ExecuteNonQuerySP("sec.sp_User_Update", userObject.UserId, userObject.Name, userObject.Email, userObject.Status, userObject.Description, userObject.TenantId);
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
                Status = (Entities.UserStatus)reader["Status"],
                Description = reader["Description"] as string,
                TenantId = Convert.ToInt32(reader["TenantId"])
            };
        }

        #endregion
    }
}

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
        public List<Entities.User> GetFilteredUsers(int fromRow, int toRow, string name, string email)
        {
            return GetItemsSP("mainmodule.sp_User_GetFilteredUsers", UserMapper, fromRow, toRow, name, email);
        }

        public User GetUser(int userId)
        {
            return GetItemSP("sec.sp_User_GetUser", UserMapper, userId);
        }

        public bool AddUser(User userObject, out int insertedId)
        {
            object userID;
            
            string password = RandomPasswordHelper.Generate(8, 10);
            //TODO: implement an encryption module
            //string encPassword = manager.EncodePassword(password);

            string isActive = (userObject.Status == Entities.UserStatus.Active) ? "Y" : "N";

            int recordesEffected = ExecuteNonQuerySP("sec.sp_User_Insert", out userID, userObject.Name,
                password, !string.IsNullOrEmpty(userObject.Email) ? userObject.Email : null, isActive,
                !string.IsNullOrEmpty(userObject.Description) ? userObject.Description : null);

            insertedId = (int)userID;
            return (recordesEffected > 0);
        }

        public bool UpdateUser(User userObject)
        {
            string isActive = (userObject.Status == Entities.UserStatus.Active) ? "Y" : "N";

            int recordesEffected = ExecuteNonQuerySP("sec.sp_User_Update", userObject.UserId, userObject.Name,
               !string.IsNullOrEmpty(userObject.Email) ? userObject.Email : null, isActive,
               !string.IsNullOrEmpty(userObject.Description) ? userObject.Description : null);
            
            return (recordesEffected > 0);
        }

        public bool ResetPassword(int userId, string password)
        {
            //TODO: implement an encryption module
            //string encPassword = manager.EncodePassword(password);
            return ExecuteNonQuerySP("sec.sp_User_ResetPassword", userId, password) > 0;
        }

        public bool CheckUserName(string name)
        {
            bool result = (bool) ExecuteScalarSP("sec.sp_User_CheckUserName", name);
            return result;
        }

        private User UserMapper(IDataReader reader)
        {
            return new Entities.User
            {
                UserId = Convert.ToInt32(reader["Id"]),
                Name = reader["Name"] as string,
                Email = reader["Email"] as string,
                LastLogin = GetReaderValue<DateTime>(reader, "LastLogin"),
                Status = reader["IsActive"].ToString().ToUpper().Equals("Y") ? Entities.UserStatus.Active : Entities.UserStatus.Inactive,
                Description = reader["Description"] as string
            };
        }
    }
}

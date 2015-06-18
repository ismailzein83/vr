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
            return GetItemsSP("sec.sp_User_GetFiltered", UserMapper, fromRow, toRow, name, email);
        }

        public List<User> GetUsers()
        {
            return GetItemsSP("sec.sp_User_GetAll", UserMapper);
        }

        public List<User> GetMembers(int roleId)
        {
            return GetItemsSP("sec.sp_User_GetMembers", UserMapper, roleId);
        }

        public User GetUserbyId(int userId)
        {
            return GetItemSP("sec.sp_User_GetbyId", UserMapper, userId);
        }

        public User GetUserbyEmail(string email)
        {
            return GetItemSP("sec.sp_User_GetbyEmail", UserMapper, email);
        }

        public bool AddUser(User userObject, out int insertedId)
        {
            object userID;
            
            //string password = RandomPasswordHelper.Generate(8, 10);
            string password = "1";
            //TODO: implement an encryption module
            //string encPassword = manager.EncodePassword(password);

            //string isActive = (userObject.Status == Entities.UserStatus.Active) ? "Y" : "N";

            int recordesEffected = ExecuteNonQuerySP("sec.sp_User_Insert", out userID, userObject.Name,
                password, !string.IsNullOrEmpty(userObject.Email) ? userObject.Email : null, userObject.Status,
                !string.IsNullOrEmpty(userObject.Description) ? userObject.Description : null);

            insertedId = (int)userID;
            return (recordesEffected > 0);
        }

        public bool UpdateUser(User userObject)
        {
            //string isActive = (userObject.Status == Entities.UserStatus.Active) ? "Y" : "N";

            int recordesEffected = ExecuteNonQuerySP("sec.sp_User_Update", userObject.UserId, userObject.Name,
               !string.IsNullOrEmpty(userObject.Email) ? userObject.Email : null, userObject.Status,
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
                Password = reader["Password"] as string,
                Email = reader["Email"] as string,
                LastLogin = GetReaderValue<DateTime>(reader, "LastLogin"),
                Status = (int.Parse(reader["Status"].ToString()) == 0) ? Entities.UserStatus.Inactive : Entities.UserStatus.Active,
                Description = reader["Description"] as string
            };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TOne.Entities;
using System.Web;
using TOne.Business;
using System.Threading.Tasks;
using TOne.Data;

namespace TOne.Data.SQL
{
    public class UserDataManager : BaseTOneDataManager, IUserDataManager
    {
        public List<Entities.User> GetFilteredUsers(int fromRow, int toRow, string name, string email)
        {
            return GetItemsSP("mainmodule.sp_User_GetFilteredUsers", (reader) =>
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
            }, fromRow, toRow, name, email);
        }

        public User GetUser(int userId)
        {
            return GetItemsSP("mainmodule.sp_User_GetUser", UserMapper, userId).FirstOrDefault();
        }

        public bool AddUser(User userObject, out int insertedId)
        {
            object userID;
            UserManager manager = new UserManager();
            string password = RandomPasswordHelper.Generate(8,10);
            string encPassword = manager.EncodePassword(password);

                    string status = null;
                    if (userObject.Status == Entities.UserStatus.Active)
                status = "Y";
            else
                status = "N";

            int recordesEffected = ExecuteNonQuerySP("mainmodule.sp_User_Insert", out userID,
            !string.IsNullOrEmpty(userObject.Name) ? userObject.Name : null,
               encPassword,
               !string.IsNullOrEmpty(userObject.Email) ? userObject.Email : null,
               status,
               !string.IsNullOrEmpty(userObject.Description) ? userObject.Description : null
            );
            insertedId = (int)userID;
            if (recordesEffected > 0)
                return true;
            return false;
        }

        public bool UpdateUser(User userObject)
        {
            string status = null;
            if (userObject.Status == Entities.UserStatus.Active)
                status = "Y";
            else
                status = "N";

            int recordesEffected = ExecuteNonQuerySP("mainmodule.sp_User_Update",
                 userObject.UserId,
                 !string.IsNullOrEmpty(userObject.Name) ? userObject.Name : null,
               !string.IsNullOrEmpty(userObject.Email) ? userObject.Email : null,
               status,
               !string.IsNullOrEmpty(userObject.Description) ? userObject.Description : null
            );
            if (recordesEffected > 0)
                return true;
            return false;
        }

        public bool ResetPassword(int userId, string password)
        {
            UserManager manager = new UserManager();
            string encPassword = manager.EncodePassword(password);
            if (ExecuteNonQuerySP("mainmodule.sp_User_ResetPassword", userId, encPassword) > 0)
                return true;
            else
                return false;
        }

        public bool CheckUserName(string name)
        {

            if ((bool)ExecuteScalarSP("mainmodule.sp_User_CheckUserName", name) )
                return true;
            else
                return false;
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

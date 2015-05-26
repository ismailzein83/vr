using System;
using System.Collections.Generic;
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
        public List<Entities.User> GetUsers(int fromRow, int toRow)
        {
            return GetItemsSP("mainmodule.sp_User_GetAll", (reader) =>
            {
                return new Entities.User
                {
                    UserId = Convert.ToInt32(reader["Id"]),
                    Name = reader["Name"] as string,
                    Email = reader["Email"] as string,
                    //Password = reader["Password"] as string,
                    LastLogin = GetReaderValue<DateTime>(reader, "LastLogin"),
                    Status = reader["IsActive"].ToString().ToUpper().Equals("Y") ? Entities.UserStatus.Active : Entities.UserStatus.Inactive,
                    Description = reader["Description"] as string
                };
            }, fromRow, toRow);
        }

        public void DeleteUser(int Id)
        {
            ExecuteNonQuerySP("mainmodule.sp_User_Delete", Id);
        }

        public User AddUser(User user)
        {
            object obj;
            UserManager manager = new UserManager();
            string password = RandomPasswordHelper.Generate(8,10);
            string EncPassword = manager.EncodePassword(password);

            string status = null;
            if (user.Status == Entities.UserStatus.Active)
                status = "Y";
            else
                status = "N";

            if (ExecuteNonQuerySP("mainmodule.sp_User_Insert", out obj, user.Name, EncPassword, user.Email, status, user.Description) > 0)
            {
                user.UserId = (int)obj;
                return user;
            }
            else
                return null;
        }

        public bool UpdateUser(User user)
        {
            string status = null;
            if (user.Status == Entities.UserStatus.Active)
                status = "Y";
            else
                status = "N";

            if (ExecuteNonQuerySP("mainmodule.sp_User_Update", user.UserId, user.Name, user.Email, status, user.Description) > 0)
                return true;
            else
                return false;
        }

        public bool ResetPassword(int userId, string password)
        {
            UserManager manager = new UserManager();
            string EncPassword = manager.EncodePassword(password);
            if (ExecuteNonQuerySP("mainmodule.sp_User_ResetPassword", userId, EncPassword) > 0)
                return true;
            else
                return false;
        }

        public List<Entities.User> SearchUser(string name, string email)
        {
            return GetItemsSP("mainmodule.sp_User_Search", (reader) =>
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
            }, name, email);
        }

        public bool CheckUserName(string name)
        {

            if ((bool)ExecuteScalarSP("mainmodule.sp_User_CheckUserName", name) )
                return true;
            else
                return false;
        }
    }
}

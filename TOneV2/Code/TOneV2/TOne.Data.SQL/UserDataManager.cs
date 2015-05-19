using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOne.Entities;
using System.Web;
using TOne.Business;

using System.Threading.Tasks;
using TOne.Entities;
using TOne.Data;

namespace TOne.Data.SQL
{
    public class UserDataManager : BaseTOneDataManager, IUserDataManager
    {
        public List<Entities.User> GetUsers(int FromRow, int ToRow)
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
            }, FromRow, ToRow);
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
            if (ExecuteNonQuerySP("mainmodule.sp_User_Insert", out obj, user.Name, EncPassword, user.Email, user.Description) > 0)
            {
                user.UserId = (int)obj;
                return user;
            }
            else
                return null;
        }

        public bool UpdateUser(User user)
        {
            if (ExecuteNonQuerySP("mainmodule.sp_User_Update", user.UserId, user.Name, user.Email, user.Description) > 0)
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
    }
}

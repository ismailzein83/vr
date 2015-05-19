using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOne.Entities;

namespace TOne.Data.SQL
{
    public class SecurityDataManager : BaseTOneDataManager, ISecurityDataManager
    {
        public List<Entities.User> GetUsers()
        {
            return GetItemsSP("mainmodule.sp_User_GetAll", (reader) =>
            {
                return new Entities.User
                {
                    UserId = Convert.ToInt32(reader["Id"]),
                    Name = reader["Name"] as string,
                    Email = reader["Email"] as string,
                    Password = reader["Password"] as string,
                    LastLogin = GetReaderValue<DateTime>(reader,"LastLogin"),
                    Status = reader["IsActive"].ToString().ToUpper().Equals("Y") ? Entities.UserStatus.Active : Entities.UserStatus.Inactive,
                    Description = reader["Description"] as string
                };
            });
        }

        public void DeleteUser(int Id)
        {
            ExecuteScalarSP("mainmodule.sp_User_Delete", Id);
        }

        public void AddUser(User user)
        {
            ExecuteScalarSP("mainmodule.sp_User_Insert", user.Name, user.Password, user.Email, user.Description);
        }

        public void EditUser(User user)
        {
            ExecuteScalarSP("mainmodule.sp_User_Update", user.UserId, user.Name, user.Password, user.Email, user.Description);
        }

        public List<Entities.User> SearchUser(string name, string email)
        {
            return GetItemsSP("mainmodule.sp_User_Search", (reader) =>
            {
                return new Entities.User
                {
                    UserId = Convert.ToInt32(reader["Id"]),
                    Name = reader["Name"] as string,
                    Password = reader["Password"] as string,
                    Email = reader["Email"] as string,
                    LastLogin = GetReaderValue<DateTime>(reader, "LastLogin"),
                    Status = reader["IsActive"].ToString().ToUpper().Equals("Y") ? Entities.UserStatus.Active : Entities.UserStatus.Inactive,
                    Description = reader["Description"] as string
                };
            },name,email);
        }
    }
}
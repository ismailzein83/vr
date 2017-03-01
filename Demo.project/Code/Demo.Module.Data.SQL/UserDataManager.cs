using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Demo.Module.Data.SQL
{
    class UserDataManager : BaseSQLDataManager, IUserDataManager 
    {
        public UserDataManager()
            : base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoProject_DBConnStringKey"))
        {

        }

        public List<Entities.User> GetUsers()
        {
            return GetItemsSP("[dbo].[sp_User_GetAll]", UserMapper);
        }

        public bool Update(Entities.User user)
        {
            int recordsEffected = ExecuteNonQuerySP("[dbo].[sp_User_Update]", user.Id, user.Name);
            return (recordsEffected > 0);
        }

        public bool Insert(Entities.User user, out int insertedId)
        {
            object Id;
            int recordsEffected = ExecuteNonQuerySP("[dbo].[sp_User_Insert]", out Id, user.Name);
            insertedId = (int)Id;
            return (recordsEffected > 0);
        }

        public User GetUser(int Id)
        {
            return GetItemSP("[dbo].[sp_User_GetUser]", UserMapper, Id);
        }


      User UserMapper(IDataReader reader)
        {
           User user = new User();
            user.Id = (int)reader["ID"];
           user.Name = reader["Name"] as string;

            return user;
        }

     
    }
}

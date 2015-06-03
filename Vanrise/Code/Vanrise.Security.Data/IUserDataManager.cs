using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Data
{
    public interface IUserDataManager : IDataManager
    {
        List<User> GetFilteredUsers(int fromRow, int toRow, string name, string email);

        User GetUser(int userId);

        bool AddUser(User user, out int insertedId);

        bool UpdateUser(User user);

        bool ResetPassword(int userId, string password);

        bool CheckUserName(string name);
    }
}

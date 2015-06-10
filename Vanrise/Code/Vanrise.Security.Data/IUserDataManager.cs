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

        List<User> GetUsers();

        List<User> GetMembers(int roleId);

        User GetUserbyId(int userId);

        User GetUserbyEmail(string email);

        bool AddUser(User user, out int insertedId);

        bool UpdateUser(User user);

        bool ResetPassword(int userId, string password);

        bool CheckUserName(string name);
    }
}

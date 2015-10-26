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
        List<User> GetUsers();

        bool AddUser(User user, out int insertedId);

        bool UpdateUser(User user);

        bool UpdateLastLogin(int userID);

        bool ResetPassword(int userId, string password);

        bool ChangePassword(int userId, string newPassword);

        bool EditUserProfile(string name, int userId);

        bool AreUsersUpdated(ref object updateHandle);

        List<User> GetMembers(int roleId);
       
    }
}

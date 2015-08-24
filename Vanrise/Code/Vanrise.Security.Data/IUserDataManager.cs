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
        Vanrise.Entities.BigResult<User> GetFilteredUsers(Vanrise.Entities.DataRetrievalInput<UserQuery> input);

        List<User> GetUsers();

        List<User> GetMembers(int roleId);

        User GetUserbyId(int userId);

        User GetUserbyEmail(string email);

        bool AddUser(User user, out int insertedId);

        bool UpdateUser(User user);

        bool ResetPassword(int userId, string password);

        bool CheckUserName(string name);

        bool ChangePassword(int userId,string oldPassword,string newPassword);

        bool EditUserProfile(string name,int userId);
    }
}

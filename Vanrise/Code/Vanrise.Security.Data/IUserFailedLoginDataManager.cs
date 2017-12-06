using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Data
{
    public interface IUserFailedLoginDataManager : IDataManager
    {
        IEnumerable<UserFailedLogin> GetUserFailedLoginByUserId(int userId, DateTime startInterval, DateTime endInterval);

        bool AddUserFailedLogin(UserFailedLogin failedLogin, out int insertedId);
        bool DeleteUserFailedLogin(int userId);
    }
}

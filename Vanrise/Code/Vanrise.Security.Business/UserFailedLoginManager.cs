using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Data;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Business
{
    public class UserFailedLoginManager
    {
        public IEnumerable<UserFailedLogin> GetUserFailedLoginByUserId(int userId, DateTime startInterval, DateTime endInterval)
        {
            IUserFailedLoginDataManager manager = SecurityDataManagerFactory.GetDataManager<IUserFailedLoginDataManager>();
            return manager.GetUserFailedLoginByUserId(userId, startInterval, endInterval);
        }
        public bool AddUserFailedLogin(UserFailedLogin failedLogin, out int insertedId)
        {
            IUserFailedLoginDataManager manager = SecurityDataManagerFactory.GetDataManager<IUserFailedLoginDataManager>();
            return manager.AddUserFailedLogin(failedLogin, out insertedId);
        }
        public bool DeleteUserFailedLogin(int userId)
        {
            IUserFailedLoginDataManager manager = SecurityDataManagerFactory.GetDataManager<IUserFailedLoginDataManager>();
            return manager.DeleteUserFailedLogin(userId);
        }

    }
}

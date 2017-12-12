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

        string GetUserPassword(int userId);

        string GetUserTempPassword(int userId);

        bool AddUser(User user, string tempPassword, out int insertedId);

        bool UpdateUser(User user);

        bool DisableUser(int userID);

        bool EnableUser(int userID);
        bool UnlockUser(int userID);
        bool UpdateLastLogin(int userID);

        bool ResetPassword(int userId, string password);

        bool ChangePassword(int userId, string newPassword);

        bool EditUserProfile(string name, int userId);

        bool AreUsersUpdated(ref object updateHandle);

        bool ActivatePassword(string email, string password, string name);

        bool UpdateTempPasswordById(int userId, string password, DateTime? passwordValidTill);
        
        bool UpdateTempPasswordByEmail(string email, string password, DateTime? passwordValidTill);

        bool UpdateDisableTill(int userID, DateTime disableTill);
        bool UpdateUserSetting(UserSetting userSetting, int userId);
    }
}

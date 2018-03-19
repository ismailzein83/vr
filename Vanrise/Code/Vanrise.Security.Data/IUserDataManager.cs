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

        bool DisableUser(int userID, int lastModifiedBy);

        bool EnableUser(int userID, int lastModifiedBy);
        bool UnlockUser(int userID, int lastModifiedBy);
        bool UpdateLastLogin(int userID, int lastModifiedBy);

        bool ResetPassword(int userId, string password, int lastModifiedBy);

        bool ChangePassword(int userId, string newPassword, int lastModifiedBy);

        bool EditUserProfile(string name, int userId, UserSetting settings, int lastModifiedBy);

        bool AreUsersUpdated(ref object updateHandle);

        bool ActivatePassword(string email, string password, string name, int lastModifiedBy);

        bool UpdateTempPasswordById(int userId, string password, DateTime? passwordValidTill, int lastModifiedBy);
        
        bool UpdateTempPasswordByEmail(string email, string password, DateTime? passwordValidTill, int lastModifiedBy);

        bool UpdateDisableTill(int userID, DateTime disableTill, int lastModifiedBy);
        bool UpdateUserSetting(UserSetting userSetting, int userId, int lastModifiedBy);
    }
}

using PartnerPortal.CustomerAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Business;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Security.Entities;
using Vanrise.Common.Business;

namespace PartnerPortal.CustomerAccess.Business
{

    public class BaseAccountUserManager
    {
        public List<UserDetailInfo> GetUsersStatus(UserStatusInput userStatusInput)
        {
            List<UserDetailInfo> userDetailInfo = new List<UserDetailInfo>();
            userStatusInput.UserIds.ThrowIfNull("UserStatusInput");
            foreach (var userId in userStatusInput.UserIds)
            {
                var userStatus = GetUserStatusByUserId(userId);
                UserDetailInfo userDetail = new UserDetailInfo()
                {
                    UserId = userId,
                    UserStatus = userStatus
                };
                userDetailInfo.Add(userDetail);
            }
            return userDetailInfo;
        }
        public UpdateOperationOutput<UserDetail> EnableUser(int userId)
        {
            UserManager userManager = new UserManager();
            return userManager.EnableUser(userId);
        }
        public UserStatus GetUserStatusByUserId(int userId)
        {
            UserManager userManager = new UserManager();
            UserStatus userStatus;
            userManager.IsUserEnable(userId, out userStatus);
            return userStatus;
        }
        public UpdateOperationOutput<UserDetail> DisableUser(int userId)
        {
            UserManager userManager = new UserManager();
            return userManager.DisableUser(userId);
        }
        public UpdateOperationOutput<UserDetail> UnlockPortalAccount(int userId)
        {
            UserManager userManager = new UserManager();
            var user = userManager.GetUserbyId(userId);
            user.ThrowIfNull("user");
            return userManager.UnlockUser(user);
        }
        public UpdateOperationOutput<object> ResetPassword(int userId, string password)
        {
            return new UserManager().ResetPassword(userId, password);
        }
    }
    //public class WhSAccountUserManager : BaseAccountUserManager
    //{ 
        
    //    #region Public Methods
    //    public WhSAccountInfo GetWhSAccountInfo(int userId)
    //    {
    //        UserManager userManager = new UserManager();
    //        var whSAccountSettings = userManager.GetUserExtendedSettings<WhSAccountSettings>(userId);

    //        if (whSAccountSettings == null)
    //            throw new NullReferenceException(string.Format("whSAccountSettings for userId: {0}", userId));

    //        return new WhSAccountInfo
    //        {
    //            AccountType = whSAccountSettings.AccountType,
    //            CarrierAccountIds = whSAccountSettings.CarrierAccountIds,
    //            CarrierProfileId = whSAccountSettings.CarrierProfileId
    //        };
    //    }
    //    public UpdateOperationOutput<UserDetail> UpdateWhSAccountUser(WhSAccountToUpdate whSAccount)
    //    {
    //        UserManager userManager = new UserManager();
    //       var user = userManager.GetUserbyId(whSAccount.UserId);
    //       user.ThrowIfNull("User");
    //       user.Name = whSAccount.Name;
    //       user.Email = whSAccount.Email;
    //        UserToUpdate userToUpdate = new UserToUpdate{
    //            UserId = user.UserId,
    //            Name = user.Name,
    //            Email = user.Email,
    //            TenantId = user.TenantId,
    //            Description = user.Description,
    //            EnabledTill = user.EnabledTill,
    //            ExtendedSettings = user.ExtendedSettings,
    //            GroupIds = whSAccount.GroupIds
    //        };
    //       return userManager.UpdateUser(userToUpdate);
    //    }
    //    public InsertOperationOutput<Vanrise.Security.Entities.UserDetail> AddWhSAccountUser(WhSAccount whSAccount)
    //    {
    //        object userExtendedSettings;
    //        UserManager userManager = new UserManager();

    //        UserToAdd user = new UserToAdd()
    //        {
    //            Email = whSAccount.Email,
    //            Name = whSAccount.Name,
    //            Description = whSAccount.Description,
    //            EnabledTill = whSAccount.EnabledTill,
    //            TenantId = whSAccount.TenantId,
    //            ExtendedSettings = new Dictionary<string, object>(),
    //        };

    //        string whSAccountSettingsFullName = typeof(WhSAccountSettings).FullName;
    //        user.ExtendedSettings.Add(whSAccountSettingsFullName, new WhSAccountSettings()
    //        {
    //            CarrierAccountIds = whSAccount.CarrierAccountIds,
    //            AccountType = whSAccount.AccountType,
    //            CarrierProfileId=whSAccount.CarrierProfileId,
    //        });

    //        Vanrise.Entities.InsertOperationOutput<Vanrise.Security.Entities.UserDetail> insertOperationOutput = userManager.AddUser(user);

    //        if (insertOperationOutput.Result == Vanrise.Entities.InsertOperationResult.SameExists)
    //        {
    //            var existedUser = userManager.GetUserbyEmail(whSAccount.Email);
    //            if (existedUser.ExtendedSettings != null && existedUser.ExtendedSettings.TryGetValue(whSAccountSettingsFullName, out userExtendedSettings))
    //            {
    //                if (((WhSAccountSettings)userExtendedSettings).CarrierProfileId == whSAccount.CarrierProfileId)
    //                {
    //                    UserToUpdate userToUpdate = new UserToUpdate()
    //                    {
    //                        UserId = existedUser.UserId,
    //                        Email = whSAccount.Email,
    //                        Name = whSAccount.Name,
    //                        Description = whSAccount.Description,
    //                        EnabledTill = whSAccount.EnabledTill,
    //                        TenantId = whSAccount.TenantId,
    //                        ExtendedSettings = new Dictionary<string, object>()

    //                    };

    //                    userToUpdate.ExtendedSettings.Add(whSAccountSettingsFullName, new WhSAccountSettings()
    //                    {
    //                        CarrierProfileId = whSAccount.CarrierProfileId,
    //                        AccountType = whSAccount.AccountType,
    //                        CarrierAccountIds = whSAccount.CarrierAccountIds
    //                    });

    //                    Vanrise.Entities.UpdateOperationOutput<Vanrise.Security.Entities.UserDetail> updateOperationOutput = userManager.UpdateUser(userToUpdate);
    //                    if (updateOperationOutput.Result == Vanrise.Entities.UpdateOperationResult.Succeeded)
    //                    {
    //                        insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
    //                        insertOperationOutput.InsertedObject = updateOperationOutput.UpdatedObject;
    //                    }
    //                }
    //            }
    //        }

    //        return insertOperationOutput;
    //    }

    //    #endregion

     
    //}
}

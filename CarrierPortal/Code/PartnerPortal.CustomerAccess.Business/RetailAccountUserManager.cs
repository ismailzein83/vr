using PartnerPortal.CustomerAccess.Entities;
using System;
using System.Collections.Generic;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;
using Vanrise.Entities;

namespace PartnerPortal.CustomerAccess.Business
{
    public class RetailAccountUserManager
    {
        public long GetRetailAccountId(int userId)
        {
            UserManager userManager = new UserManager();
            var retailAccountSettings = userManager.GetUserExtendedSettings<RetailAccountSettings>(userId);

            if (retailAccountSettings == null)
                throw new NullReferenceException(string.Format("retailAccountSettings for userId: {0}", userId));

            return retailAccountSettings.AccountId;
        }

        public InsertOperationOutput<Vanrise.Security.Entities.UserDetail> AddRetailAccountUser(RetailAccount retailAccount)
        {
            object userExtendedSettings;
            UserManager userManager = new UserManager();

            UserToAdd user = new UserToAdd()
            {
                Email = retailAccount.Email,
                Name = retailAccount.Name,
                Description = retailAccount.Description,
                EnabledTill = retailAccount.EnabledTill,
                TenantId = retailAccount.TenantId,
                ExtendedSettings = new Dictionary<string, object>()
            };

            string retailAccountSettingsFullName = typeof(RetailAccountSettings).FullName;
            user.ExtendedSettings.Add(retailAccountSettingsFullName, new RetailAccountSettings() { AccountId = retailAccount.AccountId });

            Vanrise.Entities.InsertOperationOutput<Vanrise.Security.Entities.UserDetail> insertOperationOutput = userManager.AddUser(user);

            if (insertOperationOutput.Result == Vanrise.Entities.InsertOperationResult.SameExists)
            {
                var existedUser = userManager.GetUserbyEmail(retailAccount.Email);
                if (existedUser.ExtendedSettings != null && existedUser.ExtendedSettings.TryGetValue(retailAccountSettingsFullName, out userExtendedSettings))
                {
                    if (((RetailAccountSettings)userExtendedSettings).AccountId == retailAccount.AccountId)
                    {
                        UserToUpdate userToUpdate = new UserToUpdate()
                        {
                            UserId = existedUser.UserId,
                            Email = retailAccount.Email,
                            Name = retailAccount.Name,
                            Description = retailAccount.Description,
                            EnabledTill = retailAccount.EnabledTill,
                            TenantId = retailAccount.TenantId,
                            ExtendedSettings = new Dictionary<string, object>()

                        };
                        userToUpdate.ExtendedSettings.Add(retailAccountSettingsFullName, new RetailAccountSettings() { AccountId = retailAccount.AccountId });
                        Vanrise.Entities.UpdateOperationOutput<Vanrise.Security.Entities.UserDetail> updateOperationOutput = userManager.UpdateUser(userToUpdate);
                        if (updateOperationOutput.Result == Vanrise.Entities.UpdateOperationResult.Succeeded)
                        {
                            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                            insertOperationOutput.InsertedObject = updateOperationOutput.UpdatedObject;
                        }
                    }
                }
            }

            return insertOperationOutput;
        }

        public UpdateOperationOutput<object> ResetPassword(int userId, string password)
        {
            return new UserManager().ResetPassword(userId, password);
        }
    }
}
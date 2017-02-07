using PartnerPortal.CustomerAccess.Entities;
using System;
using System.Collections.Generic;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;

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

        public Vanrise.Entities.InsertOperationOutput<Vanrise.Security.Entities.UserDetail> AddRetailAccountUser(RetailAccount retailAccount)
        {
            object userExtendedSettings;
            UserManager userManager = new UserManager();

            User user = new User()
            {
                Description = retailAccount.Description,
                Email = retailAccount.Email,
                EnabledTill = retailAccount.EnabledTill,
                Name = retailAccount.Name,
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
                        user.UserId = existedUser.UserId;
                        Vanrise.Entities.UpdateOperationOutput<Vanrise.Security.Entities.UserDetail> updateOperationOutput = userManager.UpdateUser(user);
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
    }
}
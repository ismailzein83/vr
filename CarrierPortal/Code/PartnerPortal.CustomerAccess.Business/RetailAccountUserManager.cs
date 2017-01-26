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
            User user = new User()
            {
                Description = retailAccount.Description,
                Email = retailAccount.Email,
                EnabledTill = retailAccount.EnabledTill,
                Name = retailAccount.Name,
                TenantId = retailAccount.TenantId,
                ExtendedSettings = new Dictionary<string, object>()
            };
            user.ExtendedSettings.Add(typeof(RetailAccountSettings).FullName, new RetailAccountSettings() { AccountId = retailAccount.AccountId });

            UserManager userManager = new UserManager();
            return userManager.AddUser(user);
        }
    }
}

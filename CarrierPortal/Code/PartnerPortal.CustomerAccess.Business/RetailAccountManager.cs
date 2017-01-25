using PartnerPortal.CustomerAccess.Entities;
using System;
using Vanrise.Security.Business;

namespace PartnerPortal.CustomerAccess.Business
{
    public class RetailAccountManager
    {
        public long GetRetailAccountId(int userId)
        {
            UserManager userManager = new UserManager();
            var retailAccount = userManager.GetUserExtendedSettings<RetailAccount>(userId);

            if (retailAccount == null)
                throw new NullReferenceException(string.Format("retailAccount for userId: {0}", userId));
            
            return retailAccount.AccountId;
        }
    }
}

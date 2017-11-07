using PartnerPortal.CustomerAccess.Entities;
using Retail.BusinessEntity.APIEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;

namespace PartnerPortal.CustomerAccess.Business
{
    public class RetailAccountUserManager
    {
        #region Public Methods

        public RetailAccountInfo GetRetailAccountInfo(int userId)
        {
            UserManager userManager = new UserManager();
            var retailAccountSettings = userManager.GetUserExtendedSettings<RetailAccountSettings>(userId);

            if (retailAccountSettings == null)
                throw new NullReferenceException(string.Format("retailAccountSettings for userId: {0}", userId));

            return new RetailAccountInfo
            {
                AccountBEDefinitionId = retailAccountSettings.AccountBEDefinitionId,
                AccountId = retailAccountSettings.AccountId
            };
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
                ExtendedSettings = new Dictionary<string, object>(),
            };

            string retailAccountSettingsFullName = typeof(RetailAccountSettings).FullName;
            user.ExtendedSettings.Add(retailAccountSettingsFullName, new RetailAccountSettings()
            {
                AccountId = retailAccount.AccountId,
                AccountBEDefinitionId = retailAccount.AccountBEDefinitionId
            });

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

                        userToUpdate.ExtendedSettings.Add(retailAccountSettingsFullName, new RetailAccountSettings()
                        {
                            AccountId = retailAccount.AccountId,
                            AccountBEDefinitionId = retailAccount.AccountBEDefinitionId
                        });

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

        public Dictionary<long, ClientAccountInfo> GetClientRetailAccountsInfo(Guid vrConnectionId)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            var accountInfo = GetRetailAccountInfo(userId);
            accountInfo.ThrowIfNull("accountInfo", userId);

            var retailAccountCacheKey = new RetailAccountCacheKey
            {
                AccountBEDefinitionId = accountInfo.AccountBEDefinitionId,
                AccountId = accountInfo.AccountId,
                VRConnectionId = vrConnectionId
            };

            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(retailAccountCacheKey,
              () =>
              {
                  VRConnectionManager connectionManager = new VRConnectionManager();
                  var vrConnection = connectionManager.GetVRConnection<VRInterAppRestConnection>(vrConnectionId);
                  VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;
                  IEnumerable<ClientAccountInfo> clientAccountsInfo = connectionSettings.Get<IEnumerable<ClientAccountInfo>>(string.Format("/api/Retail_BE/AccountBE/GetClientChildAccountsInfo?accountBEDefinitionId={0}&accountId={1}&withSubChildren={2}", accountInfo.AccountBEDefinitionId, accountInfo.AccountId, true));
                  return clientAccountsInfo == null ? null : clientAccountsInfo.ToDictionary(acc => acc.AccountId, acc => acc);
              });
        }

        #endregion

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            protected override bool IsTimeExpirable
            {
                get { return true; }
            }
        }

        public struct RetailAccountCacheKey
        {
            public long AccountId { get; set; }
            public Guid AccountBEDefinitionId { get; set; }
            public Guid VRConnectionId { get; set; }
        }

        #endregion
    }
}
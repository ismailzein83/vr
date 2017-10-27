using PartnerPortal.CustomerAccess.Entities;
using Retail.BusinessEntity.APIEntities;
using System;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;

namespace PartnerPortal.CustomerAccess.Business
{
    public class RetailAccountUserManager
    {
        #region Ctor/Fields

        static BusinessEntityDefinitionManager s_businessEntityDefinitionManager = new BusinessEntityDefinitionManager();

        #endregion

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

        public IEnumerable<ClientChildAccountInfo> GetClientChildAccountsInfo(Guid businessEntityDefinitionId)
        {
            var retailBusinessEntityDefinitionSettings = GetRetailUserSubaccountsBEDefinition(businessEntityDefinitionId);
            retailBusinessEntityDefinitionSettings.ThrowIfNull("retailBusinessEntityDefinitionSettings");
            int userId = SecurityContext.Current.GetLoggedInUserId();
            var accountInfo = GetRetailAccountInfo(userId);
            accountInfo.ThrowIfNull("accountInfo", userId);
            string accountId = accountInfo.AccountId.ToString();
            VRConnectionManager connectionManager = new VRConnectionManager();
            var vrConnection = connectionManager.GetVRConnection<VRInterAppRestConnection>(retailBusinessEntityDefinitionSettings.VRConnectionId);
            VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;
            var clientChildAccountsInfo = connectionSettings.Get<IEnumerable<ClientChildAccountInfo>>(string.Format("/api/Retail_BE/AccountBE/GetClientChildAccountsInfo?accountBEDefinitionId={0}&accountId={1}&withSubChildren={2}", accountInfo.AccountBEDefinitionId, accountId, true));

            Func<ClientChildAccountInfo, bool> filterExpression = (clientChildAccountInfo) =>
            {
                if (retailBusinessEntityDefinitionSettings.AccountTypeIds != null && !retailBusinessEntityDefinitionSettings.AccountTypeIds.Contains(clientChildAccountInfo.TypeId))
                    return false;
                return true;
            };

            return clientChildAccountsInfo != null ? clientChildAccountsInfo.FindAllRecords(filterExpression) : null; ;
        }

        public RetailUserSubaccountsBEDefinition GetRetailUserSubaccountsBEDefinition(Guid businessEntityDefinitionId)
        {
            var businessEntityDefinition = s_businessEntityDefinitionManager.GetBusinessEntityDefinition(businessEntityDefinitionId);
            businessEntityDefinition.ThrowIfNull("businessEntityDefinition", businessEntityDefinitionId);
            return businessEntityDefinition.Settings.CastWithValidate<RetailUserSubaccountsBEDefinition>("businessEntityDefinition.Settings", businessEntityDefinitionId);
        }

        #endregion
    }
}
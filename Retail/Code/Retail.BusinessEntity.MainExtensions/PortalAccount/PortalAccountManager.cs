using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;

namespace Retail.BusinessEntity.MainExtensions.PortalAccount
{
    public class PortalAccountManager
    {
        static SecurityManager sManager = new SecurityManager();
        public PortalAccountSettings GetPortalAccountSettings(Guid accountBEDefinitionId, long accountId, Guid accountViewDefinitionId)
        {
            return new AccountBEManager().GetExtendedSettings<PortalAccountSettings>(accountBEDefinitionId, accountId);
        }

        public Vanrise.Entities.InsertOperationOutput<PortalAccountSettings> AddPortalAccount(Guid accountBEDefinitionId, long accountId, Guid accountViewDefinitionId, string name, string email)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<PortalAccountSettings>();
            insertOperationOutput.InsertedObject = null;
            
            PortalAccount portalAccount = new AccountBEDefinitionManager().GetAccountViewDefinitionSettings<PortalAccount>(accountBEDefinitionId, accountViewDefinitionId);

            var retailAccount = new PartnerPortal.CustomerAccess.Entities.RetailAccount()
            {
                AccountId = accountId,
                Name = name,
                Email = email,
                TenantId = portalAccount.TenantId,
                AccountBEDefinitionId = accountBEDefinitionId
            };

            VRConnectionManager connectionManager = new VRConnectionManager();
            var vrConnection = connectionManager.GetVRConnection<VRInterAppRestConnection>(portalAccount.ConnectionId);
            VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;

            InsertOperationOutput<UserDetail> userDetails =
                connectionSettings.Post<PartnerPortal.CustomerAccess.Entities.RetailAccount, InsertOperationOutput<UserDetail>>("/api/PartnerPortal_CustomerAccess/RetailAccountUser/AddRetailAccountUser", retailAccount);

            insertOperationOutput.Result = userDetails.Result;
            insertOperationOutput.Message = userDetails.Message;

            switch (userDetails.Result)
            {
                case InsertOperationResult.Succeeded:
                    PortalAccountSettings portalAccountSettings = new PortalAccountSettings()
                    {
                        UserId = userDetails.InsertedObject.Entity.UserId,
                        Name = userDetails.InsertedObject.Entity.Name,
                        Email = userDetails.InsertedObject.Entity.Email,
                        TenantId = userDetails.InsertedObject.Entity.TenantId
                    };
                    AccountBEManager accountBEManager = new AccountBEManager();

                    bool IsAccountExtendedSettingsUpdated = accountBEManager.UpdateAccountExtendedSetting(accountBEDefinitionId, accountId, portalAccountSettings);
                    if (IsAccountExtendedSettingsUpdated)
                    {
                        accountBEManager.TrackAndLogObjectCustomAction(accountBEDefinitionId, accountId, "Configure Portal Account", string.Format("Email: {0}", portalAccountSettings.Email), portalAccountSettings);
                        insertOperationOutput.InsertedObject = portalAccountSettings;
                    }
                    else
                    {
                        insertOperationOutput.Result = InsertOperationResult.Failed;
                        insertOperationOutput.Message = "Update Retail Account has failed";
                    }
                    break;


                case InsertOperationResult.Failed:
                case InsertOperationResult.SameExists:
                default: break;

            }

            return insertOperationOutput;
        }
        public Vanrise.Entities.InsertOperationOutput<AdditionalPortalAccountSettings> AddAdditionalPortalAccount(Guid accountBEDefinitionId, long accountId, Guid accountViewDefinitionId, string name, string email)
        {
            List<AdditionalPortalAccountSettings> additionalPortalAccountSettingsList = new List<AdditionalPortalAccountSettings>();
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<AdditionalPortalAccountSettings>();
            insertOperationOutput.InsertedObject = null;

            PortalAccount portalAccount = new AccountBEDefinitionManager().GetAccountViewDefinitionSettings<PortalAccount>(accountBEDefinitionId, accountViewDefinitionId);

            var retailAccount = new PartnerPortal.CustomerAccess.Entities.RetailAccount()
            {
                AccountId = accountId,
                Name = name,
                Email = email,
                TenantId = portalAccount.TenantId,
                AccountBEDefinitionId = accountBEDefinitionId
            };

            VRConnectionManager connectionManager = new VRConnectionManager();
            var vrConnection = connectionManager.GetVRConnection<VRInterAppRestConnection>(portalAccount.ConnectionId);
            VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;

            InsertOperationOutput<UserDetail> userDetails =
                connectionSettings.Post<PartnerPortal.CustomerAccess.Entities.RetailAccount, InsertOperationOutput<UserDetail>>("/api/PartnerPortal_CustomerAccess/RetailAccountUser/AddRetailAccountUser", retailAccount);

            insertOperationOutput.Result = userDetails.Result;
            insertOperationOutput.Message = userDetails.Message;

            switch (userDetails.Result)
            {
                case InsertOperationResult.Succeeded:
                    var additionalPortalAccountSettings = new AdditionalPortalAccountSettings
                    {
                        UserId = userDetails.InsertedObject.Entity.UserId,
                        Name = userDetails.InsertedObject.Entity.Name,
                        Email = userDetails.InsertedObject.Entity.Email,
                        TenantId = userDetails.InsertedObject.Entity.TenantId
                    };

                    var portalAccountSettings = GetPortalAccountSettings( accountBEDefinitionId,  accountId,  accountViewDefinitionId);
                    if (portalAccountSettings != null)
                    {
                        if (portalAccountSettings.AdditionalUsers == null)
                            portalAccountSettings.AdditionalUsers = new List<AdditionalPortalAccountSettings>();

                       
                       portalAccountSettings.AdditionalUsers.Add(additionalPortalAccountSettings);
                    }
                   
                    AccountBEManager accountBEManager = new AccountBEManager();

                    bool IsAccountExtendedSettingsUpdated = accountBEManager.UpdateAccountExtendedSetting(accountBEDefinitionId, accountId, portalAccountSettings);
                    if (IsAccountExtendedSettingsUpdated)
                    {
                        accountBEManager.TrackAndLogObjectCustomAction(accountBEDefinitionId, accountId, "Configure Portal Account", string.Format("Email: {0}", portalAccountSettings.Email), portalAccountSettings);
                        insertOperationOutput.InsertedObject = additionalPortalAccountSettings;
                    }
                    else
                    {
                        insertOperationOutput.Result = InsertOperationResult.Failed;
                        insertOperationOutput.Message = "Update Retail Account has failed";
                    }
                    break;


                case InsertOperationResult.Failed:
                case InsertOperationResult.SameExists:
                default: break;

            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<object> ResetPassword(Guid accountBEDefinitionId, long accountId, Guid accountViewDefinitionId, string password,int userId)
        {
            var portalAccountSettings = GetPortalAccountSettings(accountBEDefinitionId, accountId, accountViewDefinitionId);
          
            PortalAccount portalAccount = new AccountBEDefinitionManager().GetAccountViewDefinitionSettings<PortalAccount>(accountBEDefinitionId, accountViewDefinitionId);
            var resetPasswordInput = new Vanrise.Security.Entities.ResetPasswordInput()
            {
                UserId = userId,
                Password = password
            };
            if (userId == portalAccountSettings.UserId || (portalAccountSettings.AdditionalUsers != null && portalAccountSettings.AdditionalUsers.Any(item => item.UserId == userId)))
            {
                VRConnectionManager connectionManager = new VRConnectionManager();
                var vrConnection = connectionManager.GetVRConnection<VRInterAppRestConnection>(portalAccount.ConnectionId);
                VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;
                return connectionSettings.Post<Vanrise.Security.Entities.ResetPasswordInput, Vanrise.Entities.UpdateOperationOutput<object>>("/api/PartnerPortal_CustomerAccess/RetailAccountUser/ResetPassword", resetPasswordInput);
            }

            throw new Exception();
        }

        #region Security
        public bool DosesUserHaveViewAccess( Guid accountBEDefinitionId,Guid accountViewDefinitionId)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            return DoesUserHaveAccess(userId, accountBEDefinitionId, accountViewDefinitionId, (sec) => sec.ViewRequiredPermission);

        }
        public bool DosesUserHaveConfigureAccess(Guid accountBEDefinitionId, Guid accountViewDefinitionId)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            return DoesUserHaveAccess(userId, accountBEDefinitionId, accountViewDefinitionId, (sec) => sec.ConfigureRequiredPermission);

        }
        public bool DosesUserHaveResetPasswordAccess(Guid accountBEDefinitionId, Guid accountViewDefinitionId)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            return DoesUserHaveAccess(userId, accountBEDefinitionId, accountViewDefinitionId, (sec) => sec.ResetPasswordRequiredPermission);

        }

        #endregion

        #region Private Methods
        private bool DoesUserHaveAccess(int userId, Guid accountBEDefinitionId, Guid accountViewDefinitionId, Func<AccountViewDefinitionSecurity, Vanrise.Security.Entities.RequiredPermissionSettings> getRequiredPermissionSetting)
        {
            var accountViewDefinitionSettings = new AccountBEDefinitionManager().GetAccountViewDefinitionSettings<PortalAccount>(accountBEDefinitionId, accountViewDefinitionId);
            if (accountViewDefinitionSettings != null && accountViewDefinitionSettings.Security != null && getRequiredPermissionSetting(accountViewDefinitionSettings.Security) != null)
                return sManager.IsAllowed(getRequiredPermissionSetting(accountViewDefinitionSettings.Security), userId);
            else
                return true;
        }

        #endregion
    }
}

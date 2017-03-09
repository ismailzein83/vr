using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Security.Entities;

namespace Retail.BusinessEntity.MainExtensions.PortalAccount
{
    public class PortalAccountManager
    {
        public PortalAccountSettings GetPortalAccountSettings(Guid accountBEDefinitionId, long accountId)
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
                TenantId = portalAccount.TenantId
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

                    bool IsAccountExtendedSettingsUpdated = new AccountBEManager().UpdateAccountExtendedSetting(accountBEDefinitionId, accountId, portalAccountSettings);
                    if (IsAccountExtendedSettingsUpdated)
                    {
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

        public Vanrise.Entities.UpdateOperationOutput<object> ResetPassword(Guid accountBEDefinitionId, long accountId, Guid accountViewDefinitionId, string password)
        {
            PortalAccount portalAccount = new AccountBEDefinitionManager().GetAccountViewDefinitionSettings<PortalAccount>(accountBEDefinitionId, accountViewDefinitionId);

            var portalAccountSettings = new AccountBEManager().GetExtendedSettings<PortalAccountSettings>(accountBEDefinitionId, accountId);
            if (portalAccountSettings == null)
                throw new NullReferenceException(String.Format("portalAccountSettings of accountBEDefinitionId '{0}' and accountId '{1}'", accountBEDefinitionId, accountId));

            var resetPasswordInput = new Vanrise.Security.Entities.ResetPasswordInput()
            {
                UserId = portalAccountSettings.UserId,
                Password = password
            };

            VRConnectionManager connectionManager = new VRConnectionManager();
            var vrConnection = connectionManager.GetVRConnection<VRInterAppRestConnection>(portalAccount.ConnectionId);
            VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;

            return connectionSettings.Post<Vanrise.Security.Entities.ResetPasswordInput, Vanrise.Entities.UpdateOperationOutput<object>>("/api/PartnerPortal_CustomerAccess/RetailAccountUser/ResetPassword", resetPasswordInput);
        }
    }
}

using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Security.Entities;
using Vanrise.Entities;

namespace Retail.BusinessEntity.Business
{
    public class PortalAccountManager
    {
        public Vanrise.Entities.InsertOperationOutput<PortalAccountSettings> AddPortalAccount(Guid accountBEDefinitionId, long accountId, string name, string email, Guid connectionId)
        {
            VRConnectionManager connectionManager = new VRConnectionManager();
            var vrConnection = connectionManager.GetVRConnection(connectionId);

            if (vrConnection == null)
                throw new NullReferenceException("vrConnection");

            if (vrConnection.Settings == null)
                throw new NullReferenceException("vrConnection.Settings");

            VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;
            if (connectionSettings == null)
                throw new Exception(String.Format("vrConnection.Settings is not of type VRInterAppRestConnection. it is of type '{0}'.", vrConnection.Settings.GetType()));

            var retailAccount = new PartnerPortal.CustomerAccess.Entities.RetailAccount()
            {
                AccountId = accountId,
                Name = name,
                Email = email
            };

            InsertOperationOutput<UserDetail> userDetails = 
                connectionSettings.Post<PartnerPortal.CustomerAccess.Entities.RetailAccount, InsertOperationOutput<UserDetail>>("/api/PartnerPortal_CustomerAccess/RetailAccountUser/AddRetailAccountUser", retailAccount);

            PortalAccountSettings portalAccountSettings = null;
            if (userDetails != null && userDetails.InsertedObject != null && userDetails.InsertedObject.Entity != null)
            {
                portalAccountSettings = new PortalAccountSettings()
                {
                    UserId = userDetails.InsertedObject.Entity.UserId,
                    Name = userDetails.InsertedObject.Entity.Name,
                    Email = userDetails.InsertedObject.Entity.Email
                };
            }

            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<PortalAccountSettings>();
            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            bool IsAccountExtendedSettingsUpdated = new AccountBEManager().UpdateAccountExtendedSetting(accountBEDefinitionId, accountId, portalAccountSettings);
            if (portalAccountSettings != null)
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = portalAccountSettings;
            }

            return insertOperationOutput;
        }

        public PortalAccountSettings GetPortalUserAccount(Guid accountBEDefinitionId, long accountId)
        {
            return new AccountBEManager().GetExtendedSettings<PortalAccountSettings>(accountBEDefinitionId, accountId);
        }
    }
}

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

namespace Retail.BusinessEntity.MainExtensions.AccountSynchronizerInsertHandlers
{
    public class PortalAccountInsertHandler : AccountSynchronizerInsertHandlerSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("1CBD8BEF-14D6-4D47-BB9B-264ECC0E92B4"); }
        }
        public string AccountNameMappingField { get; set; }
        public string AccountEmailMappingField { get; set; }
        public Guid ConnectionId { get; set; }
        public int TenantId { get; set; }


        public override void OnPreInsert(IAccountSynchronizerInsertHandlerPreInsertContext context)
        {

        }

        public override void OnPostInsert(IAccountSynchronizerInsertHandlerPostInsertContext context)
        {
            var account = context.Account;
            var accountBEDefinitionId = context.AccountBEDefinitionId;

            VRConnectionManager connectionManager = new VRConnectionManager();
            var vrConnection = connectionManager.GetVRConnection<VRInterAppRestConnection>(this.ConnectionId);
            VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;

            List<string> accountGenericFields = new List<string>();
            accountGenericFields.Add(this.AccountNameMappingField);
            accountGenericFields.Add(this.AccountEmailMappingField);

            Dictionary<string, object> accountGenericFieldValues = new AccountTypeManager().GetAccountGenericFieldValues(accountBEDefinitionId, account, accountGenericFields);

            object name = null, email = null;
            if (accountGenericFieldValues != null)
            {
                accountGenericFieldValues.TryGetValue(this.AccountNameMappingField, out name);
                accountGenericFieldValues.TryGetValue(this.AccountEmailMappingField, out email);
            }

            var retailAccount = new PartnerPortal.CustomerAccess.Entities.RetailAccount()
            {
                AccountId = account.AccountId,
                Name = name != null ? name.ToString() : null,
                Email = email != null ? email.ToString() : null,
                TenantId = this.TenantId
            };

            InsertOperationOutput<UserDetail> userDetails =
                connectionSettings.Post<PartnerPortal.CustomerAccess.Entities.RetailAccount, InsertOperationOutput<UserDetail>>("/api/PartnerPortal_CustomerAccess/RetailAccountUser/AddRetailAccountUser", retailAccount);

            if (userDetails.Result == InsertOperationResult.Succeeded)
            {
                PortalAccountSettings portalAccountSettings = new PortalAccountSettings()
                {
                    UserId = userDetails.InsertedObject.Entity.UserId,
                    Name = userDetails.InsertedObject.Entity.Name,
                    Email = userDetails.InsertedObject.Entity.Email,
                    TenantId = userDetails.InsertedObject.Entity.TenantId
                };

                new AccountBEManager().UpdateAccountExtendedSetting(accountBEDefinitionId, account.AccountId, portalAccountSettings);
            }
        }
    }
}

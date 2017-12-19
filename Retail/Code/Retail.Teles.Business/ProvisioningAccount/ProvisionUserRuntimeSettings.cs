using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using Retail.Teles.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Entities;

namespace Retail.Teles.Business.Provisioning
{
    public enum UserType { BusinessTrunk = 0, Subscriber = 1 }

    public class ProvisionUserRuntimeSettings : AccountProvisioner
    {
        TelesSiteManager _telesSiteManager = new TelesSiteManager();
        TelesEnterpriseManager _telesEnterpriseManager = new TelesEnterpriseManager();

        ProvisionAccountManager _provisionAccountManager = new ProvisionAccountManager();
        AccountBEManager _accountBEManager = new AccountBEManager();
        TelesGatewayManager _telesGatewayManager = new TelesGatewayManager();

        public UserType UserType { get; set; }
        public string TelesDomainId { get; set; }
        public string TelesEnterpriseId { get; set; }
        public string TelesSiteId { get; set; }
        public string TelesGatewayId { get; set; }
        public long TelesSiteRoutingGroupId { get; set; }
        public UserAccountSetting Settings { get; set; }


        public override void Execute(IAccountProvisioningContext context)
        {
            var definitionSettings = context.DefinitionSettings as ProvisionUserDefinitionSettings;
            if (definitionSettings == null)
                throw new NullReferenceException("definitionSettings");

            Account account = _accountBEManager.GetAccount(context.AccountBEDefinitionId, context.AccountId);
            account.ThrowIfNull("account", context.AccountId);

            var accountDefinitionAction = new AccountBEDefinitionManager().GetAccountActionDefinition(context.AccountBEDefinitionId, context.ActionDefinitionId);
            accountDefinitionAction.ThrowIfNull("accountDefinitionAction");
            if (accountDefinitionAction.AvailabilityCondition != null)
            {
                if (!_accountBEManager.EvaluateAccountCondition(account, accountDefinitionAction.AvailabilityCondition))
                {
                    throw new Exception("Not Allow to Provision User");
                }
            }
            var parentAccount = _accountBEManager.GetParentAccount(account);
            parentAccount.ThrowIfNull("account", account.ParentAccountId);
            var enterprise = _telesEnterpriseManager.GetEnterprise(definitionSettings.VRConnectionId, this.TelesEnterpriseId, this.TelesDomainId);

            var siteName = _telesSiteManager.GetSiteName(definitionSettings.VRConnectionId, TelesSiteId);
            string gatewayName = _telesGatewayManager.GetGatewayeName(definitionSettings.VRConnectionId, this.TelesGatewayId);
            _provisionAccountManager.CreateUserWithScreenedNumbers(context, definitionSettings.VRConnectionId, definitionSettings.CountryCode, TelesDomainId, TelesEnterpriseId, TelesSiteId, context.AccountBEDefinitionId, account, siteName, Settings, gatewayName, this.UserType, enterprise.EnterpriseType, TelesSiteRoutingGroupId);
        }
    }
}

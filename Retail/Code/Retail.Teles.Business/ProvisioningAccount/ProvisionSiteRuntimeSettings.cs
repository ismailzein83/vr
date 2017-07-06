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
    public class ProvisionSiteRuntimeSettings : AccountProvisioner
    {
        TelesEnterpriseManager _telesEnterpriseManager = new TelesEnterpriseManager();
        ProvisionAccountManager _provisionAccountManager = new ProvisionAccountManager();
        AccountBEManager _accountBEManager = new AccountBEManager();
        public ProvisionSiteSetting Settings { get; set; }
        public override void Execute(IAccountProvisioningContext context)
        {
            var definitionSettings = context.DefinitionSettings as ProvisionSiteDefinitionSettings;
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
                    throw new Exception("Not Allow to Provision Site");
                }
            }
            var parentAccount = _accountBEManager.GetParentAccount(account);
            parentAccount.ThrowIfNull("account", account.ParentAccountId);

            EnterpriseAccountMappingInfo enterpriseAccountMappingInfo = _accountBEManager.GetExtendedSettings<EnterpriseAccountMappingInfo>(parentAccount);
            if (enterpriseAccountMappingInfo != null)
            {
                var enterpriseName = _telesEnterpriseManager.GetEnterpriseName(definitionSettings.VRConnectionId, enterpriseAccountMappingInfo.TelesEnterpriseId);
                _provisionAccountManager.CreateSiteWithScreenedNumbers(context, definitionSettings.VRConnectionId, definitionSettings.CountryCode, enterpriseAccountMappingInfo.TelesEnterpriseId, context.AccountBEDefinitionId, account, enterpriseName, Settings.SiteAccountSetting, Settings.CentrexFeatSet);
            }
        }
    }
}

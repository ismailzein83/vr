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
    public class ProvisionAccountRuntimeSettings : AccountProvisioner
    {
        TelesEnterpriseManager _telesEnterpriseManager = new TelesEnterpriseManager();
        AccountBEManager _accountBEManager = new AccountBEManager();
        ProvisionAccountManager _provisionAccountManager = new ProvisionAccountManager();
        public string EnterpriseName { get; set; }
        public ProvisionAccountSetting Settings { get; set; }
        public override void Execute(IAccountProvisioningContext context)
        {
            var definitionSettings = context.DefinitionSettings as ProvisionAccountDefinitionSettings;
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
                   throw new Exception("Not Allow to Provision Enterprise");
               }
           }
           CreateEnterprise(context, definitionSettings, context.AccountBEDefinitionId, account);
        }
        private void CreateEnterprise(IAccountProvisioningContext context, ProvisionAccountDefinitionSettings definitionSettings, Guid accountBEDefinitionId, Account account)
        {
            Enterprise enterprise = new Enterprise
            {
                description = account.Name,
                name = this.EnterpriseName,
                maxCalls = Settings.EnterpriseAccountSetting.EnterpriseMaxCalls,
                maxCallsPerUser = Settings.EnterpriseAccountSetting.EnterpriseMaxCallsPerUser,
                maxRegistrations = Settings.EnterpriseAccountSetting.EnterpriseMaxRegistrations,
                maxRegsPerUser = Settings.EnterpriseAccountSetting.EnterpriseMaxRegsPerUser,
                maxSubsPerUser = Settings.EnterpriseAccountSetting.EnterpriseMaxSubsPerUser,
                maxBusinessTrunkCalls = Settings.EnterpriseAccountSetting.EnterpriseMaxBusinessTrunkCalls,
                maxUsers = Settings.EnterpriseAccountSetting.EnterpriseMaxUsers,
            };
            var enterpriseId = _telesEnterpriseManager.CreateEnterprise(definitionSettings.VRConnectionId, Settings.CentrexFeatSet, enterprise);            
            _telesEnterpriseManager.TryMapEnterpriseToAccount(accountBEDefinitionId, account.AccountId, enterpriseId, ProvisionStatus.Started);
            context.WriteTrackingMessage(LogEntryType.Information, string.Format("Enterprise {0} created.", this.EnterpriseName));
            CreateSites(context, definitionSettings, enterpriseId, accountBEDefinitionId, account.AccountId);
            _telesEnterpriseManager.TryMapEnterpriseToAccount(accountBEDefinitionId, account.AccountId, enterpriseId, ProvisionStatus.Completed);
            context.TrackActionExecuted(account.AccountId, string.Format("Teles enterprise {0}", this.EnterpriseName), null);

        }
        private void CreateSites(IAccountProvisioningContext context, ProvisionAccountDefinitionSettings definitionSettings, string enterpriseId, Guid accountBEDefinitionId, long accountId)
        {
            var sites = _accountBEManager.GetChildAccounts(accountBEDefinitionId, accountId, false);
            if (sites != null)
            {
                foreach (var site in sites)
                {
                    _provisionAccountManager.CreateSiteWithScreenedNumbers(context, definitionSettings.VRConnectionId, definitionSettings.CountryCode, enterpriseId, accountBEDefinitionId, site, this.EnterpriseName, Settings.SiteAccountSetting, Settings.CentrexFeatSet);
                }
            }
        }

    }
    public class Enterprise
    {
        public string name { get; set; }
        public string description { get; set; }
        public int maxCalls { get; set; }
        public int maxCallsPerUser { get; set; }
        public int maxRegistrations { get; set; }
        public int maxRegsPerUser { get; set; }
        public int maxSubsPerUser { get; set; }
        public int maxBusinessTrunkCalls { get; set; }
        public int maxUsers { get; set; }
    }
    
    public class ScreenedNumber
    {
        public string cc { get; set; }
        public string sn { get; set; }
        public string dir { get; set; }
        public string type { get; set; }
        public bool exactMatch { get; set; }
        public bool netNumber { get; set; }
        public bool geoNumber { get; set; }
    }
}

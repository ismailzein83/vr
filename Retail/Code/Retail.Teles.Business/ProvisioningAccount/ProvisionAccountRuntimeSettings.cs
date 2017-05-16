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
        DIDManager _didManager = new DIDManager();
        public string EnterpriseName { get; set; }
       
        public ProvisionAccountSetting Settings { get; set; }

        public override void Execute(IAccountProvisioningContext context)
        {
            var definitionSettings = context.DefinitionSettings as ProvisionAccountDefinitionSettings;
            if (definitionSettings == null)
                throw new NullReferenceException("definitionSettings");
            CreateEnterprise(context,definitionSettings, context.AccountBEDefinitionId, context.AccountId);
        }
        private void CreateEnterprise(IAccountProvisioningContext context,ProvisionAccountDefinitionSettings definitionSettings, Guid accountBEDefinitionId, long accountId)
        {
            Account account = _accountBEManager.GetAccount(accountBEDefinitionId, accountId);
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
            _telesEnterpriseManager.TryMapEnterpriseToAccount(accountBEDefinitionId, accountId, null, ProvisionStatus.Started);
            var enterpriseId = _telesEnterpriseManager.CreateEnterprise(definitionSettings.VRConnectionId, Settings.CentrexFeatSet, enterprise);
            _telesEnterpriseManager.TryMapEnterpriseToAccount(accountBEDefinitionId, accountId, enterpriseId, ProvisionStatus.Completed);
            context.WriteTrackingMessage(LogEntryType.Information, string.Format("Enterprise {0} created.", this.EnterpriseName));
            CreateSites(context, definitionSettings, enterpriseId, accountBEDefinitionId, accountId);
        }
        private void CreateSites(IAccountProvisioningContext context, ProvisionAccountDefinitionSettings definitionSettings, dynamic enterpriseId, Guid accountBEDefinitionId, long accountId)
        {
            var sites = _accountBEManager.GetChildAccounts(accountBEDefinitionId, accountId, false);
            if (sites != null)
            {
                foreach (var site in sites)
                {
                    Site newsite = new Site
                    {
                        name = string.Format("{0}.{1}", site.Name.ToLower().Replace(" ", ""), this.EnterpriseName),
                        description = site.Name,
                        maxCalls = Settings.SiteAccountSetting.SiteMaxCalls,
                        maxCallsPerUser = Settings.SiteAccountSetting.SiteMaxCallsPerUser,
                        maxRegistrations = Settings.SiteAccountSetting.SiteMaxRegistrations,
                        maxRegsPerUser = Settings.SiteAccountSetting.SiteMaxRegsPerUser,
                        maxSubsPerUser = Settings.SiteAccountSetting.SiteMaxSubsPerUser,
                        maxBusinessTrunkCalls = Settings.SiteAccountSetting.SiteMaxBusinessTrunkCalls,
                        maxUsers = Settings.SiteAccountSetting.SiteMaxUsers,
                        presenceEnabled = true,
                        registrarAuthRequired = true,
                        registrarEnabled = true,
                        ringBackUri = "ringback",
                    };
                    _telesEnterpriseManager.TryMapSiteToAccount(accountBEDefinitionId, site.AccountId, null, ProvisionStatus.Started);

                    dynamic siteId = _telesEnterpriseManager.CreateSite(definitionSettings.VRConnectionId, enterpriseId, Settings.CentrexFeatSet, newsite);
                    _telesEnterpriseManager.TryMapSiteToAccount(accountBEDefinitionId, site.AccountId, siteId, ProvisionStatus.Completed);
                    context.WriteTrackingMessage(LogEntryType.Information, string.Format("Site {0} created.", site.Name));
                    CreateScreendedNumbers(context, definitionSettings, site.AccountId, siteId);
                }
            }
        }

        private void CreateScreendedNumbers(IAccountProvisioningContext context, ProvisionAccountDefinitionSettings definitionSettings, long siteAccountId, dynamic siteId)
        {
            var dids = _didManager.GetDIDsByParentId(siteAccountId.ToString(), DateTime.Now);
            if (dids != null)
            {
                foreach (var did in dids)
                {
                    DIDNumberType didNumberType = _didManager.GetDIDNumberType(did);
                    switch (didNumberType)
                    {
                        case DIDNumberType.Number:
                            foreach (string number in did.Settings.Numbers)
                            {
                                CreateScreenedNumber(context,definitionSettings, siteId, number);

                            }
                            break;
                        case DIDNumberType.Range:
                            foreach (DIDRange range in did.Settings.Ranges)
                            {
                                long from = range.From.TryParseWithValidate<long>(long.TryParse);
                                long to = range.To.TryParseWithValidate<long>(long.TryParse);
                                for (var index = from; index <= to; index++)
                                {
                                    CreateScreenedNumber(context,definitionSettings, siteId, index.ToString());
                                }
                            }
                            break;
                        default: throw new Exception("Invalid Type for DID.");
                    }
                }
            }
        }

        private void CreateScreenedNumber(IAccountProvisioningContext context, ProvisionAccountDefinitionSettings definitionSettings, dynamic siteId, string number)
        {
            ScreenedNumber screenedNumber = new ScreenedNumber
            {
                cc = definitionSettings.CountryCode,
                sn = number,
                dir = "IN_OUT",
                exactMatch = true,
                geoNumber = true,
                netNumber = true,
                type = "FIXED_NETWORK",
            };
            _telesEnterpriseManager.CreateScreenedNumber(definitionSettings.VRConnectionId, siteId, screenedNumber);
            context.WriteTrackingMessage(LogEntryType.Information, string.Format("Screened Number {0} created.", number));
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
        public class Site
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
            public string ringBackUri { get; set; }
            public bool registrarEnabled { get; set; }
            public bool registrarAuthRequired { get; set; }
            public bool presenceEnabled { get; set; }

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
}

using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Teles.Business.Provisioning
{
    public class ProvisionAccountRuntimeSettings : AccountProvisioner
    {
        TelesEnterpriseManager _telesEnterpriseManager = new TelesEnterpriseManager();
        AccountBEManager _accountBEManager = new AccountBEManager();
        DIDManager _didManager = new DIDManager();
        public string EnterpriseName { get; set; }
        public override void Execute(IAccountProvisioningContext context)
        {
            var definitionSettings = context.DefinitionSettings as ProvisionAccountDefinitionSettings;
            if (definitionSettings == null)
                throw new NullReferenceException("definitionSettings");
            CreateEnterprise(definitionSettings,context.AccountBEDefinitionId, context.AccountId);
        }
        private void CreateEnterprise(ProvisionAccountDefinitionSettings definitionSettings,Guid accountBEDefinitionId,long accountId )
        {
            Account account = _accountBEManager.GetAccount(accountBEDefinitionId, accountId);
            Enterprise enterprise = new Enterprise
            {
                description = account.Name,
                name = this.EnterpriseName,
                maxCalls = definitionSettings.EnterpriseMaxCalls,
                maxCallsPerUser = definitionSettings.EnterpriseMaxCallsPerUser,
                maxRegistrations = definitionSettings.EnterpriseMaxRegistrations,
                maxRegsPerUser = definitionSettings.EnterpriseMaxRegsPerUser,
                maxSubsPerUser = definitionSettings.EnterpriseMaxSubsPerUser,
                maxBusinessTrunkCalls = definitionSettings.EnterpriseMaxBusinessTrunkCalls,
                maxUsers = definitionSettings.EnterpriseMaxUsers,
            };
            var enterpriseId = _telesEnterpriseManager.CreateEnterprise(definitionSettings.VRConnectionId, definitionSettings.CentrexFeatSet, enterprise);

            _telesEnterpriseManager.TryMapEnterpriseToAccount(accountBEDefinitionId, accountId, enterpriseId);
            CreateSites(definitionSettings, enterpriseId,accountBEDefinitionId, accountId);
        }
        private void CreateSites(ProvisionAccountDefinitionSettings definitionSettings, dynamic enterpriseId, Guid accountBEDefinitionId,long accountId)
        {
            var sites = _accountBEManager.GetChildAccounts(accountBEDefinitionId, accountId, false);
            if(sites != null)
            {
                foreach(var site in sites)
                {
                    Site newsite = new Site
                    {
                        name = string.Format("{0}.{1}", site.Name.ToLower().Replace(" ", ""), this.EnterpriseName),
                        description = site.Name,
                        maxCalls = definitionSettings.SiteMaxCalls,
                        maxCallsPerUser = definitionSettings.SiteMaxCallsPerUser,
                        maxRegistrations = definitionSettings.SiteMaxRegistrations,
                        maxRegsPerUser = definitionSettings.SiteMaxRegsPerUser,
                        maxSubsPerUser = definitionSettings.SiteMaxSubsPerUser,
                        maxBusinessTrunkCalls = definitionSettings.SiteMaxBusinessTrunkCalls,
                        maxUsers = definitionSettings.SiteMaxUsers,
                        presenceEnabled = true,
                        registrarAuthRequired = true,
                        registrarEnabled = true,
                        ringBackUri = "ringback",
                    };
                    dynamic siteId = _telesEnterpriseManager.CreateSite(definitionSettings.VRConnectionId, enterpriseId, definitionSettings.CentrexFeatSet, newsite);
                    CreateScreendedNumbers(definitionSettings, site.AccountId,siteId);
                }
            }
        }

        private void CreateScreendedNumbers(ProvisionAccountDefinitionSettings definitionSettings, long siteAccountId, dynamic siteId)
        {
           var dids = _didManager.GetDIDsByParentId(siteAccountId.ToString(),DateTime.Now);
            if(dids != null)
            {
                foreach( var did in dids)
                {
                    ScreenedNumber screenedNumber = new ScreenedNumber
                    {
                        cc = definitionSettings.CountryCode,
                        sn = did.Number,
                        dir ="IN_OUT",
                        exactMatch = true,
                        geoNumber = true,
                        netNumber = true,
                        type = "FIXED_NETWORK",
                    };
                    _telesEnterpriseManager.CreateScreenedNumber(definitionSettings.VRConnectionId, siteId, screenedNumber);
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

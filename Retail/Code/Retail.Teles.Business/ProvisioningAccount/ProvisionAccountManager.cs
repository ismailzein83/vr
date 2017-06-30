using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using Retail.Teles.Business.Provisioning;
using Retail.Teles.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Entities;
namespace Retail.Teles.Business
{
    public class ProvisionAccountManager
    {
        DIDManager _didManager = new DIDManager();
        AccountBEManager _accountBEManager = new AccountBEManager();
        TelesSiteManager _telesSiteManager = new TelesSiteManager();
        public void CreateSiteWithScreenedNumbers(IAccountProvisioningContext context, Guid vrConnectionId, string countryCode, string enterpriseId, Guid accountBEDefinitionId, Account site, string enterpriseName, SiteAccountSetting siteAccountSetting, string centrexFeatSet)
        {

            Site newsite = new Site
            {
                name = string.Format("{0}.{1}", Regex.Replace(site.Name.ToLower(), @"(\s*)(\.*)", string.Empty), enterpriseName),
                description = site.Name,
                maxCalls = siteAccountSetting.SiteMaxCalls,
                maxCallsPerUser = siteAccountSetting.SiteMaxCallsPerUser,
                maxRegistrations = siteAccountSetting.SiteMaxRegistrations,
                maxRegsPerUser = siteAccountSetting.SiteMaxRegsPerUser,
                maxSubsPerUser = siteAccountSetting.SiteMaxSubsPerUser,
                maxBusinessTrunkCalls = siteAccountSetting.SiteMaxBusinessTrunkCalls,
                maxUsers = siteAccountSetting.SiteMaxUsers,
                presenceEnabled = true,
                registrarAuthRequired = true,
                registrarEnabled = true,
                ringBackUri = "ringback",
            };
            string siteId = _telesSiteManager.CreateSite(vrConnectionId, enterpriseId, centrexFeatSet, newsite);
            _telesSiteManager.TryMapSiteToAccount(accountBEDefinitionId, site.AccountId, siteId, ProvisionStatus.Started);
            _accountBEManager.TrackAndLogObjectCustomAction(accountBEDefinitionId, site.AccountId, "Provision", string.Format("Teles site {0}", site.Name), null);
            context.WriteTrackingMessage(LogEntryType.Information, string.Format("Site {0} created.", site.Name));
            CreateScreendedNumbers(context, vrConnectionId, countryCode, site.AccountId, siteId);
            _telesSiteManager.TryMapSiteToAccount(accountBEDefinitionId, site.AccountId, siteId, ProvisionStatus.Completed);
        }

        private void CreateScreendedNumbers(IAccountProvisioningContext context, Guid vrConnectionId, string countryCode, long siteAccountId, string siteId)
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
                                CreateScreenedNumber(context, vrConnectionId, countryCode, siteId, number);

                            }
                            break;
                        case DIDNumberType.Range:
                            foreach (DIDRange range in did.Settings.Ranges)
                            {
                                long from = range.From.TryParseWithValidate<long>(long.TryParse);
                                long to = range.To.TryParseWithValidate<long>(long.TryParse);
                                for (var index = from; index <= to; index++)
                                {
                                    CreateScreenedNumber(context, vrConnectionId, countryCode, siteId, index.ToString());
                                }
                            }
                            break;
                        default: throw new Exception("Invalid Type for DID.");
                    }
                }
            }

        }

        private void CreateScreenedNumber(IAccountProvisioningContext context, Guid vrConnectionId, string countryCode, string siteId, string number)
        {
            ScreenedNumber screenedNumber = new ScreenedNumber
            {
                cc = countryCode,
                sn = number,
                dir = "IN_OUT",
                exactMatch = true,
                geoNumber = true,
                netNumber = true,
                type = "FIXED_NETWORK",
            };
            _telesSiteManager.CreateScreenedNumber(vrConnectionId, siteId, screenedNumber);
            context.WriteTrackingMessage(LogEntryType.Information, string.Format("Screened Number {0} created.", number));
        }
    }
}

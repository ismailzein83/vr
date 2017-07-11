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
            context.WriteTrackingMessage(LogEntryType.Information, string.Format("Site {0} created.", site.Name));
            CreateScreendedNumbers(context, vrConnectionId, countryCode, site.AccountId, siteId);
            _telesSiteManager.TryMapSiteToAccount(accountBEDefinitionId, site.AccountId, siteId, ProvisionStatus.Completed);
            context.TrackActionExecuted(site.AccountId, string.Format("Teles site {0}", site.Name), null);
        }

        private void CreateScreendedNumbers(IAccountProvisioningContext context, Guid vrConnectionId, string countryCode, long siteAccountId, string siteId)
        {
            var dids = _didManager.GetDIDsByParentId(siteAccountId.ToString(), DateTime.Now);

            if (dids != null)
            {
                List<string> trackingNumbers = new List<string>();
                foreach (var did in dids)
                {
                    List<string> didNumbers = _didManager.GetAllDIDNumbers(did);
                    if (didNumbers != null)
                    {
                        foreach (string number in didNumbers)
                        {
                            trackingNumbers.Add(number);
                            if (trackingNumbers.Count == 10)
                            {
                                context.WriteTrackingMessage(LogEntryType.Information, string.Format("Numbers Created: {0}", string.Join(",", trackingNumbers)));
                                trackingNumbers = new List<string>();
                            }
                            CreateScreenedNumber(context, vrConnectionId, countryCode, siteId, number);
                        }
                    }
                }
                if (trackingNumbers.Count > 0 )
                {
                    context.WriteTrackingMessage(LogEntryType.Information, string.Format("Numbers Created: {0}", string.Join(",", trackingNumbers)));
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

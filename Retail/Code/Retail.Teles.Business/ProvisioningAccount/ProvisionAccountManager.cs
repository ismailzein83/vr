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
        TelesUserManager _telesUserManager = new TelesUserManager();

        public void CreateUserWithScreenedNumbers(IAccountProvisioningContext context, Guid vrConnectionId, string countryCode, string telesDomainId, string telesEnterpriseId, string telesSiteId, Guid accountBEDefinitionId, Account user, string siteName, UserAccountSetting userAccountSetting, string gateway, UserType userType, EnterpriseType enterpriseType, long telesSiteRoutingGroupId)
        {


            User newuser = new User
            {

                firstName = userAccountSetting.FirstName,
                lastName = userAccountSetting.LastName,
                loginName = userAccountSetting.LoginName,
                loginPassword = userAccountSetting.LoginPassword,
                role = userType == UserType.BusinessTrunk ? "BT" : "END_USER",
                auth = "FULL",
                pin = userAccountSetting.Pin,
                maxRegistrations = userAccountSetting.MaxRegistrations,
                routingGroupId = telesSiteRoutingGroupId,
            };

            List<NumberRange> numberRanges = null;
            if(userType == UserType.BusinessTrunk)
            {
                numberRanges = GetNumberRanges(context, countryCode, user.AccountId);
                if (numberRanges == null || numberRanges.Count == 0)
                    throw new Exception("At least one did should be created.");

                newuser.numberRanges = numberRanges;
                newuser.specialTarget = new SpecialTarget {   maxCalls = userAccountSetting.MaxCalls } ;
            }
            //if (userType == UserType.BusinessTrunk)
            //    gateway = null;
            string telesUserId = _telesUserManager.CreateUser(vrConnectionId, telesSiteId, gateway, newuser);
            _telesUserManager.TryMapUserToAccount(accountBEDefinitionId, user.AccountId, telesDomainId, telesEnterpriseId, telesSiteId, telesUserId, ProvisionStatus.Started);
            string trackingMessage = string.Format("User {0} created.", user.Name);
            if (numberRanges != null && numberRanges.Count > 0)
            {
                trackingMessage += string.Format(" With Number Ranges: {0}.", string.Join<string>(",", numberRanges.MapRecords(x => x.startSn)));
            }
            context.WriteBusinessTrackingMsg(LogEntryType.Information, trackingMessage);
            if (numberRanges == null)
             CreateUserScreenNumbers(context, vrConnectionId, countryCode, user.AccountId, telesUserId, userType, enterpriseType);
            _telesUserManager.TryMapUserToAccount(accountBEDefinitionId, user.AccountId, telesDomainId, telesEnterpriseId, telesSiteId, telesUserId, ProvisionStatus.Completed);
            context.TrackActionExecuted(user.AccountId, string.Format("Teles User {0}", user.Name), null);
        }
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
            string siteId = _telesSiteManager.CreateSite(vrConnectionId, enterpriseId, centrexFeatSet, newsite,null);
            _telesSiteManager.TryMapSiteToAccount(accountBEDefinitionId, site.AccountId, siteId, ProvisionStatus.Started);
            context.WriteTrackingMessage(LogEntryType.Information, string.Format("Site {0} created.", site.Name));
            CreateScreendedNumbers(context, vrConnectionId, countryCode, site.AccountId, siteId);
            _telesSiteManager.TryMapSiteToAccount(accountBEDefinitionId, site.AccountId, siteId, ProvisionStatus.Completed);
            context.TrackActionExecuted(site.AccountId, string.Format("Teles site {0}", site.Name), null);
        }

        private void CreateScreendedNumbers(IAccountProvisioningContext context, Guid vrConnectionId, string countryCode, long accountId, string telesAccountId)
        {
            var dids = _didManager.GetDIDsByParentId(accountId.ToString(), DateTime.Now);

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
                            CreateScreenedNumber(context, vrConnectionId, countryCode, telesAccountId, number);
                        }
                    }
                }
                if (trackingNumbers.Count > 0 )
                {
                    context.WriteTrackingMessage(LogEntryType.Information, string.Format("Numbers Created: {0}", string.Join(",", trackingNumbers)));
                }
            }
        }

        private void CreateScreenedNumber(IAccountProvisioningContext context, Guid vrConnectionId, string countryCode, string telesAccountId, string number)
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
            _telesSiteManager.CreateScreenedNumber(vrConnectionId, telesAccountId, screenedNumber);
            context.WriteTrackingMessage(LogEntryType.Information, string.Format("Screened Number {0} created.", number));
        }

        private void CreateUserScreenNumbers(IAccountProvisioningContext context, Guid vrConnectionId, string countryCode, long accountId, string telesUserId, UserType userType, EnterpriseType enterpriseType)
        {
            var dids = _didManager.GetDIDsByParentId(accountId.ToString(), DateTime.Now);
            if (dids != null)
            {
                foreach (var did in dids)
                {
                    List<string> didNumbers = _didManager.GetAllDIDNumbers(did);
                    if (didNumbers != null)
                    {
                        switch (userType)
                        {
                            case UserType.BusinessTrunk:
                                //  CreateNumberRanges(context, vrConnectionId, countryCode, telesUserId, didNumbers);

                                break;
                            case UserType.Subscriber:
                                switch (enterpriseType)
                                {
                                    case EnterpriseType.Enterprise:
                                     //   CreatePhoneNumbers(context, vrConnectionId, countryCode, telesUserId, didNumbers);
                                        break;
                                    case EnterpriseType.Residential:
                                        CreateMSNs(context, vrConnectionId, countryCode, telesUserId, didNumbers);
                                        break;
                                }
                                break;
                        }
                    }
                   
                }
            }
        }

        private List<NumberRange> GetNumberRanges(IAccountProvisioningContext context, string countryCode, long accountId)
        {
            var dids = _didManager.GetDIDsByParentId(accountId.ToString(), DateTime.Now);
            List<NumberRange> numberRangs = null;
            if (dids != null)
            {
                foreach (var did in dids)
                {
                    List<string> didNumbers = _didManager.GetAllDIDNumbers(did);
                    if (didNumbers != null)
                    {
                        if (numberRangs == null)
                            numberRangs = new List<NumberRange>();
                        foreach (var didNumber in didNumbers)
                        {
                            numberRangs.Add(new NumberRange
                            {
                                startCc = countryCode,
                                startSn = didNumber,
                                flexibleDdi = false,
                                maxDdiDigits = 0,
                                featureRange = false,
                                systemRange = numberRangs.Count == 0 ? true : false,
                                ported = "NOT_PORTED",
                            });
                        }
                    }
                }
            }
           
            if (numberRangs != null)
            {
              //  context.WriteTrackingMessage(LogEntryType.Information, string.Format("Number Ranges {0} created.", string.Join(",", didNumbers)));
            }
            return numberRangs;
        }
        private void CreateMSNs(IAccountProvisioningContext context, Guid vrConnectionId, string countryCode, string telesUserId, List<string> didNumbers)
        {
            if(didNumbers != null)
            {
                if (didNumbers.Count > 10)
                {
                    throw new Exception("Only 10 dids can be created.");
                }
                foreach(var didNumber in didNumbers)
                {
                    var phoneNumber = new PhoneNumber
                    {
                        sn = didNumber,
                        cc = countryCode,
                    };
                    _telesUserManager.CreateMSN(vrConnectionId, telesUserId, phoneNumber);
                    context.WriteTrackingMessage(LogEntryType.Information, string.Format("Phone Number {0} created.", didNumber));

                }
            }
           
        }
        private void CreatePhoneNumbers(IAccountProvisioningContext context, Guid vrConnectionId, string countryCode, string telesUserId, List<string> didNumbers)
        {
            List<PhoneNumber> phoneNumbers = null;
            if (didNumbers != null)
            {
                phoneNumbers = new List<PhoneNumber>();
                foreach (var didNumber in didNumbers)
                {
                    phoneNumbers.Add(new PhoneNumber
                    {
                        sn = didNumber,
                        cc = countryCode,
                    });
                }
            }
            if (phoneNumbers != null)
            {
                _telesUserManager.CreatePhoneNumbers(vrConnectionId, telesUserId, phoneNumbers);
                context.WriteTrackingMessage(LogEntryType.Information, string.Format("Phone Numbers {0} created.", string.Join(",", didNumbers)));
            }
        }
    }
    public class NumberRange
    {
       // public string id { get; set; }
      //  public string userId { get; set; }

        public string startCc { get; set; }

        public string startSn { get; set; }

        public bool flexibleDdi { get; set; }
        public int maxDdiDigits { get; set; }

        public bool featureRange { get; set; }

        public bool systemRange { get; set; }

        public string ported { get; set; }

    }
    public class PhoneNumber
    {
        public string id { get; set; }
        public string userId { get; set; }
        public string siteId { get; set; }
        public string enterpriseId { get; set; }
        public string type { get; set; }
        public string dir { get; set; }
        public string main { get; set; }
        public string emergency { get; set; }
        public string e164 { get; set; }
        public string cc { get; set; }
        public string ndc { get; set; }
        public string sn { get; set; }
        public string screenNumId { get; set; }
        public string portedStatus { get; set; }
        public string portedTime { get; set; }
        public string addToAddressBook { get; set; }
        public string netNumber { get; set; }
        public string hide { get; set; }
        public string serviceType { get; set; }

    }
    public class MSN
    {
        public string cc { get; set; }
        public string sn { get; set; }
     
    }

    public class User
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string loginName { get; set; }
        public string loginPassword { get; set; }
        public string role { get; set; }
        public string auth { get; set; }
        public string pin { get; set; }
        public int? maxRegistrations { get; set; }
        public List<NumberRange> numberRanges { get; set; }
        public SpecialTarget specialTarget { get; set; }
        public long routingGroupId { get; set; }
    }
    public class SpecialTarget
    {
        public dynamic maxCalls { get; set; }
    }
}

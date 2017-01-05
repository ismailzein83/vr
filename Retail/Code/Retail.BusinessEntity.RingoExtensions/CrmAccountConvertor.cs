using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using Retail.BusinessEntity.MainExtensions.AccountParts;
using Retail.Ringo.MainExtensions.AccountParts;
using Vanrise.BEBridge.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Retail.Ringo.MainExtensions
{
    public class CrmAccountConvertor : TargetBEConvertor
    {
        public override void ConvertSourceBEs(ITargetBEConvertorConvertSourceBEsContext context)
        {
            CRMSourceBatch fileBatch = context.SourceBEBatch as CRMSourceBatch;

            List<ITargetBE> lstTargets = new List<ITargetBE>();



            foreach (var account in fileBatch.EntityList)
            {
                SourceAccountData accountData = new SourceAccountData
                {
                    Account = new Account { TypeId = Guid.Parse("19A97F72-8C56-441E-A74D-AA185961B242") }
                };
                var sourceId = account.accountid;
                accountData.Account.Name = account.name;
                accountData.Account.SourceId = sourceId;
                FillAccountSettings(accountData, account);
                accountData.Account.StatusId = Guid.Parse("DDB6A5B8-B9E5-4050-BEE8-0F030E801B8B");
                lstTargets.Add(accountData);
            }

            context.TargetBEs = lstTargets;
        }

        public override void MergeTargetBEs(ITargetBEConvertorMergeTargetBEsContext context)
        {
            SourceAccountData existingBe = context.ExistingBE as SourceAccountData;
            SourceAccountData newBe = context.NewBE as SourceAccountData;

            SourceAccountData finalBe = new SourceAccountData
            {
                Account = Serializer.Deserialize<Account>(Serializer.Serialize(existingBe.Account))
            };

            UpdatePersonalInfoPart(finalBe, newBe);
            UpdateResidentialInfoPart(finalBe, newBe);
            UpdateActivationInfoPart(finalBe, newBe);
            context.FinalBE = finalBe;
        }

        #region Account Settings Part

        void FillAccountSettings(SourceAccountData accountData, dynamic account)
        {
            accountData.Account.Settings = new AccountSettings
            {
                Parts = new AccountPartCollection()
            };

            FillPersonalInfoPart(accountData, account);
            FillResidentialProfilePart(accountData, account);
            FillActivationPart(accountData, account);
        }

        void FillActivationPart(SourceAccountData accountData, dynamic account)
        {
            DateTime activationDate;
            DateTime.TryParseExact(account.createdon.ToString(), "dd/MM/yyyy hh:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out activationDate);
            accountData.Account.Settings.Parts.Add(AccountPartActivation._ConfigId, new AccountPart
            {
                Settings = new AccountPartActivation
                {
                    ActivationDate = activationDate
                }
            });
        }

        void FillPersonalInfoPart(SourceAccountData accountData, dynamic account)
        {

            accountData.Account.Settings.Parts.Add(AccountPartPersonalInfo._ConfigId, new AccountPart
            {
                Settings = new AccountPartPersonalInfo
                {
                    FirstName = account.name
                }
            });
        }

        void FillResidentialProfilePart(SourceAccountData accountData, dynamic account)
        {
            CountryManager countryManager = new CountryManager();
            string countryName = account.address1_country;
            Country country = countryManager.GetCountry(countryName);
            accountData.Account.Settings.Parts.Add(AccountPartResidentialProfile._ConfigId, new AccountPart
            {
                Settings = new AccountPartResidentialProfile
                {
                    CountryId = country != null ? (int?)country.CountryId : null,
                    Email = account.emailaddress1,
                    ZipCode = account.address1_postalcode,
                    PhoneNumbers = new List<string>() { account.telephone1.ToString(), account.telephone2.ToString(), account.telephone3.ToString() },
                    Town = account.address1_stateorprovince,
                    Provence = account.address1_stateorprovince
                }
            });
        }

        void UpdatePersonalInfoPart(SourceAccountData finalBe, SourceAccountData newBe)
        {
            AccountPartPersonalInfo personalInfo = newBe.Account.Settings.Parts[AccountPartPersonalInfo._ConfigId].Settings as AccountPartPersonalInfo;
            AccountPartPersonalInfo newPersonalInfo = finalBe.Account.Settings.Parts[AccountPartPersonalInfo._ConfigId].Settings as AccountPartPersonalInfo;

            newPersonalInfo.CityId = personalInfo.CityId;
            newPersonalInfo.CountryId = personalInfo.CountryId;
            newPersonalInfo.BirthDate = personalInfo.BirthDate;
            newPersonalInfo.FirstName = personalInfo.FirstName;
            newPersonalInfo.Gender = personalInfo.Gender;
            newPersonalInfo.LastName = personalInfo.LastName;
            finalBe.Account.Name = string.Format("{0} {1}", newPersonalInfo.FirstName, newPersonalInfo.LastName);
            finalBe.Account.Settings.Parts[AccountPartPersonalInfo._ConfigId].Settings = newPersonalInfo;
        }

        void UpdateResidentialInfoPart(SourceAccountData finalBe, SourceAccountData newBe)
        {
            AccountPartResidentialProfile residentialProfile = newBe.Account.Settings.Parts[AccountPartResidentialProfile._ConfigId].Settings as AccountPartResidentialProfile;
            AccountPartResidentialProfile newResidentialProfile = finalBe.Account.Settings.Parts[AccountPartResidentialProfile._ConfigId].Settings as AccountPartResidentialProfile;

            newResidentialProfile.CityId = residentialProfile.CityId;
            newResidentialProfile.CountryId = residentialProfile.CountryId;
            newResidentialProfile.Email = residentialProfile.Email;
            newResidentialProfile.Faxes = residentialProfile.Faxes;
            newResidentialProfile.PhoneNumbers = residentialProfile.PhoneNumbers;
            newResidentialProfile.Provence = residentialProfile.Provence;
            newResidentialProfile.Street = residentialProfile.Street;
            newResidentialProfile.Town = residentialProfile.Town;
            newResidentialProfile.ZipCode = residentialProfile.ZipCode;

            finalBe.Account.Settings.Parts[AccountPartResidentialProfile._ConfigId].Settings = newResidentialProfile;
        }

        private void UpdateActivationInfoPart(SourceAccountData finalBe, SourceAccountData newBe)
        {
            AccountPartActivation activationPart = newBe.Account.Settings.Parts[AccountPartActivation._ConfigId].Settings as AccountPartActivation;
            AccountPartActivation newActivationPart = finalBe.Account.Settings.Parts[AccountPartActivation._ConfigId].Settings as AccountPartActivation;

            newActivationPart.ActivationDate = activationPart.ActivationDate;
            newActivationPart.Status = activationPart.Status;
            finalBe.Account.Settings.Parts[AccountPartActivation._ConfigId].Settings = newActivationPart;
        }

        #endregion

    }
}

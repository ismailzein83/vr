using System;
using System.Collections.Generic;
using Microsoft.VisualBasic.FileIO;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Entities;
using Retail.BusinessEntity.MainExtensions.AccountParts;
using Vanrise.BEBridge.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Retail.BusinessEntity.Business;
using Vanrise.Common;

namespace Retail.BusinessEntity.RingoExtensions
{
    public class RingoFileAccountConvertor : TargetBEConvertor
    {
        public override void ConvertSourceBEs(ITargetBEConvertorConvertSourceBEsContext context)
        {

            FileSourceBatch fileBatch = context.SourceBEBatch as FileSourceBatch;

            List<ITargetBE> lstTargets = new List<ITargetBE>();
            using (Stream stream = new MemoryStream(fileBatch.Content))
            {
                TextFieldParser parser = new TextFieldParser(stream) { Delimiters = new string[] { "," } };
                while (true)
                {
                    string[] accountRecords = parser.ReadFields();
                    if (accountRecords != null)
                    {
                        SourceAccountData accountData = new SourceAccountData
                        {
                            Account = new Account { TypeId = 19 }
                        };
                        accountRecords = accountRecords.Select(s => s.Trim(new char[] { '\'' })).ToArray();
                        var sourceId = accountRecords[22];
                        accountData.Account.Name = string.Format("{0} {1}", accountRecords[2], accountRecords[3]);
                        accountData.Account.SourceId = sourceId;
                        FillAccountSettings(accountData, accountRecords);

                        lstTargets.Add(accountData);
                    }
                    else
                        break;
                }
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

        public override bool CompareBeforeUpdate
        {
            get
            {
                return base.CompareBeforeUpdate;
            }
        }

        #region Account Settings Part

        void FillAccountSettings(SourceAccountData accountData, string[] accountRecords)
        {
            accountData.Account.Settings = new AccountSettings
            {
                Parts = new AccountPartCollection()
            };

            FillPersonalInfoPart(accountData, accountRecords);
            FillResidentialProfilePart(accountData, accountRecords);
            FillActivationPart(accountData, accountRecords);
            FillFinancialPart(accountData, accountRecords);
        }

        void FillFinancialPart(SourceAccountData accountData, string[] accountRecords)
        {
            accountData.Account.Settings.Parts.Add(AccountPartFinancial.ExtensionConfigId, new AccountPart
            {
                Settings = new AccountPartFinancial
                {
                }
            });
        }

        void FillActivationPart(SourceAccountData accountData, string[] accountRecords)
        {
            DateTime activationDate;
            DateTime.TryParseExact(accountRecords[16], "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out activationDate);
            accountData.Account.Settings.Parts.Add(AccountPartActivation.ExtensionConfigId, new AccountPart
            {
                Settings = new AccountPartActivation
                {
                    ActivationDate = activationDate
                }
            });
        }

        void FillPersonalInfoPart(SourceAccountData accountData, string[] accountRecords)
        {
            CountryManager countryManager = new CountryManager();
            Country country = countryManager.GetCountry(accountRecords[5]);
            DateTime birthDate;
            DateTime.TryParseExact(accountRecords[16], "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out birthDate);
            accountData.Account.Settings.Parts.Add(AccountPartPersonalInfo.ExtensionConfigId, new AccountPart
            {
                Settings = new AccountPartPersonalInfo
                {
                    FirstName = accountRecords[2],
                    LastName = accountRecords[3],
                    Gender = accountRecords[4] == "M" ? Gender.Male : Gender.Female,
                    BirthCountryId = country != null ? (int?)country.CountryId : null,
                    BirthCityId = null,
                    BirthDate = birthDate
                }
            });
        }

        void FillResidentialProfilePart(SourceAccountData accountData, string[] accountRecords)
        {
            CountryManager countryManager = new CountryManager();
            Country country = countryManager.GetCountry(accountRecords[8]);
            accountData.Account.Settings.Parts.Add(AccountPartResidentialProfile.ExtensionConfigId, new AccountPart
            {
                Settings = new AccountPartResidentialProfile
                {
                    CountryId = country != null ? (int?)country.CountryId : null,
                    Email = accountRecords[10],
                    ZipCode = accountRecords[11],
                    PhoneNumbers = new List<string>() { accountRecords[12], accountRecords[19], accountRecords[20] },
                    Town = accountRecords[17],
                    Provence = accountRecords[18]
                }
            });
        }

        void UpdatePersonalInfoPart(SourceAccountData finalBe, SourceAccountData newBe)
        {
            AccountPartPersonalInfo personalInfo = newBe.Account.Settings.Parts[AccountPartPersonalInfo.ExtensionConfigId].Settings as AccountPartPersonalInfo;
            AccountPartPersonalInfo newPersonalInfo = finalBe.Account.Settings.Parts[AccountPartPersonalInfo.ExtensionConfigId].Settings as AccountPartPersonalInfo;

            newPersonalInfo.BirthCityId = personalInfo.BirthCityId;
            newPersonalInfo.BirthCountryId = personalInfo.BirthCountryId;
            newPersonalInfo.BirthDate = personalInfo.BirthDate;
            newPersonalInfo.FirstName = personalInfo.FirstName;
            newPersonalInfo.Gender = personalInfo.Gender;
            newPersonalInfo.LastName = personalInfo.LastName;
            finalBe.Account.Name = string.Format("{0} {1}", newPersonalInfo.FirstName, newPersonalInfo.LastName);
            finalBe.Account.Settings.Parts[AccountPartPersonalInfo.ExtensionConfigId].Settings = newPersonalInfo;
        }

        void UpdateResidentialInfoPart(SourceAccountData finalBe, SourceAccountData newBe)
        {
            AccountPartResidentialProfile residentialProfile = newBe.Account.Settings.Parts[AccountPartResidentialProfile.ExtensionConfigId].Settings as AccountPartResidentialProfile;
            AccountPartResidentialProfile newResidentialProfile = finalBe.Account.Settings.Parts[AccountPartResidentialProfile.ExtensionConfigId].Settings as AccountPartResidentialProfile;

            newResidentialProfile.CityId = residentialProfile.CityId;
            newResidentialProfile.CountryId = residentialProfile.CountryId;
            newResidentialProfile.Email = residentialProfile.Email;
            newResidentialProfile.Faxes = residentialProfile.Faxes;
            newResidentialProfile.PhoneNumbers = residentialProfile.PhoneNumbers;
            newResidentialProfile.Provence = residentialProfile.Provence;
            newResidentialProfile.Street = residentialProfile.Street;
            newResidentialProfile.Town = residentialProfile.Town;
            newResidentialProfile.ZipCode = residentialProfile.ZipCode;

            finalBe.Account.Settings.Parts[AccountPartResidentialProfile.ExtensionConfigId].Settings = newResidentialProfile;
        }

        private void UpdateActivationInfoPart(SourceAccountData finalBe, SourceAccountData newBe)
        {
            AccountPartActivation activationPart = newBe.Account.Settings.Parts[AccountPartActivation.ExtensionConfigId].Settings as AccountPartActivation;
            AccountPartActivation newActivationPart = finalBe.Account.Settings.Parts[AccountPartActivation.ExtensionConfigId].Settings as AccountPartActivation;

            newActivationPart.ActivationDate = activationPart.ActivationDate;
            newActivationPart.Status = activationPart.Status;
            finalBe.Account.Settings.Parts[AccountPartActivation.ExtensionConfigId].Settings = newActivationPart;
        }

        #endregion
    }
}

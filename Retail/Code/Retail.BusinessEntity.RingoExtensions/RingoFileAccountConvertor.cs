﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Entities;
using Retail.BusinessEntity.MainExtensions.AccountParts;
using Vanrise.BEBridge.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Retail.BusinessEntity.RingoExtensions
{
    public class RingoFileAccountConvertor : TargetBEConvertor
    {
        public override void ConvertSourceBEs(ITargetBEConvertorConvertSourceBEsContext context)
        {
            FileSourceBatch fileBatch = context.SourceBEBatch as FileSourceBatch;
            List<ITargetBE> lstTargets = new List<ITargetBE>();
            string fileContent = System.Text.Encoding.UTF8.GetString(fileBatch.Content, 0, fileBatch.Content.Length);
            using (StringReader reader = new StringReader(fileContent))
            {
                while (true)
                {
                    string recordLine = reader.ReadLine();
                    if (!string.IsNullOrEmpty(recordLine))
                    {
                        SourceAccountData accountData = new SourceAccountData
                        {
                            Account = new Account()
                        };
                        string[] accountRecords = recordLine.Split(',');
                        accountData.Account.Name = string.Format("{0} {1}", accountRecords[2], accountRecords[3]);
                        accountData.Account.SourceId = accountRecords[22];
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
            SourceAccountData existingBE = context.ExistingBE as SourceAccountData;
            SourceAccountData newBE = context.NewBE as SourceAccountData;

            SourceAccountData finalBE = new SourceAccountData
            {
                Account = new Account
                {
                    AccountId = existingBE.Account.AccountId,
                    SourceId = newBE.SourceBEId as string,
                    Settings = newBE.Account.Settings,
                    Name = newBE.Account.Name
                }
            };
            context.FinalBE = finalBE;
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
        }

        void FillActivationPart(SourceAccountData accountData, string[] accountRecords)
        {
            accountData.Account.Settings.Parts.Add(AccountPartActivation.ExtensionConfigId, new AccountPart
            {
                Settings = new AccountPartActivation
                {
                    ActivationDate = DateTime.ParseExact(accountRecords[1], "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture)
                }
            });
        }

        void FillPersonalInfoPart(SourceAccountData accountData, string[] accountRecords)
        {
            CountryManager countryManager = new CountryManager();
            Country country = countryManager.GetCountry(accountRecords[5]);

            accountData.Account.Settings.Parts.Add(AccountPartPersonalInfo.ExtensionConfigId, new AccountPart
            {
                Settings = new AccountPartPersonalInfo
                {
                    FirstName = accountRecords[2],
                    LastName = accountRecords[3],
                    Gender = accountRecords[4] == "M" ? Gender.Male : Gender.Female,
                    BirthCountryId = country != null ? (int?)country.CountryId : null,
                    BirthCityId = null,
                    BirthDate = DateTime.ParseExact(accountRecords[16], "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture)
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

        #endregion
    }
}

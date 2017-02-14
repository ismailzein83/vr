using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualBasic.FileIO;
using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using Retail.BusinessEntity.MainExtensions.AccountParts;
using Retail.Ringo.MainExtensions.AccountParts;
using Vanrise.BEBridge.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues;
using Vanrise.GenericData.Transformation.Entities;

namespace Retail.Ringo.MainExtensions
{
    public class AccountConvertor : TargetBEConvertor
    {     

        #region Properties
        public Guid AccountBEDefinitionId { get; set; }
        public Guid AccountTypeId { get; set; }
        public Guid InitialStatusId { get; set; }
        public Guid AgentBEDefinitionId { get; set; }
        public Guid DistributorBEDefinitionId { get; set; }
        public Guid PosBEDefinitionId { get; set; }
        public Guid ActivationPartDefinitionId { get; set; }
        public Guid OtherPartDefinitionId { get; set; }
        public Guid DealersPartDefinitionId { get; set; }
        public Guid FinancialPartDefinitionId { get; set; }
        public Guid ResidentialProfilePartDefinitionId { get; set; }
        public Guid PersonalInfoPartDefinitionId { get; set; }
        
        public override string Name
        {
            get
            {
                return "Ringo Account Convertor";
            }
        }

        #endregion

        #region Public Methods
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
                        accountRecords = accountRecords.Select(s => s.Trim(new char[] { '\'' })).ToArray();
                        var sourceId = accountRecords[22];
                        if (sourceId == "NA")
                            continue;
                        string accountName = string.Format("{0} {1}", accountRecords[2], accountRecords[3]);
                        SourceAccountData accountData = new SourceAccountData
                        {
                            Account = new Account(),
                            IdentificationRulesToInsert = new List<MappingRule>
                            {
                                GetMappingRule(accountRecords[22], accountName)
                            },
                            IdentificationRulesToUpdate = new List<MappingRule>()
                        };

                        accountData.Account.Name = accountName;
                        accountData.Account.SourceId = sourceId;
                        accountData.Account.TypeId = this.AccountTypeId;// new Guid("19A97F72-8C56-441E-A74D-AA185961B242");
                        FillAccountSettings(accountData, accountRecords);
                        accountData.Account.StatusId = this.InitialStatusId;// Guid.Parse("DDB6A5B8-B9E5-4050-BEE8-0F030E801B8B");
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
            UpdateEntitiesPart(finalBe, newBe);
            finalBe.Account.TypeId = newBe.Account.TypeId;
            context.FinalBE = finalBe;
        }
        public override bool CompareBeforeUpdate
        {
            get
            {
                return base.CompareBeforeUpdate;
            }
        }

        #endregion

        #region Private Methods

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
            FillEntitiesPart(accountData, accountRecords);
            FillOtherPart(accountData, accountRecords);
        }

        void FillOtherPart(SourceAccountData accountData, string[] accountRecords)
        {
            bool isTheft = false;
            bool.TryParse(accountRecords[28], out isTheft);
            accountData.Account.Settings.Parts.Add(this.OtherPartDefinitionId, new AccountPart
            {
                Settings = new AccountPartOtherInfo
                {
                    CNIC = accountRecords[14],
                    IsTheft = isTheft,
                    TaxCode = accountRecords[9]
                }
            });
        }

        void FillActivationPart(SourceAccountData accountData, string[] accountRecords)
        {
            DateTime activationDate;
            DateTime.TryParseExact(accountRecords[16], "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out activationDate);
            accountData.Account.Settings.Parts.Add(this.ActivationPartDefinitionId, new AccountPart
            {
                Settings = new AccountPartActivation
                {
                    ActivationDate = activationDate
                }
            });
        }

        void FillEntitiesPart(SourceAccountData accountData, string[] accountRecords)
        {
            AccountBEManager accountBeManager = new AccountBEManager();

            Account pointOfSale = accountBeManager.GetAccountBySourceId(this.PosBEDefinitionId, accountRecords[30]);
            Account agent = accountBeManager.GetAccountBySourceId(this.AgentBEDefinitionId, accountRecords[33]);
            Account distributor = accountBeManager.GetAccountBySourceId(this.DistributorBEDefinitionId, accountRecords[36]);

            accountData.Account.Settings.Parts.Add(this.DealersPartDefinitionId, new AccountPart
            {
                Settings = new AccountPartEntitiesInfo
                {
                    PosId = pointOfSale == null ? 0 : pointOfSale.AccountId,
                    AgentId = agent == null ? 0 : agent.AccountId,
                    DistributorId = distributor == null ? 0 : distributor.AccountId
                }
            });
        }

        void FillPersonalInfoPart(SourceAccountData accountData, string[] accountRecords)
        {
            CountryManager countryManager = new CountryManager();
            Country country = countryManager.GetCountry(accountRecords[5]);
            DateTime birthDate;
            DateTime.TryParseExact(accountRecords[16], "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out birthDate);
            accountData.Account.Settings.Parts.Add(this.PersonalInfoPartDefinitionId, new AccountPart
            {
                Settings = new AccountPartPersonalInfo
                {
                    FirstName = accountRecords[2],
                    LastName = accountRecords[3],
                    Gender = accountRecords[4] == "M" ? Gender.Male : Gender.Female,
                    CountryId = country != null ? (int?)country.CountryId : null,
                    CityId = null,
                    BirthDate = birthDate
                }
            });
        }

        void FillResidentialProfilePart(SourceAccountData accountData, string[] accountRecords)
        {
            CountryManager countryManager = new CountryManager();
            Country country = countryManager.GetCountry(accountRecords[8]);
            accountData.Account.Settings.Parts.Add(this.ResidentialProfilePartDefinitionId, new AccountPart
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
            AccountPartPersonalInfo personalInfo = newBe.Account.Settings.Parts[this.PersonalInfoPartDefinitionId].Settings as AccountPartPersonalInfo;
            AccountPartPersonalInfo newPersonalInfo = finalBe.Account.Settings.Parts[this.PersonalInfoPartDefinitionId].Settings as AccountPartPersonalInfo;

            newPersonalInfo.CityId = personalInfo.CityId;
            newPersonalInfo.CountryId = personalInfo.CountryId;
            newPersonalInfo.BirthDate = personalInfo.BirthDate;
            newPersonalInfo.FirstName = personalInfo.FirstName;
            newPersonalInfo.Gender = personalInfo.Gender;
            newPersonalInfo.LastName = personalInfo.LastName;
            finalBe.Account.Name = string.Format("{0} {1}", newPersonalInfo.FirstName, newPersonalInfo.LastName);
            finalBe.Account.Settings.Parts[this.PersonalInfoPartDefinitionId].Settings = newPersonalInfo;
        }

        void UpdateResidentialInfoPart(SourceAccountData finalBe, SourceAccountData newBe)
        {
            AccountPartResidentialProfile residentialProfile = newBe.Account.Settings.Parts[this.ResidentialProfilePartDefinitionId].Settings as AccountPartResidentialProfile;
            AccountPartResidentialProfile newResidentialProfile = finalBe.Account.Settings.Parts[this.ResidentialProfilePartDefinitionId].Settings as AccountPartResidentialProfile;

            newResidentialProfile.CityId = residentialProfile.CityId;
            newResidentialProfile.CountryId = residentialProfile.CountryId;
            newResidentialProfile.Email = residentialProfile.Email;
            newResidentialProfile.Faxes = residentialProfile.Faxes;
            newResidentialProfile.PhoneNumbers = residentialProfile.PhoneNumbers;
            newResidentialProfile.Provence = residentialProfile.Provence;
            newResidentialProfile.Street = residentialProfile.Street;
            newResidentialProfile.Town = residentialProfile.Town;
            newResidentialProfile.ZipCode = residentialProfile.ZipCode;

            finalBe.Account.Settings.Parts[this.ResidentialProfilePartDefinitionId].Settings = newResidentialProfile;
        }

        private void UpdateActivationInfoPart(SourceAccountData finalBe, SourceAccountData newBe)
        {
            AccountPartActivation activationPart = newBe.Account.Settings.Parts[this.ActivationPartDefinitionId].Settings as AccountPartActivation;
            AccountPartActivation newActivationPart = finalBe.Account.Settings.Parts[this.ActivationPartDefinitionId].Settings as AccountPartActivation;

            newActivationPart.ActivationDate = activationPart.ActivationDate;
            newActivationPart.Status = activationPart.Status;
            finalBe.Account.Settings.Parts[this.ActivationPartDefinitionId].Settings = newActivationPart;
        }

        void UpdateEntitiesPart(SourceAccountData finalBe, SourceAccountData newBe)
        {
            AccountPartEntitiesInfo oldPart = newBe.Account.Settings.Parts[this.DealersPartDefinitionId].Settings as AccountPartEntitiesInfo;
            AccountPartEntitiesInfo newPart = finalBe.Account.Settings.Parts[this.DealersPartDefinitionId].Settings as AccountPartEntitiesInfo;

            newPart.PosId = oldPart.PosId;
            newPart.AgentId = oldPart.AgentId;
            newPart.DistributorId = oldPart.DistributorId;
            finalBe.Account.Settings.Parts[this.DealersPartDefinitionId].Settings = newPart;
        }

        #endregion

        private MappingRule GetMappingRule(string msisdn, string accountName)
        {
            MappingRule rule = new MappingRule
            {
                BeginEffectiveTime = DateTime.Parse("2000-01-01"),
                Settings = new MappingRuleSettings(),
                Criteria = new GenericRuleCriteria
                {
                    FieldsValues = new Dictionary<string, GenericRuleCriteriaFieldValues>()
                },
                DefinitionId = new Guid("E30037DA-29C6-426A-A581-8EB0EDD1D5E3"),
                Description = string.Format("{0} Identification Rule", accountName)
            };
            rule.Criteria.FieldsValues.Add("MSISDN", new StaticValues
            {
                Values = new List<object>
                {
                    msisdn
                }
            });
            return rule;
        }

        #endregion

    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;
using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using Retail.BusinessEntity.MainExtensions.AccountParts;
using Vanrise.BEBridge.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues;
using Vanrise.GenericData.Transformation.Entities;

namespace Retail.BusinessEntity.MainExtensions.FileAccountConvertors
{
    public class UserConvertor : TargetBEConvertor
    {
        public override string Name
        {
            get
            {
                return "Users";
            }
        }
        CurrencySettingData _currencySettingData;
        public override void ConvertSourceBEs(ITargetBEConvertorConvertSourceBEsContext context)
        {
            FileSourceBatch fileBatch = context.SourceBEBatch as FileSourceBatch;

            List<ITargetBE> lstTargets = new List<ITargetBE>();
            SettingManager settingManager = new SettingManager();
            var _systemCurrencySetting = settingManager.GetSettingByType("VR_Common_BaseCurrency");
            _currencySettingData = (CurrencySettingData)_systemCurrencySetting.Data;


            using (Stream stream = new MemoryStream(fileBatch.Content))
            {
                TextFieldParser parser = new TextFieldParser(stream) { Delimiters = new string[] { "," } };
                while (true)
                {
                    string[] accountRecords = parser.ReadFields();
                    if (accountRecords != null)
                    {
                        var sourceId = accountRecords[0];
                        string accountName = string.Format("{0} {1}", accountRecords[1], accountRecords[2]);

                        string companyId = accountRecords[4];

                        SourceAccountData accountData = new SourceAccountData
                        {
                            Account = new Account(),
                            IdentificationRulesToInsert = new List<MappingRule>
                            {
                                GetMappingRule(accountRecords[3], accountName)
                            },
                            IdentificationRulesToUpdate = new List<MappingRule>()
                        };

                        if (string.IsNullOrEmpty(companyId))
                        {
                            accountData.Account.TypeId = Guid.Parse("19A97F72-8C56-441E-A74D-AA185961B242");
                        }
                        else
                        {
                            accountData.Account.TypeId = Guid.Parse("CEA8ABA9-CDAA-4755-B30D-3734EAE52E83");
                            AccountManager accountManager = new AccountManager();
                            var company = accountManager.GetAccountBySourceId(companyId);
                            if (company == null)
                                throw new NullReferenceException("company");
                            accountData.Account.ParentAccountId = company.AccountId;
                        }
                        accountData.Account.Name = accountName;
                        accountData.Account.SourceId = sourceId;
                        accountData.Account.StatusId = Guid.Parse("DDB6A5B8-B9E5-4050-BEE8-0F030E801B8B");
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


            context.FinalBE = finalBe;
        }

        void UpdatePersonalInfoPart(SourceAccountData finalBe, SourceAccountData newBe)
        {
            AccountPartPersonalInfo personalInfo = newBe.Account.Settings.Parts[AccountPartPersonalInfo._ConfigId].Settings as AccountPartPersonalInfo;
            AccountPartPersonalInfo newPersonalInfo = finalBe.Account.Settings.Parts[AccountPartPersonalInfo._ConfigId].Settings as AccountPartPersonalInfo;
            newPersonalInfo.FirstName = personalInfo.FirstName;
            newPersonalInfo.LastName = personalInfo.LastName;
            finalBe.Account.Name = string.Format("{0} {1}", newPersonalInfo.FirstName, newPersonalInfo.LastName);
            finalBe.Account.Settings.Parts[AccountPartPersonalInfo._ConfigId].Settings = newPersonalInfo;
        }

        #region Private Methods
        MappingRule GetMappingRule(string number, string accountName)
        {
            MappingRule rule = new MappingRule
            {
                BeginEffectiveTime = DateTime.Parse("2000-01-01"),
                Settings = new MappingRuleSettings(),
                Criteria = new GenericRuleCriteria
                {
                    FieldsValues = new Dictionary<string, GenericRuleCriteriaFieldValues>()
                },
                DefinitionId = new Guid("7282858D-C7B5-44EB-BCF2-68A02DE45F8B"),
                Description = string.Format("{0} Identification Rule", accountName)
            };
            rule.Criteria.FieldsValues.Add("Number", new StaticValues
            {
                Values = new List<object>
                {
                    number
                }
            });
            return rule;
        }

        #endregion
    }

}
